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
#if NETCOREAPP1_1 || NETCOREAPP1_0 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
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