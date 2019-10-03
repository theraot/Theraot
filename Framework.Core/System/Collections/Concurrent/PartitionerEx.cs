using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if LESSTHAN_NET45 || NETSTANDARD1_0

using Theraot.Collections;
using Theraot.Core;

#endif

namespace System.Collections.Concurrent
{
    public static class PartitionerEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source, EnumerablePartitionerOptions partitionerOptions)
        {
#if LESSTHAN_NET45 || NETSTANDARD1_0
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if ((partitionerOptions & ~EnumerablePartitionerOptions.NoBuffering) != 0)
            {
                // We could support NoBuffering, no problem.
                // In fact, what is that about chunking? We do nothing of that.
                // I bet I could just use the current implementation as is...
                // ... it was created to partition on demand, no buffering.
                // However this is the documented behaviour.
                // Which also means there are no examples for a dynamic no buffering partition.
                throw new ArgumentOutOfRangeException(nameof(partitionerOptions));
            }

            return new DynamicOrderablePartitioner<TSource>(source);
#else
            return Partitioner.Create(source, partitionerOptions);
#endif
        }
    }
}