using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe wait-free queue.
    /// </summary>
    [Serializable]
    public sealed class SafeQueue<T> : IEnumerable<T>
    {
        private readonly Mapper<T> _entries;
        private int _indexDequeue;
        private int _indexEnqueue;
        private int _preCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeQueue{T}" /> class.
        /// </summary>
        public SafeQueue()
        {
            _preCount = 0;
            _indexEnqueue = 0;
            _indexDequeue = 0;
            _entries = new Mapper<T>();
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
        /// Attempts to Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="InvalidOperationException">The queue is full. The capacity of the queue is 2^32 - consider a queue of queues.</exception>
        /// <remarks>This method throws when the queue is full (2^32 items) if that's a problem, consider using TryAdd.</remarks>
        public void Add(T item)
        {
            Interlocked.Increment(ref _preCount);
            var index = Interlocked.Increment(ref _indexEnqueue) - 1;
            if (_entries.Insert(index, item))
            {
                return;
            }
            Interlocked.Decrement(ref _preCount);
            throw new InvalidOperationException("The queue is full.");
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
        /// <exception cref="System.InvalidOperationException">No more items to be taken.</exception>
        public T Peek()
        {
            T item;
            int index = Interlocked.Add(ref _indexEnqueue, 0);
            if (index > 0 && _entries.TryGet(index, out item))
            {
                return item;
            }
            throw new InvalidOperationException("Empty");
        }

        /// <summary>
        /// Attempts to Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(T item)
        {
            Interlocked.Increment(ref _preCount);
            var index = Interlocked.Increment(ref _indexEnqueue) - 1;
            if (_entries.Insert(index, item))
            {
                return true;
            }
            Interlocked.Decrement(ref _preCount);
            return false;
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
        public bool TryPeek(out T item)
        {
            item = default(T);
            int index = Interlocked.Add(ref _indexEnqueue, 0);
            return index > 0 && _entries.TryGet(index, out item);
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
                    var index = Interlocked.Increment(ref _indexDequeue) - 1;
                    if (_entries.RemoveAt(index, out item))
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