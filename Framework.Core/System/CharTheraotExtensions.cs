#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

using System.Runtime.CompilerServices;
using Theraot;

namespace System
{
    public static class CharTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ToString(this char chr, IFormatProvider provider)
        {
            No.Op(provider);
            return char.ToString(chr);
        }
    }
}

#endif