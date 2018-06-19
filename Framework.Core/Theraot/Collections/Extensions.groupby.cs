#if FAT

using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<IGrouping<TKey, TSource>> GroupProgressiveBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return GroupProgressiveBy(source, keySelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupProgressiveBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            return CreateGroupByIterator(source, keySelector, comparer);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupProgressiveBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return GroupProgressiveBy(source, keySelector, elementSelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupProgressiveBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return CreateGroupByIterator(source, keySelector, resultSelector, comparer);
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return GroupProgressiveBy(source, keySelector, elementSelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return CreateGroupByIterator(source, keySelector, elementSelector, resultSelector, comparer);
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return GroupProgressiveBy(source, keySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return CreateGroupByIterator(source, keySelector, resultSelector, comparer);
        }
    }

    public static partial class Extensions
    {
        //GroupBy progressive implementation

        private static IEnumerable<IGrouping<TKey, TSource>> CreateGroupByIterator<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            // NOTICE this method has no null check
            var groups = new Dictionary<TKey, List<TSource>>(comparer);
            var nullList = new List<TSource>();
            var counter = 0;
            var nullCounter = -1;

            foreach (TSource element in source)
            {
                var key = keySelector(element);
                if (ReferenceEquals(key, null))
                {
                    nullList.Add(element);
                    if (nullCounter == -1)
                    {
                        nullCounter = counter;
                        counter++;
                    }
                }
                else
                {
                    List<TSource> group;
                    if (!groups.TryGetValue(key, out group))
                    {
                        group = new List<TSource>();
                        groups.Add(key, group);
                        counter++;
                    }
                    group.Add(element);
                }
            }

            counter = 0;
            foreach (var group in groups)
            {
                if (counter == nullCounter)
                {
                    yield return new Grouping<TKey, TSource>(default(TKey), nullList);
                    counter++;
                }

                yield return new Grouping<TKey, TSource>(group.Key, group.Value);
                counter++;
            }

            if (counter == nullCounter)
            {
                yield return new Grouping<TKey, TSource>(default(TKey), nullList);
                // counter++;
            }
        }

        private static IEnumerable<IGrouping<TKey, TElement>> CreateGroupByIterator<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            // NOTICE this method has no null check
            var groups = new Dictionary<TKey, List<TElement>>(comparer);
            var nullList = new List<TElement>();
            var counter = 0;
            var nullCounter = -1;

            foreach (TSource item in source)
            {
                var key = keySelector(item);
                var element = elementSelector(item);
                if (ReferenceEquals(key, null))
                {
                    nullList.Add(element);
                    if (nullCounter == -1)
                    {
                        nullCounter = counter;
                        counter++;
                    }
                }
                else
                {
                    List<TElement> group;
                    if (!groups.TryGetValue(key, out group))
                    {
                        group = new List<TElement>();
                        groups.Add(key, group);
                        counter++;
                    }
                    group.Add(element);
                }
            }

            counter = 0;
            foreach (var group in groups)
            {
                if (counter == nullCounter)
                {
                    yield return new Grouping<TKey, TElement>(default(TKey), nullList);
                    counter++;
                }

                yield return new Grouping<TKey, TElement>(group.Key, group.Value);
                counter++;
            }

            if (counter == nullCounter)
            {
                yield return new Grouping<TKey, TElement>(default(TKey), nullList);
                // counter++;
            }
        }

        private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            // NOTICE this method has no null check
            var groups = GroupProgressiveBy(source, keySelector, elementSelector, comparer);

            foreach (IGrouping<TKey, TElement> group in groups)
            {
                yield return resultSelector(group.Key, group);
            }
        }

        private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            // NOTICE this method has no null check
            var groups = GroupProgressiveBy(source, keySelector, comparer);

            foreach (IGrouping<TKey, TSource> group in groups)
            {
                yield return resultSelector(group.Key, group);
            }
        }
    }
}

#endif