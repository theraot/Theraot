using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
    public sealed class SafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        // TODO: add GetOrAdd and AddOrUpdate

        private const int INT_DefaultCapacity = 64;
        private const int INT_DefaultProbing = 1;

        private readonly IEqualityComparer<TKey> _keyComparer;
        private Mapper<KeyValuePair<TKey, TValue>> _mapper;
        private int _probing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        public SafeDictionary()
            : this(INT_DefaultCapacity, EqualityComparer<TKey>.Default, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        public SafeDictionary(int capacity)
            : this(capacity, EqualityComparer<TKey>.Default, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="initialProbing">The number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">initialProbing;initialProbing must be nonnegative and less than capacity.</exception>
        public SafeDictionary(int capacity, int initialProbing)
            : this(capacity, EqualityComparer<TKey>.Default, initialProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The key comparer.</param>
        public SafeDictionary(IEqualityComparer<TKey> comparer)
            : this(INT_DefaultCapacity, comparer, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="comparer">The key comparer.</param>
        /// <param name="initialProbing">The maximum number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">initialProbing;initialProbing must be nonnegative and less than capacity.</exception>
        public SafeDictionary(IEqualityComparer<TKey> comparer, int initialProbing)
            : this(INT_DefaultCapacity, comparer, initialProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The key comparer.</param>
        public SafeDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : this(capacity, comparer, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <param name="initialProbing">The number of steps in linear probing.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">initialProbing;initialProbing must be nonnegative and less than capacity.</exception>
        public SafeDictionary(int capacity, IEqualityComparer<TKey> comparer, int initialProbing)
        {
            if (initialProbing < 0 || initialProbing >= capacity)
            {
                throw new ArgumentOutOfRangeException("initialProbing", "initialProbing must be nonnegative and less than capacity.");
            }
            _keyComparer = comparer ?? EqualityComparer<TKey>.Default;
            _mapper = new Mapper<KeyValuePair<TKey, TValue>>();
            _probing = initialProbing;
        }

        public int Count
        {
            get
            {
                return _mapper.Count;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
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
                // TODO
                throw new NotImplementedException();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                // TODO
                throw new NotImplementedException();
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

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            AddNew(item.Key, item.Value);
        }

        /// <summary>
        /// Adds the specified key and associated value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">the key is already present</exception>
        public void AddNew(TKey key, TValue value)
        {
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var hashcode = _keyComparer.GetHashCode(key);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> found;
                if (_mapper.Insert(hashcode + attempts, neo, out found))
                {
                    return;
                }
                if (_keyComparer.Equals(found.Key, key))
                {
                    throw new ArgumentException("the key is already present");
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
            var hashcode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
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
        /// <param name="hashcode">The hashcode to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is contained; otherwise, <c>false</c>.
        /// </returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "hashcode")]
        public bool ContainsKey(int hashcode, Predicate<TKey> keyCheck)
        {
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (keyCheck(found.Key))
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

        /// <summary>
        /// Gets the pairs contained in this object.
        /// </summary>
        public IList<KeyValuePair<TKey, TValue>> GetPairs()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(_mapper.Count);
            foreach (var pair in _mapper)
            {
                result.Add(pair);
            }
            return result;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            int hashcode = _keyComparer.GetHashCode(item.Key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, item.Key))
                    {
                        if (EqualityComparer<TValue>.Default.Equals(found.Value, item.Value))
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            int hashcode = _keyComparer.GetHashCode(item.Key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, item.Key))
                    {
                        // Since this class will never relocate a key, we can just remove at this position
                        if (EqualityComparer<TValue>.Default.Equals(found.Value, item.Value))
                        {
                            if (_mapper.RemoveAt(hashcode + attempts, out found))
                            {
                                return true;
                            }
                        }
                        // Another thread removed first - or the value did not match
                        return false;
                    }
                }
            }
            return false;
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            AddNew(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            var hashcode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, key))
                    {
                        // Since this class will never relocate a key, we can just remove at this position
                        if (_mapper.RemoveAt(hashcode + attempts))
                        {
                            return true;
                        }
                        // Another thread removed first
                        return false;
                    }
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
            var hashcode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, key))
                    {
                        // Since this class will never relocate a key, we can just remove at this position
                        if (_mapper.RemoveAt(hashcode + attempts, out found))
                        {
                            value = found.Value;
                            return true;
                        }
                        // Another thread removed first
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a key by hashcode and a key predicate.
        /// </summary>
        /// <param name="hashcode">The hashcode to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key was removed; otherwise, <c>false</c>.
        /// </returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "hashcode")]
        public bool Remove(int hashcode, Predicate<TKey> keyCheck, out TValue value)
        {
            value = default(TValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (keyCheck(found.Key))
                    {
                        // Since this class will never relocate a key, we can just remove at this position
                        if (_mapper.RemoveAt(hashcode + attempts, out found))
                        {
                            value = found.Value;
                            return true;
                        }
                        // Another thread removed first
                        return false;
                    }
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
            var matches = _mapper.Where(pair => keyCheck(pair.Key));
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
            var matches = _mapper.Where(pair => keyCheck(pair.Key));
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
            var hashcode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (_keyComparer.Equals(found.Key, key))
                    {
                        // Since this class will never relocate a key, we can just set at this position
                        bool isNew;
                        _mapper.Set(hashcode + attempts, neo, out isNew);
                        // Done
                        return;
                    }
                }
                else
                {
                    // This is an empty slot to store this value...
                    if (_mapper.Insert(hashcode + attempts, neo))
                    {
                        // Done
                        return;
                    }
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
            var hashcode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                KeyValuePair<TKey, TValue> found;
                if (_mapper.Insert(hashcode + attempts, neo, out found))
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
            var hashcode = _keyComparer.GetHashCode(key);
            var neo = new KeyValuePair<TKey, TValue>(key, value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                if (_mapper.Insert(hashcode + attempts, neo, out stored))
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
            var hashcode = _keyComparer.GetHashCode(key);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
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

        /// <summary>
        /// Tries to retrieve the value by hashcode and key predicate.
        /// </summary>
        /// <param name="hashcode">The hashcode to look for.</param>
        /// <param name="keyCheck">The key predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "hashcode")]
        public bool TryGetValue(int hashcode, Predicate<TKey> keyCheck, out TValue value)
        {
            value = default(TValue);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                KeyValuePair<TKey, TValue> found;
                if (_mapper.TryGet(hashcode + attempts, out found))
                {
                    if (keyCheck(found.Key))
                    {
                        value = found.Value;
                        return true;
                    }
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
            var matches = _mapper.Where(pair => keyCheck(pair.Key));
            foreach (var pair in matches)
            {
                yield return pair.Value;
            }
        }

        private void ExtendProbingIfNeeded(int attempts)
        {
            var diff = attempts - _probing;
            if (diff > 0)
            {
                Interlocked.Add(ref _probing, diff);
            }
        }
    }
}