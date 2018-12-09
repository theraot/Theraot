using System;
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
            var groupBuilder = new GroupBuilder<TKey, TSource, TSource>(comparer, source.GetEnumerator(), keySelector, item => item);
            return groupBuilder.GetGroups();
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
            var groupBuilder = new GroupBuilder<TKey, TSource, TElement>(comparer, source.GetEnumerator(), keySelector, resultSelector);
            return groupBuilder.GetGroups();
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

        private sealed class GroupBuilder<TKey, TSource, TElement> : IDisposable
        {
            private readonly Func<TSource, TKey> _keySelector;
            private readonly SafeQueue<Grouping<TKey, TElement>> _results;
            private readonly Func<TSource, TElement> _resultSelector;
            private IEnumerator<TSource> _enumerator;
            private NullAwareDictionary<TKey, ProxyObservable<TElement>> _proxies;
            private TSource _current;

            public GroupBuilder(IEqualityComparer<TKey> comparer, IEnumerator<TSource> enumerator, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
            {
                _proxies = new NullAwareDictionary<TKey, ProxyObservable<TElement>>(comparer);
                _enumerator = enumerator;
                _results = new SafeQueue<Grouping<TKey, TElement>>();
                _keySelector = keySelector;
                _resultSelector = resultSelector;
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

            public IEnumerable<IGrouping<TKey, TElement>> GetGroups()
            {
                try
                {
                    while (true)
                    {
                        var advanced = Advance();
                        foreach (var pendingResult in GetPendingGroups())
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
                    Dispose();
                }
            }

            private void Add(TKey key, ICollection<TElement> items, ProxyObservable<TElement> proxy)
            {
                var result = new Grouping<TKey, TElement>(key, items);
                _proxies.Add(key, proxy);
                _results.Add(result);
            }

            private bool Advance()
            {
                if (MoveNext())
                {
                    var element = _current;
                    ProcessElement(element);
                    return true;
                }
                Dispose();
                return false;
            }

            private IEnumerable<Grouping<TKey, TElement>> GetPendingGroups()
            {
                while (_results.TryTake(out var result))
                {
                    yield return result;
                }
            }

            private bool MoveNext()
            {
                var enumerator = Volatile.Read(ref _enumerator);
                if (enumerator != null && enumerator.MoveNext())
                {
                    _current = enumerator.Current;
                    return true;
                }
                return false;
            }

            private void ProcessElement(TSource item)
            {
                var key = _keySelector(item);
                var element = _resultSelector(item);
                if (!TryGetProxy(key, out var proxy))
                {
                    proxy = new ProxyObservable<TElement>();
                    var items = ProgressiveCollection<TElement>.Create<SafeCollection<TElement>>
                    (
                        proxy,
                        () => { Advance(); },
                        EqualityComparer<TElement>.Default
                    );
                    Add(key, items, proxy);
                }
                proxy.OnNext(element);
            }

            private bool TryGetProxy(TKey key, out ProxyObservable<TElement> proxy)
            {
                return _proxies.TryGetValue(key, out proxy);
            }
        }
    }
}