using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveCollection<T> : IReadOnlyCollection<T>, IExtendedReadOnlyCollection<T>, IExtendedCollection<T>, ICollection<T>
    {
        private readonly ICollection<T> _cache;
        private readonly IEqualityComparer<T> _comparer;
        private readonly IProgressor<T> _progressor;

        public ProgressiveCollection(IEnumerable<T> wrapped)
            : this(wrapped, new ExtendedSet<T>(), null)
        {
            //Empty
        }

        public ProgressiveCollection(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new ExtendedSet<T>(comparer), comparer)
        {
            //Empty
        }

        protected ProgressiveCollection(IEnumerable<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<T>(wrapped);
            _progressor.SubscribeAction(item => _cache.Add(item));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        protected ProgressiveCollection(IProgressor<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = Check.NotNullArgument(wrapped, "wrapped");
            _progressor.SubscribeAction(item => _cache.Add(item));
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public int Count
        {
            get
            {
                _progressor.TakeAll();
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        IReadOnlyCollection<T> IExtendedCollection<T>.AsReadOnly
        {
            get
            {
                return this;
            }
        }

        protected IEqualityComparer<T> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        protected IProgressor<T> Progressor
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
            _progressor.TakeAll();
            _cache.CopyTo(array);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _progressor.TakeAll();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.TakeWhile(() => _cache.Count < countLimit);
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool IExtendedCollection<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual bool CacheContains(T item)
        {
            return _cache.Contains(item, _comparer);
        }
    }
}