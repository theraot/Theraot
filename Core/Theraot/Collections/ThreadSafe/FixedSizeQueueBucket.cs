using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe wait-free queue.
    /// </summary>
    internal sealed class FixedSizeQueueBucket<T> : IEnumerable<T>
    {
        private readonly Bucket<T> _bucket;
        private readonly int _capacity;

        private int _indexDequeue;
        private int _indexEnqueue;
        private int _preCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeQueueBucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public FixedSizeQueueBucket(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _preCount = 0;
            _indexEnqueue = 0;
            _indexDequeue = 0;
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
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _bucket.Count;
            }
        }

        /// <summary>
        /// Gets the index where the next item removed with TryDequeue will be taken from.
        /// </summary>
        /// <remarks>IndexDequeue increases each time a new item is removed with TryDequeue.</remarks>
        public int IndexDequeue
        {
            get
            {
                return Thread.VolatileRead(ref _indexDequeue) & (_capacity - 1);
            }

            //HACK
            internal set
            {
                _indexDequeue = value & (_capacity - 1);
            }
        }

        /// <summary>
        /// Gets the index where the last item added with Enqueue was placed.
        /// </summary>
        /// <remarks>IndexEnqueue increases each time a new item is added with Enqueue.</remarks>
        public int IndexEnqueue
        {
            get
            {
                return Thread.VolatileRead(ref _indexEnqueue) & (_capacity - 1);
            }

            //HACK
            internal set
            {
                _indexEnqueue = value & (_capacity - 1);
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
        /// Attempts to Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was added; otherwise, <c>false</c>.
        /// </returns>
        public bool Enqueue(T item)
        {
            if (_bucket.Count < _capacity)
            {
                var preCount = Interlocked.Increment(ref _preCount);
                if (preCount <= _capacity)
                {
                    var index = (Interlocked.Increment(ref _indexEnqueue) - 1) & (_capacity - 1);
                    if (_bucket.Insert(index, item))
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
            return _bucket.GetEnumerator();
        }

        /// <summary>
        /// Returns the next item to be taken from the back without removing it.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">No more items to be taken.</exception>
        public T Peek()
        {
            T item;
            int index = Interlocked.Add(ref _indexEnqueue, 0);
            if (index < _capacity && index > 0 && _bucket.TryGet(index, out item))
            {
                return item;
            }
            else
            {
                throw new System.InvalidOperationException("Empty");
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Attempts to retrieve and remove the next item from the back.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was taken; otherwise, <c>false</c>.
        /// </returns>
        public bool TryDequeue(out T item)
        {
            if (_bucket.Count > 0)
            {
                var preCount = Interlocked.Decrement(ref _preCount);
                if (preCount >= 0)
                {
                    var index = (Interlocked.Increment(ref _indexDequeue) - 1) & (_capacity - 1);
                    if (_bucket.RemoveAt(index, out item))
                    {
                        return true;
                    }
                }
                Interlocked.Increment(ref _preCount);
            }
            item = default(T);
            return false;
        }

        /// <summary>
        /// Tries the retrieve the item at an specified index.
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
            return _bucket.TryGet(index, out item);
        }

        //HACK
        internal bool Set(int index, T item, out bool isNew)
        {
            if (_bucket.Set(index, item, out isNew))
            {
                if (isNew)
                {
                    Interlocked.Increment(ref _preCount);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}