#if LESSTHAN_NET40

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable S112 // General exceptions should never be thrown

using System.Runtime.CompilerServices;

namespace System.Text
{
    public static class StringBuilderTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Clear(this StringBuilder stringBuilder)
        {
            if (stringBuilder == null)
            {
                throw new NullReferenceException();
            }
            stringBuilder.Length = 0;
        }
    }
}

#endif