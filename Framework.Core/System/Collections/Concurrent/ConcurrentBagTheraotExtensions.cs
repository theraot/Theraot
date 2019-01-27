#if GREATERTHAN_NET35 || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

using System.Runtime.CompilerServices;

namespace System.Collections.Concurrent
{
    public static class ConcurrentBagTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Clear<T>(this ConcurrentBag<T> concurrentBag)
        {
            while (concurrentBag.TryTake(out _))
            {
                // Empty
            }
        }
    }
}

#endif