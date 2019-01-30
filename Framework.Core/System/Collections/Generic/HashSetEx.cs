#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Diagnostics;
using System.Runtime.Serialization;
using Theraot;

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
#if (LESSTHAN_NET46 && GREATERTHAN_NET30) || LESSTHAN_NETSTANDARD13
        , IReadOnlyCollection<T>
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

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET
        protected HashSetEx(SerializationInfo info, StreamingContext context)
            : base(info, context)
#else
        [Obsolete("This target platform does not support binary serialization.")]
        protected HashSetEx(SerializationInfo info, StreamingContext context)
#endif
        {
            No.Op(info);
            No.Op(context);
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

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
        public static IEqualityComparer<HashSet<T>> CreateSetComparer()
        {
            return HashSetEqualityComparer<T>.Instance;
        }
#endif
    }
}