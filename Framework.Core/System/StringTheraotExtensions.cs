#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Runtime.CompilerServices;
using Theraot;

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

            No.Op(provider);
            return str;
        }
    }
}

#endif