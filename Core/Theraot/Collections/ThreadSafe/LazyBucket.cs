using System;
using System.Collections.Generic;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe wait-free fixed size bucket with lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="ICollection{T}" /> or any other desired interface.
    /// </remarks>
    public sealed class LazyBucket<T> : IEnumerable<T>
    {
        private readonly Bucket<LazyNeedle<T>> _entries;
        private Converter<int, T> _valueFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket{T}" /> class.
        /// </summary>
        /// <param name = "valueFactory">The delegate that is invoked to do the lazy initialization of the items given their index.</param>
        /// <param name="capacity">The capacity.</param>
        public LazyBucket(Converter<int, T> valueFactory, int capacity)
        {
            if (ReferenceEquals(valueFactory, null))
            {
                throw new ArgumentNullException("valueFactory");
            }
            else
            {
                _valueFactory = valueFactory;
                _entries = new Bucket<LazyNeedle<T>>(capacity);
            }
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _entries.Capacity;
            }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _entries.Count;
            }
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
            GetValues().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Retrieve or creates a new item item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public T Get(int index)
        {
            LazyNeedle<T> _previous;
            var newItem = new LazyNeedle<T>(() => _valueFactory.Invoke(index));
            if (_entries.Insert(index, newItem, out _previous))
            {
                return newItem.Value;
            }
            else
            {
                //_previous should be null because null is never added
                return _previous.Value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Extensions.ConvertProgressiveFiltered<LazyNeedle<T>, T>(_entries, input => input.Value, input => input.IsCompleted).GetEnumerator();
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            return _entries.GetValues<T>(input => input.Value);
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetValues<TOutput>(Converter<T, TOutput> converter)
        {
            return _entries.GetValues<TOutput>(input => converter.Invoke(input.Value));
        }

        /// <summary>
        /// Inserts the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the item was overwritten or removed.
        /// </remarks>
        public bool Insert(int index, T item)
        {
            return _entries.Insert(index, new LazyNeedle<T>(null, item));
        }

        /// <summary>
        /// Inserts the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the item was overwritten or removed.
        /// </remarks>
        public bool Insert(int index, T item, out T previous)
        {
            LazyNeedle<T> _previous;
            if (_entries.Insert(index, new LazyNeedle<T>(null, item), out _previous))
            {
                previous = default(T);
                return true;
            }
            else
            {
                //_previous should be null because null is never added
                if (_previous.IsCompleted)
                {
                    previous = _previous.Value;
                }
                else
                {
                    previous = default(T);
                }
                return false;
            }
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index)
        {
            return _entries.RemoveAt(index);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index, out T previous)
        {
            LazyNeedle<T> _previous;
            if (_entries.RemoveAt(index, out _previous))
            {
                //_previous should be null because null is never added
                if (_previous.IsCompleted)
                {
                    previous = _previous.Value;
                }
                else
                {
                    previous = default(T);
                }
                return true;
            }
            else
            {
                previous = default(T);
                return false;
            }
        }

        /// <summary>
        /// Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item was set; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool Set(int index, T item, out bool isNew)
        {
            return _entries.Set(index, new LazyNeedle<T>(null, item), out isNew);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Tries to retrieve the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool TryGet(int index, out T value)
        {
            LazyNeedle<T> _previous;
            if (_entries.TryGet(index, out _previous))
            {
                value = _previous.Value;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }
    }
}