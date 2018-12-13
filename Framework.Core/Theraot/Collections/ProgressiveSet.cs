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
        public ProgressiveSet(IEnumerable<T> enumerable)
            : this(Progressor<T>.CreateFromIEnumerable(enumerable), new ExtendedSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveSet(IObservable<T> observable)
            : this(Progressor<T>.CreateFromIObservable(observable), new ExtendedSet<T>(), null)
        {
            // Empty
        }

        public ProgressiveSet(IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
            : this(Progressor<T>.CreateFromIEnumerable(enumerable), new ExtendedSet<T>(comparer), null)
        {
            // Empty
        }

        public ProgressiveSet(IObservable<T> observable, IEqualityComparer<T> comparer)
           : this(Progressor<T>.CreateFromIObservable(observable), new ExtendedSet<T>(comparer), null)
        {
            // Empty
        }

        protected ProgressiveSet(Progressor<T> progressor, ISet<T> cache, IEqualityComparer<T> comparer)
            : base(progressor, cache, comparer)
        {
            // Empty
        }

        bool ICollection<T>.IsReadOnly => true;

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

#if FAT
        internal new static ProgressiveSet<T> Create<TSet>(Progressor<T> progressor, IEqualityComparer<T> comparer)
            where TSet : ISet<T>, new()
        {
            return new ProgressiveSet<T>(progressor, new TSet(), comparer);
        }
#endif
    }
}