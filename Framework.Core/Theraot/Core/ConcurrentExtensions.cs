// Needed for Workaround

using System.Collections.Concurrent;

namespace Theraot.Core
{
    public static class ConcurrentExtensions
    {
        public static void Clear<T>(this ConcurrentBag<T> concurrentBag)
        {
            while(concurrentBag.TryTake(out _))
            {
                // Empty
            }
        }

        public static void Clear<T>(this ConcurrentQueue<T> concurrentQueue)
        {
            while(concurrentQueue.TryDequeue(out _))
            {
                // Empty
            }
        }
    }
}