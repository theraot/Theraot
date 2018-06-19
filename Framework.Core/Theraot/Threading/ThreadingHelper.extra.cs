#if FAT

namespace Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        // Leaked
        private static readonly TrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId> _threadRuntimeUniqueId = new TrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId>(RuntimeUniqueIdProdiver.GetNextId);

        public static bool HasThreadUniqueId
        {
            get { return _threadRuntimeUniqueId.IsValueCreated; }
        }

        public static RuntimeUniqueIdProdiver.UniqueId ThreadUniqueId
        {
            get { return _threadRuntimeUniqueId.Value; }
        }
    }
}

#endif