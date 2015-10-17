using System;

namespace Theraot.Core
{
    public static class EnvironmentHelper
    {
        // Every request to Environment.ProcessorCount will result in an external call
        // EnvironmentHelper.ProcessorCount is a cached result.
        // TODO: can this value change for a running process / without reboot?
        private static readonly int ProcessorCount = Environment.ProcessorCount;

        public static bool Is64BitProcess
        {
            get
            {
                // "The value of this property is 4 in a 32-bit process, and 8 in a 64-bit process." -- MSDN
                return IntPtr.Size == 8;
            }
        }

        public static bool IsSingleCPU
        {
            get
            {
                return ProcessorCount == 1;
            }
        }
    }
}
