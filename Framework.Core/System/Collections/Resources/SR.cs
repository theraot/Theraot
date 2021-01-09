using System;
using System.Collections.Generic;
using System.Text;

namespace Theraot.Core.System.Collections.Resources
{
    internal partial class SR
    {
        public static string Format(string str1, params object[] str2)
        {
            return string.Format(str1, str2);
        }
    }
}
