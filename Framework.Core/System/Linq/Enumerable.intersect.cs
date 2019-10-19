#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            return IntersectExtracted(first, second, EqualityComparer<TSource>.Default);
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            return IntersectExtracted(first, second, comparer ?? EqualityComparer<TSource>.Default);
        }

        private static IEnumerable<TSource> IntersectExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            // We create a HashSet of the second IEnumerable.
            // By doing so, duplicates are lost.
            // Then by removing the contents of the first IEnumerable from the HashSet,
            // Those elements that we can remove from the HashSet are the intersection of both IEnumerables
            var items = new HashSet<TSource>(second, comparer);
            foreach (var element in first)
            {
                if (items.Remove(element))
                {
                    yield return element;
                }
            }
        }
    }
}

#endif