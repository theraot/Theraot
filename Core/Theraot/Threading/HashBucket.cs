using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading
{
    /// <summary>
    /// Represent a thread-safe lock-free hash based dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="IDictionary{TKey, TValue}" /> or any other desired interface.
    /// </remarks>
    public sealed class HashBucket<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int INT_DefaultCapacity = 64;
        private const int INT_DefaultMaxProbing = 1;
        private const int INT_SpinWaitHint = 80;

        private int _copyingThreads;
        private int _copyPosition;
        private int _count;
        private FixedSizeHashBucket<TKey, TValue> _entriesNew;
        private FixedSizeHashBucket<TKey, TValue> _entriesOld;
        private IEqualityComparer<TKey> _keyComparer;
        private int _maxProbing;
        private volatile int _revision;
        private int _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        public HashBucket()
            : this(INT_DefaultCapacity, EqualityComparer<TKey>.Default, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        public HashBucket(int capacity)
            : this(capacity, EqualityComparer<TKey>.Default, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="maxProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">maxProbing;maxProbing must be greater or equal to 1 and less than capacity.</exception>
        public HashBucket(int capacity, int maxProbing)
            : this(capacity, EqualityComparer<TKey>.Default, maxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The key comparer.</param>
        public HashBucket(IEqualityComparer<TKey> comparer)
            : this(INT_DefaultCapacity, comparer, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The key comparer.</param>
        /// <param name="maxProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">maxProbing;maxProbing must be greater or equal to 1 and less than capacity.</exception>
        public HashBucket(IEqualityComparer<TKey> comparer, int maxProbing)
            : this(INT_DefaultCapacity, comparer, maxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The key comparer.</param>
        public HashBucket(int capacity, IEqualityComparer<TKey> comparer)
            : this(capacity, comparer, INT_DefaultMaxProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashBucket{TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <param name="maxProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">maxProbing;maxProbing must be greater or equal to 1 and less than capacity.</exception>
        public HashBucket(int capacity, IEqualityComparer<TKey> comparer, int maxProbing)
        {
            if (maxProbing < 1 || maxProbing >= capacity)
            {
                throw new ArgumentOutOfRangeException("maxProbing", "maxProbing must be greater or equal to 1 and less than capacity.");
            }
            else
            {
                _keyComparer = comparer ?? EqualityComparer<TKey>.Default;
                _entriesOld = null;
                _entriesNew = new FixedSizeHashBucket<TKey, TValue>(capacity, _keyComparer);
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
        /// Gets the key comparer.
        /// </summary>
        public IEqualityComparer<TKey> KeyComparer
        {
            get
            {
                return _entriesNew.KeyComparer;
            }
        }

        /// <summary>
        /// Gets the keys and associated values contained in this object.
        /// </summary>
        public IList<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                return _entriesNew.Pairs;
            }
        }

        /// <summary>
        /// Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">the key is already present</exception>
        public void Add(TKey key, TValue value)
        {
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    bool isCollision = false;
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        if (AddExtracted(key, value, entries, out isCollision) != -1)
                        {
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe == 0)
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
                                    var oldStatus = Interlocked.CompareExchange(ref _status, 1, 0);
                                    if (oldStatus == 0)
                                    {
                                        _revision++;
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException("the key is already present");
                                }
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
            _entriesNew = new FixedSizeHashBucket<TKey, TValue>(INT_DefaultCapacity, _keyComparer);
            Thread.VolatileWrite(ref _status, 0);
            Thread.VolatileWrite(ref _count, 0);
            _revision++;
        }

        /// <summary>
        /// Determines whether the specified key is contained.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        if (ContainsKeyExtracted(key, entries))
                        {
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
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _entriesNew.GetKeyValuePairEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key)
        {
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        if (RemoveExtracted(key, entries))
                        {
                            result = true;
                        }
                    }
                    finally
                    {
                        var isOperationSafe = IsOperationSafe(entries, revision);
                        if (isOperationSafe == 0)
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

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(TKey key, TValue value)
        {
            while (true)
            {
                int revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    bool isNew;
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    if (SetExtracted(key, value, entries, out isNew) != -1)
                    {
                        if (IsOperationSafe(entries, revision) == 0)
                        {
                            if (isNew)
                            {
                                Interlocked.Increment(ref _count);
                            }
                            break;
                        }
                        else
                        {
                            int oldStatus = Interlocked.CompareExchange(ref _status, 1, 0);
                            if (oldStatus == 0)
                            {
                                _revision++;
                            }
                        }
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
        /// Tries to retrieve the key and associated value at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet(int index, out TKey key, out TValue value)
        {
            value = default(TValue);
            key = default(TKey);
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        TValue tmpValue;
                        TKey tmpKey;
                        if (TryGetExtracted(index, entries, out tmpKey, out tmpValue))
                        {
                            key = tmpKey;
                            value = tmpValue;
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
        /// Tries to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            bool result = false;
            int revision;
            while (true)
            {
                revision = _revision;
                if (IsOperationSafe() == 0)
                {
                    var entries = ThreadingHelper.VolatileRead(ref _entriesNew);
                    bool done = false;
                    try
                    {
                        TValue tmpValue;
                        if (TryGetValueExtracted(key, entries, out tmpValue))
                        {
                            value = tmpValue;
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

        private int AddExtracted(TKey key, TValue value, FixedSizeHashBucket<TKey, TValue> entries, out bool isCollision)
        {
            isCollision = true;
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    int index = entries.Add(key, value, attempts, out isCollision);
                    if (index != -1 || !isCollision)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        private bool ContainsKeyExtracted(TKey key, FixedSizeHashBucket<TKey, TValue> entries)
        {
            for (int attempts = 0; attempts < _maxProbing; attempts++)
            {
                if (entries.ContainsKey(key, attempts) != -1)
                {
                    return true;
                }
            }
            return false;
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

                        // This area is only accessed by one thread, if that thread is aborted, we are doomed.
                        // This class is not abort safe, aside from a thread being aborted here, a thread being aborted on status == 2 will mean lost items
                        var priority = Thread.CurrentThread.Priority;
                        oldStatus = Interlocked.CompareExchange(ref _status, 2, 1);
                        if (oldStatus == 1)
                        {
                            try
                            {
                                // The progress of other threads depend of this one, we should not allow a priority inversion.
                                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                                Thread.VolatileWrite(ref _copyPosition, -1);
                                var newCapacity = _entriesNew.Capacity * 2;
                                _entriesOld = Interlocked.Exchange(ref _entriesNew, new FixedSizeHashBucket<TKey, TValue>(newCapacity, _keyComparer));
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

                    case 3:

                        // It is time to cooperate to copy the old storage to the new one
                        var old = _entriesOld;
                        if (old != null)
                        {
                            // This class is not abort safe, aside from a thread being aborted here (causing lost items) a thread being aborted on status == 1 will mean a livelock
                            _revision++;
                            Interlocked.Increment(ref _copyingThreads);
                            TKey key;
                            TValue value;
                            int index = Interlocked.Increment(ref _copyPosition);
                            for (; index < old.Capacity; index = Interlocked.Increment(ref _copyPosition))
                            {
                                if (old.TryGet(index, out key, out value))
                                {
                                    // We have read an item, so let's try to add it to the new storage
                                    bool dummy;
                                    if (SetExtracted(key, value, _entriesNew, out dummy) == -1)
                                    {
                                        GC.KeepAlive(dummy);
                                    }
                                }
                            }
                            Interlocked.CompareExchange(ref _status, 4, 3);
                            _revision++;
                            Interlocked.Decrement(ref _copyingThreads);
                        }
                        break;

                    case 4:

                        // Our copy is finished, we don't need the old storage anymore
                        oldStatus = Interlocked.CompareExchange(ref _status, 2, 4);
                        if (oldStatus == 4)
                        {
                            _revision++;
                            Interlocked.Exchange(ref _entriesOld, null);
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

        private bool RemoveExtracted(TKey key, FixedSizeHashBucket<TKey, TValue> entries)
        {
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    if (entries.Remove(key, attempts) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int SetExtracted(TKey key, TValue value, FixedSizeHashBucket<TKey, TValue> entries, out bool isNew)
        {
            isNew = false;
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    int index = entries.Set(key, value, attempts, out isNew);
                    if (index != -1)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        private bool TryGetExtracted(int index, FixedSizeHashBucket<TKey, TValue> entries, out TKey key, out TValue value)
        {
            value = default(TValue);
            key = default(TKey);
            if (entries != null)
            {
                if (entries.TryGet(index, out key, out value))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryGetValueExtracted(TKey key, FixedSizeHashBucket<TKey, TValue> entries, out TValue value)
        {
            value = default(TValue);
            if (entries != null)
            {
                for (int attempts = 0; attempts < _maxProbing; attempts++)
                {
                    if (entries.TryGetValue(key, attempts, out value) != -1)
                    {
                        return true;
                    }
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