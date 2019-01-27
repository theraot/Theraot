#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Concurrent
{
    public abstract class OrderablePartitioner<TSource> : Partitioner<TSource>
    {
        protected OrderablePartitioner(bool keysOrderedInEachPartition, bool keysOrderedAcrossPartitions, bool keysNormalized)
        {
            KeysOrderedInEachPartition = keysOrderedInEachPartition;
            KeysOrderedAcrossPartitions = keysOrderedAcrossPartitions;
            KeysNormalized = keysNormalized;
        }

        public bool KeysNormalized { get; }

        public bool KeysOrderedAcrossPartitions { get; }

        public bool KeysOrderedInEachPartition { get; }

        public override IEnumerable<TSource> GetDynamicPartitions()
        {
            return GetOrderableDynamicPartitions().Select(item => item.Value);
        }

        public virtual IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
        {
            throw new NotSupportedException();
        }

        public abstract IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount);

        public override IList<IEnumerator<TSource>> GetPartitions(int partitionCount)
        {
            var orderablePartitions = GetOrderablePartitions(partitionCount);
            var partitions = new IEnumerator<TSource>[partitionCount];
            for (var i = 0; i < partitionCount; i++)
            {
                partitions[i] = Enumerator(orderablePartitions[i]);
            }

            return partitions;

            IEnumerator<TSource> Enumerator(IEnumerator<KeyValuePair<long, TSource>> enumerator)
            {
                try
                {
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current.Value;
                    }
                }
                finally
                {
                    enumerator.Dispose();
                }
            }
        }
    }
}

#endif