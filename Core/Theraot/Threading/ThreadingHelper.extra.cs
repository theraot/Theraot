namespace Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        private static readonly RuntimeUniqueIdProdiver _threadIdProvider = new RuntimeUniqueIdProdiver();
        private static readonly NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId> _threadRuntimeUniqueId = new NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId>(_threadIdProvider.GetNextId);

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