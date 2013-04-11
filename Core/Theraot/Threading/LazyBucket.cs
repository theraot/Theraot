#if FAT
﻿using System;
using System.Collections.Generic;

namespace Theraot.Threading
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
        private Converter<int, T> _valueFactory;

        private Bucket<T> _wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket{T}" /> class.
        /// </summary>
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
                _wrapped = new Bucket<T>(capacity);
            }
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _wrapped.Capacity;
            }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> Values
        {
            get
            {
                return _wrapped.Values;
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
            Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
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
            return _wrapped.Insert(index, item);
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
            T _previous;
            if (_wrapped.Insert(index, item, out _previous))
            {
                previous = default(T);
                return true;
            }
            else
            {
                if (!ReferenceEquals(_previous, null))
                {
                    previous = _previous;
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
            return _wrapped.RemoveAt(index);
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
            T _previous;
            if (_wrapped.RemoveAt(index, out _previous))
            {
                if (!ReferenceEquals(_previous, null))
                {
                    previous = _previous;
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
            return _wrapped.Set(index, item, out isNew);
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
            T _previous;
            var newItem = _valueFactory.Invoke(index);
            if (_wrapped.Insert(index, newItem, out _previous))
            {
                value = newItem;
                return true;
            }
            else
            {
                if (!ReferenceEquals(_previous, null))
                {
                    value = _previous;
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
}
#endif