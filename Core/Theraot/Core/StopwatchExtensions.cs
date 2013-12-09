using System.Diagnostics;

namespace Theraot.Core
{
    public static class StopwatchExtensions
    {
        public static void Restart(this Stopwatch stopwatch)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}