#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

#pragma warning disable CA1305 // Specify IFormatProvider

using System.Runtime.CompilerServices;

namespace System
{
    public static class BooleanTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ToString(this bool boolean, IFormatProvider provider)
        {
            _ = provider;
            return boolean.ToString();
        }
    }
}

#endif