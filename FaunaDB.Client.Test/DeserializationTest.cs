﻿using System.Collections.Generic;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class DeserializationTest
    {
        static void AssertJsonEqual(Value value, string json)
        {
            var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            Assert.AreEqual(value, JsonConvert.DeserializeObject<Value>(json, settings));
        }

        [Test] public void TestString()
        {
            AssertJsonEqual(StringV.Of("a string"), "\"a string\"");
        }

        [Test] public void TestBoolean()
        {
            AssertJsonEqual(BooleanV.True, "true");
            AssertJsonEqual(BooleanV.False, "false");
        }

        [Test] public void TestLong()
        {
            AssertJsonEqual(LongV.Of(long.MaxValue), long.MaxValue.ToString());
            AssertJsonEqual(LongV.Of(long.MinValue), long.MinValue.ToString());
            AssertJsonEqual(LongV.Of(10), "10");
        }

        [Test] public void TestDouble()
        {
            AssertJsonEqual(DoubleV.Of(3.14), "3.14");
        }

        [Test] public void TestRef()
        {
            AssertJsonEqual(new RefV("classes"), "{\"@ref\":\"classes\"}");
        }

        [Test] public void TestArray()
        {
            AssertJsonEqual(ArrayV.Of(LongV.Of(1), StringV.Of("a string"), DoubleV.Of(3.14), BooleanV.True, NullV.Instance),
                "[1, \"a string\", 3.14, true, null]");

            AssertJsonEqual(ArrayV.Empty, "[]");
        }

        [Test] public void TestDate()
        {
            AssertJsonEqual(new DateV("2000-01-01"), "{\"@date\":\"2000-01-01\"}");
        }

        [Test] public void TestTimestamp()
        {
            AssertJsonEqual(new TimeV("2000-01-01T01:10:30.123Z"), "{\"@ts\":\"2000-01-01T01:10:30.123Z\"}");
        }

        [Test] public void TestObject()
        {
            AssertJsonEqual(ObjectV.With("foo", ObjectV.With("bar", ArrayV.Of(BooleanV.True, NullV.Instance))),
                "{\"foo\":{\"bar\":[true,null]}}");

            AssertJsonEqual(ObjectV.Empty, "{}");
        }

        [Test] public void TestNull()
        {
            AssertJsonEqual(NullV.Instance, "null");
        }

        [Test] public void TestObjectLiteral()
        {
            AssertJsonEqual(ObjectV.With("@name", StringV.Of("Test")),
                "{\"@obj\":{\"@name\":\"Test\"}}");

            AssertJsonEqual(ObjectV.Empty, "{\"@obj\":{}}");
        }

        [Test] public void TestSetRef()
        {
            var expected = new SetRefV(new Dictionary<string, Value> {
                { "match", new RefV("indexes/spells_by_element") },
                { "terms", StringV.Of("fire") } });
            
            AssertJsonEqual(expected,
                "{" +
                "  \"@set\": {" +
                "    \"match\": { \"@ref\": \"indexes/spells_by_element\" }," +
                "    \"terms\": \"fire\"" +
                "  }" +
                "}");

            AssertJsonEqual(new SetRefV(new Dictionary<string, Value>()),
                "{\"@set\":{}}");
        }

        [Test] public void TestBytes()
        {
            AssertJsonEqual(new BytesV(0x1, 0x2, 0x3, 0x4),
                "{\"@bytes\": \"AQIDBA==\"}");
        }

        [Test] public void TestBytesUrlSafe()
        {
            AssertJsonEqual(new BytesV(0xf8), "{\"@bytes\":\"-A==\"}");
            AssertJsonEqual(new BytesV(0xf9), "{\"@bytes\":\"-Q==\"}");
            AssertJsonEqual(new BytesV(0xfa), "{\"@bytes\":\"-g==\"}");
            AssertJsonEqual(new BytesV(0xfb), "{\"@bytes\":\"-w==\"}");
            AssertJsonEqual(new BytesV(0xfc), "{\"@bytes\":\"_A==\"}");
            AssertJsonEqual(new BytesV(0xfd), "{\"@bytes\":\"_Q==\"}");
            AssertJsonEqual(new BytesV(0xfe), "{\"@bytes\":\"_g==\"}");
            AssertJsonEqual(new BytesV(0xff), "{\"@bytes\":\"_w==\"}");
        }

        [Test]
        public void TestQuery()
        {
            AssertJsonEqual(QueryV.Of("x", Add(Var("x"), 1)), "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");
            AssertJsonEqual(QueryV.Of(x => Add(x, 1)), "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");
        }
    }
}