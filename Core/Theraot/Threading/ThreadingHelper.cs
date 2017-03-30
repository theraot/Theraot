// Needed for NET40

using System;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static partial class ThreadingHelper
    {
        internal const int _sleepCountHint = 10;
        private const int _maxTime = 200;

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