// Needed for NET35 (ConditionalWeakTable)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    // TODO: this is actually a Weak Key dictionary useful to extend objects, there could also be Weak Value dictionaries useful for caches, and fully weak dictionary useful for the combination.
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : class
    {
        private readonly KeyCollection<TKey, TValue> _keyCollection;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ValueCollection<TKey, TValue> _valueCollection;
        private readonly SafeDictionary<WeakNeedle<TKey>, TValue> _wrapped;
        private readonly NeedleReservoir<TKey, WeakNeedle<TKey>> _reservoir;
        private EventHandler _handle;

        public WeakDictionary()
            : this(null)
        {
            // Empty
        }

        public WeakDictionary(IEqualityComparer<TKey> comparer)
        {
            _keyComparer = comparer ?? EqualityComparer<TKey>.Default;
            var needleComparer = new NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey>(_keyComparer);
            _wrapped = new SafeDictionary<WeakNeedle<TKey>, TValue>(needleComparer);
            _keyCollection = new KeyCollection<TKey, TValue>(this);
            _valueCollection = new ValueCollection<TKey, TValue>(this);
            _reservoir = new NeedleReservoir<TKey, WeakNeedle<TKey>>(key => new WeakNeedle<TKey>(key));
        }

        public WeakDictionary(IEqualityComparer<TKey> comparer, int initialProbing)
        {
            _keyComparer = comparer ?? EqualityComparer<TKey>.Default;
            var needleComparer = new NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey>(_keyComparer);
            _wrapped = new SafeDictionary<WeakNeedle<TKey>, TValue>(needleComparer, initialProbing);
            _keyCollection = new KeyCollection<TKey, TValue>(this);
            _valueCollection = new ValueCollection<TKey, TValue>(this);
            _reservoir = new NeedleReservoir<TKey, WeakNeedle<TKey>>(key => new WeakNeedle<TKey>(key));
        }

        public bool AutoRemoveDeadItems
        {
            get { return _handle != null; }

            set
            {
                var handle = _handle;
                if (value)
                {
                    var created = new EventHandler((sender, args) => RemoveDeadItems());
                    if (handle == null && Interlocked.CompareExchange(ref _handle, created, null) == null)
                    {
                        GCMonitor.Collected += created;
                    }
                }
                else
                {
                    if (handle != null && Interlocked.CompareExchange(ref _handle, null, handle) == handle)
                    {
                        GCMonitor.Collected -= handle;
                    }
                }
            }
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public IEqualityComparer<TKey> KeyComparer
        {
            get { return _keyComparer; }
        }

        public ICollection<TKey> Keys
        {
            get { return _keyCollection; }
        }

        public ICollection<TValue> Values
        {
            get { return _valueCollection; }
        }

        protected SafeDictionary<WeakNeedle<TKey>, TValue> Wrapped
        {
            get { return _wrapped; }
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

            set { Set(key, value); }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            AddNew(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            // No risk of dead needles here
            AddNew(item.Key, item.Value);
        }

        /// <summary>
        /// Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">An item with the same key has already been added</exception>
        public void AddNew(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            try
            {
                _wrapped.AddNew(needle, input => !input.IsAlive, value);
            }
            catch (ArgumentException)
            {
                _reservoir.DonateNeedle(needle);
                throw;
            }
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (addValueFactory == null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }
            var needle = PrivateGetNeedle(key);
            Func<WeakNeedle<TKey>, TValue, TValue> factory = (pairKey, foundValue) =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(pairKey, out foundKey))
                {
                    return updateValueFactory(foundKey, foundValue);
                }
                return addValueFactory(key);
            };
            Func<WeakNeedle<TKey>, TValue> valueFactory = input => addValueFactory(key);
            bool added;
            var result = _wrapped.AddOrUpdate
                (
                    needle,
                    valueFactory,
                    factory,
                    out added
                );
            if (!added)
            {
                _reservoir.DonateNeedle(needle);
            }
            return result;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }
            var needle = PrivateGetNeedle(key);
            Func<WeakNeedle<TKey>, TValue, TValue> factory = (pairKey, foundValue) =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(pairKey, out foundKey))
                {
                    return updateValueFactory(foundKey, foundValue);
                }
                return addValue;
            };
            bool added;
            var result = _wrapped.AddOrUpdate
                (
                    needle,
                    addValue,
                    factory,
                    out added
                );
            if (!added)
            {
                _reservoir.DonateNeedle(needle);
            }
            return result;
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, out bool added)
        {
            if (addValueFactory == null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }
            var needle = PrivateGetNeedle(key);
            Func<WeakNeedle<TKey>, TValue, TValue> factory = (pairKey, foundValue) =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(pairKey, out foundKey))
                {
                    return updateValueFactory(foundKey, foundValue);
                }
                return addValueFactory(key);
            };
            Func<WeakNeedle<TKey>, TValue> valueFactory = input => addValueFactory(key);
            var result = _wrapped.AddOrUpdate
                (
                    needle,
                    valueFactory,
                    factory,
                    out added
                );
            if (!added)
            {
                _reservoir.DonateNeedle(needle);
            }
            return result;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory, out bool added)
        {
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }
            var needle = PrivateGetNeedle(key);
            Func<WeakNeedle<TKey>, TValue, TValue> factory = (pairKey, foundValue) =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(pairKey, out foundKey))
                {
                    return updateValueFactory(foundKey, foundValue);
                }
                return addValue;
            };
            var result = _wrapped.AddOrUpdate
                (
                    needle,
                    addValue,
                    factory,
                    out added
                );
            if (!added)
            {
                _reservoir.DonateNeedle(needle);
            }
            return result;
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            foreach (var item in _wrapped.ClearEnumerable())
            {
                _reservoir.DonateNeedle(item.Key);
            }
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> ClearEnumerable()
        {
            // No risk of dead needles here
            foreach (var item in _wrapped.ClearEnumerable())
            {
                TKey foundKey;
                if (PrivateTryGetValue(item.Key, out foundKey))
                {
                    var value = item.Value;
                    yield return new KeyValuePair<TKey, TValue>(foundKey, value);
                    _reservoir.DonateNeedle(item.Key);
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            // No risk of dead needles here
            Predicate<WeakNeedle<TKey>> check = input =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(input, out foundKey))
                {
                    return _keyComparer.Equals(foundKey, item.Key);
                }
                return false;
            };
            return _wrapped.ContainsKey
                (
                    _keyComparer.GetHashCode(item.Key),
                    check,
                    input => EqualityComparer<TValue>.Default.Equals(input, item.Value)
                );
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
            return _wrapped.ContainsKey
                (
                    _keyComparer.GetHashCode(key),
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return _keyComparer.Equals(foundKey, key);
                        }
                        return false;
                    }
                );
        }

        /// <summary>
        /// Determines whether the specified key is contained.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            return _wrapped.ContainsKey
                (
                    hashCode,
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck(foundKey);
                        }
                        return false;
                    }
                );
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck, Predicate<TValue> valueCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }
            return _wrapped.ContainsKey
                (
                    hashCode,
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck(foundKey);
                        }
                        return false;
                    },
                    valueCheck
                );
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
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }
            if (_wrapped.Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
            GetPairs().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            // No risk of dead needles here
            foreach (var pair in _wrapped)
            {
                TKey foundKey;
                if (PrivateTryGetValue(pair.Key, out foundKey))
                {
                    yield return new KeyValuePair<TKey, TValue>(foundKey, pair.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            TValue result;
            if (!_wrapped.TryGetOrAdd(needle, input => !input.IsAlive, value, out result))
            {
                _reservoir.DonateNeedle(needle);
            }
            return result;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var needle = PrivateGetNeedle(key);
            TValue result;
            TKey foundKey;
            Func<WeakNeedle<TKey>, TValue, TValue> factory = (pairKey, foundValue) => result = valueFactory(PrivateTryGetValue(pairKey, out foundKey) ? foundKey : key);
            if (_wrapped.TryGetOrAdd(needle, () => valueFactory(key), factory, out result))
            {
                return result;
            }
            _reservoir.DonateNeedle(needle);
            return result;
        }

        /// <summary>
        /// Gets the pairs contained in this object.
        /// </summary>
        public IList<KeyValuePair<TKey, TValue>> GetPairs()
        {
            // No risk of dead needles here
            var result = new List<KeyValuePair<TKey, TValue>>(_wrapped.Count);
            foreach (var pair in _wrapped)
            {
                TKey foundKey;
                if (PrivateTryGetValue(pair.Key, out foundKey))
                {
                    var value = pair.Value;
                    result.Add(new KeyValuePair<TKey, TValue>(foundKey, value));
                }
            }
            return result;
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
            return _wrapped.Remove
                (
                    _keyComparer.GetHashCode(key),
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return _keyComparer.Equals(foundKey, key);
                        }
                        return false;
                    },
                    valueCheck,
                    out value
                );
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
            TValue value;
            return _wrapped.Remove
            (
                _keyComparer.GetHashCode(key),
                input =>
                {
                    TKey foundKey;
                    if (PrivateTryGetValue(input, out foundKey))
                    {
                        return _keyComparer.Equals(foundKey, key);
                    }
                    return false;
                },
                out value
            );
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
            return _wrapped.Remove
                (
                    _keyComparer.GetHashCode(key),
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return _keyComparer.Equals(foundKey, key);
                        }
                        return false;
                    },
                    out value
                );
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public bool Remove(int hashCode, Predicate<TKey> keyCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            return _wrapped.Remove
                (
                    hashCode,
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck.Invoke(foundKey);
                        }
                        return false;
                    },
                    out value
                );
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public bool Remove(int hashCode, Predicate<TKey> keyCheck, Predicate<TValue> valueCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }
            return _wrapped.Remove
                (
                    hashCode,
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck.Invoke(foundKey);
                        }
                        return false;
                    },
                    valueCheck,
                    out value
                );
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            // No risk of dead needles here
            Predicate<WeakNeedle<TKey>> check = input =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(input, out foundKey))
                {
                    return _keyComparer.Equals(foundKey, item.Key);
                }
                return false;
            };
            TValue found;
            return _wrapped.Remove
                (
                    _keyComparer.GetHashCode(item.Key),
                    check,
                    input => EqualityComparer<TValue>.Default.Equals(input, item.Value),
                    out found
                );
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhereKey(key => !key.IsAlive);
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public int RemoveWhereKey(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            return _wrapped.RemoveWhereKey
                (
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck.Invoke(foundKey);
                        }
                        return false;
                    }
                );
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public IEnumerable<TValue> RemoveWhereKeyEnumerable(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            return _wrapped.RemoveWhereKeyEnumerable
                (
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck.Invoke(foundKey);
                        }
                        return false;
                    }
                );
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
        /// <exception cref="ArgumentNullException"><paramref name="valueCheck"/> is <c>null</c>.</exception>
        public int RemoveWhereValue(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }
            return _wrapped.RemoveWhereValue(valueCheck);
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
        /// <exception cref="ArgumentNullException"><paramref name="valueCheck"/> is <c>null</c>.</exception>
        public IEnumerable<TValue> RemoveWhereValueEnumerable(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }
            return _wrapped.RemoveWhereValueEnumerable(valueCheck);
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            _wrapped.Set(needle, input => !input.IsAlive, value);
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="isNew">if set to <c>true</c> the item value was set.</param>
        public void Set(TKey key, TValue value, out bool isNew)
        {
            var needle = PrivateGetNeedle(key);
            _wrapped.Set(needle, input => !input.IsAlive, value, out isNew);
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
            var needle = PrivateGetNeedle(key);
            if (_wrapped.TryAdd(needle, input => !input.IsAlive, value))
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
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
            // No risk of dead needles here
            var needle = PrivateGetNeedle(key);
            Predicate<WeakNeedle<TKey>> check = found =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(found, out foundKey))
                {
                    // Keeping the found key alive
                    // If we found a key, key will be the key found
                    // If we didn't key will be the key added
                    // So, either way key is the key that is stored
                    // By having it here, we don't need to read _stored.Key
                    key = foundKey;
                    return false;
                }
                return true;
            };
            KeyValuePair<WeakNeedle<TKey>, TValue> storedPair;
            var result = _wrapped.TryAdd(needle, check, value, out storedPair);
            if (!result)
            {
                _reservoir.DonateNeedle(needle);
            }
            stored = new KeyValuePair<TKey, TValue>(key, storedPair.Value);
            return result;
        }

        public bool TryGetOrAdd(TKey key, Func<TKey, TValue> valueFactory, out TValue stored)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            var needle = PrivateGetNeedle(key);
            TKey foundKey;
            Func<WeakNeedle<TKey>, TValue, TValue> factory = (pairKey, foundValue) => valueFactory(PrivateTryGetValue(pairKey, out foundKey) ? foundKey : key);
            if (_wrapped.TryGetOrAdd(needle, () => valueFactory(key), factory, out stored))
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
        }

        public bool TryGetOrAdd(TKey key, TValue value, out TValue stored)
        {
            var needle = PrivateGetNeedle(key);
            if (_wrapped.TryGetOrAdd(needle, input => !input.IsAlive, value, out stored))
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
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
            Predicate<WeakNeedle<TKey>> check = found =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(found, out foundKey))
                {
                    return _keyComparer.Equals(key, foundKey);
                }
                return false;
            };
            return _wrapped.TryGetValue(_keyComparer.GetHashCode(key), check, out value);
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public bool TryGetValue(int hashCode, Predicate<TKey> keyCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            Predicate<WeakNeedle<TKey>> check = found =>
            {
                TKey foundKey;
                if (PrivateTryGetValue(found, out foundKey))
                {
                    return keyCheck(foundKey);
                }
                return false;
            };
            return _wrapped.TryGetValue(hashCode, check, out value);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            var needle = PrivateGetNeedle(key);
            if (_wrapped.TryUpdate(needle, newValue, comparisonValue))
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
        }

        public bool TryUpdate(TKey key, TValue newValue, Predicate<TValue> valueCheck)
        {
            var needle = PrivateGetNeedle(key);
            if (_wrapped.TryUpdate(needle, newValue, valueCheck))
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
        }

        public bool TryUpdate(TKey key, Func<TValue, TValue> newValue)
        {
            var needle = PrivateGetNeedle(key);
            if (_wrapped.TryUpdate(needle, newValue))
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
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
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck"/> is <c>null</c>.</exception>
        public IEnumerable<TValue> Where(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }
            return _wrapped.Where
                (
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyCheck(foundKey);
                        }
                        return false;
                    }
                );
        }

        /// <summary>
        /// Returns the values where the value satisfies the predicate.
        /// </summary>
        /// <param name="valueCheck">The predicate.</param>
        /// <returns>
        /// An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the matched pairs.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be returned.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="valueCheck"/> is <c>null</c>.</exception>
        public IEnumerable<TValue> WhereValue(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }
            return _wrapped.WhereValue(valueCheck);
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
            var needle = PrivateGetNeedle(key);
            try
            {
                _wrapped.AddNew
                    (
                        needle,
                        input =>
                        {
                            TKey foundKey;
                            if (PrivateTryGetValue(input, out foundKey))
                            {
                                return keyOverwriteCheck(foundKey);
                            }
                            return true;
                        },
                        value
                    );
            }
            catch (ArgumentException)
            {
                _reservoir.DonateNeedle(needle);
                throw;
            }
        }

        internal TValue GetOrAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var needle = PrivateGetNeedle(key);
            TValue stored;
            if (
                !_wrapped.TryGetOrAdd
                (
                    needle,
                    input =>
                    {
                        TKey foundKey;
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyOverwriteCheck(foundKey);
                        }
                        return true;
                    },
                    value,
                    out stored
                )
            )
            {
                _reservoir.DonateNeedle(needle);
            }
            return stored;
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
            var needle = PrivateGetNeedle(key);
            TKey foundKey;
            _wrapped.Set
                (
                    needle,
                    input =>
                    {
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyOverwriteCheck(foundKey);
                        }
                        return true;
                    },
                    value
                );
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
            var needle = PrivateGetNeedle(key);
            TKey foundKey;
            _wrapped.Set
                (
                    needle,
                    input =>
                    {
                        if (PrivateTryGetValue(input, out foundKey))
                        {
                            return keyOverwriteCheck(foundKey);
                        }
                        return true;
                    },
                    value,
                    out isNew
                );
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
            var needle = PrivateGetNeedle(key);
            TKey foundKey;
            if
                (
                    _wrapped.TryAdd
                    (
                        needle,
                        input =>
                        {
                            if (PrivateTryGetValue(input, out foundKey))
                            {
                                return keyOverwriteCheck(foundKey);
                            }
                            return true;
                        },
                        value
                    )
                )
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
        }

        internal bool TryGetOrAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out TValue stored)
        {
            // NOTICE this method has no null check
            var needle = PrivateGetNeedle(key);
            TKey foundKey;
            if
                (
                    _wrapped.TryGetOrAdd
                    (
                        needle,
                        input =>
                        {
                            if (PrivateTryGetValue(input, out foundKey))
                            {
                                return keyOverwriteCheck(foundKey);
                            }
                            return true;
                        },
                        value,
                        out stored
                    )
                )
            {
                return true;
            }
            _reservoir.DonateNeedle(needle);
            return false;
        }

        protected bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item);
        }

        private WeakNeedle<TKey> PrivateGetNeedle(TKey key)
        {
            return _reservoir.GetNeedle(key);
        }

        private static bool PrivateTryGetValue(WeakNeedle<TKey> needle, out TKey foundKey)
        {
            return needle.TryGetValue(out foundKey);
        }
    }
}