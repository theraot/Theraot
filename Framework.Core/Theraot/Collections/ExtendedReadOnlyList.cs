// Needed for NET40

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Theraot.Collections
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public sealed class ReadOnlyCollectionEx<T> : ReadOnlyCollection<T>
#if LESSTHAN_NET45
        , IReadOnlyList<T>
#endif
    {
        private readonly IList<T> _wrapped;

        public ReadOnlyCollectionEx(IList<T> wrapped)
            : base(wrapped)
        {
            _wrapped = wrapped;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array)
        {
            _wrapped.CopyTo(array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            Extensions.CanCopyTo(array, arrayIndex, count);
            Extensions.CopyTo(this, array, arrayIndex, count);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            Extensions.CanCopyTo(Count - index, array, count);
            Extensions.CopyTo(this, index, array, arrayIndex, count);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public T[] ToArray()
        {
            var array = new T[_wrapped.Count];
            CopyTo(array);
            return array;
        }
    }
}