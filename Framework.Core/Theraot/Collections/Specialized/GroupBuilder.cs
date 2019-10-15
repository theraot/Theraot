using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    internal sealed class GroupBuilder<TKey, TSource, TElement> : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly ThreadSafeDictionary<TKey, ProxyObservable<TElement>> _proxies;
        private readonly ThreadSafeQueue<Grouping<TKey, TElement>> _results;
        private readonly Func<TSource, TElement> _resultSelector;
        private IEnumerator<TSource>? _enumerator;

        private GroupBuilder(IEnumerable<TSource> source, IEqualityComparer<TKey> comparer, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
        {
            _enumerator = source.GetEnumerator();
            _results = new ThreadSafeQueue<Grouping<TKey, TElement>>();
            _proxies = new ThreadSafeDictionary<TKey, ProxyObservable<TElement>>(comparer);
            _keySelector = keySelector;
            _resultSelector = resultSelector;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public static IEnumerable<IGrouping<TKey, TElement>> CreateGroups(IEnumerable<TSource> source, IEqualityComparer<TKey> comparer, Func<TSource, TKey> keySelector, Func<TSource, TElement> resultSelector)
        {
            using (var instance = new GroupBuilder<TKey, TSource, TElement>(source, comparer, keySelector, resultSelector))
            {
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
                var items = ProgressiveCollection<TElement>.Create<ThreadSafeCollection<TElement>>
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

        public void Dispose()
        {
            _enumerator?.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}