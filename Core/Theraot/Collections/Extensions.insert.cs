using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static void InsertRange<TItem, TCollection>(this TCollection collection, int index, TItem item)
            where TCollection : class, IList<TItem>
        {
            Check.NotNullArgument(collection, "collection").Insert(index, item);
        }

        public static void InsertRange<TItem, TCollection>(this TCollection collection, int index, Func<TItem> item)
            where TCollection : class, IList<TItem>
        {
            Check.NotNullArgument(collection, "collection").Insert(index, Check.NotNullArgument(item, "item")());
        }

        public static int InsertRange<TItem, TCollection>(this TCollection collection, int index, IEnumerable<TItem> items)
            where TCollection : class, IList<TItem>
        {
            int _index = index;
            Check.NotNullArgument(collection, "collection");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                collection.Insert(index, item);
                _index.CheckedIncrement();
            }
            return _index - index;
        }

        public static int InsertRange<TItem, TCollection>(this TCollection collection, int index, IEnumerable<Func<TItem>> items)
            where TCollection : class, IList<TItem>
        {
            int _index = index;
            var _collection = Check.NotNullArgument(collection, "collection");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _collection.Insert(index, item());
                _index.CheckedIncrement();
            }
            return _index - index;
        }
    }
}