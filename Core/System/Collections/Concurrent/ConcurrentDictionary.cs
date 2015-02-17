using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace System.Collections.Concurrent
{
    [SerializableAttribute]
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private const int INT_DefaultCapacity = 31;
        private const int INT_DefaultConcurrency = 4;
        private readonly LockableContext _context;
        private readonly Pool<LockableNeedle<TValue>> _pool;
        private readonly ConvertedValueCollection<TKey, LockableNeedle<TValue>, TValue> _valueCollection;
        private readonly SafeDictionary<TKey, LockableNeedle<TValue>> _wrapped;

        public ConcurrentDictionary()
            : this(INT_DefaultConcurrency, INT_DefaultCapacity, EqualityComparer<TKey>.Default)
        {
            //Empty
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity)
            : this(concurrencyLevel, capacity, EqualityComparer<TKey>.Default)
        {
            //Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(INT_DefaultConcurrency, INT_DefaultCapacity, EqualityComparer<TKey>.Default)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }
            AddRange(collection);
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : this(INT_DefaultConcurrency, INT_DefaultCapacity, comparer)
        {
            //Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(INT_DefaultConcurrency, INT_DefaultCapacity, comparer)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }
            AddRange(collection);
        }

        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(concurrencyLevel, INT_DefaultCapacity, comparer)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }
            AddRange(collection);
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            if (ReferenceEquals(comparer, null))
            {
                throw new ArgumentNullException("comparer");
            }
            if (concurrencyLevel < 1)
            {
                throw new ArgumentOutOfRangeException("concurrencyLevel", "concurrencyLevel < 1");
            }
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "capacity < 0");
            }
            _context = new LockableContext(concurrencyLevel);
            _wrapped = new SafeDictionary<TKey, LockableNeedle<TValue>>();
            _valueCollection = new ConvertedValueCollection<TKey, LockableNeedle<TValue>, TValue>(_wrapped, input => input.Value);
            _pool = new Pool<LockableNeedle<TValue>>(64, Recycle);
        }

        public int Count
        {
            get
            {
                using (_context.Enter())
                {
                    AcquireAllLocks();
                    return _wrapped.Count;
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _wrapped.Keys;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return (ICollection)_wrapped.Keys;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return _valueCollection;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return _valueCollection;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                // key can be null
                if (ReferenceEquals(key, null))
                {
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException("key");
                }
                return _wrapped[key].Value;
            }
            set
            {
                if (ReferenceEquals(key, null))
                {
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException("key");
                }
                using (_context.Enter())
                {
                    LockableNeedle<TValue> created = null;
                    var stored = _wrapped.GetOrAdd(key, input => created = GetNeedle(value));
                    if (stored != created)
                    {
                        // created but not added
                        _pool.Donate(created);
                    }
                    stored.Value = value;
                }
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                // keep the is operator
                if (key is TKey)
                {
                    return _wrapped[(TKey)key].Value;
                }
                return null;
            }
            set
            {
                if (ReferenceEquals(key, null))
                {
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException("key");
                }
                // keep the is operator
                if (key is TKey && value is TValue)
                {
                    this[(TKey)key] = (TValue)value;
                }
                throw new ArgumentException();
            }
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // TODO: SafeDictionary implementation is pending
            using (_context.Enter())
            {
                return _wrapped.AddOrUpdate
                    (
                        key,
                        input => GetNeedle(addValueFactory(input)),
                        (inputKey, inputValue) =>
                        {
                            inputValue.Value = updateValueFactory(inputKey, inputValue.Value);
                            return inputValue;
                        }
                    ).Value;
            }
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // TODO: SafeDictionary implementation is pending
            using (_context.Enter())
            {
                return _wrapped.AddOrUpdate
                    (
                        key,
                        input => GetNeedle(addValue),
                        (inputKey, inputValue) =>
                        {
                            inputValue.Value = updateValueFactory(inputKey, inputValue.Value);
                            return inputValue;
                        }
                    ).Value;
            }
        }

        public void Clear()
        {
            using (_context.Enter())
            {
                AcquireAllLocks();
                _wrapped.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            // No existing value is set, so no locking, right?
            return _wrapped.ContainsKey(key);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            // No existing value is set, so no locking, right?
            return _wrapped.GetOrAdd(key, input => GetNeedle(valueFactory(input))).Value;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            // No existing value is set, so no locking, right?
            return _wrapped.GetOrAdd(key, input => GetNeedle(value)).Value;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            using (_context.Enter())
            {
                AcquireAllLocks();
                var result = new List<KeyValuePair<TKey, TValue>>(_wrapped.Count);
                foreach (var pair in _wrapped)
                {
                    result.Add(new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Value));
                }
                return result.ToArray();
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            // No existing value is set, so no locking, right?
            return _wrapped.TryAdd(key, GetNeedle(value));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            // TODO: Add TryUpdate to SafeDictionary and WeakDictionary
            throw new NotImplementedException();
        }

        private void AcquireAllLocks()
        {
            foreach (var resource in _wrapped)
            {
                resource.Value.CaptureAndWait();
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            using (_context.Enter())
            {
                LockableNeedle<TValue> created = GetNeedle(value);
                if (!_wrapped.TryAdd(key, created))
                {
                    _pool.Donate(created);
                    throw new ArgumentException("An item with the same key has already been added");
                }
            }
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            GC.KeepAlive(collection);
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private LockableNeedle<TValue> GetNeedle(TValue value)
        {
            LockableNeedle<TValue> result;
            if (_pool.TryGet(out result))
            {
                result.Value = value;
            }
            else
            {
                result = new LockableNeedle<TValue>(value, _context);
            }
            return result;
        }

        private void Recycle(LockableNeedle<TValue> obj)
        {
            obj.Free();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            using (_context.Enter())
            {
                // TODO: How locking should work here?
                return _wrapped.Remove(key);
            }
        }
    }
}