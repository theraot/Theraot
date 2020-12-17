#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

#pragma warning disable CA2201 // Do not raise reserved exception types

#if TARGETS_NETSTANDARD

#pragma warning disable S112 // General exceptions should never be thrown

#endif

using System.Runtime.CompilerServices;

namespace System
{
    public static class StringTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ToString(this string str, IFormatProvider provider)
        {
            if (str == null)
            {
                throw new NullReferenceException();
            }

            _ = provider;
            return str;
        }
    }
}

#endif