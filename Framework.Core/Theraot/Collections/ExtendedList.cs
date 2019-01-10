using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Theraot.Collections
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public sealed class ListEx<T> : List<T>
#if GREATERTHAN_NET40
        , IReadOnlyList<T>
#endif
    {
        public ListEx()
        {
            // Empty
        }

        public ListEx(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public ListEx(IEnumerable<T> collection)
            : base(collection)
        {
            // Empty
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            base.CopyTo(0, array, arrayIndex, count);
        }
    }
}