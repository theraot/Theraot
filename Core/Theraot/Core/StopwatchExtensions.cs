using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
