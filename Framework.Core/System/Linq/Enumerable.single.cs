#if LESSTHAN_NET35

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var found = false;
            var result = default(TSource)!;
            foreach (var item in source)
            {
                if (found)
                {
                    throw new InvalidOperationException();
                }

                found = true;
                result = item;
            }

            if (found)
            {
                return result;
            }

            throw new InvalidOperationException();
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
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

                if (found)
                {
                    throw new InvalidOperationException();
                }

                found = true;
                result = item;
            }

            if (found)
            {
                return result;
            }

            throw new InvalidOperationException();
        }

        [return: MaybeNull]
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var found = false;
            var result = default(TSource)!;
            foreach (var item in source)
            {
                if (found)
                {
                    throw new InvalidOperationException();
                }

                found = true;
                result = item;
            }

            return result;
        }

        [return: MaybeNull]
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
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

                if (found)
                {
                    throw new InvalidOperationException();
                }

                found = true;
                result = item;
            }

            return result;
        }
    }
}

#endif