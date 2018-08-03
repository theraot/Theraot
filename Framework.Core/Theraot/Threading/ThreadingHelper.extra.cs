#if FAT

namespace Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        // Leaked
        private static readonly TrackingThreadLocal<UniqueId> _threadRuntimeUniqueId = new TrackingThreadLocal<UniqueId>(RuntimeUniqueIdProdiver.GetNextId);

        public static bool HasThreadUniqueId
        {
            get { return _threadRuntimeUniqueId.IsValueCreated; }
        }

        public static UniqueId ThreadUniqueId
        {
            get { return _threadRuntimeUniqueId.Value; }
        }
    }
}

#endif