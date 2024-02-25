#if LESSTHAN_NET472 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

#pragma warning disable MA0016 // Prefer return collection abstraction instead of implementation

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class
#if LESSTHAN_NET35
        Enumerable
#else
        EnumerableTheraotExtensions
#endif
    {
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(comparer: null);

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // Don't pre-allocate based on knowledge of size, as potentially many elements will be dropped.
            return new HashSet<TSource>(source, comparer);
        }
    }
}

#endif