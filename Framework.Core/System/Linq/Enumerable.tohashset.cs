#if LESSTHAN_NET472 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

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
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            switch (source)
            {
                case null:
                    return new HashSet<TSource>();

                case HashSet<TSource> hashSet:
                    return hashSet;

                default:
                    return new HashSet<TSource>(source);
            }
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            comparer ??= EqualityComparer<TSource>.Default;
            switch (source)
            {
                case null:
                    return new HashSet<TSource>(comparer);

                case HashSet<TSource> hashSet when hashSet.Comparer.Equals(comparer):
                    return hashSet;

                default:
                    return new HashSet<TSource>(source, comparer);
            }
        }
    }
}

#endif