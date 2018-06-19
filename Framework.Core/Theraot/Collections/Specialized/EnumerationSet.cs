#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    public class EnumerationSet<T> : EnumerationCollection<T>, ISet<T>, IReadOnlySet<T>, IExtendedReadOnlySet<T>, IExtendedSet<T>
    {
        public EnumerationSet(IEnumerable<T> wrapped)
            : base(wrapped)
        {
            //Empty
        }

        public EnumerationSet(T[] wrapped)
            : base(wrapped)
        {
            //Empty
        }

        public EnumerationSet(ICollection<T> wrapped)
            : base(wrapped)
        {
            //Empty
        }

        public EnumerationSet(IEnumerable<T> wrapped, Func<int> count)
            : base(wrapped, count)
        {
            //Empty
        }

        public EnumerationSet(IEnumerable<T> wrapped, Func<T, bool> contains)
            : base(wrapped, contains)
        {
            //Empty
        }

        public EnumerationSet(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> contains)
            : base(wrapped, count, contains)
        {
            //Empty
        }

        IReadOnlySet<T> IExtendedSet<T>.AsReadOnly
        {
            get { return this; }
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

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }
    }
}

#endif