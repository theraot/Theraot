// Needed for NET40

using System;
using System.Threading;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static partial class ThreadingHelper
    {
        internal const int _sleepCountHint = 10;
        private const int _maxTime = 200;

        public static void MemoryBarrier()
        {
            Thread.MemoryBarrier();
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