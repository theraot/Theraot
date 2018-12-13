using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public static class RuntimeUniqueIdProvider
    {
        private static int _lastId;

        public static UniqueId GetNextId()
        {
            return new UniqueId(unchecked((uint)Interlocked.Increment(ref _lastId)));
        }
    }
}