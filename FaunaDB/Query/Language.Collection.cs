﻿namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Map(Expr collection, Expr lambda) =>
            UnescapedObject.With("map", lambda, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Foreach(Expr collection, Expr lambda) =>
            UnescapedObject.With("foreach", lambda, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Filter(Expr collection, Expr lambda) =>
            UnescapedObject.With("filter", lambda, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Take(Expr number, Expr collection) =>
            UnescapedObject.With("take", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Drop(Expr number, Expr collection) =>
            UnescapedObject.With("drop", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Prepend(Expr elements, Expr collection) =>
            UnescapedObject.With("prepend", elements, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>.
        /// </summary>
        public static Expr Append(Expr elements, Expr collection) =>
            UnescapedObject.With("append", elements, "collection", collection);
    }
}
