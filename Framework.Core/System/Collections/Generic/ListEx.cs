using System.Diagnostics;
using System.Runtime.CompilerServices;

#if LESSTHAN_NET45
using System.Collections.ObjectModel;
#endif

namespace System.Collections.Generic
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public sealed
#if LESSTHAN_NET45
        partial
#endif
        class ListEx<T> : List<T>
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

#if LESSTHAN_NET45
    public sealed partial class ListEx<T>
    {
        public new ReadOnlyCollectionEx<T> AsReadOnly ()
        {
            return new ReadOnlyCollectionEx<T>(this);
        }
    }
#endif
}