// Needed for Workaround

using System.Collections.Concurrent;

namespace Theraot.Core
{
    public static class ConcurrentBagExtensions
    {
        public static void Clear<T>(this ConcurrentBag<T> concurrentBag)
        {
            while(concurrentBag.TryTake(out _))
            {
                // Empty
            }
        }
    }
}