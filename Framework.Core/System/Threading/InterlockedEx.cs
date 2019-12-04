using System.Runtime.CompilerServices;

namespace System.Threading
{
    public static class InterlockedEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void MemoryBarrier()
        {
            ThreadEx.MemoryBarrier();
        }
    }
}