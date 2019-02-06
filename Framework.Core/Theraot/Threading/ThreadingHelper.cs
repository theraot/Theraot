// Needed for NET40

using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public static partial class ThreadingHelper
    {
        internal const int SleepCountHint = 10;

        public static void MemoryBarrier()
        {
#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD
            Interlocked.MemoryBarrier();
#else
            Thread.MemoryBarrier();
#endif
        }

        internal static long Milliseconds(long ticks)
        {
            return ticks / TimeSpan.TicksPerMillisecond;
        }

        internal static long TicksNow()
        {
            return DateTime.Now.Ticks;
        }
    }
}