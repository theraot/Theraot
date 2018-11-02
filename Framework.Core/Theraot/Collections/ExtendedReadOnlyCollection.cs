// Needed for NET40

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed
#if FAT
        partial
#endif
        class ExtendedReadOnlyCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
        private readonly ICollection<T> _wrapped;

        public ExtendedReadOnlyCollection(ICollection<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            _wrapped = wrapped;
        }

        public IReadOnlyCollection<T> AsReadOnly
        {
            get { return this; }
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
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