using System.Collections.Generic;
using System.Net.Configuration;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Collections.Concurrent
{
    [SerializableAttribute]
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        private const int INT_DefaultCapacity = 31;
        private const int INT_DefaultConcurrency = 4;
        private readonly LockableContext _context;
        private readonly Lockable _master;
        private readonly HashBucket<TKey, LockableNeedle<TValue, Needle<TValue>>> _wrapped;

        public ConcurrentDictionary()//
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
            else
            {
                AddRange(collection, true);
            }
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
            else
            {
                AddRange(collection, true);
            }
        }

        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(concurrencyLevel, INT_DefaultCapacity, comparer)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }
            else
            {
                AddRange(collection, false);
            }
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
        {
            if (ReferenceEquals(comparer, null))
            {
                throw new ArgumentNullException("comparer");
            }
            else if (concurrencyLevel < 1)
            {
                throw new ArgumentOutOfRangeException("concurrencyLevel", "concurrencyLevel < 1");
            }
            else if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "capacity < 0");
            }
            else
            {
                if (capacity < concurrencyLevel)
                {
                    capacity = concurrencyLevel;
                }
                _context = new LockableContext(concurrencyLevel);
                _wrapped = new HashBucket<TKey, LockableNeedle<TValue, Needle<TValue>>>();
                _master = new Lockable(_context);

            }
        }

        public int Count
        {
            get
            {
                if (AcquireMaster() && AcquireAllLocks())
                {
                    return _wrapped.Count;
                }
                else
                {
                    
                }
            }
        }

        private bool AcquireMaster()
        {
            if (_master.Capture())
            {
                return true;
            }
            else
            {
                _master.Uncapture();
                return false;
            }
        }

        private bool AcquireAllLocks()
        {
            bool ok = true;
            var got = new List<LockableNeedle<TValue, Needle<TValue>>>();
            foreach (var resource in _wrapped)
            {
                if (!resource.Value.Capture())
                {
                    ok = false;
                }
                else
                {
                    got.Add(resource.Value);
                }
            }
            if (!ok)
            {
                foreach (var value in got)
                {
                    value.Uncapture();
                }
            }
            return ok;
        }

        ICollection IDictionary.Keys
        {
            get { throw new NotImplementedException(); }
        }

        ICollection IDictionary.Values
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEmpty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
            }
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

        public object this[object key]
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

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
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

        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection, bool growLockArray)
        {
            throw new NotImplementedException();
        }
    }
}