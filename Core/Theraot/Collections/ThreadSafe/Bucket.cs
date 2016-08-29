using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe wait-free bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    [Serializable]
    public sealed class Bucket<T> : IBucket<T>
    {
        private readonly ICore<FixedSizeBucket<T>> _core;
        private int _count;

        public Bucket(GrowthStrategy strategy, int capacity)
        {
            switch (strategy)
            {
                case GrowthStrategy.Compact:
                    _core = new CompactCore<FixedSizeBucket<T>>(1 + capacity / 64);
                    break;

                case GrowthStrategy.Sparse:
                    // TODO _core = new SparseCore<FixedSizeBucket<T>>(1 + capacity / 64);
                    break;

                default:
                    throw new ArgumentException("Invalid GrowthStrategy");
            }
        }

        public Bucket(GrowthStrategy strategy, IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var collection = source as ICollection<T>;
            var capacity = collection == null ? 64 : 1 + collection.Count / 64;
            switch (strategy)
            {
                case GrowthStrategy.Compact:
                    _core = new CompactCore<FixedSizeBucket<T>>(capacity);
                    break;

                case GrowthStrategy.Sparse:
                    _core = new SparseCore<FixedSizeBucket<T>>(capacity);
                    break;

                default:
                    throw new ArgumentException("Invalid GrowthStrategy");
            }
            var index = 0;
            foreach (var partition in source.Partition(64))
            {
                var bucket = new FixedSizeBucket<T>(partition);
                _core.Set(index, bucket);
                _count += bucket.Count;
                index++;
            }
        }

        public int Capacity
        {
            get
            {
                return _core.Length * 64;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public bool Exchange(int index, T item, out T previous)
        {
            FixedSizeBucket<T> bucket;
            if (_core.TryGetValue(index / 64, out bucket))
            {
                return bucket.Exchange(index % 64, item, out previous);
            }
            previous = default(T);
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var bucket in _core)
            {
                foreach (var value in bucket)
                {
                    yield return value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Insert(int index, T item)
        {
            var bucket = _core.GetOrInsert(index / 64, () => new FixedSizeBucket<T>(64));
            if (bucket.Insert(index % 64, item))
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            return false;
        }

        public bool Insert(int index, T item, out T previous)
        {
            var bucket = _core.GetOrInsert(index / 64, () => new FixedSizeBucket<T>(64));
            if (bucket.Insert(index % 64, item, out previous))
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            return false;
        }

        public bool RemoveAt(int index)
        {
            FixedSizeBucket<T> bucket;
            if (_core.TryGetValue(index / 64, out bucket))
            {
                if (bucket.RemoveAt(index % 64))
                {
                    Interlocked.Decrement(ref _count);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveAt(int index, out T previous)
        {
            FixedSizeBucket<T> bucket;
            if (_core.TryGetValue(index / 64, out bucket))
            {
                if (bucket.RemoveAt(index % 64, out previous))
                {
                    Interlocked.Decrement(ref _count);
                    return true;
                }
                return false;
            }
            previous = default(T);
            return false;
        }

        public bool RemoveValueAt(int index, T value, out T previous)
        {
            FixedSizeBucket<T> bucket;
            if (_core.TryGetValue(index / 64, out bucket))
            {
                if (bucket.RemoveValueAt(index % 64, value, out previous))
                {
                    Interlocked.Decrement(ref _count);
                    return true;
                }
                return false;
            }
            previous = default(T);
            return false;
        }

        public void Set(int index, T item, out bool isNew)
        {
            var bucket = _core.GetOrInsert(index / 64, () => new FixedSizeBucket<T>(64));
            bucket.Set(index % 64, item, out isNew);
            if (isNew)
            {
                Interlocked.Increment(ref _count);
            }
        }

        public bool TryGet(int index, out T value)
        {
            FixedSizeBucket<T> bucket;
            if (_core.TryGetValue(index / 64, out bucket))
            {
                return bucket.TryGet(index % 64, out value);
            }
            value = default(T);
            return false;
        }

        public bool Update(int index, T item, T comparisonItem, out T previous, out bool isNew)
        {
            FixedSizeBucket<T> bucket;
            if (_core.TryGetValue(index / 64, out bucket))
            {
                return bucket.Update(index % 64, item, comparisonItem, out previous, out isNew);
            }
            previous = default(T);
            isNew = true;
            return false;
        }

        public IEnumerable<T> Where(Predicate<T> predicate)
        {
            foreach (var bucket in _core)
            {
                foreach (var value in bucket.Where(predicate))
                {
                    yield return value;
                }
            }
        }
    }
}