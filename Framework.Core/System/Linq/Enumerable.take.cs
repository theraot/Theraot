#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
#if LESSTHAN_NET35

    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return TakeWhileExtracted();

            IEnumerable<TSource> TakeWhileExtracted()
            {
                if (count > 0)
                {
                    var currentCount = 0;
                    foreach (var item in source)
                    {
                        yield return item;
                        currentCount++;
                        if (currentCount == count)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return TakeWhile(source, (item, _) => predicate(item));
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return TakeWhileExtracted();

            IEnumerable<TSource> TakeWhileExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    if (!predicate(item, count))
                    {
                        break;
                    }
                    yield return item;
                    count++;
                }
            }
        }
    }

#endif

    public static partial class
#if LESSTHAN_NET35
        Enumerable
#else
        EnumerableTheraotExtensions
#endif
    {
        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return count <= 0 ? Enumerable.Empty<TSource>() : TakeLastIterator(source, count);
        }

        private static IEnumerable<TSource> TakeLastIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            Queue<TSource> queue;
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    yield break;
                }

                queue = new Queue<TSource>();
                queue.Enqueue(e.Current);

                while (e.MoveNext())
                {
                    if (queue.Count < count)
                    {
                        queue.Enqueue(e.Current);
                    }
                    else
                    {
                        do
                        {
                            queue.Dequeue();
                            queue.Enqueue(e.Current);
                        }
                        while (e.MoveNext());
                        break;
                    }
                }
            }
            do
            {
                yield return queue.Dequeue();
            }
            while (queue.Count > 0);
        }
    }
}

#endif