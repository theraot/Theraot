using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Theraot.Collections;

namespace System.Collections.ObjectModel
{
    public static class ReadOnlyCollectionEx
    {
        public static ReadOnlyCollectionEx<T> Create<T>(params T[] list)
        {
            return new ReadOnlyCollectionEx<T>(list);
        }
    }

#if LESSTHAN_NET45

    public partial class ReadOnlyCollectionEx<T> : IReadOnlyList<T>
    {
        // Empty
    }

#endif

    [Serializable]
    [ComVisible(false)]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public
#if LESSTHAN_NET45
        partial
#endif
        class ReadOnlyCollectionEx<T> : ReadOnlyCollection<T>
    {
        public ReadOnlyCollectionEx(IList<T> wrapped)
            : base(wrapped)
        {
            Wrapped = wrapped;
        }

        internal IList<T> Wrapped { get; }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(int sourceIndex, T[] array, int index, int count)
        {
            Extensions.CanCopyTo(Count - sourceIndex, array, count);
            Extensions.CopyTo(this, sourceIndex, array, index, count);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array)
        {
            Wrapped.CopyTo(array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array, int index, int count)
        {
            Extensions.CanCopyTo(array, index, count);
            Extensions.CopyTo(this, array, index, count);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Extensions.ComputeHashCode(this);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public T[] ToArray()
        {
            var array = new T[Wrapped.Count];
            CopyTo(array);
            return array;
        }
    }
}