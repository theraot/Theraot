#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

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