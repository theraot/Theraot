using System;
using System.Collections.Generic;
using System.Text;

namespace Theraot.Core
{
    public static class EnvironmentHelper
    {
        public static bool Is64BitProcess
        {
            get
            {
                // "The value of this property is 4 in a 32-bit process, and 8 in a 64-bit process." -- MSDN
                return IntPtr.Size == 8;
            }
        }
    }
}
