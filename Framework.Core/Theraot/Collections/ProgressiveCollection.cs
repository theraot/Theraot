// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public
#if FAT
        partial
#endif
        class ProgressiveCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
        private readonly ICollection<T> _cache;

        public ProgressiveCollection(IEnumerable<T> wrapped)
            : this(wrapped, new HashSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveCollection(IObservable<T> wrapped)
            : this(wrapped, new HashSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveCollection(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new HashSet<T>(comparer), comparer)
        {
            // Empty
        }

        public ProgressiveCollection(IObservable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new HashSet<T>(comparer), comparer)
        {
            // Empty
        }

        protected ProgressiveCollection(IEnumerable<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Cache = new ExtendedReadOnlyCollection<T>(_cache);
            Progressor = new Progressor<T>(wrapped);
            Progressor.SubscribeAction(obj => _cache.Add(obj));
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(IObservable<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Cache = new ExtendedReadOnlyCollection<T>(_cache);
            Progressor = new Progressor<T>(wrapped);
            Progressor.SubscribeAction(obj => _cache.Add(obj));
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(TryTake<T> tryTake, Func<bool> isDone, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Cache = new ExtendedReadOnlyCollection<T>(_cache);
            Progressor = new Progressor<T>(tryTake, isDone);
            Progressor.SubscribeAction(obj => _cache.Add(obj));
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public IReadOnlyCollection<T> Cache { get; private set; }

        public int Count
        {
            get
            {
                Progressor.Consume();
                return _cache.Count;
            }
        }

        public bool EndOfEnumeration
        {
            get { return Progressor.IsClosed; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        protected IEqualityComparer<T> Comparer { get; private set; }

        protected Progressor<T> Progressor { get; private set; }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public void Close()
        {
            Progressor.Close();
        }

        public bool Contains(T item)
        {
            if (CacheContains(item))
            {
                return true;
            }
            while (Progressor.TryTake(out T found))
            {
                if (Comparer.Equals(item, found))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(T[] array)
        {
            Progressor.Consume();
            _cache.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Progressor.Consume();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Progressor.While(() => _cache.Count < countLimit).Consume();
            Extensions.CopyTo(_cache, array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item;
            }
            {
                while (Progressor.TryTake(out T item))
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        protected virtual bool CacheContains(T item)
        {
            return _cache.Contains(item, Comparer);
        }
    }
}