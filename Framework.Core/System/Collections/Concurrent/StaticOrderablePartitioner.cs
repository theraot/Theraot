#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Collections.Generic;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Core;

namespace System.Collections.Concurrent
{
    internal sealed class StaticOrderablePartitioner<T> : OrderablePartitioner<T>
    {
        private readonly IList<T> _source;

        public StaticOrderablePartitioner(IEnumerable<T> source)
            : base(keysOrderedInEachPartition: true, keysOrderedAcrossPartitions: false, keysNormalized: true)
        {
            _source = source.AsIList();
        }

        public override IEnumerable<T> GetDynamicPartitions()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions()
        {
            throw new NotSupportedException();
        }

        public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount)
        {
            var list = new List<IEnumerator<KeyValuePair<long, T>>>(partitionCount);
            var source = Progressor<T>.CreateFromIEnumerable(_source);
            for (var index = 0; index < partitionCount; index++)
            {
                list.Add(Enumerator(index));
            }

            return list.AsReadOnly();

            IEnumerator<KeyValuePair<long, T>> Enumerator(int index)
            {
                var subIndex = 0;
                foreach (var item in source)
                {
                    yield return new KeyValuePair<long, T>(NumericHelper.BuildInt64(index, subIndex), item);

                    subIndex++;
                }
            }
        }

        public override IList<IEnumerator<T>> GetPartitions(int partitionCount)
        {
            var index = -1;
            var groups = GroupBuilder<int, T, T>.CreateGroups
            (
                _source,
                EqualityComparer<int>.Default,
                _ => Interlocked.Increment(ref index) % partitionCount,
                obj => obj
            );
            return groups.ConvertProgressive(g => g.GetEnumerator()).WrapAsIList();
        }
    }
}

#endif