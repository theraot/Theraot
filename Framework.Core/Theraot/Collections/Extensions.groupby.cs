using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;

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
                var iterator = new Iterator<TKey, TSource, TSource>(comparer, source.GetEnumerator());
                try
                {
                    while (true)
                    {
                        var advanced = Advance(iterator);
                        foreach (var pendingResult in iterator.GetPendingResults())
                        {
                            yield return pendingResult;
                        }
                        if (!advanced)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    iterator.Dispose();
                }
            }
            void ProcessElement(TSource element, Iterator<TKey, TSource, TSource> iterator)
            {
                var key = keySelector(element);
                if (!iterator.TryGetProxy(key, out var proxy))
                {
                    proxy = new ProxyObservable<TSource>();
                    var collection = ProgressiveCollection<TSource>.Create<SafeCollection<TSource>>
                    (
                        proxy,
                        () => { Advance(iterator); },
                        EqualityComparer<TSource>.Default
                    );
                    var result = new Grouping<TKey, TSource>(key, collection);
                    iterator.AddProxy(key, proxy);
                    iterator.AddResult(result);
                }
                proxy.OnNext(element);
            }
            bool Advance(Iterator<TKey, TSource, TSource> iterator)
            {
                if (iterator.MoveNext())
                {
                    var element = iterator.Current;
                    ProcessElement(element, iterator);
                    return true;
                }
                iterator.Dispose();
                return false;
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
                var iterator = new Iterator<TKey, TSource, TElement>(comparer, source.GetEnumerator());
                try
                {
                    while (true)
                    {
                        var advanced = Advance(iterator);
                        foreach (var pendingResult in iterator.GetPendingResults())
                        {
                            yield return pendingResult;
                        }
                        if (!advanced)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    iterator.Dispose();
                }
            }
            void ProcessElement(TSource item, Iterator<TKey, TSource, TElement> iterator)
            {
                var key = keySelector(item);
                var element = resultSelector(item);
                if (!iterator.TryGetProxy(key, out var proxy))
                {
                    proxy = new ProxyObservable<TElement>();
                    var collection = ProgressiveCollection<TElement>.Create<SafeCollection<TElement>>
                    (
                        proxy,
                        () => { Advance(iterator); },
                        EqualityComparer<TElement>.Default
                    );
                    var result = new Grouping<TKey, TElement>(key, collection);
                    iterator.AddProxy(key, proxy);
                    iterator.AddResult(result);
                }
                proxy.OnNext(element);
            }
            bool Advance(Iterator<TKey, TSource, TElement> iterator)
            {
                if (iterator.MoveNext())
                {
                    var element = iterator.Current;
                    ProcessElement(element, iterator);
                    return true;
                }
                iterator.Dispose();
                return false;
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

        private sealed class Iterator<TKey, TSource, TElement> : IEnumerator<TSource>
        {
            private readonly SafeQueue<Grouping<TKey, TElement>> _results;
            private IEnumerator<TSource> _enumerator;
            private NullAwareDictionary<TKey, ProxyObservable<TElement>> _proxies;
            private TSource _current;

            public Iterator(IEqualityComparer<TKey> comparer, IEnumerator<TSource> enumerator)
            {
                _proxies = new NullAwareDictionary<TKey, ProxyObservable<TElement>>(comparer);
                _enumerator = enumerator;
                _results = new SafeQueue<Grouping<TKey, TElement>>();
            }

            public TSource Current => _enumerator.Current;

            object IEnumerator.Current => _current;

            public void AddProxy(TKey key, ProxyObservable<TElement> proxy)
            {
                _proxies.Add(key, proxy);
            }

            public void AddResult(Grouping<TKey, TElement> result)
            {
                _results.Add(result);
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref _enumerator, null)?.Dispose();
                var proxies = Interlocked.Exchange(ref _proxies, null);
                if (proxies != null)
                {
                    foreach (var group in proxies)
                    {
                        group.Value.OnCompleted();
                    }
                }
            }

            public IEnumerable<Grouping<TKey, TElement>> GetPendingResults()
            {
                while (_results.TryTake(out var result))
                {
                    yield return result;
                }
            }

            public bool MoveNext()
            {
                var enumerator = Volatile.Read(ref _enumerator);
                if (enumerator != null && enumerator.MoveNext())
                {
                    _current = enumerator.Current;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                Volatile.Read(ref _enumerator)?.Reset();
            }

            public bool TryGetProxy(TKey key, out ProxyObservable<TElement> proxy)
            {
                return _proxies.TryGetValue(key, out proxy);
            }
        }
    }
}