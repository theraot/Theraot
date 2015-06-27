// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class ProgressiveCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
        private readonly ICollection<T> _cache;
        private readonly IEqualityComparer<T> _comparer;
        private readonly Progressor<T> _progressor;

        public ProgressiveCollection(IEnumerable<T> wrapped)
            : this(wrapped, new HashSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveCollection(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new HashSet<T>(comparer), comparer)
        {
            // Empty
        }

        protected ProgressiveCollection(IEnumerable<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<T>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(obj));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(Progressor<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<T>(Check.NotNullArgument(wrapped, "wrapped"));
            _progressor.SubscribeAction(obj => _cache.Add(obj));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(TryTake<T> tryTake, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<T>(tryTake);
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
            get
            {
                return _progressor.IsClosed;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns True")]
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        protected IEqualityComparer<T> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        protected Progressor<T> Progressor
        {
            get
            {
                return _progressor;
            }
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
            else
            {
                T _item;
                while (_progressor.TryTake(out _item))
                {
                    if (_comparer.Equals(item, _item))
                    {
                        return true;
                    }
                }
                return false;
            }
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
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