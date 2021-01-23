#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;

namespace System.Collections.Concurrent
{
    [Serializable]
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private readonly ThreadSafeDictionary<TKey, TValue> _wrapped;

        // Odd use, but writes will take a read lock, and ToArray (and CopyTo) will take a write lock.
        // This class allows multiple writes concurrently. There is no reason to worry about concurrent reads other than ToArray (and CopyTo).
        // Also everything except ToArray (and CopyTo) is atomic.
        // Thus, there is no need for reads to take a lock.
        // Since multiple writes can enter they take a read lock.
        // And ToArray (and CopyTo) takes a write lock.
        // Using the slim version so we do not allocate a wait handle if we don't have to.
        // We technically leak it too.
        [NonSerialized]
        private ReaderWriterLockSlim? _lock;

        [NonSerialized]
        private ValueCollection<TKey, TValue>? _valueCollection;

        public ConcurrentDictionary()
            : this(4, 31, EqualityComparer<TKey>.Default)
        {
            // Empty
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity)
            : this(concurrencyLevel, capacity, EqualityComparer<TKey>.Default)
        {
            // Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(4, 31, EqualityComparer<TKey>.Default)
        {
            if ((collection ?? throw new ArgumentNullException(nameof(collection))).Any(pair => !_wrapped.TryAdd(pair.Key, pair.Value)))
            {
                throw new ArgumentException("The source contains duplicate keys.");
            }
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : this(4, 31, comparer)
        {
            // Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(4, 31, comparer)
        {
            if ((collection ?? throw new ArgumentNullException(nameof(collection))).Any(pair => !_wrapped.TryAdd(pair.Key, pair.Value)))
            {
                throw new ArgumentException("The source contains duplicate keys.");
            }
        }

        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(concurrencyLevel, 31, comparer)
        {
            if ((collection ?? throw new ArgumentNullException(nameof(collection))).Any(pair => !_wrapped.TryAdd(pair.Key, pair.Value)))
            {
                throw new ArgumentException("The source contains duplicate keys.");
            }
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            if (concurrencyLevel < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(concurrencyLevel), "concurrencyLevel < 1");
            }

            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "capacity < 0");
            }

            _wrapped = new ThreadSafeDictionary<TKey, TValue>(comparer ?? throw new ArgumentNullException(nameof(comparer)));
            _valueCollection = new ValueCollection<TKey, TValue>(this);
            _lock = new ReaderWriterLockSlim();
        }

        public int Count => _wrapped.Count;

        public bool IsEmpty => Count == 0;

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        public ICollection<TKey> Keys => _wrapped.Keys;

        ICollection IDictionary.Keys => (ICollection)_wrapped.Keys;

        object ICollection.SyncRoot => this;

        public ICollection<TValue> Values => GetValues();

        ICollection IDictionary.Values => GetValues();

