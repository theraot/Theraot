// Needed for Workaround

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

namespace Theraot.Core
{
    public static class ConcurrentExtensions
    {
#if NET40 || NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 ||  NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Clear<T>(this System.Collections.Concurrent.ConcurrentBag<T> concurrentBag)
        {
            while (concurrentBag.TryTake(out _))
            {
                // Empty
            }
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Clear<T>(this System.Collections.Concurrent.ConcurrentQueue<T> concurrentQueue)
        {
            while (concurrentQueue.TryDequeue(out _))
            {
                // Empty
            }
        }
#endif
    }
}