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
    public sealed class ExtendedStack<T> : ICollection<T>, ICloneable<ExtendedStack<T>>, IProducerConsumerCollection<T>
    {
        private readonly Stack<T> _wrapped;

        public ExtendedStack()
        {
            _wrapped = new Stack<T>();
        }

        public ExtendedStack(IEnumerable<T> collection)
        {
            _wrapped = new Stack<T>(collection);
        }

        public int Count => _wrapped.Count;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        public T Item => _wrapped.Peek();

        object ICollection.SyncRoot => throw new NotSupportedException();

        void ICollection<T>.Add(T item)
        {
            _wrapped.Push(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedStack<T> Clone()
        {
            return new ExtendedStack<T>(this);
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
            Extensions.CanCopyTo(Count, array, index);
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
            return Extensions.ToArray(this, Count);
        }

        public bool TryAdd(T item)
        {
            _wrapped.Push(item);
            return true;
        }

        public bool TryTake(out T item)
        {
            try
            {
                item = _wrapped.Pop();
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
            if (EqualityComparer<T>.Default.Equals(item, _wrapped.Peek()))
            {
                _wrapped.Pop();
                return true;
            }
            return false;
        }
    }
}

#endif