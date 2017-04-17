#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedStack<T> : IDropPoint<T>, IEnumerable<T>, ICollection<T>, ICloneable<ExtendedStack<T>>
    {
        private readonly IReadOnlyCollection<T> _readOnly;
        private readonly Stack<T> _wrapped;

        public ExtendedStack()
        {
            _wrapped = new Stack<T>();
            _readOnly = new ExtendedReadOnlyCollection<T>(this);
        }

        public ExtendedStack(IEnumerable<T> collection)
        {
            _wrapped = new Stack<T>(collection);
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
            _wrapped.Push(item);
            return true;
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedStack<T> Clone()
        {
            return new ExtendedStack<T>(this);
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
            _wrapped.Push(item);
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
                item = _wrapped.Pop();
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
            if (EqualityComparer<T>.Default.Equals(item, _wrapped.Peek()))
            {
                _wrapped.Pop();
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