#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return Contains(source, value, null);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return comparer == null
                ? ContainsExtracted(source, value, EqualityComparer<TSource>.Default)
                : ContainsExtracted(source, value, comparer);
        }

        private static bool ContainsExtracted<TSource>(IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            foreach (var item in source)
            {
                if (comparer.Equals(item, value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

#endif