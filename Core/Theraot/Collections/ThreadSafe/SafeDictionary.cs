// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections.Specialized;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe lock-free hash based dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="IDictionary{TKey, TValue}" /> or any other desired interface.
    /// </remarks>
    [Serializable]
    public sealed partial class SafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private const int INT_DefaultProbing = 1;

        private readonly KeyCollection<TKey, TValue> _keyCollection;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ValueCollection<TKey, TValue> _valueCollection;
        private readonly IEqualityComparer<TValue> _valueComparer;
        private Mapper<KeyValuePair<TKey, TValue>> _mapper;
        private int _probing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        public SafeDictionary()
            : this(EqualityComparer<TKey>.Default, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="initialProbing">The number of steps in linear probing.</param>
        public SafeDictionary(int initialProbing)
            : this(EqualityComparer<TKey>.Default, initialProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The key comparer.</param>
        public SafeDictionary(IEqualityComparer<TKey> comparer)
            : this(comparer, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The key comparer.</param>
        /// <param name="initialProbing">The number of steps in linear probing.</param>
        public SafeDictionary(IEqualityComparer<TKey> comparer, int initialProbing)
        {
            _keyComparer = comparer ?? EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _mapper = new Mapper<KeyValuePair<TKey, TValue>>();
            _probing = initialProbing;
            _keyCollection = new KeyCollection<TKey, TValue>(this);
            _valueCollection = new ValueCollection<TKey, TValue>(this);
        }

        public int Count
        {
            get
            {
                return _mapper.Count;
            }
        }

        public IEqualityComparer<TKey> KeyComparer
        {
            get
            {
                return _keyComparer;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return _keyCollection;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return _valueCollection;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                Set(key, value);
            }
        }

        /// <summary>
        /// Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">An item with the same key has already been added</exception>
        public void AddNew(TKey key, TValue value)
        {
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> found;
                if (_mapper.Insert(hashCode + attempts, neo, out found))
                {
                    return;
                }
                if (_keyComparer.Equals(found.Key, key))
                {
                    throw new ArgumentException("An item with the same key has already been added", "key");
                }
                attempts++;
            }
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            Interlocked.Exchange(ref _mapper, _mapper = new Mapper<KeyValuePair<TKey, TValue>>());
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        /// <returns>Returns the removed pairs.</returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> ClearEnumerable()
        {
            return Interlocked.Exchange(ref _mapper, _mapper = new Mapper<KeyValuePair<TKey, TValue>>());
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
            var hashCode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified key is contained.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck)
        {
            if (ReferenceEquals(keyCheck, null))
            {
                throw new ArgumentNullException("keyCheck");
            }
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_keyComparer.GetHashCode(found.Key) == hashCode && keyCheck(found.Key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified key is contained.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="valueCheck">The value predicate.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck, Predicate<TValue> valueCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException("keyCheck");
            }
            if (valueCheck == null)
            {
                throw new ArgumentNullException("valueCheck");
            }
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_keyComparer.GetHashCode(found.Key) == hashCode && keyCheck(found.Key) && valueCheck(found.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the items to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="System.ArgumentNullException">array</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex;Non-negative number is required.</exception>
        /// <exception cref="System.ArgumentException">array;The array can not contain the number of elements.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _mapper.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _mapper.GetEnumerator();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (ReferenceEquals(valueFactory, null))
            {
                throw new ArgumentNullException("valueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                if (_mapper.InternalTryGetOrInsert(hashCode + attempts, () => new KeyValuePair<TKey, TValue>(key, valueFactory(key)), out stored))
                {
                    return stored.Value;
                }
                if (_keyComparer.Equals(stored.Key, key))
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                if (_mapper.TryGetOrInsert(hashCode + attempts, neo, out stored))
                {
                    return stored.Value;
                }
                if (_keyComparer.Equals(stored.Key, key))
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Gets the pairs contained in this object.
        /// </summary>
        /// <returns>The pairs contained in this object</returns>
        public IList<KeyValuePair<TKey, TValue>> GetPairs()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(_mapper.Count);
            foreach (var pair in _mapper)
            {
                result.Add(pair);
            }
            return result;
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
            var hashCode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                KeyValuePair<TKey, TValue> previous;
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        done = true;
                        return true;
                    }
                    return false;
                };
                var result = _mapper.TryGetCheckRemoveAt
                             (
                                 hashCode + attempts,
                                 check,
                                 out previous
                             );
                if (done)
                {
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key, out TValue value)
        {
            value = default(TValue);
            var hashCode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                KeyValuePair<TKey, TValue> previous;
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        done = true;
                        return true;
                    }
                    return false;
                };
                var result = _mapper.TryGetCheckRemoveAt
                             (
                                 hashCode + attempts,
                                 check,
                                 out previous
                             );
                if (done)
                {
                    value = previous.Value;
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a key by hash code and a key predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(int hashCode, Predicate<TKey> keyCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException("keyCheck");
            }
            value = default(TValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                KeyValuePair<TKey, TValue> previous;
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.GetHashCode(_found.Key) == hashCode && keyCheck(_found.Key))
                    {
                        done = true;
                        return true;
                    }
                    return false;
                };
                var result = _mapper.TryGetCheckRemoveAt
                             (
                                 hashCode + attempts,
                                 check,
                                 out previous
                             );
                if (done)
                {
                    value = previous.Value;
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a key by hash code, key predicate and value predicate.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="valueCheck">The value predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key, Predicate<TValue> valueCheck, out TValue value)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException("valueCheck");
            }
            value = default(TValue);
            var hashCode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                KeyValuePair<TKey, TValue> previous;
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        done = true;
                        if (valueCheck(_found.Value))
                        {
                            return true;
                        }
                    }
                    return false;
                };
                var result = _mapper.TryGetCheckRemoveAt
                             (
                                 hashCode + attempts,
                                 check,
                                 out previous
                             );
                if (done)
                {
                    value = previous.Value;
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a key by hash code, key predicate and value predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="valueCheck">The value predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(int hashCode, Predicate<TKey> keyCheck, Predicate<TValue> valueCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException("keyCheck");
            }
            if (valueCheck == null)
            {
                throw new ArgumentNullException("valueCheck");
            }
            value = default(TValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                KeyValuePair<TKey, TValue> previous;
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.GetHashCode(_found.Key) == hashCode && keyCheck(_found.Key))
                    {
                        done = true;
                        if (valueCheck(_found.Value))
                        {
                            return true;
                        }
                    }
                    return false;
                };
                var result = _mapper.TryGetCheckRemoveAt
                             (
                                 hashCode + attempts,
                                 check,
                                 out previous
                             );
                if (done)
                {
                    value = previous.Value;
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the keys and associated values where the key satisfies the predicate.
        /// </summary>
        /// <param name="keyCheck">The predicate.</param>
        /// <returns>
        /// The number or removed pairs of keys and associated values.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        public int RemoveWhereKey(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException("keyCheck");
            }
            var matches = _mapper.InternalWhere(pair => keyCheck(pair.Key));
            var count = 0;
            foreach (var pair in matches)
            {
                if (Remove(pair.Key))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Removes the keys and associated values where the key satisfies the predicate.
        /// </summary>
        /// <param name="keyCheck">The predicate.</param>
        /// <returns>
        /// An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the removed pairs.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        public IEnumerable<TValue> RemoveWhereKeyEnumerable(Predicate<TKey> keyCheck)
        {
            if (ReferenceEquals(keyCheck, null))
            {
                throw new ArgumentNullException("keyCheck");
            }
            var matches = _mapper.InternalWhere(pair => keyCheck(pair.Key));
            foreach (var pair in matches)
            {
                if (Remove(pair.Key))
                {
                    yield return pair.Value;
                }
            }
        }

        /// <summary>
        /// Removes the keys and associated values where the value satisfies the predicate.
        /// </summary>
        /// <param name="valueCheck">The predicate.</param>
        /// <returns>
        /// The number or removed pairs of keys and associated values.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        public int RemoveWhereValue(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException("valueCheck");
            }
            var matches = _mapper.InternalWhere(pair => valueCheck(pair.Value));
            var count = 0;
            foreach (var pair in matches)
            {
                if (Remove(pair.Key))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Removes the keys and associated values where the value satisfies the predicate.
        /// </summary>
        /// <param name="valueCheck">The predicate.</param>
        /// <returns>
        /// An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the removed pairs.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        public IEnumerable<TValue> RemoveWhereValueEnumerable(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException("valueCheck");
            }
            var matches = _mapper.InternalWhere(pair => valueCheck(pair.Value));
            foreach (var pair in matches)
            {
                if (Remove(pair.Key))
                {
                    yield return pair.Value;
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
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                bool isNew;
                if (_mapper.TryGetCheckSet(hashCode + attempts, neo, found => _keyComparer.Equals(((KeyValuePair<TKey, TValue>)found).Key, key), out isNew))
                {
                    return;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="isNew">if set to <c>true</c> the item value was set.</param>
        public void Set(TKey key, TValue value, out bool isNew)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                if (_mapper.TryGetCheckSet(hashCode + attempts, neo, found => _keyComparer.Equals(((KeyValuePair<TKey, TValue>)found).Key, key), out isNew))
                {
                    return;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(TKey key, TValue value)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> found;
                if (_mapper.Insert(hashCode + attempts, neo, out found))
                {
                    return true;
                }
                if (_keyComparer.Equals(found.Key, key))
                {
                    return false;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="stored">The stored pair independently of success.</param>
        /// <returns>
        ///   <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(TKey key, TValue value, out KeyValuePair<TKey, TValue> stored)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                if (_mapper.Insert(hashCode + attempts, neo, out stored))
                {
                    stored = neo;
                    return true;
                }
                if (_keyComparer.Equals(stored.Key, key))
                {
                    return false;
                }
                attempts++;
            }
        }

        public bool TryGetOrAdd(TKey key, TValue value, out TValue stored)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> _stored;
                if (_mapper.TryGetOrInsert(hashCode + attempts, neo, out _stored))
                {
                    stored = _stored.Value;
                    return true;
                }
                if (_keyComparer.Equals(_stored.Key, key))
                {
                    stored = _stored.Value;
                    return false;
                }
                attempts++;
            }
        }

        public bool TryGetOrAdd(TKey key, Func<TKey, TValue> valueFactory, out TValue stored)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> _stored;
                if (_mapper.InternalTryGetOrInsert(hashCode + attempts, () => new KeyValuePair<TKey, TValue>(key, valueFactory(key)), out _stored))
                {
                    stored = _stored.Value;
                    return true;
                }
                if (_keyComparer.Equals(_stored.Key, key))
                {
                    stored = _stored.Value;
                    return false;
                }
                attempts++;
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
            var hashCode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, key))
                    {
                        value = found.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, newValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                bool keyMatch = false;
                ExtendProbingIfNeeded(attempts);
                Predicate<KeyValuePair<TKey, TValue>> check = found =>
                {
                    keyMatch = _keyComparer.Equals(found.Key, key);
                    return keyMatch && _valueComparer.Equals(found.Value, comparisonValue);
                };
                if (_mapper.TryUpdate(hashCode + attempts, neo, check))
                {
                    return true;
                }
                if (keyMatch)
                {
                    return false;
                }
            }
            return false;
        }

        public bool TryUpdate(TKey key, TValue newValue, Predicate<TValue> valueCheck)
        {
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, newValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                bool keyMatch = false;
                ExtendProbingIfNeeded(attempts);
                Predicate<KeyValuePair<TKey, TValue>> check = found =>
                {
                    keyMatch = _keyComparer.Equals(found.Key, key);
                    return keyMatch && valueCheck(found.Value);
                };
                if (_mapper.TryUpdate(hashCode + attempts, neo, check))
                {
                    return true;
                }
                if (keyMatch)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the values where the key satisfies the predicate.
        /// </summary>
        /// <param name="keyCheck">The predicate.</param>
        /// <returns>
        /// An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the matched pairs.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be returned.
        /// </remarks>
        public IEnumerable<TValue> Where(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException("keyCheck");
            }
            var matches = _mapper.InternalWhere(pair => keyCheck(pair.Key));
            foreach (var pair in matches)
            {
                yield return pair.Value;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            AddNew(item.Key, item.Value);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            AddNew(key, value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            int hashCode = _keyComparer.GetHashCode(item.Key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, item.Key))
                    {
                        if (_valueComparer.Equals(found.Value, item.Value))
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            int hashCode = _keyComparer.GetHashCode(item.Key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                KeyValuePair<TKey, TValue> previous;
                var result = _mapper.TryGetCheckRemoveAt
                    (
                                 hashCode + attempts,
                                 found =>
                    {
                        var _found = (KeyValuePair<TKey, TValue>)found;
                        if (_keyComparer.Equals(_found.Key, item.Key))
                        {
                            done = true;
                            if (_valueComparer.Equals(_found.Value, item.Value))
                            {
                                return true;
                            }
                        }
                        return false;
                    },
                                 out previous
                             );
                if (done)
                {
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">An item with the same key has already been added</exception>
        internal void AddNew(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        // This is the item that has been stored with the key
                        // Throw to abort overwrite
                        throw new ArgumentException("An item with the same key has already been added", "key");
                    }
                    // This is not the key, overwrite?
                    return keyOverwriteCheck(_found.Key);
                };
                // No try-catch - let the exception go.
                bool isNew;
                // TryGetCheckSet will add if no item is found, otherwise it calls check
                if (_mapper.TryGetCheckSet(hashCode + attempts, neo, check, out isNew))
                {
                    // It added a new item
                    return;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                bool isNew;
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return _keyComparer.Equals(_found.Key, key) || keyOverwriteCheck(_found.Key);
                };
                if (_mapper.TryGetCheckSet(hashCode + attempts, neo, check, out isNew))
                {
                    return;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <param name="isNew">if set to <c>true</c> the item value was set.</param>
        internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out bool isNew)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return _keyComparer.Equals(_found.Key, key) || keyOverwriteCheck(_found.Key);
                };
                if (_mapper.TryGetCheckSet(hashCode + attempts, neo, check, out isNew))
                {
                    return;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        internal bool TryAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        // This is the item that has been stored with the key
                        // Throw to abort overwrite
                        throw new ArgumentException("An item with the same key has already been added", "key");
                    }
                    // This is not the key, overwrite?
                    return keyOverwriteCheck(_found.Key);
                };
                try
                {
                    bool isNew;
                    // TryGetCheckSet will add if no item is found, otherwise it calls check
                    if (_mapper.TryGetCheckSet(hashCode + attempts, neo, check, out isNew))
                    {
                        // It added a new item
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // An item with the same key has already been added
                    return false;
                }
                attempts++;
            }
        }

        internal bool TryGetOrAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out TValue stored)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        // This is the item that has been stored with the key
                        value = _found.Value;
                        // Throw to abort overwrite
                        throw new ArgumentException("An item with the same key has already been added", "key");
                    }
                    // This is not the key, overwrite?
                    return keyOverwriteCheck(_found.Key);
                };
                try
                {
                    bool isNew;
                    // TryGetCheckSet will add if no item is found, otherwise it calls check
                    if (_mapper.TryGetCheckSet(hashCode + attempts, neo, check, out isNew))
                    {
                        // It added a new item
                        stored = value;
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // An item with the same key has already been added
                    // Return it
                    stored = value;
                    return false;
                }
                attempts++;
            }
        }

        private void ExtendProbingIfNeeded(int attempts)
        {
            var diff = 1 + attempts - _probing;
            if (diff > 0)
            {
                Interlocked.Add(ref _probing, diff);
            }
        }
    }

    public sealed partial class SafeDictionary<TKey, TValue>
    {
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (ReferenceEquals(addValueFactory, null))
            {
                throw new ArgumentNullException("addValueFactory");
            }
            if (ReferenceEquals(updateValueFactory, null))
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                bool isNew;
                Func<object> itemFactory = () => new KeyValuePair<TKey, TValue>(key, addValueFactory(key));
                Func<object, object> itemUpdateFactory = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return new KeyValuePair<TKey, TValue>(key, updateValueFactory(_found.Key, _found.Value));
                };
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return _keyComparer.Equals(key, _found.Key);
                };
                var result = _mapper.InternalInsertOrUpdate
                             (
                                 hashCode + attempts,
                                 itemFactory,
                                 itemUpdateFactory,
                                 check,
                                 out stored,
                                 out isNew
                             );
                if (result)
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (ReferenceEquals(updateValueFactory, null))
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            var neo = new KeyValuePair<TKey, TValue>(key, addValue);
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                bool isNew;
                Func<object, object> updateFactory = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return new KeyValuePair<TKey, TValue>(key, updateValueFactory(_found.Key, _found.Value));
                };
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return _keyComparer.Equals(key, _found.Key);
                };
                var result = _mapper.InternalInsertOrUpdate
                             (
                                 hashCode + attempts,
                                 neo,
                                 updateFactory,
                                 check,
                                 out stored,
                                 out isNew
                             );
                if (result)
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, out bool added)
        {
            if (ReferenceEquals(addValueFactory, null))
            {
                throw new ArgumentNullException("addValueFactory");
            }
            if (ReferenceEquals(updateValueFactory, null))
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                Func<object> valueFactory = () => new KeyValuePair<TKey, TValue>(key, addValueFactory(key));
                Func<object, object> updateFactory = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return new KeyValuePair<TKey, TValue>(key, updateValueFactory(_found.Key, _found.Value));
                };
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return _keyComparer.Equals(key, _found.Key);
                };
                var result = _mapper.InternalInsertOrUpdate
                             (
                                 hashCode + attempts,
                                 valueFactory,
                                 updateFactory,
                                 check,
                                 out stored,
                                 out added
                             );
                if (result)
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory, out bool added)
        {
            if (ReferenceEquals(updateValueFactory, null))
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            var neo = new KeyValuePair<TKey, TValue>(key, addValue);
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                Func<object, object> updateFactory = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return new KeyValuePair<TKey, TValue>(key, updateValueFactory(_found.Key, _found.Value));
                };
                Predicate<object> check = found =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    return _keyComparer.Equals(key, _found.Key);
                };
                var result = _mapper.InternalInsertOrUpdate
                             (
                                 hashCode + attempts,
                                 neo,
                                 updateFactory,
                                 check,
                                 out stored,
                                 out added
                             );
                if (result)
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Tries to retrieve the value by hash code and key predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(int hashCode, Predicate<TKey> keyCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException("keyCheck");
            }
            value = default(TValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_keyComparer.GetHashCode(found.Key) == hashCode && keyCheck(found.Key))
                    {
                        value = found.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        internal TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, TryConvert<KeyValuePair<TKey, TValue>, TValue> updateValueFactory, out bool added)
        {
            if (ReferenceEquals(addValueFactory, null))
            {
                throw new ArgumentNullException("addValueFactory");
            }
            if (ReferenceEquals(updateValueFactory, null))
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                Func<object> valueFactory = () => new KeyValuePair<TKey, TValue>(key, addValueFactory(key));
                TryConvert<object, object> itemUpdateFactory = (object found, out object output) =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    TValue value;
                    if (_keyComparer.Equals(key, _found.Key) && updateValueFactory(_found, out value))
                    {
                        output = new KeyValuePair<TKey, TValue>(key, value);
                        return true;
                    }
                    output = null;
                    return false;
                };
                var result = _mapper.InternalInsertOrUpdate
                             (
                                 hashCode + attempts,
                                 valueFactory,
                                 itemUpdateFactory,
                                 out stored,
                                 out added
                             );
                if (result)
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        internal TValue AddOrUpdate(TKey key, TValue addValue, TryConvert<KeyValuePair<TKey, TValue>, TValue> updateValueFactory, out bool added)
        {
            if (ReferenceEquals(updateValueFactory, null))
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> stored;
                Func<object> valueFactory = () => new KeyValuePair<TKey, TValue>(key, addValue);
                TryConvert<object, object> itemUpdateFactory = (object found, out object output) =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    TValue value;
                    if (_keyComparer.Equals(key, _found.Key) && updateValueFactory(_found, out value))
                    {
                        output = new KeyValuePair<TKey, TValue>(key, value);
                        return true;
                    }
                    output = null;
                    return false;
                };
                var result = _mapper.InternalInsertOrUpdate
                             (
                                 hashCode + attempts,
                                 valueFactory,
                                 itemUpdateFactory,
                                 out stored,
                                 out added
                             );
                if (result)
                {
                    return stored.Value;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <param name="stored">The stored pair independently of success.</param>
        /// <returns>
        ///   <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        internal bool TryAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out KeyValuePair<TKey, TValue> stored)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                KeyValuePair<TKey, TValue> _found = neo;
                ExtendProbingIfNeeded(attempts);
                Predicate<object> check = found =>
                {
                    _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        // This is the item that has been stored with the key
                        // Throw to abort overwrite
                        throw new ArgumentException("An item with the same key has already been added", "key");
                    }
                    // This is not the key, overwrite?
                    return keyOverwriteCheck(_found.Key);
                };
                try
                {
                    bool isNew;
                    // TryGetCheckSet will add if no item is found, otherwise it calls check
                    if (_mapper.TryGetCheckSet(hashCode + attempts, neo, check, out isNew))
                    {
                        // It added a new item
                        stored = neo;
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // An item with the same key has already been added
                    stored = _found;
                    return false;
                }
                attempts++;
            }
        }

        internal bool TryGetOrAdd(TKey key, Func<TValue> addValueFactory, TryConvert<KeyValuePair<TKey, TValue>, TValue> updateValueFactory, out TValue stored)
        {
            // NOTICE this method has no null check
            var hashCode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                TValue value = default(TValue);
                ExtendProbingIfNeeded(attempts);
                Func<object> itemFactory = () => new KeyValuePair<TKey, TValue>(key, value = addValueFactory());
                TryConvert<object, object> itemUpdateFactory = (object found, out object output) =>
                {
                    var _found = (KeyValuePair<TKey, TValue>)found;
                    if (_keyComparer.Equals(_found.Key, key))
                    {
                        // This is the item that has been stored with the key
                        value = _found.Value;
                        // Throw to abort overwrite
                        throw new ArgumentException("An item with the same key has already been added", "key");
                    }
                    // This is not the key, overwrite?
                    if (updateValueFactory(_found, out value))
                    {
                        output = new KeyValuePair<TKey, TValue>(key, value);
                        return true;
                    }
                    output = null;
                    return false;
                };
                try
                {
                    bool isNew;
                    if (_mapper.TryGetCheckSet(hashCode + attempts, itemFactory, itemUpdateFactory, out isNew))
                    {
                        // It added a new item
                        stored = value;
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // An item with the same key has already been added
                    // Return it
                    stored = value;
                    return false;
                }
                attempts++;
            }
        }
    }
}