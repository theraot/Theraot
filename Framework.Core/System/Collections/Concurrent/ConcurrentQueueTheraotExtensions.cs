#if GREATERTHAN_NET35 || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD
using System.Runtime.CompilerServices;

namespace System.Collections.Concurrent
{
    public static class ConcurrentQueueTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Clear<T>(this ConcurrentQueue<T> concurrentQueue)
        {
            if (concurrentQueue == null)
            {
                throw new NullReferenceException();
            }
            while (concurrentQueue.TryDequeue(out _))
            {
                // Empty
            }
        }
    }
}

#endif