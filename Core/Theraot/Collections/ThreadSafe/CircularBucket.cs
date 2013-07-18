#if FAT

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represents a thread-safe wait-free fixed size circular bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    /// The items added an instance of this type will be overwritten after the entry point reaches the end of the bucket. This class was created for the purpose of storing in-memory logs for debugging threaded software.
    /// </remarks>
    public sealed class CircularBucket<T> : IEnumerable<T>
    {
        private readonly Bucket<T> _bucket;
        private readonly int _capacity;

        private int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public CircularBucket(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _index = -1;
            _bucket = new Bucket<T>(_capacity);
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
        /// Gets the number of items that has been added.
        /// </summary>
        public int Count
        {
            get
            {
                return _bucket.Count;
            }
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            return _bucket.GetValues();
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetValues<TOutput>(Converter<T, TOutput> converter)
        {
            return _bucket.GetValues<TOutput>(converter);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the position where the item was added.</returns>
        public int Add(T item)
        {
            var index = Interlocked.Increment(ref _index) & (_capacity - 1);
            bool isNew;
            _bucket.Set(index, item, out isNew);
            return index;
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _bucket.GetEnumerator();
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index)
        {
            return _bucket.RemoveAt(index);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Tries to retrieve the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool TryGet(int index, out T value)
        {
            return _bucket.TryGet(index, out value);
        }
    }
}

#endif