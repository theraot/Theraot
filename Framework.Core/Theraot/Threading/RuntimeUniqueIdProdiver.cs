using System;
using System.Globalization;
using System.Threading;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class RuntimeUniqueIdProdiver
    {
        private static int _lastId = int.MinValue;

        public static UniqueId GetNextId()
        {
            return new UniqueId(Interlocked.Increment(ref _lastId));
        }
    }
}