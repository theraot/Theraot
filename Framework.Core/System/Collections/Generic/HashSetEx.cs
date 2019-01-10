using System.Diagnostics;
using System.Runtime.Serialization;

#if NET35

using System.Runtime.CompilerServices;

#endif

namespace System.Collections.Generic
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public class HashSetEx<T> : HashSet<T>
#if NET35
        , ISet<T>
#endif
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

#if NET35

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.Add(T item)
        {
            return Add(item);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            ExceptWith(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            IntersectWith(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsProperSubsetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsProperSupersetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.Overlaps(IEnumerable<T> other)
        {
            return Overlaps(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.SetEquals(IEnumerable<T> other)
        {
            return SetEquals(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            SymmetricExceptWith(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            UnionWith(other);
        }

#endif
    }
}