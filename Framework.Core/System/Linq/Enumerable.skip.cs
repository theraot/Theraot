// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
#if LESSTHAN_NET35

    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (count > 0)
            {
                return SkipWhile(source, (_, i) => i < count);
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source;
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return SkipWhile(source, (item, _) => predicate(item));
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SkipWhileExtracted();

            IEnumerable<TSource> SkipWhileExtracted()
            {
                var enumerator = source.GetEnumerator();
                using (enumerator)
                {
                    for (var count = 0; enumerator.MoveNext(); count++)
                    {
                        if (!predicate(enumerator.Current, count))
                        {
                            while (true)
                            {
                                yield return enumerator.Current;
                                if (!enumerator.MoveNext())
                                {
                                    yield break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

#endif

#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

    public static partial class
#if LESSTHAN_NET35
        Enumerable
#else
        EnumerableTheraotExtensions
#endif
    {
        public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return count <= 0 ?
                source :
                SkipLastIterator(source, count);
        }

        private static IEnumerable<TSource> SkipLastIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            var queue = new Queue<TSource>();

            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (queue.Count == count)
                    {
                        do
                        {
                            yield return queue.Dequeue();
                            queue.Enqueue(e.Current);
                        }
                        while (e.MoveNext());
                        break;
                    }

                    queue.Enqueue(e.Current);
                }
            }
        }
    }

#endif
}