// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class ProgressiveSet<T> : ISet<T>, IHasComparer<T>, IProgressive<T>, IClosable
    {
        private readonly ProgressiveCollection<T> _wrapped;

        private ProgressiveSet(Progressor<T> progressor, ISet<T> cache, IEqualityComparer<T>? comparer)
        {
            Comparer = comparer ?? EqualityComparer<T>.Default;
            _wrapped = ProgressiveCollection<T>.Create(progressor, cache, Comparer);
        }

        public ICollection<T> Cache => _wrapped.Cache;
        public IEqualityComparer<T> Comparer { get; }
        public int Count => _wrapped.Count;
        public bool IsClosed => _wrapped.IsClosed;
        bool ICollection<T>.IsReadOnly => true;

        public static ProgressiveSet<T> Create(Progressor<T> progressor, ISet<T> cache, IEqualityComparer<T>? comparer)
        {
            return new ProgressiveSet<T>(progressor, cache, comparer);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public void Close()
        {
            _wrapped.Close();
        }

        public bool Contains(T item)
        {
            return _wrapped.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public IEnumerable<T> Progress()
        {
            return _wrapped.Progress();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }
    }
}