using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    internal class GroupBuilder<TKey, TSource, TElement>
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private IEnumerator<TSource> _enumerator;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly SafeDictionary<TKey, ProxyObservable<TElement>> _proxies;
        private readonly SafeQueue<Grouping<TKey, TElement>> _results;
        private readonly Func<TSource, TElement> _resultSelector;

        private GroupBuilder(IEnumerable<TSource> source, IEqualityComparer<TKey> comparer, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
        {
            _enumerator = source.GetEnumerator();
            _results = new SafeQueue<Grouping<TKey, TElement>>();
            _proxies = new SafeDictionary<TKey, ProxyObservable<TElement>>(comparer);
            _keySelector = keySelector;
            _resultSelector = resultSelector;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public static IEnumerable<IGrouping<TKey, TElement>> CreateGroups(IEnumerable<TSource> source, IEqualityComparer<TKey> comparer, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
        {
            var instance = new GroupBuilder<TKey, TSource, TElement>(source, comparer, keySelector, resultSelector);
            bool advanced;
            do
            {
                advanced = instance.MoveNext();
                while (instance.GetPendingResults(out var pendingResult))
                {
                    yield return pendingResult;
                }
            } while (advanced);
        }

        private void Advance()
        {
            MoveNext();
        }

        private bool GetPendingResults(out Grouping<TKey, TElement> pendingResult)
        {
            return _results.TryTake(out pendingResult);
        }

        private bool MoveNext()
        {
            TSource item;
            if (_enumerator == null)
            {
                return false;
            }
            lock (_enumerator)
            {
                if (!_enumerator.MoveNext())
                {
                    _enumerator.Dispose();
                    _cancellationTokenSource.Cancel();
                    _enumerator = null;
                    return false;
                }
                item = _enumerator.Current;
            }
            var key = _keySelector(item);
            var element = _resultSelector(item);
            if (_proxies.TryGetOrAdd(key, _ => new ProxyObservable<TElement>(), out var proxy))
            {
                var progressor = Progressor<TElement>.CreateFromIObservable
                (
                    proxy,
                    Advance,
                    _cancellationTokenSource.Token
                );
                var items = ProgressiveCollection<TElement>.Create<SafeCollection<TElement>>
                (
                    progressor,
                    EqualityComparer<TElement>.Default
                );
                var result = new Grouping<TKey, TElement>(key, items);
                _results.Add(result);
            }
            proxy.OnNext(element);
            return true;
        }
    }
}