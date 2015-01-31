using System.Collections.Generic;
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
        private readonly HashBucket<TKey, LockableNeedle<TValue>> _wrapped;

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
            _wrapped = new HashBucket<TKey, LockableNeedle<TValue>>();
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

        public ICollection<TKey> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
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

        ICollection IDictionary.Keys
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        ICollection IDictionary.Values
        {
            get { throw new NotImplementedException(); }
        }

        public TValue this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        object IDictionary.this[object key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            throw new NotImplementedException();
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            throw new NotImplementedException();
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(TKey key, TValue value)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void AcquireAllLocks()
        {
            foreach (var resource in _wrapped)
            {
                resource.Value.Capture();
            }
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
    }
}