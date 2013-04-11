using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading
{
    /// <summary>
    /// Represent a thread-safe wait-free fixed size bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="ICollection{T}" /> or any other desired interface.
    /// </remarks>
    public sealed class Bucket<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private readonly object[] _entries;

        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public Bucket(int capacity)
        {
            _capacity = capacity;
            _count = 0;
            _entries = new object[_capacity];
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> Values
        {
            get
            {
                var result = new List<T>();
                for (int index = 0; index < _entries.Length; index++)
                {
                    var entry = Interlocked.CompareExchange(ref _entries[index], null, null);
                    if (entry != null)
                    {
                        if (ReferenceEquals(entry, BucketHelper.Null))
                        {
                            result.Add(default(T));
                        }
                        else
                        {
                            result.Add((T)entry);
                        }
                    }
                }
                return result;
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
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (_count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
            try
            {
                foreach (var entry in _entries)
                {
                    if (entry != null)
                    {
                        if (ReferenceEquals(entry, BucketHelper.Null))
                        {
                            array[arrayIndex] = default(T);
                        }
                        else
                        {
                            array[arrayIndex] = (T)entry;
                        }
                        arrayIndex++;
                    }
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentOutOfRangeException("array", exception.Message);
            }
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in _entries)
            {
                if (entry != null)
                {
                    if (ReferenceEquals(entry, BucketHelper.Null))
                    {
                        yield return default(T);
                    }
                    else
                    {
                        yield return (T)entry;
                    }
                }
            }
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
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
            }
            else
            {
                object _previous;
                if (InsertExtracted(index, item, out _previous))
                {
                    Interlocked.Increment(ref _count);
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                object _previous;
                if (InsertExtracted(index, item, out _previous))
                {
                    previous = default(T);
                    Interlocked.Increment(ref _count);
                    return true;
                }
                else
                {
                    if (ReferenceEquals(_previous, BucketHelper.Null))
                    {
                        previous = default(T);
                    }
                    else
                    {
                        previous = (T)_previous;
                    }
                    return false;
                }
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
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                object _previous;
                if (RemoveAtExtracted(index, out _previous))
                {
                    Interlocked.Decrement(ref _count);
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                object _previous;
                if (RemoveAtExtracted(index, out _previous))
                {
                    Interlocked.Decrement(ref _count);
                    if (ReferenceEquals(_previous, BucketHelper.Null))
                    {
                        previous = default(T);
                    }
                    else
                    {
                        previous = (T)_previous;
                    }
                    return true;
                }
                else
                {
                    previous = default(T);
                    return false;
                }
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
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                if (SetExtracted(index, item, out isNew))
                {
                    if (isNew)
                    {
                        Interlocked.Increment(ref _count);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                var entry = Interlocked.CompareExchange(ref _entries[index], null, null);
                if (entry == null)
                {
                    value = default(T);
                    return false;
                }
                else
                {
                    if (ReferenceEquals(entry, BucketHelper.Null))
                    {
                        value = default(T);
                    }
                    else
                    {
                        value = (T)entry;
                    }
                    return true;
                }
            }
        }

        private bool InsertExtracted(int index, object item, out object previous)
        {
            previous = Interlocked.CompareExchange(ref _entries[index], item ?? BucketHelper.Null, null);
            return previous == null;
        }

        private bool RemoveAtExtracted(int index, out object previous)
        {
            previous = Interlocked.Exchange(ref _entries[index], null);
            return previous != null;
        }

        private bool SetExtracted(int index, object item, out bool isNew)
        {
            isNew = Interlocked.Exchange(ref _entries[index], item ?? BucketHelper.Null) == null;
            return true;
        }
    }
}