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
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            return CreateGroupByIterator();
            IEnumerable<IGrouping<TKey, TSource>> CreateGroupByIterator()
            {
                var groups = new NullAwareDictionary<TKey, Grouping<TKey, TSource>>(comparer);
                foreach (var element in source)
                {
                    var key = keySelector(element);
                    if (!groups.TryGetValue(key, out var group))
                    {
                        group = new Grouping<TKey, TSource>(key);
                        groups.Add(key, group);
                    }
                    group.Add(element);
                }
                foreach (var group in groups)
                {
                    yield return group.Value;
                }
            }
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupProgressiveBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return GroupProgressiveBy(source, keySelector, elementSelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupProgressiveBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return CreateGroupByIterator();
            IEnumerable<IGrouping<TKey, TElement>> CreateGroupByIterator()
            {
                var groups = new NullAwareDictionary<TKey, Grouping<TKey, TElement>>(comparer);
                foreach (var item in source)
                {
                    var key = keySelector(item);
                    var element = resultSelector(item);
                    if (!groups.TryGetValue(key, out var group))
                    {
                        group = new Grouping<TKey, TElement>(key);
                        groups.Add(key, group);
                    }
                    group.Add(element);
                }
                foreach (var group in groups)
                {
                    yield return group.Value;
                }
            }
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return GroupProgressiveBy(source, keySelector, elementSelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException(nameof(elementSelector));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return CreateGroupByIterator();
            IEnumerable<TResult> CreateGroupByIterator()
            {
                var groups = GroupProgressiveBy(source, keySelector, elementSelector, comparer);

                foreach (var group in groups)
                {
                    yield return resultSelector(group.Key, group);
                }
            }
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return GroupProgressiveBy(source, keySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupProgressiveBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return CreateGroupByIterator();

            IEnumerable<TResult> CreateGroupByIterator()
            {
                var groups = GroupProgressiveBy(source, keySelector, comparer);

                foreach (var group in groups)
                {
                    yield return resultSelector(group.Key, group);
                }
            }
        }
    }
}