// Needed for NET40

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    ///     Represent a fixed size thread-safe wait-free queue.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the queue.</typeparam>
    [Serializable]
    public sealed class FixedSizeQueue<T> : IProducerConsumerCollection<T>
    {
        private readonly FixedSizeBucket<T> _entries;
        private int _indexDequeue;
        private int _indexEnqueue;
        private int _preCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeQueue{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public FixedSizeQueue(int capacity)
        {
            Capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _preCount = 0;
            _indexEnqueue = 0;
            _indexDequeue = 0;
            _entries = new FixedSizeBucket<T>(Capacity);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeQueue{T}" /> class.
        /// </summary>
        public FixedSizeQueue(IEnumerable<T> source)
        {
            _indexDequeue = 0;
            _entries = new FixedSizeBucket<T>(source);
            Capacity = _entries.Capacity;
            _indexEnqueue = _entries.Count;
            _preCount = _indexEnqueue;
        }

        /// <summary>
        ///     Gets the capacity.
        /// </summary>
        public int Capacity { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the number of items actually contained.
        /// </summary>
        public int Count => _entries.Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        public void CopyTo(T[] array, int index)
        {
            _entries.CopyTo(array, index);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an <see cref="IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        public T[] ToArray()
        {
            return this.ToArray(Count);
        }

        /// <summary>
        ///     Attempts to Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <c>true</c> if the item was added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(T item)
        {
            if (_entries.Count >= Capacity)
            {
                return false;
            }

            var preCount = Interlocked.Increment(ref _preCount);
            if (preCount <= Capacity)
            {
                var index = (Interlocked.Increment(ref _indexEnqueue) - 1) & (Capacity - 1);
                if (_entries.InsertInternal(index, item))
                {
                    return true;
                }
            }

            Interlocked.Decrement(ref _preCount);
            return false;
        }

        /// <summary>
        ///     Attempts to retrieve and remove the next item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <c>true</c> if the item was taken; otherwise, <c>false</c>.
        /// </returns>
        public bool TryTake(out T item)
        {
            if (_entries.Count > 0)
            {
                var preCount = Interlocked.Decrement(ref _preCount);
                if (preCount >= 0)
                {
                    var index = (Interlocked.Increment(ref _indexDequeue) - 1) & (Capacity - 1);
                    if (_entries.RemoveAtInternal(index, out item))
                    {
                        return true;
                    }
                }

                Interlocked.Increment(ref _preCount);
            }

            item = default!;
            return false;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Returns the next item to be taken without removing it.
        /// </summary>
        /// <returns>The next item to be taken.</returns>
        /// <exception cref="InvalidOperationException">No more items to be taken.</exception>
        public T Peek()
        {
            var index = Interlocked.Add(ref _indexEnqueue, 0);
            if (index < Capacity && index > 0 && _entries.TryGet(index, out var item))
            {
                return item;
            }

            throw new InvalidOperationException("Empty");
        }

        /// <summary>
        ///     Attempts to retrieve the next item to be taken without removing it.
        /// </summary>
        /// <param name="item">The item retrieved.</param>
        /// <returns>
        ///     <c>true</c> if an item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryPeek(out T item)
        {
            item = default!;
            var index = Interlocked.Add(ref _indexDequeue, 0);
            return index < Capacity && index > 0 && _entries.TryGetInternal(index, out item);
        }
    }
}