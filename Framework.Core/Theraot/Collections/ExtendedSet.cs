// Needed for NET40

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Theraot.Collections
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public class HashSetEx<T> : HashSet<T>, ISet<T>
    {
        public HashSetEx()
        {
            // Empty
        }

        public HashSetEx(IEnumerable<T> collection)
            : base(collection)
        {
            // Empty
        }

        public HashSetEx(IEqualityComparer<T> comparer)
            : base(comparer)
        {
            // Empty
        }

        public HashSetEx(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(collection, comparer)
        {
            // Empty
        }

        protected HashSetEx(SerializationInfo info, StreamingContext context)
#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET
            : base(info, context)
#endif
        {
            GC.KeepAlive(info);
            GC.KeepAlive(context);
        }

        bool ISet<T>.Add(T item)
        {
            return Add(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            ExceptWith(other);
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            IntersectWith(other);
        }

        bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsProperSubsetOf(other);
        }

        bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsProperSupersetOf(other);
        }

        bool ISet<T>.IsSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other);
        }

        bool ISet<T>.IsSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other);
        }

        bool ISet<T>.Overlaps(IEnumerable<T> other)
        {
            return Overlaps(other);
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }
            foreach (var foundItem in this.RemoveWhereEnumerable(input => comparer.Equals(input, item)))
            {
                GC.KeepAlive(foundItem);
                return true;
            }
            return false;
        }

        bool ISet<T>.SetEquals(IEnumerable<T> other)
        {
            return SetEquals(other);
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            SymmetricExceptWith(other);
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            UnionWith(other);
        }
    }
}