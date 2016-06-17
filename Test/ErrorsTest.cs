﻿using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class ErrorsTest : TestCase
    {
        [Test] public async Task TestRequestResult()
        {
            var err = await AssertU.Throws<BadRequest>(() => TestClient.Query(UnescapedObject.With("foo", "bar")));
            Assert.AreEqual(err.RequestResult.RequestContent, UnescapedObject.With("foo", "bar"));
        }

        [Test] public async Task TestInvalidResponse()
        {
            // Response must be valid JSON
            await AssertU.Throws<InvalidResponseException>(() => MockClient("I like fine wine").Get(""));
            // Response must have "resource"
            //todo: is this the right error to throw?
            await AssertU.Throws<KeyNotFoundException>(() => MockClient("{\"resoars\": 1}").Get(""));
        }

        #region HTTP errors
        [Test] public async Task TestHttpBadRequest()
        {
            await AssertU.Throws<BadRequest>(() => TestClient.Query(UnescapedObject.With("foo", "bar")));
        }

        [Test] public async Task TestHttpUnauthorized()
        {
            var client = GetClient(password: "bad_key");
            await AssertHttpException<Unauthorized>("unauthorized", () => client.Get(DbRef));
        }

        [Test] public async Task TestHttpPermissionDenied()
        {
            await AssertHttpException<PermissionDenied>("permission denied", () => TestClient.Get("databases"));
        }

        [Test] public async Task TestHttpNotFound()
        {
            await AssertHttpException<NotFound>("not found", () => TestClient.Get("classes/not_found"));
        }

        [Test] public async Task TestHttpMethodNotAllowed()
        {
            await AssertHttpException<MethodNotAllowed>("method not allowed", () => TestClient.Delete("classes"));
        }

        [Test] public async Task TestInternalError()
        {
            await AssertHttpException<InternalError>("internal server error", () => TestClient.Get("error"));
        }

        [Test] public async Task TestUnavailableError()
        {
            var client = MockClient("{\"errors\": [{\"code\": \"unavailable\", \"description\": \"on vacation\"}]}", HttpStatusCode.ServiceUnavailable);
            await AssertHttpException<UnavailableError>("unavailable", () => client.Get(""));
        }
        #endregion

        #region ErrorData
        [Test] public async Task TestInvalidExpression()
        {
            await AssertQueryException<BadRequest>(UnescapedObject.With("foo", "bar"), "invalid expression", ArrayV.Empty);
        }

        [Test] public async Task TestUnboundVariable()
        {
            await AssertQueryException<BadRequest>(Var("x"), "unbound variable", ArrayV.Empty);
        }

        [Test] public async Task TestInvalidArgument()
        {
            await AssertQueryException<BadRequest>(Add(Arr(1, "two")), "invalid argument", Arr("add", 1));
        }

        [Test] public async Task TestInstanceNotFound()
        {
            // Must be a reference to a real class or else we get InvalidExpression
            await TestClient.Post("classes", UnescapedObject.With("name", "foofaws"));
            await AssertQueryException<NotFound>(Get(Ref("classes/foofaws/123")), "instance not found", ArrayV.Empty);
        }

        [Test] public async Task TestValueNotFound()
        {
            await AssertQueryException<NotFound>(Select("a", Obj()), "value not found", ArrayV.Empty);
        }

        [Test] public async Task TestInstanceAlreadyExists()
        {
            await TestClient.Post("classes", UnescapedObject.With("name", "duplicates"));
            var @ref = (Ref) ((ObjectV) (await TestClient.Post("classes/duplicates", UnescapedObject.Empty)))["ref"];
            await AssertQueryException<BadRequest>(Create(@ref, Obj()), "instance already exists", Arr("create"));
        }
        #endregion

        #region InvalidData
        [Test] public async Task TestInvalidType()
        {
            await AssertInvalidData("classes", UnescapedObject.With("name", 123), "invalid type", Arr("name"));
        }

        [Test] public async Task TestValueRequired()
        {
            await AssertInvalidData("classes", UnescapedObject.Empty, "value required", Arr("name"));
        }

        [Test] public async Task TestDuplicateValue()
        {
            await TestClient.Post("classes", UnescapedObject.With("name", "gerbils"));
            await TestClient.Post("indexes", UnescapedObject.With(
                "name", "gerbils_by_x",
                "source", Ref("classes/gerbils"),
                "terms", Arr(UnescapedObject.With("path", "data.x")),
                "unique", true
            ));
            await TestClient.Post("classes/gerbils", UnescapedObject.With("data", UnescapedObject.With("x", 1)));
        }

        async Task AssertInvalidData(string className, Expr data, string code, ArrayV field)
        {
            var exception = await AssertU.Throws<BadRequest>(() => TestClient.Post(className, data));
            AssertException(exception, "validation failed", ArrayV.Empty);
            var failures = ((ValidationFailed) exception.Errors.First()).Failures;
            Assert.AreEqual(1, failures.Count());
            var failure = failures.First();
            Assert.AreEqual(code, failure.Code);
            Assert.AreEqual(field, failure.Field);
        }
        #endregion

        [Test] public void TestToString()
        {
            var err = new ErrorData("code", "desc", null);
            Assert.AreEqual(err.ToString(), "ErrorData(code, desc, null)");

            var failure = new Failure("code", "desc", Arr("a", "b"));
            var vf = new ValidationFailed("vf_desc", Arr("vf"), (new[] { failure }.ToList()));
            Assert.AreEqual(
                "ValidationFailed(vf_desc, Arr(StringV(vf)), [Failure(code, desc, Arr(StringV(a), StringV(b)))])",
                vf.ToString());
        }

        async Task AssertHttpException<TException>(string code, Func<Task> action) where TException : FaunaException
        {
            var exception = await AssertU.Throws<TException>(action);
            AssertException(exception, code);
        }

        void AssertException(FaunaException exception, string code, ArrayV position = null)
        {
            Assert.AreEqual(1, exception.Errors.Count());
            var error = exception.Errors.First();
            Assert.AreEqual(code, error.Code);
            Assert.AreEqual(position, error.Position);
        }

        async Task AssertQueryException<TException>(Expr query, string code, ArrayV position = null)
            where TException  : FaunaException
        {
            var exception = await AssertU.Throws<TException>(() => TestClient.Query(query));
            AssertException(exception, code, position);
        }
    }
}
