#if LESSTHAN_NET35

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is ICollection<TSource> collection && collection.Count == 0)
            {
                throw new InvalidOperationException();
            }
            else if (source is IList<TSource> list)
            {
                return list[list.Count - 1];
            }
            else
            {
                var found = false;
                var result = default(TSource)!;
                foreach (var item in source)
                {
                    result = item;
                    found = true;
                }
                if (found)
                {
                    return result;
                }
                throw new InvalidOperationException();
            }
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var result = default(TSource)!;
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    continue;
                }
                result = item;
                found = true;
            }
            if (found)
            {
                return result;
            }

            throw new InvalidOperationException();
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is IList<TSource> list)
            {
                return list.Count > 0 ? list[list.Count - 1] : default;
            }
            var found = false;
            var result = default(TSource)!;
            foreach (var item in source)
            {
                result = item;
                found = true;
            }

            return found ? result : default;
        }

        [return: MaybeNull]
        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var result = default(TSource)!;
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    continue;
                }
                result = item;
            }
            return result;
        }
    }
}

#endif