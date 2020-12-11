#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA2201 // Do not raise reserved exception types

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