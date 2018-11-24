// Needed for NET40

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public
#if FAT
        partial
# endif
        class ProgressiveSet<T> : ProgressiveCollection<T>, ISet<T>
    {
        // Note: these constructors uses ExtendedSet because HashSet is not an ISet<T> in .NET 3.5 and base class needs an ISet<T>
        public ProgressiveSet(IEnumerable<T> wrapped)
            : this(wrapped, new ExtendedSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveSet(IObservable<T> wrapped)
            : this(wrapped, new ExtendedSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveSet(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new ExtendedSet<T>(comparer), null)
        {
            // Empty
        }

        public ProgressiveSet(IObservable<T> wrapped, IEqualityComparer<T> comparer)
           : this(wrapped, new ExtendedSet<T>(comparer), null)
        {
            // Empty
        }

        protected ProgressiveSet(IEnumerable<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped ?? throw new ArgumentNullException(nameof(wrapped)), cache, comparer)
        {
            // Empty
        }

        protected ProgressiveSet(IObservable<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped ?? throw new ArgumentNullException(nameof(wrapped)), cache, comparer)
        {
            // Empty
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
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

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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