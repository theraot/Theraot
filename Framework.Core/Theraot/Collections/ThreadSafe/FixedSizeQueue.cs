// Needed for NET40

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe wait-free queue.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the queue.</typeparam>
    public sealed class FixedSizeQueue<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private readonly FixedSizeBucket<T> _entries;
        private int _indexDequeue;
        private int _indexEnqueue;
        private int _preCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeQueue{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public FixedSizeQueue(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _preCount = 0;
            _indexEnqueue = 0;
            _indexDequeue = 0;
            _entries = new FixedSizeBucket<T>(_capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeQueue{T}" /> class.
        /// </summary>
        public FixedSizeQueue(IEnumerable<T> source)
        {
            _indexDequeue = 0;
            _entries = new FixedSizeBucket<T>(source);
            _capacity = _entries.Capacity;
            _indexEnqueue = _entries.Count;
            _preCount = _indexEnqueue;
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        /// Attempts to Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was added; otherwise, <c>false</c>.
        /// </returns>
        public bool Add(T item)
        {
            if (_entries.Count < _capacity)
            {
                var preCount = Interlocked.Increment(ref _preCount);
                if (preCount <= _capacity)
                {
                    var index = (Interlocked.Increment(ref _indexEnqueue) - 1) & (_capacity - 1);
                    if (_entries.InsertInternal(index, item))
                    {
                        return true;
                    }
                }
                Interlocked.Decrement(ref _preCount);
            }
            return false;
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        /// <summary>
        /// Returns the next item to be taken from the back without removing it.
        /// </summary>
        /// <returns>The next item to be taken from the back.</returns>
        /// <exception cref="System.InvalidOperationException">No more items to be taken.</exception>
        public T Peek()
        {
            T item;
            var index = Interlocked.Add(ref _indexEnqueue, 0);
            if (index < _capacity && index > 0 && _entries.TryGet(index, out item))
            {
                return item;
            }
            throw new InvalidOperationException("Empty");
        }

        /// <summary>
        /// Attempts to retrieve the item at an specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Although items are ordered, they are not guaranteed to start at index 0.
        /// </remarks>
        public bool TryGet(int index, out T item)
        {
            return _entries.TryGet(index, out item);
        }

        /// <summary>
        /// Attempts to retrieve the next item to be taken from the back without removing it.
        /// </summary>
        /// <param name="item">The item retrieved.</param>
        /// <returns>
        ///   <c>true</c> if an item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryPeek(out T item)
        {
            item = default(T);
            var index = Interlocked.Add(ref _indexDequeue, 0);
            return index < _capacity && index > 0 && _entries.TryGetInternal(index, out item);
        }

        /// <summary>
        /// Attempts to retrieve and remove the next item from the back.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was taken; otherwise, <c>false</c>.
        /// </returns>
        public bool TryTake(out T item)
        {
            if (_entries.Count > 0)
            {
                var preCount = Interlocked.Decrement(ref _preCount);
                if (preCount >= 0)
                {
                    var index = (Interlocked.Increment(ref _indexDequeue) - 1) & (_capacity - 1);
                    if (_entries.RemoveAtInternal(index, out item))
                    {
                        return true;
                    }
                }
                Interlocked.Increment(ref _preCount);
            }
            item = default(T);
            return false;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}