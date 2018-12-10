using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    internal sealed class GroupBuilder<TKey, TSource, TElement>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly SafeQueue<Grouping<TKey, TElement>> _results;
        private readonly Func<TSource, TElement> _resultSelector;
        private IEnumerator<TSource> _enumerator;
        private NullAwareDictionary<TKey, ProxyObservable<TElement>> _proxies;

        private GroupBuilder(IEnumerator<TSource> enumerator, IEqualityComparer<TKey> comparer, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
        {
            _proxies = new NullAwareDictionary<TKey, ProxyObservable<TElement>>(comparer);
            _enumerator = enumerator;
            _results = new SafeQueue<Grouping<TKey, TElement>>();
            _keySelector = keySelector;
            _resultSelector = resultSelector;
        }

        public static IEnumerable<IGrouping<TKey, TElement>> CreateGroups(IEnumerable<TSource> source, IEqualityComparer<TKey> comparer, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
        {
            var builder = new GroupBuilder<TKey, TSource, TElement>(source.GetEnumerator(), comparer, keySelector, resultSelector);
            return builder.GetGroups();
        }

        private void Add(TKey key, ICollection<TElement> items, ProxyObservable<TElement> proxy)
        {
            var result = new Grouping<TKey, TElement>(key, items);
            _proxies.Add(key, proxy);
            _results.Add(result);
        }

        private void Advance()
        {
            var enumerator = Volatile.Read(ref _enumerator);
            if (enumerator != null && !MoveNext(enumerator))
            {
                Dispose();
            }
        }

        private void Dispose()
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

        private IEnumerable<IGrouping<TKey, TElement>> GetGroups()
        {
            try
            {
                var enumerator = _enumerator;
                while (true)
                {
                    var advanced = MoveNext(enumerator);
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

        private IEnumerable<Grouping<TKey, TElement>> GetPendingGroups()
        {
            while (_results.TryTake(out var result))
            {
                yield return result;
            }
        }

        private bool MoveNext(IEnumerator<TSource> enumerator)
        {
            lock (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    ProcessElement(enumerator.Current);
                    return true;
                }
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
                var progressor = Progressor<TElement>.CreateFromIObservable(proxy, Advance);
                var items = ProgressiveCollection<TElement>.Create<SafeCollection<TElement>>(
                    progressor,
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