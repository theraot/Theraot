#if LESSTHAN_NET40

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections.ThreadSafe;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///     Builder for read only collections.
    /// </summary>
    /// <typeparam name="T">The type of the collection element.</typeparam>
    public sealed class ReadOnlyCollectionBuilder<T> : IList<T>, IList
    {
        private const int _defaultCapacity = 4;

        private T[] _items;
        private int _version;

        /// <summary>
        ///     Constructs a <see cref="ReadOnlyCollectionBuilder{T}" />.
        /// </summary>
        public ReadOnlyCollectionBuilder()
        {
            _items = ArrayReservoir<T>.EmptyArray;
        }

        /// <summary>
        ///     Constructs a <see cref="ReadOnlyCollectionBuilder{T}" /> with a given initial capacity.
        ///     The contents are empty but builder will have reserved room for the given
        ///     number of elements before any reallocations are required.
        /// </summary>
        /// <param name="capacity">Initial capacity of the builder.</param>
        public ReadOnlyCollectionBuilder(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _items = new T[capacity];
        }

        /// <summary>
        ///     Constructs a <see cref="ReadOnlyCollectionBuilder{T}" />, copying contents of the given collection.
        /// </summary>
        /// <param name="collection">The collection whose elements to copy to the builder.</param>
        public ReadOnlyCollectionBuilder(IEnumerable<T> collection)
        {
            switch (collection)
            {
                case null:
                    throw new ArgumentNullException(nameof(collection));
                case ICollection<T> c:
                    var count = c.Count;
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    Count = count;
                    break;
                default:
                    Count = 0;
                    _items = new T[_defaultCapacity];

                    foreach (var item in collection)
                    {
                        Add(item);
                    }

                    break;
            }
        }

        /// <summary>
        ///     Gets and sets the capacity of this <see cref="ReadOnlyCollectionBuilder{T}" />.
        /// </summary>
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value == _items.Length)
                {
                    return;
                }

                if (value > 0)
                {
                    var newItems = new T[value];
                    if (Count > 0)
                    {
                        Array.Copy(_items, 0, newItems, 0, Count);
                    }

                    _items = newItems;
                }
                else
                {
                    _items = ArrayReservoir<T>.EmptyArray;
                }
            }
        }

        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                ValidateNullValue(value, nameof(value));

                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException($"The value '{value?.GetType() as object ?? "null"}' is not of type '{typeof(T)}' and cannot be used in this collection.", nameof(value));
                }
            }
        }

        int IList.Add(object value)
        {
            ValidateNullValue(value, nameof(value));
            try
            {
                Add((T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"The value '{value?.GetType() as object ?? "null"}' is not of type '{typeof(T)}' and cannot be used in this collection.", nameof(value));
            }

            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return IsCompatibleObject(value) && Contains((T)value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException(string.Empty, nameof(array));
            }

            Array.Copy(_items, 0, array, index, Count);
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            ValidateNullValue(value, nameof(value));
            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"The value '{value?.GetType() as object ?? "null"}' is not of type '{typeof(T)}' and cannot be used in this collection.", nameof(value));
            }
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        /// <summary>
        ///     Returns number of elements in the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </summary>
        public int Count { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc />
        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _items[index];
            }
            set
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _items[index] = value;
                _version++;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Adds an item to the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </summary>
        /// <param name="item">
        ///     The object to add to the
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </param>
        public void Add(T item)
        {
            if (Count == _items.Length)
            {
                EnsureCapacity(Count + 1);
            }

            _items[Count++] = item;
            _version++;
        }

        /// <summary>
        ///     Removes all items from the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(_items, 0, Count);
                Count = 0;
            }

            _version++;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" /> contains a
        ///     specific value
        /// </summary>
        /// <param name="item">
        ///     the object to locate in the
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </param>
        /// <returns>
        ///     true if item is found in the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />;
        ///     otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            if (item == null)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }

                return false;
            }

            var c = EqualityComparer<T>.Default;
            for (var i = 0; i < Count; i++)
            {
                if (c.Equals(_items[i], item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" /> to an
        ///     <see cref="T:System.Array" />,
        ///     starting at particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, Count);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the index of the first occurrence of a given value in the builder.
        /// </summary>
        /// <param name="item">An item to search for.</param>
        /// <returns>The index of the first occurrence of an item.</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, Count);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Inserts an item to the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" /> at the
        ///     specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">
        ///     The object to insert into the
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </param>
        public void Insert(int index, T item)
        {
            if (index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (Count == _items.Length)
            {
                EnsureCapacity(Count + 1);
            }

            if (index < Count)
            {
                Array.Copy(_items, index, _items, index + 1, Count - index);
            }

            _items[index] = item;
            Count++;
            _version++;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </summary>
        /// <param name="item">
        ///     The object to remove from the
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </param>
        /// <returns>
        ///     true if item was successfully removed from the
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />;
        ///     otherwise, false. This method also returns false if item is not found in the original
        ///     <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" />.
        /// </returns>
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        /// <summary>
        ///     Removes the <see cref="T:System.Runtime.CompilerServices.ReadOnlyCollectionBuilder`1" /> item at the specified
        ///     index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Count--;
            if (index < Count)
            {
                Array.Copy(_items, index + 1, _items, index, Count - index);
            }

            _items[Count] = default;
            _version++;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Reverses the order of the elements in the entire <see cref="ReadOnlyCollectionBuilder{T}" />.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, Count);
        }

        /// <summary>
        ///     Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            Array.Reverse(_items, index, count);
            _version++;
        }

        /// <summary>
        ///     Copies the elements of the <see cref="ReadOnlyCollectionBuilder{T}" /> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="ReadOnlyCollectionBuilder{T}" />.</returns>
        public T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(_items, 0, array, 0, Count);
            return array;
        }

        /// <summary>
        ///     Creates a <see cref="ReadOnlyCollection{T}" /> containing all of the elements of the
        ///     <see cref="ReadOnlyCollectionBuilder{T}" />,
        ///     avoiding copying the elements to the new array if possible. Resets the <see cref="ReadOnlyCollectionBuilder{T}" />
        ///     after the
        ///     <see cref="ReadOnlyCollection{T}" /> has been created.
        /// </summary>
        /// <returns>A new instance of <see cref="ReadOnlyCollection{T}" />.</returns>
        public ReadOnlyCollection<T> ToReadOnlyCollection()
        {
            // Can we use the stored array?
            var items = Count == _items.Length ? _items : ToArray();
            _items = ArrayReservoir<T>.EmptyArray;
            Count = 0;
            _version++;

            return ReadOnlyCollectionEx.Create(items);
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || (value == null && default(T) == null);
        }

        private static void ValidateNullValue(object value, string argument)
        {
            if (value == null && default(T) != null)
            {
                throw new ArgumentException($"The value null is not of type '{typeof(T)}' and cannot be used in this collection.", argument);
            }
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length >= min)
            {
                return;
            }

            var newCapacity = _defaultCapacity;
            if (_items.Length > 0)
            {
                newCapacity = _items.Length * 2;
            }

            if (newCapacity < min)
            {
                newCapacity = min;
            }

            Capacity = newCapacity;
        }

        private class Enumerator : IEnumerator<T>
        {
            private readonly ReadOnlyCollectionBuilder<T> _builder;
            private readonly int _version;
            private int _index;

            internal Enumerator(ReadOnlyCollectionBuilder<T> builder)
            {
                _builder = builder;
                _version = builder._version;
                _index = 0;
                Current = default;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index > _builder.Count)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return Current;
                }
            }

            public void Dispose()
            {
                // Empty
            }

            public bool MoveNext()
            {
                if (_version != _builder._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
                }

                if (_index < _builder.Count)
                {
                    Current = _builder._items[_index++];
                    return true;
                }

                _index = _builder.Count + 1;
                Current = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_version != _builder._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
                }

                _index = 0;
                Current = default;
            }
        }
    }
}

#endif