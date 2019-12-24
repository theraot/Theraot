#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

using System.Runtime.CompilerServices;

namespace System
{
    public static class CharTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ToString(this char chr, IFormatProvider provider)
        {
            _ = provider;
            return char.ToString(chr);
        }
    }
}

#endif