#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static void InsertRange<TItem, TCollection>(this TCollection collection, int index, TItem item)
            where TCollection : class, IList<TItem>
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            collection.Insert(index, item);
        }

        public static void InsertRange<TItem, TCollection>(this TCollection collection, int index, Func<TItem> item)
            where TCollection : class, IList<TItem>
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            collection.Insert(index, item());
        }

        public static int InsertRange<TItem, TCollection>(this TCollection collection, int index, IEnumerable<TItem> items)
            where TCollection : class, IList<TItem>
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            var initialIndex = index;
            foreach (var item in items)
            {
                collection.Insert(initialIndex, item);
                checked
                {
                    index++;
                }
            }
            return index - initialIndex;
        }

        public static int InsertRange<TItem, TCollection>(this TCollection collection, int index, IEnumerable<Func<TItem>> items)
            where TCollection : class, IList<TItem>
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            var initialIndex = index;
            foreach (var item in items)
            {
                collection.Insert(initialIndex, item());
                checked
                {
                    index++;
                }
            }
            return index - initialIndex;
        }
    }
}

#endif