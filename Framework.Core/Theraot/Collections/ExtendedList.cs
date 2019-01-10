using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public sealed class ListEx<T> : List<T>
#if LESSTHAN_NET45
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
            CopyTo(0, array, arrayIndex, count);
        }
    }
}