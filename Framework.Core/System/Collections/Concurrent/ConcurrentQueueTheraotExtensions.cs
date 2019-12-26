#if GREATERTHAN_NET35 || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable S112 // General exceptions should never be thrown

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