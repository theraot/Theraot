#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

using System.Runtime.CompilerServices;
using Theraot;

namespace System
{
    public static class BooleanTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ToString(this bool boolean, IFormatProvider provider)
        {
            No.Op(provider);
            return boolean.ToString();
        }
    }
}

#endif