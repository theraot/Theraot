#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;
using Theraot.Core;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            var _func = Check.NotNullArgument(func, "func");
            IEnumerator<TSource> enumerator = Check.NotNullArgument(source, "source").GetEnumerator();
            using (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    TSource folded = enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        folded = _func(folded, enumerator.Current);
                    }
                    return folded;
                }
                else
                {
                    throw new InvalidOperationException("No elements in source list");
                }
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            var _func = Check.NotNullArgument(func, "func");
            TAccumulate folded = seed;
            foreach (TSource item in Check.NotNullArgument(source, "source"))
            {
                folded = _func(folded, item);
            }
            return folded;
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            var _func = Check.NotNullArgument(func, "func");
            var result = seed;
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                result = _func(result, item);
            }
            return _resultSelector(result);
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var enumerator = Check.NotNullArgument(source, "source").GetEnumerator();
            using (enumerator)
            {
                while (enumerator.MoveNext())
                {
                    if (!_predicate(enumerator.Current))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var collection = _source as ICollection<TSource>;
            if (collection == null)
            {
                using (var enumerator = _source.GetEnumerator())
                {
                    return enumerator.MoveNext();
                }
            }
            else
            {
                return collection.Count > 0;
            }
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TSource item in Check.NotNullArgument(source, "source"))
            {
                if (_predicate(item))
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
            return Enumerable.CastExtracted<TResult>(Check.NotNullArgument(source, "source"));
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return Enumerable.ConcatExtracted(Check.NotNullArgument(first, "firs"), Check.NotNullArgument(second, "second"));
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return Contains(source, value, null);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            var _source = Check.NotNullArgument(source, "source");
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            foreach (var item in _source)
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
            Check.NotNullArgument(source, "source");
            var collection = source as ICollection<TSource>;
            if (collection == null)
            {
                int result = 0;
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
            else
            {
                return collection.Count;
            }
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Count(source.Where(predicate));
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        {
            TSource item = default(TSource);
            return DefaultIfEmpty(source, item);
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            return Enumerable.DefaultIfEmptyExtracted(Check.NotNullArgument(source, "source"), defaultValue);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return Distinct(source, null);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            return Enumerable.DistinctExtracted(Check.NotNullArgument(source, "source"), comparer);
        }

        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            var _source = Check.NotNullArgument(source, "source");
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "index < 0");
            }
            else
            {
                var list = _source as IList<TSource>;
                if (list != null)
                {
                    return list[index];
                }
                var readOnlyList = source as IReadOnlyList<TSource>;
                if (readOnlyList != null)
                {
                    return readOnlyList[index];
                }
                long count = 0L;
                foreach (var item in _source)
                {
                    if (index == count)
                    {
                        return item;
                    }
                    count++;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
        {
            var _source = Check.NotNullArgument(source, "source");
            if (index < 0)
            {
                return default(TSource);
            }
            else
            {
                var list = _source as IList<TSource>;
                if (list != null)
                {
                    if (index < list.Count)
                    {
                        return list[index];
                    }
                    else
                    {
                        return default(TSource);
                    }
                }
                var readOnlyList = source as IReadOnlyList<TSource>;
                if (readOnlyList != null)
                {
                    if (index < readOnlyList.Count)
                    {
                        return readOnlyList[index];
                    }
                    else
                    {
                        return default(TSource);
                    }
                }
                long count = 0L;
                foreach (var item in _source)
                {
                    if (index == count)
                    {
                        return item;
                    }
                    count++;
                }
                return default(TSource);
            }
        }

        public static IEnumerable<TResult> Empty<TResult>()
        {
            yield break;
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return Except(first, second, null);
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            var _first = Check.NotNullArgument(first, "first");
            var _second = Check.NotNullArgument(second, "second");
            var items = new HashSet<TSource>(_second, comparer);
            foreach (var item in _first)
            {
                if (items.Add(item))
                {
                    yield return item;
                }
            }
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var list = _source as IList<TSource>;
            if (list == null)
            {
                using (var enumerator = _source.GetEnumerator())
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
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                if (_predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException();
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                return item;
            }
            return default(TSource);
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return FirstOrDefault(source.Where(predicate));
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return GroupBy(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return source.GroupBy(keySelector, elementSelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return Check.NotNullArgument(source, "source").ToLookup(Check.NotNullArgument(keySelector, "keySelector"), Check.NotNullArgument(elementSelector, "elementSelector"), comparer ?? EqualityComparer<TKey>.Default);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return source.GroupBy(keySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return Select
                   (
                       ToLookup
                       (
                           Check.NotNullArgument(source, "source"),
                           Check.NotNullArgument(keySelector, "keySelector"),
                           comparer ?? EqualityComparer<TKey>.Default
                       ),
                       grouping => resultSelector(grouping.Key, grouping)
                   );
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return source.GroupBy(keySelector, elementSelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return Select
                   (
                       ToLookup
                       (
                           Check.NotNullArgument(source, "source"),
                           Check.NotNullArgument(keySelector, "keySelector"),
                           Check.NotNullArgument(elementSelector, "elementSelector"),
                           comparer ?? EqualityComparer<TKey>.Default
                       ),
                       grouping => Check.NotNullArgument(resultSelector, "resultSelector")(grouping.Key, grouping)
                   );
        }

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            var _outerKeySelector = Check.NotNullArgument(outerKeySelector, "outerKeySelector");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            var lookup = Check.NotNullArgument(inner, "inner").ToLookup(Check.NotNullArgument(innerKeySelector, "innerKeySelector"), comparer);
            return Select(Check.NotNullArgument(outer, "outer"), outerItem => _resultSelector(outerItem, lookup[_outerKeySelector(outerItem)]));
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return Intersect(first, second, null);
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            comparer = comparer ?? EqualityComparer<TSource>.Default;
            var _first = Check.NotNullArgument(first, "first");
            var _second = Check.NotNullArgument(second, "second");
            var items = new HashSet<TSource>(_second, comparer);
            foreach (TSource element in _first)
            {
                if (items.Remove(element))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            var _outerKeySelector = Check.NotNullArgument(outerKeySelector, "outerKeySelector");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            ILookup<TKey, TInner> lookup = Check.NotNullArgument(inner, "inner").ToLookup(Check.NotNullArgument(innerKeySelector, "innerKeySelector"), comparer);
            return SelectMany(Check.NotNullArgument(outer, "outer"), outerItem => lookup[_outerKeySelector.Invoke(outerItem)], _resultSelector);
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var collection = _source as ICollection<TSource>;
            if (collection != null && collection.Count == 0)
            {
                throw new InvalidOperationException();
            }
            else
            {
                var list = _source as IList<TSource>;
                if (list == null)
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
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    return list[list.Count - 1];
                }
            }
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var found = false;
            var result = default(TSource);
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                if (!_predicate(item))
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
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var list = _source as IList<TSource>;
            if (list == null)
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
                else
                {
                    return default(TSource);
                }
            }
            else
            {
                return list.Count > 0 ? list[list.Count - 1] : default(TSource);
            }
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var found = false;
            var result = default(TSource);
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                if (!_predicate(item))
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
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var array = _source as TSource[];
            if (array == null)
            {
                long count = 0;
                using (var item = _source.GetEnumerator())
                {
                    while (item.MoveNext())
                    {
                        count++;
                    }
                }
                return count;
            }
            else
            {
                return array.LongLength;
            }
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return LongCount(source.Where(predicate));
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            foreach (object item in Check.NotNullArgument(source, "source"))
            {
                if (item is TResult)
                {
                    yield return (TResult)item;
                }
            }
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.OrderBy(keySelector, null);
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.OrderByDescending(keySelector, null);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer.Reverse());
        }

        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "count < 0");
            }
            else
            {
                return Enumerable.RepeatExtracted(element, count);
            }
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            return ReverseExtracted(Check.NotNullArgument(source, "source"));
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            else
            {
                return Select(source, (item, i) => selector(item));
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            return Enumerable.SelectExtracted(Check.NotNullArgument(source, "source"), Check.NotNullArgument(selector, "selector"));
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            var _selector = Check.NotNullArgument(selector, "selector");
            foreach (TSource key in Check.NotNullArgument(source, "source"))
            {
                foreach (TResult item in _selector(key))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            var _selector = Check.NotNullArgument(selector, "selector");
            int count = 0;
            foreach (TSource key in Check.NotNullArgument(source, "source"))
            {
                foreach (TResult item in _selector(key, count))
                {
                    yield return item;
                }
                count++;
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            var _collectionSelector = Check.NotNullArgument(collectionSelector, "collectionSelector");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            foreach (TSource element in Check.NotNullArgument(source, "source"))
            {
                foreach (TCollection collection in _collectionSelector(element))
                {
                    yield return _resultSelector(element, collection);
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            var _collectionSelector = Check.NotNullArgument(collectionSelector, "collectionSelector");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            int count = 0;
            foreach (TSource element in Check.NotNullArgument(source, "source"))
            {
                foreach (TCollection collection in _collectionSelector(element, count))
                {
                    yield return _resultSelector(element, collection);
                }
                count++;
            }
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }
            using (IEnumerator<TSource> first_enumerator = first.GetEnumerator(), second_enumerator = second.GetEnumerator())
            {
                while (first_enumerator.MoveNext())
                {
                    if (!second_enumerator.MoveNext())
                    {
                        return false;
                    }
                    if (!comparer.Equals(first_enumerator.Current, second_enumerator.Current))
                    {
                        return false;
                    }
                }
                return !second_enumerator.MoveNext();
            }
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            var found = false;
            var result = default(TSource);
            foreach (var item in Check.NotNullArgument(source, "source"))
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
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var found = false;
            var result = default(TSource);
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                if (!_predicate(item))
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
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            var found = false;
            var result = default(TSource);
            foreach (var item in Check.NotNullArgument(source, "source"))
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
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var found = false;
            var result = default(TSource);
            foreach (var item in Check.NotNullArgument(source, "source"))
            {
                if (!_predicate(item))
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
            return SkipWhile(source, (item, i) => i < count);
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            return SkipWhile(source, (item, i) => _predicate(item));
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            return Enumerable.SkipWhileExtracted(Check.NotNullArgument(source, "source"), Check.NotNullArgument(predicate, "predicate"));
        }

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            return TakeWhile(source, (item, index) => index < count);
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            return TakeWhile(source, (item, i) => _predicate(item));
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            return Check.NotNullArgument(source, "source").TakeWhileExtracted(Check.NotNullArgument(predicate, "predicate"));
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ThenBy(keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return Check.NotNullArgument(source, "source").CreateOrderedEnumerable(keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ThenByDescending(keySelector, null);
        }

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return Check.NotNullArgument(source, "source").CreateOrderedEnumerable(keySelector, comparer, true);
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            return ToList(source).ToArray();
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToDictionary(source, keySelector, elementSelector, null);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _elementSelector = Check.NotNullArgument(elementSelector, "elementSelector");
            var _keySelector = Check.NotNullArgument(keySelector, "keySelector");
            comparer = comparer ?? EqualityComparer<TKey>.Default;
            var result = new Dictionary<TKey, TElement>(comparer);
            foreach (var item in _source)
            {
                result.Add(_keySelector(item), _elementSelector(item));
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
            return new List<TSource>(Check.NotNullArgument(source, "source"));
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
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            return Where(source, (item, i) => _predicate(item));
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            return Enumerable.WhereExtracted(Check.NotNullArgument(source, "source"), Check.NotNullArgument(predicate, "predicate"));
        }

        private static IEnumerable<TResult> CastExtracted<TResult>(IEnumerable source)
        {
            foreach (object obj in source)
            {
                yield return (TResult)obj;
            }
        }

        private static IEnumerable<TSource> ConcatExtracted<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            foreach (TSource item in first)
            {
                yield return item;
            }
            var enumerator = second.GetEnumerator();
            using (enumerator)
            {
                while (enumerator.MoveNext())
                {
                    TSource current = enumerator.Current;
                    yield return current;
                }
            }
        }

        private static IEnumerable<TSource> DefaultIfEmptyExtracted<TSource>(IEnumerable<TSource> source, TSource defaultValue)
        {
            var enumerator = source.GetEnumerator();
            using (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    while (true)
                    {
                        yield return enumerator.Current;
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                    }
                }
                else
                {
                    yield return defaultValue;
                }
            }
        }

        private static IEnumerable<TSource> DistinctExtracted<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            var found = new Dictionary<TSource, object>(comparer);
            bool foundNull = false;
            foreach (var item in source)
            {
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

        private static IEnumerable<TResult> RepeatExtracted<TResult>(TResult element, int count)
        {
            for (int index = 0; index < count; index++)
            {
                yield return element;
            }
        }

        private static IEnumerable<TSource> ReverseExtracted<TSource>(IEnumerable<TSource> source)
        {
            var stack = new Stack<TSource>();
            foreach (TSource item in source)
            {
                stack.Push(item);
            }
            foreach (TSource item in stack)
            {
                yield return item;
            }
        }

        private static IEnumerable<TResult> SelectExtracted<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            int count = 0;
            foreach (TSource item in source)
            {
                yield return selector(item, count);
                count++;
            }
        }

        private static IEnumerable<TSource> SkipWhileExtracted<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            IEnumerator<TSource> enumerator = source.GetEnumerator();
            using (enumerator)
            {
                int count = 0;
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
                    else
                    {
                        count++;
                    }
                }
            }
        }

        private static IEnumerable<TSource> TakeWhileExtracted<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            int count = 0;
            foreach (TSource item in source)
            {
                if (!predicate(item, count))
                {
                    break;
                }
                yield return item;
                count++;
            }
        }

        private static IEnumerable<TSource> WhereExtracted<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            int count = 0;
            foreach (TSource item in source)
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
}

#endif