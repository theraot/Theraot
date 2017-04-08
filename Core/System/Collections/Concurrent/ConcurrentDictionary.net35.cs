#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;

namespace System.Collections.Concurrent
{
    [SerializableAttribute]
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private readonly ValueCollection<TKey, TValue> _valueCollection;
        private readonly SafeDictionary<TKey, TValue> _wrapped;

        public ConcurrentDictionary()
            : this(4, 31, EqualityComparer<TKey>.Default)
        {
            //Empty
        }

        public ConcurrentDictionary(int concurrencyLevel, int capacity)
            : this(concurrencyLevel, capacity, EqualityComparer<TKey>.Default)
        {
            //Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(4, 31, EqualityComparer<TKey>.Default)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }
            AddRange(collection);
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : this(4, 31, comparer)
        {
            //Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(4, 31, comparer)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }
            AddRange(collection);
        }

        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(concurrencyLevel, 31, comparer)
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
            _wrapped = new SafeDictionary<TKey, TValue>();
            _valueCollection = new ValueCollection<TKey, TValue>(_wrapped);
        }

        public int Count
        {
            get
            {
                // This should be an snaptshot operation
                return _wrapped.Count;
            }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        public ICollection<TKey> Keys
        {
            get { return _wrapped.Keys; }
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection)_wrapped.Keys; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        public ICollection<TValue> Values
        {
            get { return _valueCollection; }
        }

        ICollection IDictionary.Values
        {
            get { return _valueCollection; }
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
                return _wrapped[key];
            }
            set
            {
                if (ReferenceEquals(key, null))
                {
                    // ConcurrentDictionary hates null
                    throw new ArgumentNullException("key");
                }
                _wrapped.Set(key, value);
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
                    TValue result;
                    if (_wrapped.TryGetValue((TKey)key, out result))
                    {
                        return result;
                    }
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

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            _wrapped.AddNew(key, value);
        }

        void IDictionary.Add(object key, object value)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // keep the is operator
            if (key is TKey && value is TValue)
            {
                _wrapped.AddNew((TKey)key, (TValue)value);
            }
            throw new ArgumentException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (ReferenceEquals(item.Key, null))
            {
                // ConcurrentDictionary hates null
                // While technically item is not null and item.Key is not an argument...
                // This is what happens when you do the call on Microsoft's implementation
                throw CreateArgumentNullExceptionKey(item.Key);
            }
            _wrapped.AddNew(item.Key, item.Value);
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // addValueFactory and updateValueFactory are checked for null inside the call
            var result = _wrapped.AddOrUpdate
                         (
                             key,
                             addValueFactory,
                             updateValueFactory
                         );
            return result;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // updateValueFactory is checked for null inside the call
            var result = _wrapped.AddOrUpdate
                         (
                             key,
                             addValue,
                             updateValueFactory
                         );
            return result;
        }

        public void Clear()
        {
            // This should be an snaptshot operation
            _wrapped.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // keep the is operator
            if (key is TKey)
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (ReferenceEquals(item.Key, null))
            {
                // ConcurrentDictionary hates null
                // While technically item is not null and item.Key is not an argument...
                // This is what happens when you do the call on Microsoft's implementation
                throw CreateArgumentNullExceptionKey(item.Key);
            }
            TValue found;
            if (_wrapped.TryGetValue(item.Key, out found))
            {
                if (EqualityComparer<TValue>.Default.Equals(found, item.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // No existing value is set, so no locking, right?
            return _wrapped.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            // This should be an snaptshot operation
            Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
            this.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            // WORST API EVER - I shouldn't be supporting this
            // I'm checking size before checking type - I have no plans to fix that
            Extensions.CanCopyTo(_wrapped.Count, array, index);
            try
            {
                var pairs = array as KeyValuePair<TKey, TValue>[]; // most decent alternative
                if (pairs != null)
                {
                    var _array = pairs;
                    foreach (var pair in _wrapped)
                    {
                        _array[index] = pair;
                        index++;
                    }
                    return;
                }
                var objects = array as object[];
                if (objects != null)
                {
                    var _array = objects;
                    foreach (var pair in _wrapped)
                    {
                        _array[index] = pair;
                        index++;
                    }
                    return;
                }
                var entries = array as DictionaryEntry[];
                // that thing exists, I was totally unaware, I may as well use it.
                if (entries != null)
                {
                    var _array = entries;
                    foreach (var pair in _wrapped)
                    {
                        _array[index] = new DictionaryEntry(pair.Key, pair.Value);
                        index++;
                    }
                    return;
                }
                throw new ArgumentException("Not supported array type"); // A.K.A ScrewYouException
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException("array", exception.Message);
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
            // No existing value is set, so no locking, right?
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // valueFactory is checked for null inside the call
            return _wrapped.GetOrAdd(key, valueFactory);
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            // No existing value is set, so no locking, right?
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            return _wrapped.GetOrAdd(key, value);
        }

        void IDictionary.Remove(object key)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            // keep the is operator
            if (key is TKey)
            {
                _wrapped.Remove((TKey)key);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (ReferenceEquals(item.Key, null))
            {
                // ConcurrentDictionary hates null
                // While technically item is not null and item.Key is not an argument...
                // This is what happens when you do the call on Microsoft's implementation
                throw CreateArgumentNullExceptionKey(item.Key);
            }
            TValue found;
            return _wrapped.Remove(item.Key, input => EqualityComparer<TValue>.Default.Equals(item.Value, item.Value), out found);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            TValue bundle;
            return TryRemove(key, out bundle);
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            // This should be an snaptshot operation
            var result = new List<KeyValuePair<TKey, TValue>>(_wrapped.Count);
            foreach (var pair in _wrapped)
            {
                result.Add(pair);
            }
            return result.ToArray();
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            return _wrapped.TryAdd(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            return _wrapped.TryGetValue(key, out value);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            return _wrapped.Remove(key, out value);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            if (ReferenceEquals(key, null))
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException("key");
            }
            return _wrapped.TryUpdate(key, newValue, comparisonValue);
        }

        private static ArgumentNullException CreateArgumentNullExceptionKey(TKey key)
        {
            GC.KeepAlive(key);
            return new ArgumentNullException("key");
        }

        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
            {
                if (!_wrapped.TryAdd(pair.Key, pair.Value))
                {
                    throw new ArgumentException("The source contains duplicate keys.");
                }
            }
        }

        private sealed class DictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> _wrapped;

            internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> wrapped)
            {
                _wrapped = wrapped.GetEnumerator();
            }

            public object Current
            {
                get { return Entry; }
            }

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
                    return current.Key;
                }
            }

            public object Value
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
                // Which means this method takes the responsability of disposing
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