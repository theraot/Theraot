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
                throw new ArgumentNullException(nameof(collection));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
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
                throw new ArgumentNullException(nameof(collection));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            return AddRangeEnumerableExtracted();

            IEnumerable<T> AddRangeEnumerableExtracted()
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                    yield return item;
                }
            }
        }
    }
}