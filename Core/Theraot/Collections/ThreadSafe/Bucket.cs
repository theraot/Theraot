using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe wait-free fixed size bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="ICollection{T}" /> or any other desired interface.
    /// </remarks>
    [Serializable]
    public sealed class Bucket<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private int _count;
        private object[] _entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public Bucket(int capacity)
        {
            _count = 0;
            _entries = ArrayReservoir<object>.GetArray(capacity);
            _capacity = _entries.Length;
        }

        ~Bucket()
        {
            if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                RecycleExtracted();
            }
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
        /// Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was new; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool Exchange(int index, T item, out T previous)
        {
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                previous = default(T);
                object _previous;
                ExchangeExtracted(index, item, out _previous);
                if (_previous == null)
                {
                    Interlocked.Increment(ref _count);
                    return true;
                }
                if (!ReferenceEquals(_previous, BucketHelper.Null))
                {
                    previous = (T)_previous;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
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
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            var result = new List<T>();
            for (int index = 0; index < _entries.Length; index++)
            {
                var entry = Interlocked.CompareExchange(ref _entries[index], null, null);
                if (entry != null)
                {
                    if (!ReferenceEquals(entry, BucketHelper.Null))
                    {
                        result.Add((T)entry);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetValues<TOutput>(Converter<T, TOutput> converter)
        {
            if (ReferenceEquals(converter, null))
            {
                throw new ArgumentNullException("converter");
            }
            else
            {
                var result = new List<TOutput>();
                for (int index = 0; index < _entries.Length; index++)
                {
                    var entry = Interlocked.CompareExchange(ref _entries[index], null, null);
                    if (entry != null)
                    {
                        if (!ReferenceEquals(entry, BucketHelper.Null))
                        {
                            result.Add(converter.Invoke((T)entry));
                        }
                    }
                }
                return result;
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
                return InsertExtracted(index, item, out previous);
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
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public void Set(int index, T item, out bool isNew)
        {
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            else
            {
                SetExtracted(index, item, out isNew);
                if (isNew)
                {
                    Interlocked.Increment(ref _count);
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
                return TryGetExtracted(index, out value);
            }
        }

        internal bool InsertExtracted(int index, T item, out T previous)
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

        internal bool TryGetExtracted(int index, out T value)
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

        private void ExchangeExtracted(int index, object item, out object previous)
        {
            previous = Interlocked.Exchange(ref _entries[index], item ?? BucketHelper.Null);
        }

        private bool InsertExtracted(int index, object item, out object previous)
        {
            previous = Interlocked.CompareExchange(ref _entries[index], item ?? BucketHelper.Null, null);
            return previous == null;
        }

        private void RecycleExtracted()
        {
            ArrayReservoir<object>.DonateArray(_entries);
            _entries = null;
        }

        private bool RemoveAtExtracted(int index, out object previous)
        {
            previous = Interlocked.Exchange(ref _entries[index], null);
            return previous != null;
        }

        private void SetExtracted(int index, object item, out bool isNew)
        {
            isNew = Interlocked.Exchange(ref _entries[index], item ?? BucketHelper.Null) == null;
        }
    }
}