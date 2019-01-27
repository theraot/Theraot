#if FAT
namespace Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        // Leaked
        private static readonly TrackingThreadLocal<UniqueId> _threadRuntimeUniqueId = new TrackingThreadLocal<UniqueId>(RuntimeUniqueIdProvider.GetNextId);

        public static bool HasThreadUniqueId => _threadRuntimeUniqueId.IsValueCreated;

        public static UniqueId ThreadUniqueId => _threadRuntimeUniqueId.Value;
    }
}

#endif