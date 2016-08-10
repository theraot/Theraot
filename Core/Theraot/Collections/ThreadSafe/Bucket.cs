// Needed for NET40

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
    public sealed class Bucket<T> : IEnumerable<T>, IBucket<T>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket{T}" /> class.
        /// </summary>
        public Bucket(IEnumerable<T> source)
        {
            var collection = source as ICollection<T>;
            _entries = ArrayReservoir<object>.GetArray(collection == null ? 64 : collection.Count);
            _capacity = _entries.Length;
            foreach (var item in source)
            {
                if (_count == _capacity)
                {
                    var old = _entries;
                    _entries = ArrayReservoir<object>.GetArray(_capacity << 1);
                    if (old != null)
                    {
                        Array.Copy(old, 0, _entries, 0, _count);
                        ArrayReservoir<object>.DonateArray(old);
                    }
                    _capacity = _entries.Length;
                }
                _entries[_count] = (object) item ?? BucketHelper.Null;
                _count++;
            }
        }

        ~Bucket()
        {
            if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    ArrayReservoir<object>.DonateArray(_entries);
                    _entries = null;
                }
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
            return ExchangeInternal(index, item, out previous);
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
            return InsertInternal(index, item);
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
            return InsertInternal(index, item, out previous);
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
            var found = Interlocked.Exchange(ref _entries[index], null);
            if (found != null)
            {
                Interlocked.Decrement(ref _count);
                return true;
            }
            return false;
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
            return RemoveAtInternal(index, out previous);
        }

        /// <summary>
        /// Removes the item at the specified index if it matches the specified value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value intended to remove.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveValueAt(int index, T value, out T previous)
        {
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            previous = default(T);
            var found = Interlocked.CompareExchange(ref _entries[index], null, value);
            if (found == null)
            {
                return false;
            }
            Interlocked.Decrement(ref _count);
            if (!ReferenceEquals(found, BucketHelper.Null))
            {
                previous = (T)found;
            }
            return true;
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
            SetInternal(index, item, out isNew);
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
            return TryGetInternal(index, out value);
        }

        /// <summary>
        /// Replaces the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The new item.</param>
        /// <param name="comparisonItem">The old item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the item was overwritten or removed.
        /// </remarks>
        public bool Update(int index, T item, T comparisonItem, out T previous, out bool isNew)
        {
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
            }
            return UpdateInternal(index, item, comparisonItem, out previous, out isNew);
        }

        public IEnumerable<T> Where(Predicate<T> predicate)
        {
            foreach (var entry in _entries)
            {
                if (entry != null)
                {
                    T yield = default(T);
                    if (!ReferenceEquals(entry, BucketHelper.Null))
                    {
                        yield = (T)entry;
                    }
                    if (predicate(yield))
                    {
                        yield return yield;
                    }
                }
            }
        }

        internal bool ExchangeInternal(int index, T item, out T previous)
        {
            previous = default(T);
            var found = Interlocked.Exchange(ref _entries[index], (object) item ?? BucketHelper.Null);
            if (found == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            if (!ReferenceEquals(found, BucketHelper.Null))
            {
                previous = (T)found;
            }
            return false;
        }

        internal bool InsertInternal(int index, T item, out T previous)
        {
            previous = default(T);
            var found = Interlocked.CompareExchange(ref _entries[index], (object) item ?? BucketHelper.Null, null);
            if (found == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            if (!ReferenceEquals(found, BucketHelper.Null))
            {
                previous = (T) found;
            }
            return false;
        }

        internal bool InsertInternal(int index, T item)
        {
            var found = Interlocked.CompareExchange(ref _entries[index], (object) item ?? BucketHelper.Null, null);
            if (found == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            return false;
        }

        internal bool RemoveAtInternal(int index, out T previous)
        {
            previous = default(T);
            var found = Interlocked.Exchange(ref _entries[index], null);
            if (found != null)
            {
                Interlocked.Decrement(ref _count);
                if (!ReferenceEquals(found, BucketHelper.Null))
                {
                    previous = (T) found;
                }
                return true;
            }
            return false;
        }

        internal void SetInternal(int index, T item, out bool isNew)
        {
            isNew = Interlocked.Exchange(ref _entries[index], (object) item ?? BucketHelper.Null) == null;
            if (isNew)
            {
                Interlocked.Increment(ref _count);
            }
        }

        internal bool TryGetInternal(int index, out T value)
        {
            var entry = Interlocked.CompareExchange(ref _entries[index], null, null);
            if (entry == null)
            {
                value = default(T);
                return false;
            }
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

        internal bool UpdateInternal(int index, T item, T comparisonItem, out T previous, out bool isNew)
        {
            previous = default(T);
            isNew = false;
            var check = (object) comparisonItem ?? BucketHelper.Null;
            var found = Interlocked.CompareExchange(ref _entries[index], (object) item ?? BucketHelper.Null, check);
            if (found == check)
            {
                if (found == null)
                {
                    Interlocked.Increment(ref _count);
                    isNew = true;
                }
                if (!ReferenceEquals(found, BucketHelper.Null))
                {
                    previous = (T) found;
                }
                return true;
            }
            if (found == null)
            {
                isNew = true;
            }
            return false;
        }
    }
}