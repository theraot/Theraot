// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public class ProgressiveSet<T> : ProgressiveCollection<T>, ISet<T>
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

        protected ProgressiveSet(IObservable<T> wrapped, Action exhaustedCallback, ISet<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped ?? throw new ArgumentNullException(nameof(wrapped)), exhaustedCallback, cache, comparer)
        {
            // Empty
        }

        bool ICollection<T>.IsReadOnly => true;

        public new static ProgressiveSet<T> Create<TSet>(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
                    where TSet : ISet<T>, new()
        {
            return new ProgressiveSet<T>(wrapped, new TSet(), comparer);
        }

        public new static ProgressiveSet<T> Create<TSet>(IObservable<T> wrapped, IEqualityComparer<T> comparer)
            where TSet : ISet<T>, new()
        {
            return new ProgressiveSet<T>(wrapped, new TSet(), comparer);
        }

        public new static ProgressiveSet<T> Create<TSet>(IObservable<T> wrapped, Action exhaustedCallback, IEqualityComparer<T> comparer)
            where TSet : ISet<T>, new()
        {
            return new ProgressiveSet<T>(wrapped, exhaustedCallback, new TSet(), comparer);
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