#if LESSTHAN_NET35

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
// ReSharper disable LoopCanBeConvertedToQuery

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
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("No elements in source list");
                }

                var folded = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    folded = func(folded, enumerator.Current);
                }
                return folded;
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
            if (source is ICollection<TSource> collection)
            {
                return collection.Count > 0;
            }

            using (var enumerator = source.GetEnumerator())
            {
                return enumerator.MoveNext();
            }
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

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is ICollection<TSource> collection)
            {
                return collection.Count;
            }
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

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Count(source.Where(predicate));
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        {
            var item = default(TSource)!;
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

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return DistinctExtracted();

            IEnumerable<TSource> DistinctExtracted()
            {
                var found = new Dictionary<TSource, object?>(comparer);
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

        public static IEnumerable<TResult> Empty<TResult>()
        {
            yield break;
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is TSource[] array)
            {
                return array.LongLength;
            }
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

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
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

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
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

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (comparer == null)
            {
                return SequenceEqualExtracted(EqualityComparer<TSource>.Default);
            }
            return SequenceEqualExtracted(comparer);

            bool SequenceEqualExtracted(IEqualityComparer<TSource> nonNullComparer)
            {
                using (IEnumerator<TSource> firstEnumerator = first.GetEnumerator(), secondEnumerator = second.GetEnumerator())
                {
                    while (firstEnumerator.MoveNext())
                    {
                        if (!secondEnumerator.MoveNext())
                        {
                            return false;
                        }
                        if (!nonNullComparer.Equals(firstEnumerator.Current, secondEnumerator.Current))
                        {
                            return false;
                        }
                    }
                    return !secondEnumerator.MoveNext();
                }
            }
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenBy(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (comparer == null)
            {
                return source.CreateOrderedEnumerable(keySelector, Comparer<TKey>.Default, false);
            }
            return source.CreateOrderedEnumerable(keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenByDescending(source, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (comparer == null)
            {
                return source.CreateOrderedEnumerable(keySelector, Comparer<TKey>.Default, true);
            }
            return source.CreateOrderedEnumerable(keySelector, comparer, true);
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case ICollection<TSource> collection:
                    {
                        var result = new TSource[collection.Count];
                        collection.CopyTo(result, 0);
                        return result;
                    }
                case string str:
                    return (TSource[])(object)str.ToCharArray();

                default:
                    return new List<TSource>(source).ToArray();
            }
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToDictionary(source, keySelector, elementSelector, null);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
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
            comparer ??= EqualityComparer<TKey>.Default;
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

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        {
            return ToDictionary(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!(source is string str))
            {
                return new List<TSource>(source);
            }

            var array = (TSource[])(object)str.ToCharArray();
            var result = new List<TSource>(array.Length);
            result.AddRange(array);
            return result;
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

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
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
    }
}

#endif