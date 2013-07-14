#if FAT
﻿using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading
{
    /// <summary>
    /// Represent a thread-safe lock-free queue.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public sealed class QueueBucket<T> : IEnumerable<T>
    {
        private const int INT_DefaultCapacity = 64;
        private const int INT_SpinWaitHint = 80;

        private object _synclock = new object();
        private int _copyingThreads;
        private int _workingThreads;
        private int _copySourcePosition;
        private int _count;
        private FixedSizeQueueBucket<T> _entriesNew;
        private FixedSizeQueueBucket<T> _entriesOld;
        private volatile int _revision;
        private int _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueBucket{T}" /> class.
        /// </summary>
        public QueueBucket()
            : this(INT_DefaultCapacity)
        {
            //Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueBucket{T}" /> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public QueueBucket(int initialCapacity)
        {
            _entriesOld = null;
            _entriesNew = new FixedSizeQueueBucket<T>(initialCapacity);
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _entriesNew.Capacity;
            }
        }

        /// <summary>
        /// Gets the number of keys actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Gets the items contained in this object.
        /// </summary>
        public IList<T> Values
        {
            get
            {
                return _entriesNew.Values;
            }
        }

        /// <summary>
        /// Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Enqueue(T item)
        {
            bool result = false;
            while (true)
            {
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        Interlocked.Increment(ref _workingThreads);
                        if (entries.Enqueue(item))
                        {
                            result = true;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _workingThreads);
                        if (result)
                        {
                            Interlocked.Increment(ref _count);
                            done = true;
                        }
                        else
                        {
                            var oldStatus = Interlocked.CompareExchange(ref _status, 1, 0);
                            if (oldStatus == 0)
                            {
                                _revision++;
                            }
                        }
                    }
                    if (done)
                    {
                        return;
                    }
                }
                else
                {
                    CooperativeGrow();
                }
            }
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            _entriesOld = null;
            _entriesNew = new FixedSizeQueueBucket<T>(INT_DefaultCapacity);
            Thread.VolatileWrite(ref _status, 0);
            Thread.VolatileWrite(ref _count, 0);
            _revision++;
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entriesNew.GetEnumerator();
        }

        /// <summary>
        /// Returns the next item to be taken from the back without removing it.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">No more items to be taken.</exception>
        public T Peek(T item)
        {
            T result = default(T);
            while (true)
            {
                int revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        result = entries.Peek();
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe == 0)
                        {
                            done = true;
                        }
                    }
                    if (done)
                    {
                        return result;
                    }
                }
                else
                {
                    CooperativeGrow();
                }
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
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Although items are ordered, they are not guaranteed to start at index 0.
        /// </remarks>
        public bool TryGet(int index, out T item)
        {
            item = default(T);
            bool result = false;
            while (true)
            {
                int revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        T tmpItem;
                        if (entries.TryGet(index, out tmpItem))
                        {
                            item = tmpItem;
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe == 0)
                        {
                            done = true;
                        }
                    }
                    if (done)
                    {
                        return result;
                    }
                }
                else
                {
                    CooperativeGrow();
                }
            }
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
            item = default(T);
            bool result = false;
            while (true)
            {
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        Interlocked.Increment(ref _workingThreads);
                        T tmpItem;
                        if (entries.TryDequeue(out tmpItem))
                        {
                            item = tmpItem;
                            result = true;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _workingThreads);
                        if (result)
                        {
                            Interlocked.Decrement(ref _count);
                        }
                        done = true;
                    }
                    if (done)
                    {
                        return result;
                    }
                }
                else
                {
                    CooperativeGrow();
                }
            }
        }

        private void CooperativeGrow()
        {
            int status = 0;
            do
            {
                status = Thread.VolatileRead(ref _status);
                int oldStatus;
                switch (status)
                {
                    case 1:
                        var priority = Thread.CurrentThread.Priority;
                        oldStatus = Interlocked.CompareExchange(ref _status, 2, 1);
                        if (oldStatus == 1)
                        {
                            try
                            {
                                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                                Thread.VolatileWrite(ref _copySourcePosition, -1);
                                var newCapacity = _entriesNew.Capacity * 2;
                                _entriesOld = Interlocked.Exchange(ref _entriesNew, new FixedSizeQueueBucket<T>(newCapacity));
                                oldStatus = Interlocked.CompareExchange(ref _status, 3, 2);
                            }
                            finally
                            {
                                Thread.CurrentThread.Priority = priority;
                                _revision++;
                            }
                        }
                        break;

                    case 2:
                        Thread.SpinWait(INT_SpinWaitHint);
                        if (Thread.VolatileRead(ref _status) == 2)
                        {
                            Thread.Sleep(0);
                        }
                        break;

                    case 3:
                        _revision++;
                        if (Thread.VolatileRead(ref _workingThreads) > 0)
                        {
                            Thread.SpinWait(INT_SpinWaitHint);
                            while (Thread.VolatileRead(ref _workingThreads) > 0)
                            {
                                Thread.Sleep(0);
                                Thread.SpinWait(INT_SpinWaitHint);
                            }
                        }
                        var old = _entriesOld;
                        if (old != null)
                        {
                            _revision++;
                            Interlocked.Increment(ref _copyingThreads);
                            T item;

                            int capacity = old.Capacity;
                            int offset = old.IndexDequeue;

                            int sourceIndex = Interlocked.Increment(ref _copySourcePosition);
                            while (sourceIndex < capacity)
                            {
                                if (old.TryGet((sourceIndex + offset) & (capacity - 1), out item))
                                {
                                    //HACK
                                    bool dummy;
                                    _entriesNew.Set(sourceIndex, item, out dummy);
                                }
                                sourceIndex = Interlocked.Increment(ref _copySourcePosition);
                            }
                            Interlocked.CompareExchange(ref _status, 2, 3);
                            _revision++;
                            if (Interlocked.Decrement(ref _copyingThreads) == 0)
                            {
                                //HACK
                                _entriesNew.IndexEnqueue = capacity;
                                Interlocked.CompareExchange(ref _status, 4, 2);
                            }
                        }
                        break;

                    case 4:
                        oldStatus = Interlocked.CompareExchange(ref _status, 2, 4);
                        if (oldStatus == 4)
                        {
                            _revision++;
                            Interlocked.Exchange(ref _entriesOld, null);
                            Thread.Sleep(1);
                            Interlocked.CompareExchange(ref _status, 0, 2);
                        }
                        break;

                    default:
                        break;
                }
            }
            while (status != 0);
        }

        private int IsOperationSafe(object entries, int revision)
        {
            int result = 5;
            bool check = _revision != revision;
            if (check)
            {
                result = 4;
            }
            else
            {
                var newEntries = Interlocked.CompareExchange(ref _entriesNew, null, null);
                if (entries != newEntries)
                {
                    result = 3;
                }
                else
                {
                    var newStatus = Interlocked.CompareExchange(ref _status, 0, 0);
                    if (newStatus != 0)
                    {
                        result = 2;
                    }
                    else
                    {
                        if (Thread.VolatileRead(ref _copyingThreads) > 0)
                        {
                            _revision++;
                            result = 1;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                }
            }
            return result;
        }

        private int IsOperationSafe()
        {
            var newStatus = Interlocked.CompareExchange(ref _status, 0, 0);
            if (newStatus != 0)
            {
                return 2;
            }
            else
            {
                if (Thread.VolatileRead(ref _copyingThreads) > 0)
                {
                    _revision++;
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
#endif