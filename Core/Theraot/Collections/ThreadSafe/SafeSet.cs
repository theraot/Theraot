using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe lock-free hash based dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    [Serializable]
    public class SafeSet<T> : IEnumerable<T>
    {
        // TODO: Implement ISet<T>

        private const int INT_DefaultProbing = 1;

        private readonly IEqualityComparer<T> _comparer;
        private Mapper<T> _mapper;
        private int _probing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeSet{T}" /> class.
        /// </summary>
        public SafeSet()
            : this(EqualityComparer<T>.Default, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeSet{T}" /> class.
        /// </summary>
        /// <param name="initialProbing">The number of steps in linear probing.</param>
        public SafeSet(int initialProbing)
            : this(EqualityComparer<T>.Default, initialProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeSet{T}" /> class.
        /// </summary>
        /// <param name="comparer">The value comparer.</param>
        public SafeSet(IEqualityComparer<T> comparer)
            : this(comparer, INT_DefaultProbing)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeSet{T}" /> class.
        /// </summary>
        /// <param name="comparer">The value comparer.</param>
        /// <param name="initialProbing">The number of steps in linear probing.</param>
        public SafeSet(IEqualityComparer<T> comparer, int initialProbing)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _mapper = new Mapper<T>();
            _probing = initialProbing;
        }

        public IEqualityComparer<T> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        public int Count
        {
            get
            {
                return _mapper.Count;
            }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">the value is already present</exception>
        public void AddNew(T value)
        {
            var hashCode = _comparer.GetHashCode(value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                T found;
                if (_mapper.Insert(hashCode + attempts, value, out found))
                {
                    return;
                }
                if (_comparer.Equals(found, value))
                {
                    throw new ArgumentException("the value is already present");
                }
                attempts++;
            }
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            _mapper = new Mapper<T>();
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public IEnumerable<T> ClearEnumerable()
        {
            return Interlocked.Exchange(ref _mapper, _mapper = new Mapper<T>());
        }

        /// <summary>
        /// Determines whether the specified value is contained.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            var hashCode = _comparer.GetHashCode(value);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                T found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_comparer.Equals(found, value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified value is contained.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="check">The value predicate.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int hashCode, Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                T found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_comparer.GetHashCode(found) == hashCode && check(found))
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
        public void CopyTo(T[] array, int arrayIndex)
        {
            _mapper.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _mapper.GetEnumerator();
        }

        /// <summary>
        /// Gets the pairs contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            var result = new List<T>(_mapper.Count);
            foreach (var pair in _mapper)
            {
                result.Add(pair);
            }
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(T value)
        {
            var hashCode = _comparer.GetHashCode(value);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                T previous;
                var result = _mapper.TryGetCheckRemoveAt
                    (
                        hashCode + attempts,
                        found =>
                        {
                            if (_comparer.Equals((T)found, value))
                            {
                                done = true;
                                return true;
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
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="previous">The found value that was removed.</param>
        /// <returns>
        ///   <c>true</c> if the specified value was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(T value, out T previous)
        {
            var hashCode = _comparer.GetHashCode(value);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                var result = _mapper.TryGetCheckRemoveAt
                    (
                        hashCode + attempts,
                        found =>
                        {
                            if (_comparer.Equals((T)found, value))
                            {
                                done = true;
                                return true;
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
            previous = default(T);
            return false;
        }

        /// <summary>
        /// Removes a value by hash code and a value predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="check">The value predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(int hashCode, Predicate<T> check, out T value)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            value = default(T);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                T previous;
                var result = _mapper.TryGetCheckRemoveAt
                    (
                        hashCode + attempts,
                        found =>
                        {
                            var _found = (T)found;
                            if (_comparer.GetHashCode(_found) == hashCode && check(_found))
                            {
                                done = true;
                                return true;
                            }
                            return false;
                        },
                        out previous
                    );
                if (done)
                {
                    value = previous;
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the values where the predicate is satisfied.
        /// </summary>
        /// <param name="check">The predicate.</param>
        /// <returns>
        /// The number or removed values.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the values that satisfies the predicate will be removed.
        /// </remarks>
        public int RemoveWhere(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            var matches = _mapper.Where(check);
            var count = 0;
            foreach (var value in matches)
            {
                if (Remove(value))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Removes the values where the predicate is satisfied.
        /// </summary>
        /// <param name="check">The predicate.</param>
        /// <returns>
        /// An <see cref="IEnumerable{TValue}" /> that allows to iterate over the removed values.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the values that satisfies the predicate will be removed.
        /// </remarks>
        public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            var matches = _mapper.Where(check);
            foreach (var value in matches)
            {
                if (Remove(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Attempts to add the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value was added; otherwise, <c>false</c>.
        /// </returns>
        public bool TryAdd(T value)
        {
            var hashCode = _comparer.GetHashCode(value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                T found;
                if (_mapper.Insert(hashCode + attempts, value, out found))
                {
                    return true;
                }
                if (_comparer.Equals(found, value))
                {
                    return false;
                }
                attempts++;
            }
        }

        /// <summary>
        /// Tries to retrieve the value by hash code and value predicate.
        /// </summary>
        /// <param name="hashCode">The hash code to look for.</param>
        /// <param name="check">The value predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the value was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(int hashCode, Predicate<T> check, out T value)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            value = default(T);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                T found;
                if (_mapper.TryGet(hashCode + attempts, out found))
                {
                    if (_comparer.GetHashCode(found) == hashCode && check(found))
                    {
                        value = found;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the values where the predicate is satisfied.
        /// </summary>
        /// <param name="check">The predicate.</param>
        /// <returns>
        /// An <see cref="IEnumerable{TValue}" /> that allows to iterate over the values.
        /// </returns>
        /// <remarks>
        /// It is not guaranteed that all the values that satisfies the predicate will be returned.
        /// </remarks>
        public IEnumerable<T> Where(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            return _mapper.Where(check);
        }

        /// <summary>
        /// Attempts to add the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueOverwriteCheck">The value predicate to approve overwriting.</param>
        /// <returns>
        ///   <c>true</c> if the specified key and associated value were added; otherwise, <c>false</c>.
        /// </returns>
        internal bool TryAdd(T value, Predicate<T> valueOverwriteCheck)
        {
            if (valueOverwriteCheck == null)
            {
                throw new ArgumentNullException("valueOverwriteCheck");
            }
            var hashCode = _comparer.GetHashCode(value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                Predicate<object> check = found =>
                {
                    var _found = (T)found;
                    if (_comparer.Equals(_found, value))
                    {
                        // This is the item that has been stored with the key
                        // Throw to abort overwrite
                        throw new ArgumentException("The item has already been added");
                    }
                    // This is not the value, overwrite?
                    return valueOverwriteCheck(_found);
                };
                try
                {
                    bool isNew;
                    // TryGetCheckSet will add if no item is found, otherwise it calls check
                    if (_mapper.TryGetCheckSet(hashCode + attempts, value, check, out isNew))
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