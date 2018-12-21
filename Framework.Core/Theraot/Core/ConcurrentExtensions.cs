// Needed for Workaround

namespace Theraot.Core
{
    public static class ConcurrentExtensions
    {
#if NET40 || NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETSANDARD1_1 || NETSANDARD1_2 || NETSANDARD1_3 || NETSANDARD1_4 || NETSANDARD1_5 || NETSANDARD1_6 || NETSANDARD2_0
        public static void Clear<T>(this System.Collections.Concurrent.ConcurrentBag<T> concurrentBag)
        {
            while(concurrentBag.TryTake(out _))
            {
                // Empty
            }
        }

        public static void Clear<T>(this System.Collections.Concurrent.ConcurrentQueue<T> concurrentQueue)
        {
            while(concurrentQueue.TryDequeue(out _))
            {
                // Empty
            }
        }
#endif
    }
}