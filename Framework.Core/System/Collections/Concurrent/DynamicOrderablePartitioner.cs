#if LESSTHAN_NET45 || NETSTANDARD1_0

using System.Collections.Generic;
using Theraot.Collections;
using Theraot.Core;

namespace System.Collections.Concurrent
{
    internal sealed class DynamicOrderablePartitioner<T> : OrderablePartitioner<T>
    {
        private readonly IEnumerable<T> _source;

        public DynamicOrderablePartitioner(IEnumerable<T> source)
            : base(true, false, true)
        {
            _source = source;
        }

        public override bool SupportsDynamicPartitions => true;

        public override IEnumerable<T> GetDynamicPartitions()
        {
            return Progressor<T>.CreateFromIEnumerable(_source);
        }

        public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions()
        {
            var source = GetDynamicPartitions();
            var subIndex = 0L;
            foreach (var item in source)
            {
                yield return new KeyValuePair<long, T>(subIndex, item);
                subIndex++;
            }
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
            var list = new List<IEnumerator<T>>(partitionCount);
            var source = Progressor<T>.CreateFromIEnumerable(_source);
            for (var index = 0; index < partitionCount; index++)
            {
                list.Add(Enumerator());
            }

            return list.AsReadOnly();

            IEnumerator<T> Enumerator()
            {
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}

#endif