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
        private int _maxIndex;
        private Bucket<T> _wrapped;

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
            Comparer = comparer ?? EqualityComparer<T>.Default;
            _wrapped = new Bucket<T>();
        }

        public IEqualityComparer<T> Comparer { get; }

        public int Count => _wrapped.Count;

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T item)
        {
            _wrapped.Set(Interlocked.Increment(ref _maxIndex), item);
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        public void Clear()
        {
            _wrapped = new Bucket<T>();
        }

        /// <summary>
        /// Removes all the elements.
        /// </summary>
        /// <returns>Returns the removed pairs.</returns>
        public IEnumerable<T> ClearEnumerable()
        {
            var replacement = new Bucket<T>();
            Interlocked.Exchange(ref _wrapped, replacement);
            return replacement;
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
            return _wrapped.Where(Check).Any();
            bool Check(T input) => Comparer.Equals(input, item);
        }

        public bool Contains(Predicate<T> itemCheck)
        {
            return _wrapped.Where(itemCheck).Any();
        }

        /// <summary>
        /// Copies the items to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="ArgumentNullException">array</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex;Non-negative number is required.</exception>
        /// <exception cref="ArgumentException">array;The array can not contain the number of elements.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(_wrapped, array, arrayIndex);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
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
            return _wrapped.RemoveWhereEnumerable(Check).Any();
            bool Check(T input) => Comparer.Equals(input, item);
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
            return _wrapped.RemoveWhere(check);
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
            return _wrapped.RemoveWhereEnumerable(check);
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
            return _wrapped.Where(check);
        }
    }
}