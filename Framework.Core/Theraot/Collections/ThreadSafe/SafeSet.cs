// Needed for NET40

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
    public class SafeSet<T> : IEnumerable<T>, ISet<T>
    {
        private const int _defaultProbing = 1;
        private Bucket<T> _bucket;
        private int _probing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeSet{T}" /> class.
        /// </summary>
        public SafeSet()
            : this(EqualityComparer<T>.Default, _defaultProbing)
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
            : this(comparer, _defaultProbing)
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
            Comparer = comparer ?? EqualityComparer<T>.Default;
            _bucket = new Bucket<T>();
            _probing = initialProbing;
        }

        public IEqualityComparer<T> Comparer { get; private set; }

        public int Count
        {
            get { return _bucket.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public bool Add(T item)
        {
            var hashCode = Comparer.GetHashCode(item);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                if (_bucket.Insert(hashCode + attempts, item, out T found))
                {
                    return true;
                }
                if (Comparer.Equals(found, item))
                {
                    return false;
                }
                attempts++;
            }
        }

        void ICollection<T>.Add(T item)
        {
            AddNew(item);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">the value is already present</exception>
        public void AddNew(T value)
        {
            var hashCode = Comparer.GetHashCode(value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                if (_bucket.Insert(hashCode + attempts, value, out T found))
                {
                    return;
                }
                if (Comparer.Equals(found, value))
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
            _bucket = new Bucket<T>();
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        /// <returns>Returns the removed pairs.</returns>
        public IEnumerable<T> ClearEnumerable()
        {
            return Interlocked.Exchange(ref _bucket, _bucket = new Bucket<T>());
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
            var hashCode = Comparer.GetHashCode(value);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                if (_bucket.TryGet(hashCode + attempts, out T found))
                {
                    if (Comparer.Equals(found, value))
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
                throw new ArgumentNullException(nameof(check));
            }
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                if (_bucket.TryGet(hashCode + attempts, out T found))
                {
                    if (Comparer.GetHashCode(found) == hashCode && check(found))
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
            _bucket.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            Extensions.ExceptWith(this, other);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _bucket.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the pairs contained in this object.
        /// </summary>
        /// <returns>The pairs contained in this object</returns>
        public IList<T> GetValues()
        {
            var result = new List<T>(_bucket.Count);
            foreach (var pair in _bucket)
            {
                result.Add(pair);
            }
            return result;
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            Extensions.IntersectWith(this, other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Extensions.Overlaps(this, other);
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
            var hashCode = Comparer.GetHashCode(value);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                var result = _bucket.RemoveAt
                    (
                        hashCode + attempts,
                        found =>
                        {
                            if (Comparer.Equals(found, value))
                            {
                                done = true;
                                return true;
                            }
                            return false;
                        }
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
            var hashCode = Comparer.GetHashCode(value);
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                var tmp = default(T);
                var result = _bucket.RemoveAt
                    (
                        hashCode + attempts,
                        found =>
                        {
                            tmp = found;
                            if (Comparer.Equals(found, value))
                            {
                                done = true;
                                return true;
                            }
                            return false;
                        }
                    );
                if (done)
                {
                    previous = tmp;
                    return result;
                }
            }
            previous = default;
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
                throw new ArgumentNullException(nameof(check));
            }
            value = default;
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                var done = false;
                var previous = default(T);
                var result = _bucket.RemoveAt
                    (
                        hashCode + attempts,
                        found =>
                        {
                            previous = found;
                            if (Comparer.GetHashCode(found) == hashCode && check(found))
                            {
                                done = true;
                                return true;
                            }
                            return false;
                        }
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
                throw new ArgumentNullException(nameof(check));
            }
            var matches = _bucket.Where(check);
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
                throw new ArgumentNullException(nameof(check));
            }
            return RemoveWhereEnumerableExtracted();

            IEnumerable<T> RemoveWhereEnumerableExtracted()
            {
                var matches = _bucket.Where(check);
                foreach (var value in matches)
                {
                    if (Remove(value))
                    {
                        yield return value;
                    }
                }
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Extensions.SymmetricExceptWith(this, other);
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
                throw new ArgumentNullException(nameof(check));
            }
            value = default;
            for (var attempts = 0; attempts < _probing; attempts++)
            {
                if (_bucket.TryGet(hashCode + attempts, out T found))
                {
                    if (Comparer.GetHashCode(found) == hashCode && check(found))
                    {
                        value = found;
                        return true;
                    }
                }
            }
            return false;
        }

        public void UnionWith(IEnumerable<T> other)
        {
            Extensions.UnionWith(this, other);
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
                throw new ArgumentNullException(nameof(check));
            }
            return _bucket.Where(check);
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
                throw new ArgumentNullException(nameof(valueOverwriteCheck));
            }
            var hashCode = Comparer.GetHashCode(value);
            var attempts = 0;
            while (true)
            {
                ExtendProbingIfNeeded(attempts);
                bool check(T found)
                {
                    if (Comparer.Equals(found, value))
                    {
                        // This is the item that has been stored with the key
                        // Throw to abort overwrite
                        throw new ArgumentException("The item has already been added");
                    }
                    // This is not the value, overwrite?
                    return valueOverwriteCheck(found);
                }
                try
                {
                    // TryGetCheckSet will add if no item is found, otherwise it calls check
                    if (_bucket.InsertOrUpdateChecked(hashCode + attempts, value, check, out bool isNew))
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