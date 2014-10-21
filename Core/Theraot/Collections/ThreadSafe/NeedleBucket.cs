#if FAT

using System;
using System.Collections.Generic;
using System.Globalization;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe wait-free fixed size bucket with lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <typeparam name="TNeedle">THe type of the needles</typeparam>
    /// <remarks>
    /// Consider wrapping this class to implement <see cref="ICollection{T}" /> or any other desired interface.
    /// </remarks>
    [Serializable]
    public sealed class NeedleBucket<T, TNeedle> : IEnumerable<T>
        where TNeedle : INeedle<T>
    {
        private readonly Bucket<TNeedle> _entries;
        private readonly Converter<int, TNeedle> _needleFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket{T}" /> class.
        /// </summary>
        /// <param name = "valueFactory">The delegate that is invoked to do the lazy initialization of the items given their index.</param>
        /// <param name="capacity">The capacity.</param>
        public NeedleBucket(Converter<int, T> valueFactory, int capacity)
        {
            if (ReferenceEquals(valueFactory, null))
            {
                throw new ArgumentNullException("valueFactory");
            }
            else
            {
                if (!NeedleHelper.CanCreateNeedle<T, TNeedle>())
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                }
                else
                {
                    // TODO: recycle wasted needles
                    _needleFactory = index => NeedleHelper.CreateNeedle<T, TNeedle>(valueFactory(index));
                    _entries = new Bucket<TNeedle>(capacity);
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
        /// Retrieve or creates a new item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public T Get(int index)
        {
            TNeedle _previous;
            var newNeedle = _needleFactory(index);
            if (_entries.Insert(index, newNeedle, out _previous))
            {
                return newNeedle.Value;
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
            foreach (TNeedle needle in _entries)
            {
                T item;
                if (needle.TryGet(out item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Retrieve or creates a new needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The needle.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public TNeedle GetNeedle(int index)
        {
            TNeedle _previous;
            var newNeedle = _needleFactory(index);
            if (_entries.Insert(index, newNeedle, out _previous))
            {
                return newNeedle;
            }
            else
            {
                //_previous should be null because null is never added
                return _previous;
            }
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TNeedle> GetNeedles()
        {
            return _entries.GetValues();
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetNeedles<TOutput>(Converter<TNeedle, TOutput> converter)
        {
            return _entries.GetValues(converter.Invoke);
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<T> GetValues()
        {
            return _entries.GetValues(input => input.Value);
        }

        /// <summary>
        /// Gets the values contained in this object.
        /// </summary>
        public IList<TOutput> GetValues<TOutput>(Converter<T, TOutput> converter)
        {
            return _entries.GetValues(input => converter.Invoke(input.Value));
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
            return _entries.Insert(index, NeedleHelper.CreateNeedle<T, TNeedle>(item));
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
            TNeedle _previous;
            if (_entries.Insert(index, NeedleHelper.CreateNeedle<T, TNeedle>(item), out _previous))
            {
                previous = default(T);
                return true;
            }
            else
            {
                //_previous should be null because null is never added
                _previous.TryGet(out previous);
                return false;
            }
        }

        /// <summary>
        /// Inserts the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="needle">The needle.</param>
        /// <returns>
        ///   <c>true</c> if the needle was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the needle was overwritten or removed.
        /// </remarks>
        public bool InsertNeedle(int index, TNeedle needle)
        {
            return _entries.Insert(index, needle);
        }

        /// <summary>
        /// Inserts the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="needle">The needle.</param>
        /// <param name="previous">The previous needle in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the needle was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the needle was overwritten or removed.
        /// </remarks>
        public bool InsertNeedle(int index, TNeedle needle, out TNeedle previous)
        {
            return _entries.Insert(index, needle, out previous);
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
            TNeedle _previous;
            if (_entries.RemoveAt(index, out _previous))
            {
                //_previous should be null because null is never added
                _previous.TryGet(out previous);
                return true;
            }
            else
            {
                previous = default(T);
                return false;
            }
        }

        /// <summary>
        /// Removes the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="previous">The previous needle in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the needle was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveNeedleAt(int index, out TNeedle previous)
        {
            return _entries.RemoveAt(index, out previous);
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
            return _entries.Set(index, NeedleHelper.CreateNeedle<T, TNeedle>(item), out isNew);
        }

        /// <summary>
        /// Sets the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="needle">The needle.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the needle was set; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool SetNeedle(int index, TNeedle needle, out bool isNew)
        {
            return _entries.Set(index, needle, out isNew);
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
            TNeedle _previous;
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

        /// <summary>
        /// Tries to retrieve the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the needle was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool TryGetNeedle(int index, out TNeedle value)
        {
            return _entries.TryGet(index, out value);
        }
    }
}

#endif