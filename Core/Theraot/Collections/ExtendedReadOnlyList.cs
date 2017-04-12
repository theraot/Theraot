#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class ExtendedReadOnlyList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IExtendedCollection<T>, IExtendedList<T>
    {
        private readonly IList<T> _wrapped;

        public ExtendedReadOnlyList(IList<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            _wrapped = wrapped;
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        IReadOnlyCollection<T> IExtendedCollection<T>.AsReadOnly
        {
            get { return this; }
        }

        IReadOnlyList<T> IExtendedList<T>.AsReadOnly
        {
            get { return this; }
        }

        T IExtendedList<T>.this[int index]
        {
            get { return this[index]; }

            set { throw new NotSupportedException(); }
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }

            set { throw new NotSupportedException(); }
        }

        public T this[int index]
        {
            get { return _wrapped[index]; }
        }

        public bool Contains(T item)
        {
            return _wrapped.Contains(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return System.Linq.Enumerable.Contains(this, item, comparer);
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

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        bool IExtendedCollection<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.RemoveRange(int index, int count)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Reverse()
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Reverse(int index, int count)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Sort(IComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Sort(int index, int count, IComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(T item)
        {
            return _wrapped.IndexOf(item);
        }

        public void Move(int oldIndex, int newIndex)
        {
            _wrapped.Move(oldIndex, newIndex);
        }

        public void Swap(int indexA, int indexB)
        {
            _wrapped.Swap(indexA, indexB);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            var array = new T[_wrapped.Count];
            CopyTo(array);
            return array;
        }
    }
}

#endif