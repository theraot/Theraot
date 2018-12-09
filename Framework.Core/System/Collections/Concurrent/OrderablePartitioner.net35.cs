using System.Collections.Generic;

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

        public abstract IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount);

        public virtual IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
        {
            throw new NotSupportedException();
        }

        public bool KeysOrderedInEachPartition { get; }

        public bool KeysOrderedAcrossPartitions { get; }

        public bool KeysNormalized { get; }

        public override IList<IEnumerator<TSource>> GetPartitions(int partitionCount)
        {
            var orderablePartitions = GetOrderablePartitions(partitionCount);
            var partitions = new IEnumerator<TSource>[partitionCount];
            for (int i = 0; i < partitionCount; i++)
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

        public override IEnumerable<TSource> GetDynamicPartitions()
        {
            foreach (var item in GetOrderableDynamicPartitions())
            {
                yield return item.Value;
            }
        }
    }
}