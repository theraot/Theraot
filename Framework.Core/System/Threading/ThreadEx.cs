using System.Runtime.CompilerServices;
using Theraot.Threading;

namespace System.Threading
{
    public static class ThreadEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void MemoryBarrier()
        {
            ThreadingHelper.MemoryBarrier();
        }
    }
}