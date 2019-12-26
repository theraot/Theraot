#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            return ExceptExtracted(first, second, null);
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            return ExceptExtracted(first, second, comparer);
        }

        private static IEnumerable<TSource> ExceptExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
        {
            comparer ??= EqualityComparer<TSource>.Default;
            var items = new HashSet<TSource>(second, comparer);
            foreach (var item in first)
            {
                if (items.Add(item))
                {
                    yield return item;
                }
            }
        }
    }
}

#endif