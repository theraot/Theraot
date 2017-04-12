// Needed for NET40

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static int AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            var count = 0;
            foreach (var item in items)
            {
                collection.Add(item);
                count++;
            }
            return count;
        }

        public static IEnumerable<T> AddRangeEnumerable<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (var item in items)
            {
                collection.Add(item);
                yield return item;
            }
        }
    }
}