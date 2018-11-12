using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe lock-free hash based collection.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public sealed class SafeCollection<T> : ICollection<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private int _maxIndex;
        private SafeDictionary<int, T> _wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeCollection{T}" /> class.
        /// </summary>
        public SafeCollection()
            : this(EqualityComparer<T>.Default)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeCollection{T}" /> class.
        /// </summary>
        /// <param name="comparer">The value comparer.</param>
        public SafeCollection(IEqualityComparer<T> comparer)
        {
            _maxIndex = -1;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _wrapped = new SafeDictionary<int, T>();
        }

        public IEqualityComparer<T> Comparer
        {
            get { return _comparer; }
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            _wrapped.Set(Interlocked.Increment(ref _maxIndex), item);
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            _wrapped = new SafeDictionary<int, T>();
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        /// <returns>Returns the removed pairs.</returns>
        public IEnumerable<T> ClearEnumerable()
        {
            var replacement = new SafeDictionary<int, T>();
            Interlocked.Exchange(ref _wrapped, replacement);
            return Enumerable(replacement);
        }

        /// <summary>
        /// Determines whether the specified value is contained.
        /// </summary>
        /// <param name="item">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            foreach (var input in this)
            {
                if (_comparer.Equals(input, item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Predicate<T> itemCheck)
        {
            if (itemCheck == null)
            {
                throw new ArgumentNullException("itemCheck");
            }
            foreach (var input in this)
            {
                if (itemCheck(input))
                {
                    return true;
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
            Enumerable(_wrapped).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var p in Enumerable(_wrapped))
            {
                yield return p;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="item">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(T item)
        {
            Predicate<T> check = input => _comparer.Equals(input, item);
            return _wrapped.RemoveWhereValueEnumerable(check).Any();
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
            var matches = _wrapped.WhereValue(check);
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
            return RemoveWhereEnumerableExtracted();

            IEnumerable<T> RemoveWhereEnumerableExtracted()
            {
                var matches = _wrapped.WhereValue(check);
                foreach (var value in matches)
                {
                    if (Remove(value))
                    {
                        yield return value;
                    }
                }
            }
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
            return _wrapped.WhereValue(check);
        }

        private static IEnumerable<T> Enumerable(SafeDictionary<int, T> wrapped)
        {
            foreach (var pair in wrapped)
            {
                yield return pair.Value;
            }
        }
    }
}