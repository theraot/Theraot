#if LESSTHAN_NET40

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable S112 // General exceptions should never be thrown

using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public static class StopwatchTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Restart(this Stopwatch stopwatch)
        {
            if (stopwatch == null)
            {
                throw new NullReferenceException();
            }
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}

#endif