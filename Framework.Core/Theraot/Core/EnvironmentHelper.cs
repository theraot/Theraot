// Needed for Workaround

using System;

namespace Theraot.Core
{
    public static class EnvironmentHelper
    {
        public static bool Is64BitProcess => IntPtr.Size == 8;

        public static bool IsSingleCPU => ProcessorCount == 1;

        public static int ProcessorCount { get; } = Environment.ProcessorCount;
    }
}