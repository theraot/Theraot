#if LESSTHAN_NET35

#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System.Collections;
using System.Collections.Generic;
using Theraot.Core;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    var folded = enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        folded = func(folded, enumerator.Current);
                    }
                    return folded;
                }

                throw new InvalidOperationException("No elements in source list");
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var folded = seed;
            foreach (var item in source)
            {
                folded = func(folded, item);
            }
            return folded;
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var result = seed;
            foreach (var item in source)
            {
                result = func(result, item);
            }
            return resultSelector(result);
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (!(source is ICollection<TSource> collection))
            {
                using (var enumerator = source.GetEnumerator())
                {
                    return enumerator.MoveNext();
                }
            }

            return collection.Count > 0;
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
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
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            return source;
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is IEnumerable<TResult> enumerable)
            {
                return enumerable;
            }
            return CastExtracted();

            IEnumerable<TResult> CastExtracted()
            {
                foreach (var obj in source)
                {
                    yield return (TResult)obj;
                }
            }
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            return ConcatExtracted();
            IEnumerable<TSource> ConcatExtracted()
            {
                foreach (var item in first)
                {
                    yield return item;
                }
                foreach (var item in second)
                {
                    yield return item;
                }
            }
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return Contains(source, value, null);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            foreach (var item in source)
            {
                if (comparer.Equals(item, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (!(source is ICollection<TSource> collection))
            {
                var result = 0;
                using (var item = source.GetEnumerator())
                {
                    while (item.MoveNext())
                    {
                        checked
                        {
                            result++;
                        }
                    }
                }
                return result;
            }

            return collection.Count;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Count(source.Where(predicate));
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        {
            var item = default(TSource);
            return DefaultIfEmpty(source, item);
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return DefaultIfEmptyExtracted();

            IEnumerable<TSource> DefaultIfEmptyExtracted()
            {
                var enumerator = source.GetEnumerator();
                using (enumerator)
                {
                    if (enumerator.MoveNext())
                    {
                        do
                        {
                            yield return enumerator.Current;
                        }
                        while (enumerator.MoveNext());
                    }
                    else
                    {
                        yield return defaultValue;
                    }
                }
            }
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return Distinct(source, null);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return DistinctExtracted();

            IEnumerable<TSource> DistinctExtracted()
            {
                var found = new Dictionary<TSource, object>(comparer);
                var foundNull = false;
                foreach (var item in source)
                {
                    // item might be null
                    if (item == null)
                    {
                        if (foundNull)
                        {
                            continue;
                        }
                        foundNull = true;
                    }
                    else
                    {
                        if (found.ContainsKey(item))
                        {
                            continue;
                        }
                        found.Add(item, null);
                    }
                    yield return item;
                }
            }
        }

        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "index < 0");
            }

            if (source is IList<TSource> list)
            {
                return list[index];
            }
            if (source is IReadOnlyList<TSource> readOnlyList)
            {
                return readOnlyList[index];
            }
            var count = 0L;
            foreach (var item in source)
            {
                if (index == count)
                {
                    return item;
                }
                count++;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (index < 0)
            {
                return default;
            }

            if (source is IList<TSource> list)
            {
                if (index < list.Count)
                {
                    return list[index];
                }

                return default;
            }
            if (source is IReadOnlyList<TSource> readOnlyList)
            {
                if (index < readOnlyList.Count)
                {
                    return readOnlyList[index];
                }

                return default;
            }
            var count = 0L;
            foreach (var item in source)
            {
                if (index == count)
                {
                    return item;
                }
                count++;
            }
            return default;
        }

        public static IEnumerable<TResult> Empty<TResult>()
        {
            yield break;
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            return ExceptExtracted(first, second, null);
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            return ExceptExtracted(first, second, comparer);
        }

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

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return FirstOrDefault(source.Where(predicate));
        }

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

            if (!(source is IList<TSource> list))
            {
                var found = false;
                var result = default(TSource);
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

            return list[list.Count - 1];
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
            var result = default(TSource);
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
            if (!(source is IList<TSource> list))
            {
                var found = false;
                var result = default(TSource);
                foreach (var item in source)
                {
                    result = item;
                    found = true;
                }
                if (found)
                {
                    return result;
                }

                return default;
            }

            return list.Count > 0 ? list[list.Count - 1] : default;
        }

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
            var result = default(TSource);
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

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (!(source is TSource[] array))
            {
                long count = 0;
                using (var item = source.GetEnumerator())
                {
                    while (item.MoveNext())
                    {
                        count++;
                    }
                }
                return count;
            }

            return array.LongLength;
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return LongCount(source.Where(predicate));
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return OfTypeExtracted();

            IEnumerable<TResult> OfTypeExtracted()
            {
                foreach (var item in source)
                {
                    if (item is TResult result)
                    {
                        yield return result;
                    }
                }
            }
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return OrderBy(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return OrderByDescending(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, true);
        }

        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "count < 0");
            }

            return RepeatExtracted();

            IEnumerable<TResult> RepeatExtracted()
            {
                for (var index = 0; index < count; index++)
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return ReverseExtracted();

            IEnumerable<TSource> ReverseExtracted()
            {
                var stack = new Stack<TSource>();
                foreach (var item in source)
                {
                    stack.Push(item);
                }
                foreach (var item in stack)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return Select(source, (item, _) => selector(item));
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return SelectExtracted(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                foreach (var key in source)
                {
                    foreach (var item in selector(key))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                var count = 0;
                foreach (var key in source)
                {
                    foreach (var item in selector(key, count))
                    {
                        yield return item;
                    }
                    count++;
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (collectionSelector == null)
            {
                throw new ArgumentNullException(nameof(collectionSelector));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                foreach (var element in source)
                {
                    foreach (var collection in collectionSelector(element))
                    {
                        yield return resultSelector(element, collection);
                    }
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (collectionSelector == null)
            {
                throw new ArgumentNullException(nameof(collectionSelector));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                var count = 0;
                foreach (var element in source)
                {
                    foreach (var collection in collectionSelector(element, count))
                    {
                        yield return resultSelector(element, collection);
                    }
                    count++;
                }
            }
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            return SequenceEqualExtracted();

            bool SequenceEqualExtracted()
            {
                using (IEnumerator<TSource> firstEnumerator = first.GetEnumerator(), secondEnumerator = second.GetEnumerator())
                {
                    while (firstEnumerator.MoveNext())
                    {
                        if (!secondEnumerator.MoveNext())
                        {
                            return false;
                        }
                        if (!comparer.Equals(firstEnumerator.Current, secondEnumerator.Current))
                        {
                            return false;
                        }
                    }
                    return !secondEnumerator.MoveNext();
                }
            }
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var result = default(TSource);
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
            var result = default(TSource);
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

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var result = default(TSource);
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
            var result = default(TSource);
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

        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            return SkipWhile(source, (_, i) => i < count);
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return SkipWhile(source, (item, _) => predicate(item));
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
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
                    var count = 0;
                    while (enumerator.MoveNext())
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
                        count++;
                    }
                }
            }
        }

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

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenBy(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (source is IOrderedEnumerable<TSource> oe)
            {
                return oe.CreateOrderedEnumerable(keySelector, comparer, false);
            }
            return source.CreateOrderedEnumerable(keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenByDescending(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            return source.CreateOrderedEnumerable(keySelector, comparer, true);
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is ICollection<TSource> collection)
            {
                var result = new TSource[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }
            if (source is string str)
            {
                return (TSource[])(object)str.ToCharArray();
            }
            return new List<TSource>(source).ToArray();
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToDictionary(source, keySelector, elementSelector, null);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException(nameof(elementSelector));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            comparer = comparer ?? EqualityComparer<TKey>.Default;
            var result = new Dictionary<TKey, TElement>(comparer);
            foreach (var item in source)
            {
                result.Add(keySelector(item), elementSelector(item));
            }
            return result;
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ToDictionary(source, keySelector, null);
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return ToDictionary(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is string str)
            {
                var array = (TSource[])(object)str.ToCharArray();
                var result = new List<TSource>(array.Length);
                result.AddRange(array);
                return result;
            }
            return new List<TSource>(source);
        }

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Lookup<TKey, TSource>.Create(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), null);
        }

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return Lookup<TKey, TSource>.Create(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return Lookup<TKey, TElement>.Create(source, keySelector, elementSelector, null);
        }

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return Lookup<TKey, TElement>.Create(source, keySelector, elementSelector, comparer);
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return Union(first, second, null);
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return Distinct(Concat(first, second), comparer);
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return Where(source, (item, _) => predicate(item));
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return WhereExtracted();

            IEnumerable<TSource> WhereExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    if (!predicate(item, count))
                    {
                        continue;
                    }
                    yield return item;
                    count++;
                }
            }
        }

        public static IEnumerable<TReturn> Zip<T1, T2, TReturn>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, TReturn> resultSelector)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return ZipExtracted();

            IEnumerable<TReturn> ZipExtracted()
            {
                using (var enumeratorFirst = first.GetEnumerator())
                using (var enumeratorSecond = second.GetEnumerator())
                {
                    while
                    (
                        enumeratorFirst.MoveNext()
                        && enumeratorSecond.MoveNext()
                    )
                    {
                        yield return resultSelector
                        (
                            enumeratorFirst.Current,
                            enumeratorSecond.Current
                        );
                    }
                }
            }
        }

        private static IEnumerable<TSource> ExceptExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            var items = new HashSet<TSource>(second, comparer);
            foreach (var item in first)
            {
                if (items.Add(item))
                {
                    yield return item;
                }
            }
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

        private static IEnumerable<TResult> SelectExtracted<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var item in source)
            {
                yield return selector(item, count);
                count++;
            }
        }
    }
}

#endif