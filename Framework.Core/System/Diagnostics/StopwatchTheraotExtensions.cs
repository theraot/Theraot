#if LESSTHAN_NET40

using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public static class StopwatchTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Restart(this Stopwatch stopwatch)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}

#endif