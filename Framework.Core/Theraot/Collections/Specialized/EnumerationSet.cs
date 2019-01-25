#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    public class EnumerationSet<T> : EnumerationList<T>, ISet<T>
    {
        public EnumerationSet(IEnumerable<T> wrapped)
            : base(wrapped)
        {
            // Empty
        }

        public EnumerationSet(T[] wrapped)
            : base(wrapped)
        {
            // Empty
        }

        public EnumerationSet(ICollection<T> wrapped)
            : base(wrapped)
        {
            // Empty
        }

        public EnumerationSet(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> contains)
            : base(wrapped, count, contains)
        {
            // Empty
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