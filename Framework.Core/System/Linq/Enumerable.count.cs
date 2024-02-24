#if TARGETS_NET || TARGETS_NETSTANDARD || LESSTHAN_NET60

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableEx
    {
        public static bool TryGetNonEnumeratedCount<TSource>(this IEnumerable<TSource> source, out int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<TSource> collectionOfT)
            {
                count = collectionOfT.Count;
                return true;
            }

            if (source is ICollection collection)
            {
                count = collection.Count;
                return true;
            }

            count = 0;
            return false;
        }
    }
}

#endif