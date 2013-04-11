using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading
{
    internal static class BucketHelper
    {
        private static readonly object _null;

        static BucketHelper()
        {
            _null = new object();
        }

        public static object Null
        {
            get
            {
                return _null;
            }
        }
    }
}