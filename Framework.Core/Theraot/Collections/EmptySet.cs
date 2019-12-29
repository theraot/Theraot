using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class EmptySet<T> : ISet<T>, IHasComparer<T>
    {
        internal EmptySet(IEqualityComparer<T>? comparer)
        {
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        private EmptySet()
            : this(null)
        {
            // Empty
        }

        public static EmptySet<T> Instance { get; } = new EmptySet<T>();

        public IEqualityComparer<T> Comparer { get; }

        public int Count => 0;

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

        public bool Contains(T item)
        {
            _ = item;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(0, array, arrayIndex);
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield break;
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
            return other.HasAtLeast(1);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return false;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return !other.Any();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return false;
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return !other.Any();
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