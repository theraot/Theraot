using System.Runtime.CompilerServices;

#if LESSTHAN_NET45

using Theraot.Collections.Specialized;

#endif

namespace System.Collections.Generic
{
    public static class ComparerEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Comparer<T> Create<T>(Comparison<T> comparison)
        {
#if LESSTHAN_NET45
            return new CustomComparer<T>(comparison);
#else
            return Comparer<T>.Create(comparison);
#endif
        }
    }
}