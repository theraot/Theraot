// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (partitionSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(partitionSize));
            }
            return new PartitionEnumerable<T>(items, partitionSize);
        }

        internal class PartitionEnumerable<T> : IEnumerable<IEnumerable<T>>
        {
            private readonly IEnumerable<T> _source;
            private readonly int _partitionSize;

            public PartitionEnumerable(IEnumerable<T> source, int partitionSize)
            {
                _source = source;
                _partitionSize = partitionSize;
            }

            public IEnumerator<IEnumerable<T>> GetEnumerator()
            {
                var group = new List<T>();
                var count = _partitionSize;
                foreach (var item in _source)
                {
                    group.Add(item);
                    count--;
                    if (count == 0)
                    {
                        yield return group;
                        group = new List<T>();
                        count = _partitionSize;
                    }
                }
                if (count < _partitionSize)
                {
                    yield return group;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}