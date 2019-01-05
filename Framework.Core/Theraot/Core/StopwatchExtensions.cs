// Needed for Workaround

using System.Diagnostics;

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

namespace Theraot.Core
{
    public static class StopwatchExtensions
    {
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif

        public static void Restart(this Stopwatch stopwatch)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}