// Needed for Workaround

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Theraot.Core
{
    public static class StopwatchExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Restart(this Stopwatch stopwatch)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}