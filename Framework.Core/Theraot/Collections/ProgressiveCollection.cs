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
        private readonly IEqualityComparer<T> _comparer;
        private readonly Progressor<T> _progressor;

        public ProgressiveCollection(IEnumerable<T> wrapped)
            : this(wrapped, new HashSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveCollection(Progressor<T> wrapped)
            : this(wrapped, new HashSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveCollection(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new HashSet<T>(comparer), comparer)
        {
            // Empty
        }

        public ProgressiveCollection(Progressor<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new HashSet<T>(comparer), comparer)
        {
            // Empty
        }

        protected ProgressiveCollection(IEnumerable<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
            _progressor = new Progressor<T>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(obj));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(Progressor<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            if (wrapped == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
            _progressor = new Progressor<T>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(obj));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(TryTake<T> tryTake, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
            _progressor = new Progressor<T>(tryTake, false); // false because the underlaying structure may change
            _progressor.SubscribeAction(obj => _cache.Add(obj));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public int Count
        {
            get
            {
                _progressor.AsEnumerable().Consume();
                return _cache.Count;
            }
        }

        public bool EndOfEnumeration
        {
            get { return _progressor.IsClosed; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        protected IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
        }

        protected Progressor<T> Progressor
        {
            get { return _progressor; }
        }

        public void Close()
        {
            _progressor.Close();
        }

        public bool Contains(T item)
        {
            if (CacheContains(item))
            {
                return true;
            }
            T found;
            while (_progressor.TryTake(out found))
            {
                if (_comparer.Equals(item, found))
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
            _progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.While(() => _cache.Count < countLimit).Consume();
            _cache.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item;
            }
            {
                T item;
                while (_progressor.TryTake(out item))
                {
                    yield return item;
                }
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
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
            return _cache.Contains(item, _comparer);
        }
    }
}