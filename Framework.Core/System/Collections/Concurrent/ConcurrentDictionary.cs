﻿#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Collections.Generic;
using System.Linq;
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
            AddRange(collection ?? throw new ArgumentNullException(nameof(collection)));
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : this(4, 31, comparer)
        {
            // Empty
        }

        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(4, 31, comparer)
        {
            AddRange(collection ?? throw new ArgumentNullException(nameof(collection)));
        }

        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(concurrencyLevel, 31, comparer)
        {
            AddRange(collection ?? throw new ArgumentNullException(nameof(collection)));
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

                _wrapped.Set(key, value);
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
                    _wrapped.AddNew(keyAsTKey, valueAsTValue);
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

            _wrapped.AddNew(key, value);
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

            _wrapped.AddNew(item.Key, item.Value);
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            // addValueFactory and updateValueFactory are checked for null inside the call
            return _wrapped.AddOrUpdate
            (
                key,
                addValueFactory,
                updateValueFactory
            );
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            // updateValueFactory is checked for null inside the call
            return _wrapped.AddOrUpdate
            (
                key,
                addValue,
                updateValueFactory
            );
        }

        public void Clear()
        {
            // This should be an snapshot operation
            _wrapped.Clear();
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
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            // This should be an snapshot operation
            Extensions.CanCopyTo(Count, array, arrayIndex);
            this.CopyTo(array, arrayIndex);
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

            // valueFactory is checked for null inside the call
            return _wrapped.GetOrAdd(key, valueFactory);
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            return _wrapped.GetOrAdd(key, value);
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
                    _wrapped.Remove(keyAsTKey);
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

            return _wrapped.Remove(item.Key, input => EqualityComparer<TValue>.Default.Equals(input, item.Value), out _);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return TryRemove(key, out _);
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            // This should be an snapshot operation
            var result = new List<KeyValuePair<TKey, TValue>>(_wrapped.Count);
            result.AddRange(_wrapped);
            return result.ToArray();
        }

        public bool TryAdd(TKey key, TValue value)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            return _wrapped.TryAdd(key, value);
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

            return _wrapped.Remove(key, out value);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            // key could be null
            if (key == null!)
            {
                // ConcurrentDictionary hates null
                throw new ArgumentNullException(nameof(key));
            }

            return _wrapped.TryUpdate(key, newValue, comparisonValue);
        }

        private static ArgumentNullException CreateArgumentNullExceptionKey(TKey key)
        {
            _ = key;
            return new ArgumentNullException(nameof(key));
        }

        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            if (collection.Any(pair => !_wrapped.TryAdd(pair.Key, pair.Value)))
            {
                throw new ArgumentException("The source contains duplicate keys.");
            }
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