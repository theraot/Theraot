// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
#if FAT
    public sealed class ExtendedSet<T> : ISet<T>, Core.ICloneable<ExtendedSet<T>>
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

#else
    public sealed class ExtendedSet<T> : ISet<T>
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
        , ICloneable
#endif
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

        public int Count => _wrapped.Count;

        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            return _wrapped.Add(item);
        }

        void ICollection<T>.Add(T item)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Add(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedSet<T> Clone()
        {
            return new ExtendedSet<T>(this);
        }

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

        object ICloneable.Clone()
        {
            return Clone();
        }

#endif

        public bool Contains(T item)
        {
            return _wrapped.Contains(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return Enumerable.Contains(_wrapped, item, comparer);
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

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
#endif

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