// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        // Based on code by Jeppe Stig Nielsen at http://stackoverflow.com/a/13711499/402022
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            if (partitionSize < 1)
            {
                throw new ArgumentOutOfRangeException("partitionSize");
            }
            return new PartitionHelper<T>(items, partitionSize);
        }

        private sealed class PartitionEnumerator<T> : IEnumerator<T>, IEnumerable<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private int _count;

            public PartitionEnumerator(IEnumerator<T> enumerator, int count)
            {
                _enumerator = enumerator;
                _count = count;
            }

            public int Count
            {
                get
                {
                    return _count;
                }
            }

            public T Current
            {
                get
                {
                    return _enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                // NoOp
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            public bool MoveNext()
            {
                if (_count == 0)
                {
                    return false;
                }
                if (_enumerator.MoveNext())
                {
                    _count--;
                    return true;
                }
                return false;
            }
        }

        private sealed class PartitionHelper<T> : IEnumerable<IEnumerable<T>>
        {
            private readonly IEnumerable<T> _items;
            private readonly int _partitionSize;

            internal PartitionHelper(IEnumerable<T> items, int partitionSize)
            {
                _items = items;
                _partitionSize = partitionSize;
            }

            public IEnumerator<IEnumerable<T>> GetEnumerator()
            {
                using (var enumerator = _items.GetEnumerator())
                {
                    while (true)
                    {
                        var batch = new PartitionEnumerator<T>(enumerator, _partitionSize);
                        if (batch.Count < _partitionSize)
                        {
                            yield return batch;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}