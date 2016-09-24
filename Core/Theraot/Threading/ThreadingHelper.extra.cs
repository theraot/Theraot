#if FAT

namespace Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        // Leaked until AppDomain unload
        private static readonly NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId> _threadRuntimeUniqueId = new NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId>(RuntimeUniqueIdProdiver.GetNextId);

        public static bool HasThreadUniqueId
        {
            get
            {
                return _threadRuntimeUniqueId.IsValueCreated;
            }
        }

        public static RuntimeUniqueIdProdiver.UniqueId ThreadUniqueId
        {
            get
            {
                return _threadRuntimeUniqueId.Value;
            }
        }
    }
}

#endif