        private ReaderWriterLockSlim Lock
        {
            get
            {
                return TypeHelper.LazyCreate(ref _lock, _wrapped);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                // key could be null
                if (key == null!)
                {
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException(nameof(key));
                }

                return _wrapped[key];
            }
            set
            {
                // key could be null
                if (key == null!)
                {
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException(nameof(key));
                }

                try
                {
                    Lock.EnterReadLock();
                    _wrapped.Set(key, value);
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        object? IDictionary.this[object? key]
        {
            get
            {
                switch (key)
                {
                    case null:
                        throw new ArgumentNullException(nameof(key));

                    case TKey keyAsTKey when _wrapped.TryGetValue(keyAsTKey, out var result):
                        return result;

                    default:
                        return null;
                }
            }
            set
            {
                switch (key)
                {
                    case null:
                        // ConcurrentDictionary hates null
                        throw new ArgumentNullException(nameof(key));

                    case TKey keyAsTKey when value is TValue valueAsTValue:
                        this[keyAsTKey] = valueAsTValue;
                        break;

                    default:
                        break;
                }

                throw new ArgumentException(string.Empty, nameof(value));
            }
        }

        void IDictionary.Add(object key, object value)
        {
            switch (key)
            {
                case null:
                    // key could be null
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException(nameof(key));
                case TKey keyAsTKey when value is TValue valueAsTValue:
                    try
                    {
                        Lock.EnterReadLock();
                        _wrapped.AddNew(keyAsTKey, valueAsTValue);
                    }
                    finally
                    {
                        Lock.ExitReadLock();
                    }

                    break;

                default:
                    break;
            }

            throw new ArgumentException(string.Empty, nameof(value));
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                _wrapped.AddNew(key, value);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            // key could be null
            if (item.Key == null!)
            {
                // ConcurrentDictionary hates null
                // While technically item is not null and item.Key is not an argument...
                // This is what happens when you do the call on Microsoft's implementation
                throw CreateArgumentNullExceptionKey(item.Key);
            }

            try
            {
                Lock.EnterReadLock();
                _wrapped.AddNew(item.Key, item.Value);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                // addValueFactory and updateValueFactory are checked for null inside the call
                return _wrapped.AddOrUpdate
                (
                    key,
                    addValueFactory,
                    updateValueFactory
                );
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                // updateValueFactory is checked for null inside the call
                return _wrapped.AddOrUpdate
                (
                    key,
                    addValue,
                    updateValueFactory
                );
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public void Clear()
        {
            try
            {
                Lock.EnterReadLock();
                // This should be an snapshot operation, however this is atomic.
                _wrapped.Clear();
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        bool IDictionary.Contains(object key)
        {
            switch (key)
            {
                case null:
                    // key could be null
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException(nameof(key));
                // keep the is operator
                case TKey keyAsTKey:
                    return ContainsKey(keyAsTKey);

                default:
                    return false;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            // key could be null
            if (item.Key == null!)
            {
                // ConcurrentDictionary hates null
                // While technically item is not null and item.Key is not an argument...
                // This is what happens when you do the call on Microsoft's implementation
                throw CreateArgumentNullExceptionKey(item.Key);
            }

            return _wrapped.TryGetValue(item.Key, out var found) && EqualityComparer<TValue>.Default.Equals(found, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            // No existing value is set, so no locking, right?
            return _wrapped.ContainsKey(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            // WORST API EVER - I shouldn't be supporting this
            // I'm checking size before checking type - I have no plans to fix that
            Extensions.CanCopyTo(_wrapped.Count, array, index);
            try
            {
                // This should be an snapshot operation, I'll make it so.
                // Please don't use this API.
                Lock.EnterWriteLock();
                switch (array)
                {
                    case KeyValuePair<TKey, TValue>[] pairs:
                        // most decent alternative
                        var keyValuePairs = pairs;
                        foreach (var pair in _wrapped)
                        {
                            keyValuePairs[index] = pair;
                            index++;
                        }

                        return;

                    case DictionaryEntry[] entries:
                        // that thing exists, I was totally unaware, I may as well use it.
                        var dictionaryEntries = entries;
                        foreach (var pair in _wrapped)
                        {
                            dictionaryEntries[index] = new DictionaryEntry(pair.Key, pair.Value);
                            index++;
                        }

                        return;

                    case object[] objects:
                        var valuePairs = objects;
                        foreach (var pair in _wrapped)
                        {
                            valuePairs[index] = pair;
                            index++;
                        }

                        return;

                    default:
                        // A.K.A ScrewYouException
                        throw new ArgumentException("Not supported array type");
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, nameof(array));
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            try
            {
                // This should be an snapshot operation
                Lock.EnterWriteLock();
                Extensions.CanCopyTo(Count, array, arrayIndex);
                this.CopyTo(array, arrayIndex);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                // valueFactory is checked for null inside the call
                return _wrapped.GetOrAdd(key, valueFactory);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                return _wrapped.GetOrAdd(key, value);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        void IDictionary.Remove(object key)
        {
            switch (key)
            {
                case null:
                    // key could be null
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException(nameof(key));
                case TKey keyAsTKey:
                    try
                    {
                        Lock.EnterReadLock();
                        _wrapped.Remove(keyAsTKey);
                    }
                    finally
                    {
                        Lock.ExitReadLock();
                    }

                    break;

                default:
                    break;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            // key could be null
            if (item.Key == null!)
            {
                // ConcurrentDictionary hates null
                // While technically item is not null and item.Key is not an argument...
                // This is what happens when you do the call on Microsoft's implementation
                throw CreateArgumentNullExceptionKey(item.Key);
            }

            try
            {
                Lock.EnterReadLock();
                return _wrapped.Remove(item.Key, input => EqualityComparer<TValue>.Default.Equals(input, item.Value), out _);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return TryRemove(key, out _);
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            try
            {
                Lock.EnterWriteLock();
                // This should be an snapshot operation
                var result = new List<KeyValuePair<TKey, TValue>>(_wrapped.Count);
                result.AddRange(_wrapped);
                return result.ToArray();
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                return _wrapped.TryAdd(key, value);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            return _wrapped.TryGetValue(key, out value);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                return _wrapped.Remove(key, out value);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                Lock.EnterReadLock();
                return _wrapped.TryUpdate(key, newValue, comparisonValue);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        private static ArgumentNullException CreateArgumentNullExceptionKey(TKey key)
        {
            _ = key;
            return new ArgumentNullException(nameof(key));
        }

        private ValueCollection<TKey, TValue> GetValues()
        {
            return TypeHelper.LazyCreate(ref _valueCollection, () => new ValueCollection<TKey, TValue>(this), _wrapped);
        }

        private sealed class DictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> _wrapped;

            internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> wrapped)
            {
                _wrapped = wrapped.GetEnumerator();
            }

            public object Current => Entry;

            public DictionaryEntry Entry
            {
                get
                {
                    var current = _wrapped.Current;
                    return new DictionaryEntry(current.Key, current.Value);
                }
            }

            public object Key
            {
                get
                {
                    var current = _wrapped.Current;
                    return current.Key!;
                }
            }

            public object? Value
            {
                get
                {
                    var current = _wrapped.Current;
                    return current.Value;
                }
            }

            public bool MoveNext()
            {
                if (_wrapped.MoveNext())
                {
                    return true;
                }

                // The DictionaryEnumerator of ConcurrentDictionary is not IDisposable...
                // Which means this method takes the responsibility of disposing
                _wrapped.Dispose();
                return false;
            }

            public void Reset()
            {
                // This is two levels deep of you should not be using this.
                // You should not be using DictionaryEnumerator
                // And you should not be calling Reset
                throw new NotSupportedException();
            }
        }
    }
}

#endif