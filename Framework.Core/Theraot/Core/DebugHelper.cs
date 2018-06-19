#if FAT

using System.Diagnostics;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public static class DebugHelper
    {
        [Conditional("DEBUG")]
        public static void Break()
        {
            Debugger.Launch();
            Debugger.Break();
        }
    }
}

#endif