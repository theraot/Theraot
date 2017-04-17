#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedQueue<T> : IDropPoint<T>, IEnumerable<T>, ICollection<T>, ICloneable<ExtendedQueue<T>>
    {
        private readonly IReadOnlyCollection<T> _readOnly;
        private readonly Queue<T> _wrapped;

        public ExtendedQueue()
        {
            _wrapped = new Queue<T>();
            _readOnly = new ExtendedReadOnlyCollection<T>(this);
        }

        public ExtendedQueue(IEnumerable<T> collection)
        {
            _wrapped = new Queue<T>(collection);
            _readOnly = new ExtendedReadOnlyCollection<T>(this);
        }

        public IReadOnlyCollection<T> AsReadOnly
        {
            get { return _readOnly; }
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public T Item
        {
            get { return _wrapped.Peek(); }
        }

        public bool Add(T item)
        {
            _wrapped.Enqueue(item);
            return true;
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedQueue<T> Clone()
        {
            return new ExtendedQueue<T>(this);
        }

        public bool Contains(T item)
        {
            return _wrapped.Contains(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return System.Linq.Enumerable.Contains(_wrapped, item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array)
        {
            _wrapped.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

#if !NETCOREAPP1_1
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        void ICollection<T>.Add(T item)
        {
            _wrapped.Enqueue(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryTake(out T item)
        {
            try
            {
                item = _wrapped.Dequeue();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

        private bool Remove(T item)
        {
            return Remove(item, EqualityComparer<T>.Default);
        }

        private bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (comparer.Equals(item, _wrapped.Peek()))
            {
                _wrapped.Dequeue();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

#endif