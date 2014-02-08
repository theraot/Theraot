using System;
using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe wait-free hash based set.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public sealed class FixedSizeSetBucket<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private readonly IEqualityComparer<T> _comparer;
        private readonly Bucket<T> _entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeSetBucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public FixedSizeSetBucket(int capacity, IEqualityComparer<T> comparer)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _entries = new Bucket<T>(_capacity);
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        /// <summary>
        /// Attempts to add the specified item at the default index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isCollision">if set to <c>true</c> the attempt resulted in a collision.</param>
        /// <returns>The index where the item were added.</returns>
        public int Add(T item, out bool isCollision)
        {
            return Add(item, 0, out isCollision);
        }

        /// <summary>
        /// Attempts to add the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="offset">The offset from the default index.</param>
        /// <param name="isCollision">if set to <c>true</c> the attempt resulted in a collision.</param>
        /// <returns>The index where the item were added.</returns>
        public int Add(T item, int offset, out bool isCollision)
        {
            int index = Index(item, offset);
            T previous;
            if (_entries.Insert(index, item, out previous))
            {
                isCollision = false;
                return index;
            }
            else
            {
                isCollision = !_comparer.Equals(previous, item);
                return -1;
            }
        }

        /// <summary>
        /// Determines whether the specified item is contained at the default index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index where the item is set; -1 otherwise.</returns>
        public int Contains(T item)
        {
            return Contains(item, 0);
        }

        /// <summary>
        /// Determines whether the specified item is contained.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="offset">The offset from the default index.</param>
        /// <returns>The index where the item is set; -1 otherwise.</returns>
        public int Contains(T item, int offset)
        {
            int index = Index(item, offset);
            T entry;
            if (_entries.TryGet(index, out entry))
            {
                if (_comparer.Equals(entry, item))
                {
                    return index;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the an <see cref="IEnumerable{T}" /> that allows to iterate over the contained items.
        /// </summary>
        public IEnumerable<T> GetEnumerable()
        {
            for (int index = 0; index < _capacity; index++)
            {
                T entry;
                if (_entries.TryGet(index, out entry))
                {
                    yield return entry;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            return _entries.GetValues();
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetValues<TOutput>(Converter<T, TOutput> converter)
        {
            return _entries.GetValues<TOutput>(converter);
        }

        /// <summary>
        /// Determinates the index for a given item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="offset">The offset from the default index.</param>
        public int Index(T item, int offset)
        {
            var hash = _comparer.GetHashCode(item);
            var index = (hash + offset) & (_capacity - 1);
            return index;
        }

        /// <summary>
        /// Attempts to removes the specified item at the default index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index where the item was set; -1 otherwise.</returns>
        public int Remove(T item)
        {
            return Remove(item, 0);
        }

        /// <summary>
        /// Attempts to removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="offset">The offset from the default index.</param>
        /// <returns>The index where the item was set; -1 otherwise.</returns>
        public int Remove(T item, int offset)
        {
            int index = Index(item, offset);
            T entry;
            if (_entries.TryGet(index, out entry))
            {
                if (_comparer.Equals(entry, item))
                {
                    if (_entries.RemoveAt(index))
                    {
                        return index;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Attempts to removes the specified item at the default index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The index where the item was set; -1 otherwise.
        /// </returns>
        public int Remove(T item, out T value)
        {
            return Remove(item, 0, out value);
        }

        /// <summary>
        /// Attempts to removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="offset">The offset from the default index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The index where the item was set; -1 otherwise.
        /// </returns>
        public int Remove(T item, int offset, out T value)
        {
            int index = Index(item, offset);
            T entry;
            if (_entries.TryGet(index, out entry))
            {
                if (_comparer.Equals(entry, item))
                {
                    if (_entries.RemoveAt(index))
                    {
                        value = entry;
                        return index;
                    }
                    else
                    {
                        value = default(T);
                        return -1;
                    }
                }
                else
                {
                    value = default(T);
                    return -1;
                }
            }
            else
            {
                value = default(T);
                return -1;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Tries the retrieve the item at an specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet(int index, out T item)
        {
            return _entries.TryGet(index, out item);
        }

        //HACK
        internal int Set(T item, int offset, out bool isNew)
        {
            int index = Index(item, offset);
            T oldEntry;
            isNew = !_entries.TryGet(index, out oldEntry);
            if ((isNew || _comparer.Equals(item, oldEntry)) && _entries.Set(index, item, out isNew))
            {
                return index;
            }
            else
            {
                return -1;
            }
        }
    }
}