#if LESSTHAN_NET40

using System.Runtime.CompilerServices;

namespace System.Text
{
    public static class StringBuilderTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Clear(this StringBuilder stringBuilder)
        {
            stringBuilder.Length = 0;
        }
    }
}

#endif