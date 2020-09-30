#if LESSTHAN_NET35

#pragma warning disable S1751 // Loops with at most one iteration should be refactored
#pragma warning disable S2971 // "IEnumerable" LINQs should be simplified

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!(source is IList<TSource> list))
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
            }
            else
            {
                if (list.Count != 0)
                {
                    return list[0];
                }
            }

            throw new InvalidOperationException("The source sequence is empty");
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            throw new InvalidOperationException();
        }

        [return: MaybeNull]
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                return item;
            }

            return default;
        }

        [return: MaybeNull]
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return FirstOrDefault(source.Where(predicate));
        }
    }
}

#endif