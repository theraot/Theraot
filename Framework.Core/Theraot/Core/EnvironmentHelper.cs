// Needed for Workaround

using System;

namespace Theraot.Core
{
    public static class EnvironmentHelper
    {
        // Every request to Environment.ProcessorCount will result in an external call
        // EnvironmentHelper.ProcessorCount is a cached result.
        // TODO: can this value change for a running process / without reboot?
        private static readonly int _processorCount = Environment.ProcessorCount;

        public static bool Is64BitProcess => IntPtr.Size == 8;

        public static bool IsSingleCPU => _processorCount == 1;
    }
}