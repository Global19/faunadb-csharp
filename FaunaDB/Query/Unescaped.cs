﻿using FaunaDB.Types;
using FaunaDB.Utils;
using Newtonsoft.Json;
using System.Linq;

namespace FaunaDB.Query
{
    public class UnescapedObject : Expr
    {
        public static readonly UnescapedObject Empty = new UnescapedObject(new OrderedDictionary<string, Expr>());

        private OrderedDictionary<string, Expr> Values;

        public UnescapedObject() : this(new OrderedDictionary<string, Expr>()) { }

        public UnescapedObject(OrderedDictionary<string, Expr> values)
        {
            Values = values;
        }

        public override bool Equals(Expr v)
        {
            var w = v as UnescapedObject;
            return w != null && w.Values.Equals(Values);
        }

        protected override int HashCode() =>
            Values.GetHashCode();

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject(Values);
        }

        public override string ToString()
        {
            var props = string.Join(",", from kv in Values select $"{kv.Key}: {kv.Value}");
            return $"FnObject({props})";
        }

        public static UnescapedObject With() =>
            new UnescapedObject();

        public static UnescapedObject With(string key1, Expr value1) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6, string key7, Expr value7) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6, key7, value7));
    }
}
