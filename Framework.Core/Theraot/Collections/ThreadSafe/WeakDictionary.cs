// Needed for NET35 (ConditionalWeakTable)

#pragma warning disable RCS1231 // Make parameter ref read-only.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Theraot.Collections.Specialized;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    // TODO: this is actually a Weak Key dictionary useful to extend objects, there could also be Weak Value dictionaries useful for caches, and fully weak dictionary useful for the combination.
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : class
    {
        private readonly KeyCollection<TKey, TValue> _keyCollection;

        private readonly NeedleReservoir<TKey, WeakNeedle<TKey>> _reservoir;

        private readonly ValueCollection<TKey, TValue> _valueCollection;

        private EventHandler _handle;

        public WeakDictionary()
            : this(null)
        {
            // Empty
        }

        public WeakDictionary(IEqualityComparer<TKey> comparer)
        {
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            var needleComparer = new NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey>(Comparer);
            Wrapped = new ThreadSafeDictionary<WeakNeedle<TKey>, TValue>(needleComparer);
            _keyCollection = new KeyCollection<TKey, TValue>(this);
            _valueCollection = new ValueCollection<TKey, TValue>(this);
            _reservoir = new NeedleReservoir<TKey, WeakNeedle<TKey>>(key => new WeakNeedle<TKey>(key));
        }

        public WeakDictionary(IEqualityComparer<TKey> comparer, int initialProbing)
        {
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            var needleComparer = new NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey>(Comparer);
            Wrapped = new ThreadSafeDictionary<WeakNeedle<TKey>, TValue>(needleComparer, initialProbing);
            _keyCollection = new KeyCollection<TKey, TValue>(this);
            _valueCollection = new ValueCollection<TKey, TValue>(this);
            _reservoir = new NeedleReservoir<TKey, WeakNeedle<TKey>>(key => new WeakNeedle<TKey>(key));
        }

        public bool AutoRemoveDeadItems
        {
            get => _handle != null;

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

        public IEqualityComparer<TKey> Comparer { get; }

        public int Count => Wrapped.Count;

        public ICollection<TKey> Keys => _keyCollection;

        public ICollection<TValue> Values => _valueCollection;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        protected ThreadSafeDictionary<WeakNeedle<TKey>, TValue> Wrapped { get; }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException();
            }

            set => Set(key, value);
        }

        /// <summary>
        ///     Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">An item with the same key has already been added</exception>
        public void AddNew(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            try
            {
                Wrapped.AddNew(needle, input => !input.IsAlive, value);
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

            TValue Factory(WeakNeedle<TKey> pairKey, TValue foundValue)
            {
                return PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValueFactory(key);
            }

            TValue ValueFactory(WeakNeedle<TKey> _)
            {
                return addValueFactory(key);
            }

            var result = Wrapped.AddOrUpdate(needle, ValueFactory, Factory, out var added);
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

            TValue Factory(WeakNeedle<TKey> pairKey, TValue foundValue)
            {
                return PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValue;
            }

            var result = Wrapped.AddOrUpdate(needle, addValue, Factory, out var added);
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

            TValue Factory(WeakNeedle<TKey> pairKey, TValue foundValue)
            {
                return PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValueFactory(key);
            }

            TValue ValueFactory(WeakNeedle<TKey> _)
            {
                return addValueFactory(key);
            }

            var result = Wrapped.AddOrUpdate(needle, ValueFactory, Factory, out added);
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

            TValue Factory(WeakNeedle<TKey> pairKey, TValue foundValue)
            {
                return PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValue;
            }

            var result = Wrapped.AddOrUpdate(needle, addValue, Factory, out added);
            if (!added)
            {
                _reservoir.DonateNeedle(needle);
            }

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Removes all the elements.
        /// </summary>
        public void Clear()
        {
            foreach (var item in Wrapped.ClearEnumerable())
            {
                _reservoir.DonateNeedle(item.Key);
            }
        }

        /// <summary>
        ///     Removes all the elements.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable" /> containing the removed elements.
        /// </returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> ClearEnumerable()
        {
            // No risk of dead needles here
            foreach (var item in Wrapped.ClearEnumerable())
            {
                if (!PrivateTryGetValue(item.Key, out var foundKey))
                {
                    continue;
                }

                var value = item.Value;
                yield return new KeyValuePair<TKey, TValue>(foundKey, value);
                _reservoir.DonateNeedle(item.Key);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the specified key is contained.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return Wrapped.ContainsKey(Comparer.GetHashCode(key), input => PrivateTryGetValue(input, out var foundKey) && Comparer.Equals(foundKey, key));
        }

        /// <summary>
        ///     Determines whether the specified key is contained.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <returns>
        ///     <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
        public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }

            return Wrapped.ContainsKey(hashCode, input => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey));
        }

        /// <summary>
        ///     Determines whether the specified key is contained.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="valueCheck">The value predicate.</param>
        /// <returns>
        ///     <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
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

            return Wrapped.ContainsKey(hashCode, input => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey), valueCheck);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Copies the items to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="T:System.ArgumentNullException">array</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex;Non-negative number is required.</exception>
        /// <exception cref="T:System.ArgumentException">array;The array can not contain the number of elements.</exception>
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

            if (Wrapped.Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }

            GetPairs().CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an <see cref="T:System.Collections.Generic.IEnumerator`1" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.IEnumerator`1" /> object that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            // No risk of dead needles here
            foreach (var pair in Wrapped)
            {
                if (PrivateTryGetValue(pair.Key, out var foundKey))
                {
                    yield return new KeyValuePair<TKey, TValue>(foundKey, pair.Value);
                }
            }
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            if (!Wrapped.TryGetOrAdd(needle, input => !input.IsAlive, value, out var result))
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

            TValue Factory(WeakNeedle<TKey> pairKey, TValue _)
            {
                return result = valueFactory(PrivateTryGetValue(pairKey, out var foundKey) ? foundKey : key);
            }

            if (Wrapped.TryGetOrAdd(needle, () => valueFactory(key), Factory, out result))
            {
                return result;
            }

            _reservoir.DonateNeedle(needle);
            return result;
        }

        /// <summary>
        ///     Gets the pairs contained in this object.
        /// </summary>
        /// <returns>
        ///     An <see cref="IList" /> containing a copy of the contents of this object.
        /// </returns>
        public IList<KeyValuePair<TKey, TValue>> GetPairs()
        {
            // No risk of dead needles here
            var result = new List<KeyValuePair<TKey, TValue>>(Wrapped.Count);
            foreach (var pair in Wrapped)
            {
                if (!PrivateTryGetValue(pair.Key, out var foundKey))
                {
                    continue;
                }

                var value = pair.Value;
                result.Add(new KeyValuePair<TKey, TValue>(foundKey, value));
            }

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key)
        {
            return Wrapped.Remove(Comparer.GetHashCode(key), input => PrivateTryGetValue(input, out var foundKey) && Comparer.Equals(foundKey, key), out _);
        }

        /// <summary>
        ///     Removes a key by hash code, key predicate and value predicate.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="valueCheck">The value predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key, Predicate<TValue> valueCheck, out TValue value)
        {
            return Wrapped.Remove(Comparer.GetHashCode(key), input => PrivateTryGetValue(input, out var foundKey) && Comparer.Equals(foundKey, key), valueCheck, out value);
        }

        /// <summary>
        ///     Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key, out TValue value)
        {
            return Wrapped.Remove(Comparer.GetHashCode(key), input => PrivateTryGetValue(input, out var foundKey) && Comparer.Equals(foundKey, key), out value);
        }

        /// <summary>
        ///     Removes a key by hash code and a key predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
        public bool Remove(int hashCode, Predicate<TKey> keyCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }

            return Wrapped.Remove(hashCode, input => PrivateTryGetValue(input, out var foundKey) && keyCheck.Invoke(foundKey), out value);
        }

        /// <summary>
        ///     Removes a key by hash code, key predicate and value predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="valueCheck">The value predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
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

            return Wrapped.Remove(hashCode, input => PrivateTryGetValue(input, out var foundKey) && keyCheck.Invoke(foundKey), valueCheck, out value);
        }

        public int RemoveDeadItems()
        {
            return Wrapped.RemoveWhereKey(key => !key.IsAlive);
        }

        /// <summary>
        ///     Removes the keys and associated values where the key satisfies the predicate.
        /// </summary>
        /// <param name="keyCheck">The predicate.</param>
        /// <returns>
        ///     The number or removed pairs of keys and associated values.
        /// </returns>
        /// <remarks>
        ///     It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
        public int RemoveWhereKey(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }

            return Wrapped.RemoveWhereKey(input => PrivateTryGetValue(input, out var foundKey) && keyCheck.Invoke(foundKey));
        }

        /// <summary>
        ///     Removes the keys and associated values where the key satisfies the predicate.
        /// </summary>
        /// <param name="keyCheck">The predicate.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the removed pairs.
        /// </returns>
        /// <remarks>
        ///     It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
        public IEnumerable<TValue> RemoveWhereKeyEnumerable(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }

            return Wrapped.RemoveWhereKeyEnumerable(input => PrivateTryGetValue(input, out var foundKey) && keyCheck.Invoke(foundKey));
        }

        /// <summary>
        ///     Removes the keys and associated values where the value satisfies the predicate.
        /// </summary>
        /// <param name="valueCheck">The predicate.</param>
        /// <returns>
        ///     The number or removed pairs of keys and associated values.
        /// </returns>
        /// <remarks>
        ///     It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="valueCheck" /> is <c>null</c>.</exception>
        public int RemoveWhereValue(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }

            return Wrapped.RemoveWhereValue(valueCheck);
        }

        /// <summary>
        ///     Removes the keys and associated values where the value satisfies the predicate.
        /// </summary>
        /// <param name="valueCheck">The predicate.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the removed pairs.
        /// </returns>
        /// <remarks>
        ///     It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be removed.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="valueCheck" /> is <c>null</c>.</exception>
        public IEnumerable<TValue> RemoveWhereValueEnumerable(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }

            return Wrapped.RemoveWhereValueEnumerable(valueCheck);
        }

        /// <summary>
        ///     Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            Wrapped.Set(needle, input => !input.IsAlive, value);
        }

        /// <summary>
        ///     Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="isNew">if set to <c>true</c> the item value was set.</param>
        public void Set(TKey key, TValue value, out bool isNew)
        {
            var needle = PrivateGetNeedle(key);
            Wrapped.Set(needle, input => !input.IsAlive, value, out isNew);
        }

        /// <summary>
        ///     Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(TKey key, TValue value)
        {
            var needle = PrivateGetNeedle(key);
            if (Wrapped.TryAdd(needle, input => !input.IsAlive, value))
            {
                return true;
            }

            _reservoir.DonateNeedle(needle);
            return false;
        }

        /// <summary>
        ///     Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="stored">The stored pair independently of success.</param>
        /// <returns>
        ///     <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(TKey key, TValue value, out KeyValuePair<TKey, TValue> stored)
        {
            // No risk of dead needles here
            var needle = PrivateGetNeedle(key);

            bool Check(WeakNeedle<TKey> found)
            {
                if (!PrivateTryGetValue(found, out var foundKey))
                {
                    return true;
                }

                // Keeping the found key alive
                // If we found a key, key will be the key found
                // If we didn't key will be the key added
                // So, either way key is the key that is stored
                // By having it here, we don't need to read _stored.Key
                key = foundKey;
                return false;
            }

            var result = Wrapped.TryAdd(needle, Check, value, out var storedPair);
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

            TValue Factory(WeakNeedle<TKey> pairKey, TValue _)
            {
                return valueFactory(PrivateTryGetValue(pairKey, out var foundKey) ? foundKey : key);
            }

            if (Wrapped.TryGetOrAdd(needle, () => valueFactory(key), Factory, out stored))
            {
                return true;
            }

            _reservoir.DonateNeedle(needle);
            return false;
        }

        public bool TryGetOrAdd(TKey key, TValue value, out TValue stored)
        {
            var needle = PrivateGetNeedle(key);
            if (Wrapped.TryGetOrAdd(needle, input => !input.IsAlive, value, out stored))
            {
                return true;
            }

            _reservoir.DonateNeedle(needle);
            return false;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Tries to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            bool Check(WeakNeedle<TKey> found)
            {
                return PrivateTryGetValue(found, out var foundKey) && Comparer.Equals(key, foundKey);
            }

            return Wrapped.TryGetValue(Comparer.GetHashCode(key), Check, out value);
        }

        /// <summary>
        ///     Tries to retrieve the value by hash code and key predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
        public bool TryGetValue(int hashCode, Predicate<TKey> keyCheck, out TValue value)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }

            bool Check(WeakNeedle<TKey> found)
            {
                return PrivateTryGetValue(found, out var foundKey) && keyCheck(foundKey);
            }

            return Wrapped.TryGetValue(hashCode, Check, out value);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            var needle = PrivateGetNeedle(key);
            if (Wrapped.TryUpdate(needle, newValue, comparisonValue))
            {
                return true;
            }

            _reservoir.DonateNeedle(needle);
            return false;
        }

        public bool TryUpdate(TKey key, TValue newValue, Predicate<TValue> valueCheck)
        {
            var needle = PrivateGetNeedle(key);
            if (Wrapped.TryUpdate(needle, newValue, valueCheck))
            {
                return true;
            }

            _reservoir.DonateNeedle(needle);
            return false;
        }

        public bool TryUpdate(TKey key, Func<TValue, TValue> newValue)
        {
            var needle = PrivateGetNeedle(key);
            if (Wrapped.TryUpdate(needle, newValue))
            {
                return true;
            }

            _reservoir.DonateNeedle(needle);
            return false;
        }

        /// <summary>
        ///     Returns the values where the key satisfies the predicate.
        /// </summary>
        /// <param name="keyCheck">The predicate.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the matched pairs.
        /// </returns>
        /// <remarks>
        ///     It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be
        ///     returned.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="keyCheck" /> is <c>null</c>.</exception>
        public IEnumerable<TValue> Where(Predicate<TKey> keyCheck)
        {
            if (keyCheck == null)
            {
                throw new ArgumentNullException(nameof(keyCheck));
            }

            return Wrapped.Where(input => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey));
        }

        /// <summary>
        ///     Returns the values where the value satisfies the predicate.
        /// </summary>
        /// <param name="valueCheck">The predicate.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values of the matched pairs.
        /// </returns>
        /// <remarks>
        ///     It is not guaranteed that all the pairs of keys and associated values that satisfies the predicate will be
        ///     returned.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="valueCheck" /> is <c>null</c>.</exception>
        public IEnumerable<TValue> WhereValue(Predicate<TValue> valueCheck)
        {
            if (valueCheck == null)
            {
                throw new ArgumentNullException(nameof(valueCheck));
            }

            return Wrapped.WhereValue(valueCheck);
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

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            // No risk of dead needles here
            bool Check(WeakNeedle<TKey> input)
            {
                return PrivateTryGetValue(input, out var foundKey) && Comparer.Equals(foundKey, item.Key);
            }

            return Wrapped.ContainsKey(Comparer.GetHashCode(item.Key), Check, input => EqualityComparer<TValue>.Default.Equals(input, item.Value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            // No risk of dead needles here
            bool Check(WeakNeedle<TKey> input)
            {
                return PrivateTryGetValue(input, out var foundKey) && Comparer.Equals(foundKey, item.Key);
            }

            return Wrapped.Remove(Comparer.GetHashCode(item.Key), Check, input => EqualityComparer<TValue>.Default.Equals(input, item.Value), out _);
        }

        /// <summary>
        ///     Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">An item with the same key has already been added</exception>
        internal void AddNew(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var needle = PrivateGetNeedle(key);
            try
            {
                Wrapped.AddNew(needle, input => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value);
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
            if (!Wrapped.TryGetOrAdd(needle, input => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value, out var stored))
            {
                _reservoir.DonateNeedle(needle);
            }

            return stored;
        }

        /// <summary>
        ///     Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var needle = PrivateGetNeedle(key);
            Wrapped.Set(needle, input => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value);
        }

        /// <summary>
        ///     Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <param name="isNew">if set to <c>true</c> the item value was set.</param>
        internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out bool isNew)
        {
            // NOTICE this method has no null check
            var needle = PrivateGetNeedle(key);
            Wrapped.Set(needle, input => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value, out isNew);
        }

        /// <summary>
        ///     Attempts to add the specified key and associated value. The value is added if the key is not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyOverwriteCheck">The key predicate to approve overwriting.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        internal bool TryAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
        {
            // NOTICE this method has no null check
            var needle = PrivateGetNeedle(key);
            if (Wrapped.TryAdd(needle, input => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value))
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
            if (Wrapped.TryGetOrAdd(needle, input => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value, out stored))
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

        private static bool PrivateTryGetValue(WeakNeedle<TKey> needle, out TKey foundKey)
        {
            return needle.TryGetValue(out foundKey);
        }

        private WeakNeedle<TKey> PrivateGetNeedle(TKey key)
        {
            return _reservoir.GetNeedle(key);
        }
    }
}