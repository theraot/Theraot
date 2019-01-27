// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public class ProgressiveCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
        private readonly ICollection<T> _cache;
        private readonly IDisposable _subscription;

        public ProgressiveCollection(IEnumerable<T> enumerable)
            : this(Progressor<T>.CreateFromIEnumerable(enumerable), new List<T>(), null)
        {
            // Empty
        }

        public ProgressiveCollection(IObservable<T> observable)
            : this(Progressor<T>.CreateFromIObservable(observable), new List<T>(), null)
        {
            // Empty
        }

        protected ProgressiveCollection(Progressor<T> progressor, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Cache = Extensions.WrapAsIReadOnlyCollection(_cache);
            Progressor = progressor ?? throw new ArgumentNullException(nameof(progressor));
            _subscription = Progressor.SubscribeAction(obj => _cache.Add(obj));
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public IReadOnlyCollection<T> Cache { get; }

        protected IEqualityComparer<T> Comparer { get; }

        private Progressor<T> Progressor { get; }

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return CacheContains(item) || ProgressorWhere(Check).Any();

            bool Check(T found)
            {
                return Comparer.Equals(item, found);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Progressor.Consume();
            _cache.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get
            {
                ConsumeAll();
                return _cache.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item;
            }

            var knownCount = _cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (_cache.Count <= knownCount)
                {
                    continue;
                }

                yield return item;
                knownCount = _cache.Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~ProgressiveCollection()
        {
            Close();
        }

        public void Close()
        {
            _subscription?.Dispose();
            Progressor?.Close();
        }

        public void CopyTo(T[] array)
        {
            Progressor.Consume();
            _cache.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Progressor.While(() => _cache.Count < countLimit).Consume();
            _cache.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerable<T> Where(Predicate<T> check)
        {
            foreach (var item in _cache)
            {
                yield return item;
            }

            foreach (var p in ProgressorWhere(check))
            {
                yield return p;
            }
        }

        internal static ProgressiveCollection<T> Create<TCollection>(Progressor<T> progressor, IEqualityComparer<T> comparer)
            where TCollection : ICollection<T>, new()
        {
            return new ProgressiveCollection<T>(progressor, new TCollection(), comparer);
        }

        protected virtual bool CacheContains(T item)
        {
            return _cache.Contains(item, Comparer);
        }

        protected void ConsumeAll()
        {
            Progressor.Consume();
        }

        protected IEnumerable<T> ProgressorWhere(Predicate<T> check)
        {
            var knownCount = _cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (_cache.Count <= knownCount)
                {
                    continue;
                }

                if (check(item))
                {
                    yield return item;
                }

                knownCount = _cache.Count;
            }
        }

        protected IEnumerable<T> ProgressorWhile(Predicate<T> check)
        {
            var knownCount = _cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (_cache.Count <= knownCount)
                {
                    continue;
                }

                if (check(item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }

                knownCount = _cache.Count;
            }
        }
    }
}