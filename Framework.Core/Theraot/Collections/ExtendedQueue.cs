#if FAT

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedQueue<T> : ICollection<T>, ICloneable<ExtendedQueue<T>>, IProducerConsumerCollection<T>
    {
        private readonly Queue<T> _wrapped;

        public ExtendedQueue()
        {
            _wrapped = new Queue<T>();
        }

        public ExtendedQueue(IEnumerable<T> collection)
        {
            _wrapped = new Queue<T>(collection);
        }

        public int Count => _wrapped.Count;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        public T Item => _wrapped.Peek();

        object ICollection.SyncRoot => throw new NotSupportedException();

        void ICollection<T>.Add(T item)
        {
            _wrapped.Enqueue(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedQueue<T> Clone()
        {
            return new ExtendedQueue<T>(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
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

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(_wrapped.Count, array, index);
            Extensions.DeprecatedCopyTo(_wrapped, array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        public T[] ToArray()
        {
            return _wrapped.ToArray();
        }

        public bool TryAdd(T item)
        {
            _wrapped.Enqueue(item);
            return true;
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
                item = default;
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
            return false;
        }
    }
}

#endif