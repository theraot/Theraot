using System.Runtime.CompilerServices;
using Theraot.Threading;

namespace System.Threading
{
    public static class InterlockedEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void MemoryBarrier()
        {
            ThreadingHelper.MemoryBarrier();
        }
    }
}