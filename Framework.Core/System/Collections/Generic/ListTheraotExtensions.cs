#if LESSTHAN_NETSTANDARD13
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class ListTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
        {
            if (list == null)
            {
                throw new NullReferenceException();
            }
            return new ReadOnlyCollection<T>(list);
        }
    }
}

#endif