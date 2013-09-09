#if FAT

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe lock-free hash based set.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="ISet{T}" /> or any other desired interface.
    /// </remarks>
    public sealed class SetBucket<T> : IEnumerable<T>, ICollection<T>
    {
        private const int INT_DefaultCapacity = 64;
        private const int INT_DefaultMaxProbing = 1;
        private const int INT_SpinWaitHint = 80;

        private IEqualityComparer<T> _comparer;
        private int _copyingThreads;
        private int _copyPosition;
        private int _count;
        private FixedSizeSetBucket<T> _entriesNew;
        private FixedSizeSetBucket<T> _entriesOld;
        private int _maxProbing;
        private volatile int _revision;
        private int _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        public SetBucket()
            : this(INT_DefaultCapacity, EqualityComparer<T>.Default, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        public SetBucket(int capacity)
            : this(capacity, EqualityComparer<T>.Default, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="maxProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">maxProbing;maxProbing must be greater or equal to 1 and less than capacity.</exception>
        public SetBucket(int capacity, int maxProbing)
            : this(capacity, EqualityComparer<T>.Default, maxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public SetBucket(IEqualityComparer<T> comparer)
            : this(INT_DefaultCapacity, comparer, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="maxProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">maxProbing;maxProbing must be greater or equal to 1 and less than capacity.</exception>
        public SetBucket(IEqualityComparer<T> comparer, int maxProbing)
            : this(INT_DefaultCapacity, comparer, maxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public SetBucket(int capacity, IEqualityComparer<T> comparer)
            : this(capacity, comparer, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The comparer.</param>
        /// <param name="maxProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">maxProbing;maxProbing must be greater or equal to 1 and less than capacity.</exception>
        public SetBucket(int capacity, IEqualityComparer<T> comparer, int maxProbing)
        {
            if (maxProbing < 1 || maxProbing >= capacity)
            {
                throw new ArgumentOutOfRangeException("maxProbing", "maxProbing must be greater or equal to 1 and less than capacity.");
            }
            else
            {
                _comparer = comparer ?? EqualityComparer<T>.Default;
                _entriesOld = null;
                _entriesNew = new FixedSizeSetBucket<T>(capacity, _comparer);
                _maxProbing = maxProbing;
            }
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
        /// Gets the item comparer.
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return _entriesNew.Comparer;
            }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the specified element is added; otherwise, <c>false</c>.
        /// </returns>
        public bool Add(T item)
        {
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe())
                {
                    bool isCollision = false;
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        if (AddExtracted(item, entries, out isCollision) != -1)
                        {
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe)
                        {
                            if (result)
                            {
                                Interlocked.Increment(ref _count);
                                done = true;
                            }
                            else
                            {
                                if (isCollision)
                                {
                                    var oldStatus = Interlocked.CompareExchange(ref _status, (int)BucketStatus.GrowRequested, (int)BucketStatus.Free);
                                    if (oldStatus == (int)BucketStatus.Free)
                                    {
                                        _revision++;
                                    }
                                }
                                else
                                {
                                    done = true;
                                }
                            }
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
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            BucketHelper.Recycle(ref _entriesOld);
            var displaced = Interlocked.Exchange(ref _entriesNew, new FixedSizeSetBucket<T>(INT_DefaultCapacity, _comparer));
            BucketHelper.Recycle(ref displaced);
            Thread.VolatileWrite(ref _status, (int)BucketStatus.Free);
            Thread.VolatileWrite(ref _count, 0);
            _revision++;
        }

        /// <summary>
        /// Determines whether the specified item is contained.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the specified item is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe())
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        if (ContainsExtracted(item, entries))
                        {
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe)
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

        public void CopyTo(T[] array, int arrayIndex)
        {
            var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
            entries.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entriesNew.GetEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Gets the items and associated values contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            return _entriesNew.GetValues();
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetValues<TOutput>(Converter<T, TOutput> converter)
        {
            return _entriesNew.GetValues<TOutput>(converter);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the specified item was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(T item)
        {
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe())
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        if (RemoveExtracted(item, entries))
                        {
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe)
                        {
                            if (result)
                            {
                                Interlocked.Decrement(ref _count);
                            }
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
        /// Tries to retrieve the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet(int index, out T item)
        {
            item = default(T);
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe())
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        T tmpItem;
                        if (TryGetExtracted(index, entries, out tmpItem))
                        {
                            item = tmpItem;
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe)
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

        private int AddExtracted(T item, FixedSizeSetBucket<T> entries, out bool isCollision)
        {
            isCollision = true;
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    int index = entries.Add(item, attempts, out isCollision);
                    if (index != -1 || !isCollision)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        private bool ContainsExtracted(T item, FixedSizeSetBucket<T> entries)
        {
            for (int attempts = 0; attempts < _maxProbing; attempts++)
            {
                if (entries.Contains(item, attempts) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        private void CooperativeGrow()
        {
            int status;
            do
            {
                status = Thread.VolatileRead(ref _status);
                int oldStatus;
                switch (status)
                {
                    case (int)BucketStatus.GrowRequested:

                        // This area is only accessed by one thread, if that thread is aborted, we are doomed.
                        // This class is not abort safe
                        // If a thread is being aborted here it's pending operation will be lost and there is risk of a livelock
                        var priority = Thread.CurrentThread.Priority;
                        oldStatus = Interlocked.CompareExchange(ref _status, (int)BucketStatus.Waiting, (int)BucketStatus.GrowRequested);
                        if (oldStatus == (int)BucketStatus.GrowRequested)
                        {
                            try
                            {
                                // The progress of other threads depend of this one, we should not allow a priority inversion.
                                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                                //_copyPosition is set to -1. _copyPosition is incremented before it is used, so the first time it is used it will be 0.
                                Thread.VolatileWrite(ref _copyPosition, -1);
                                //The new capacity is twice the old capacity, the capacity must be a power of two.;
                                var newCapacity = _entriesNew.Capacity * 2;
                                _entriesOld = Interlocked.Exchange(ref _entriesNew, new FixedSizeSetBucket<T>(newCapacity, _comparer));
                                oldStatus = Interlocked.CompareExchange(ref _status, (int)BucketStatus.Copy, (int)BucketStatus.Waiting);
                            }
                            finally
                            {
                                Thread.CurrentThread.Priority = priority;
                                _revision++;
                            }
                        }
                        break;

                    case (int)BucketStatus.Waiting:

                        // This is the whole reason why this datastructure is not wait free.
                        // Testing shows that it is uncommon that a thread enters here.
                        // _status is 2 only for a short period.
                        // Still, it happens, so this is needed for correctness.
                        // Going completely wait-free adds complexity with deminished value.
                        Thread.SpinWait(INT_SpinWaitHint);
                        if (Thread.VolatileRead(ref _status) == 2)
                        {
                            Thread.Sleep(0);
                        }
                        break;

                    case (int)BucketStatus.Copy:

                        // It is time to cooperate to copy the old storage to the new one
                        var old = _entriesOld;
                        if (old != null)
                        {
                            // This class is not abort safe
                            // If a thread is being aborted here it will causing lost items.
                            _revision++;
                            Interlocked.Increment(ref _copyingThreads);
                            T item;
                            int index = Interlocked.Increment(ref _copyPosition);
                            for (; index < old.Capacity; index = Interlocked.Increment(ref _copyPosition))
                            {
                                if (old.TryGet(index, out item))
                                {
                                    // We have read an item, so let's try to add it to the new storage
                                    bool dummy;

                                    //HACK
                                    if (SetExtracted(item, _entriesNew, out dummy) == -1)
                                    {
                                        GC.KeepAlive(dummy);
                                    }
                                }
                            }
                            Interlocked.CompareExchange(ref _status, (int)BucketStatus.CopyCleanup, (int)BucketStatus.Copy);
                            _revision++;
                            Interlocked.Decrement(ref _copyingThreads);
                        }
                        break;

                    case (int)BucketStatus.CopyCleanup:

                        // Our copy is finished, we don't need the old storage anymore
                        oldStatus = Interlocked.CompareExchange(ref _status, (int)BucketStatus.Waiting, (int)BucketStatus.CopyCleanup);
                        if (oldStatus == (int)BucketStatus.CopyCleanup)
                        {
                            _revision++;
                            Interlocked.Exchange(ref _entriesOld, null);
                            Interlocked.CompareExchange(ref _status, (int)BucketStatus.Free, (int)BucketStatus.Waiting);
                        }
                        break;

                    default:
                        break;
                }
            }
            while (status != (int)BucketStatus.Free);
        }

        private bool IsOperationSafe(object entries, int revision)
        {
            bool check = _revision != revision;
            if (check)
            {
                return false;
            }
            else
            {
                var newEntries = Interlocked.CompareExchange(ref _entriesNew, null, null);
                if (entries == newEntries)
                {
                    var newStatus = Interlocked.CompareExchange(ref _status, (int)BucketStatus.Free, (int)BucketStatus.Free);
                    if (newStatus == (int)BucketStatus.Free)
                    {
                        if (Thread.VolatileRead(ref _copyingThreads) > 0)
                        {
                            _revision++;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsOperationSafe()
        {
            var newStatus = Interlocked.CompareExchange(ref _status, (int)BucketStatus.Free, (int)BucketStatus.Free);
            if (newStatus == (int)BucketStatus.Free)
            {
                if (Thread.VolatileRead(ref _copyingThreads) > 0)
                {
                    _revision++;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool RemoveExtracted(T item, FixedSizeSetBucket<T> entries)
        {
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    if (entries.Remove(item, attempts) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //HACK
        private int SetExtracted(T item, FixedSizeSetBucket<T> entries, out bool isNew)
        {
            isNew = false;
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    int index = entries.Set(item, attempts, out isNew);
                    if (index != -1)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        private bool TryGetExtracted(int index, FixedSizeSetBucket<T> entries, out T item)
        {
            item = default(T);
            if (entries != null)
            {
                if (entries.TryGet(index, out item))
                {
                    return true;
                }
            }
            return false;
        }

        // This class will not shrink, the reason for this is that shrinking may fail, supporting it may require to add locks. [Not solved problem]
        // Enumerating this class gives no guaranties:
        //  Items may be added or removed during enumeration without causing an exception.
        //  A version mechanism is not in place.
        //  This can be added by a wrapper.
    }
}

#endif