// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
#if FAT
    public sealed class ExtendedSet<T> : IExtendedSet<T>, ICollection<T>, ISet<T>, ICloneable<ExtendedSet<T>>
    {
        private readonly IReadOnlySet<T> _readOnly;
        private readonly HashSet<T> _wrapped;

        public ExtendedSet()
        {
            _wrapped = new HashSet<T>();
            _readOnly = new ExtendedReadOnlySet<T>(this);
        }

        public ExtendedSet(IEnumerable<T> prototype)
        {
            _wrapped = new HashSet<T>();
            _readOnly = new ExtendedReadOnlySet<T>(this);
            this.AddRange(prototype);
        }

        public ExtendedSet(IEnumerable<T> prototype, IEqualityComparer<T> comparer)
        {
            _wrapped = new HashSet<T>(comparer);
            _readOnly = new ExtendedReadOnlySet<T>(this);
            this.AddRange(prototype);
        }

        public ExtendedSet(IEqualityComparer<T> comparer)
        {
            _readOnly = new ExtendedReadOnlySet<T>(this);
            _wrapped = new HashSet<T>(comparer);
        }

        public IReadOnlySet<T> AsReadOnly
        {
            get { return _readOnly; }
        }

#else
    public sealed class ExtendedSet<T> : ICloneable, ICollection<T>, ISet<T>
    {
        private readonly HashSet<T> _wrapped;

        public ExtendedSet()
        {
            _wrapped = new HashSet<T>();
        }

        public ExtendedSet(IEnumerable<T> prototype)
        {
            _wrapped = new HashSet<T>();
            this.AddRange(prototype);
        }

        public ExtendedSet(IEnumerable<T> prototype, IEqualityComparer<T> comparer)
        {
            _wrapped = new HashSet<T>(comparer);
            this.AddRange(prototype);
        }

        public ExtendedSet(IEqualityComparer<T> comparer)
        {
            _wrapped = new HashSet<T>(comparer);
        }

#endif

        public int Count
        {
            get { return _wrapped.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Add(T item)
        {
            return _wrapped.Add(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedSet<T> Clone()
        {
            return new ExtendedSet<T>(this);
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
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            _wrapped.CopyTo(array, arrayIndex, countLimit);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            Extensions.ExceptWith(this, other);
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
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _wrapped.IntersectWith(other);
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

        public bool Remove(T item)
        {
            return _wrapped.Remove(item);
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }
            foreach (var foundItem in _wrapped.RemoveWhereEnumerable(input => comparer.Equals(input, item)))
            {
                GC.KeepAlive(foundItem);
                return true;
            }
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _wrapped.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _wrapped.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _wrapped.UnionWith(other);
        }
    }
}