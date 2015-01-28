using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static int AddRange<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
        {
            int count = 0;
            var _collection = Check.NotNullArgument(collection, "collection");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _collection.Add(item);
                count++;
            }
            return count;
        }

        public static IEnumerable<TItem> AddRangeEnumerable<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _collection.Add(item);
                yield return item;
            }
        }
    }
}