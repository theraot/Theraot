#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class ExtendedReadOnlySet<T> : IReadOnlySet<T>, IExtendedReadOnlySet<T>, IReadOnlyCollection<T>, IEnumerable<T>, ISet<T>, IExtendedSet<T>, ICollection<T>, IExtendedCollection<T>
    {
        private readonly ISet<T> _wrapped;

        public ExtendedReadOnlySet(ISet<T> wrapped)
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

        IReadOnlySet<T> IExtendedSet<T>.AsReadOnly
        {
            get { return this; }
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

        bool IExtendedSet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool IExtendedSet<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _wrapped.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _wrapped.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _wrapped.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _wrapped.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _wrapped.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _wrapped.SetEquals(other);
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