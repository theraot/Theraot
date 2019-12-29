// In theory I should remove this type, however I find it too good not to have

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    ///     Represents a thread-safe wait-free fixed size circular bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    ///     The items added an instance of this type will be overwritten after the entry point reaches the end of the bucket.
    ///     This class was created for the purpose of storing in-memory logs for debugging threaded software.
    /// </remarks>
    public sealed class CircularBucket<T> : IEnumerable<T>
    {
        private readonly FixedSizeBucket<T> _entries;

        private int _index;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public CircularBucket(int capacity)
        {
            Capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _index = -1;
            _entries = new FixedSizeBucket<T>(Capacity);
        }

        /// <summary>
        ///     Gets the capacity.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        ///     Gets the number of items that has been added.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        ///     Adds the specified item. May overwrite an existing item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the position where the item was added.</returns>
        public int Add(T item)
        {
            var index = Interlocked.Increment(ref _index) & (Capacity - 1);
            _entries.SetInternal(index, item, out _);
            return index;
        }

        /// <summary>
        ///     Returns an <see cref="IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index)
        {
            return _entries.RemoveAt(index);
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///     <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index, out T previous)
        {
            return _entries.RemoveAt(index, out previous);
        }

        /// <summary>
        ///     Tries to retrieve the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool TryGet(int index, out T value)
        {
            return _entries.TryGet(index, out value);
        }
    }
}