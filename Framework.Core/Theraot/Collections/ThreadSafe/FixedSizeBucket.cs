﻿// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Theraot.Threading;
using System.Runtime.Serialization;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    ///     Represent a thread-safe wait-free fixed size bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    [Serializable]
    public sealed class FixedSizeBucket<T> : IBucket<T>, ISerializable
    {
        private readonly object?[] _entries;
        private int _count;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeBucket{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public FixedSizeBucket(int capacity)
        {
            _count = 0;
            _entries = ArrayReservoir<object>.GetArray(capacity);
            Capacity = _entries.Length;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeBucket{T}" /> class.
        /// </summary>
        public FixedSizeBucket(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var collection = source as ICollection<T>;
            _entries = ArrayReservoir<object>.GetArray(collection?.Count ?? 64);
            Capacity = _entries.Length;
            foreach (var item in source)
            {
                if (_count == Capacity)
                {
                    var old = _entries;
                    _entries = ArrayReservoir<object>.GetArray(Capacity << 1);
                    Array.Copy(old, 0, _entries, 0, _count);
                    ArrayReservoir<object?>.DonateArray(old);
                    Capacity = _entries.Length;
                }

                _entries[_count] = item ?? BucketHelper.Null;
                _count++;
            }
        }

        private FixedSizeBucket(SerializationInfo info, StreamingContext context)
        {
            _ = context;
            if
            (
                info.GetValue("count", typeof(int)) is not int count
                || info.GetValue("contents", typeof(object?[])) is not object?[] contents
            )
            {
                throw new SerializationException();
            }

            _count = Math.Min(count, contents.Length);
            _entries = contents;
            Capacity = _entries.Length;
        }

        ~FixedSizeBucket()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (GCMonitor.FinalizingForUnload)
            {
                return;
            }

            var entries = _entries;
            if (entries != null)
            {
                ArrayReservoir<object?>.DonateArray(entries);
            }
        }

        /// <summary>
        ///     Gets the capacity.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        ///     Gets the number of items actually contained.
        /// </summary>
        public int Count => _count;

        /// <summary>
        ///     Copies the items to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="ArgumentNullException">array</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex;Non-negative number is required.</exception>
        /// <exception cref="ArgumentException">array;The array can not contain the number of elements.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (_count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }

            try
            {
                foreach (var entry in _entries)
                {
                    if (entry == null)
                    {
                        continue;
                    }

                    if (entry == BucketHelper.Null)
                    {
                        array[arrayIndex] = default!;
                    }
                    else
                    {
                        array[arrayIndex] = (T)entry;
                    }

                    arrayIndex++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentOutOfRangeException(nameof(array), exception.Message);
            }
        }

        /// <summary>
        ///     Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///     <c>true</c> if the item was new; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool Exchange(int index, T item, out T previous)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            return ExchangeInternal(index, item, out previous);
        }

        /// <summary>
        ///     Returns an <see cref="IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator{T}" /> object that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in _entries)
            {
                if (entry == null)
                {
                    continue;
                }

                if (entry == BucketHelper.Null)
                {
                    yield return default!;
                }
                else
                {
                    yield return (T)entry;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("count", _count, typeof(int));
            info.AddValue("contents", _entries, typeof(object?[]));
        }

        /// <summary>
        ///     Inserts the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        ///     The insertion can fail if the index is already used or is being written by another thread.
        ///     If the index is being written it can be understood that the insert operation happened before but the item was
        ///     overwritten or removed.
        /// </remarks>
        public bool Insert(int index, T item)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity.");
            }

            return InsertInternal(index, item);
        }

        /// <summary>
        ///     Inserts the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///     <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        ///     The insertion can fail if the index is already used or is being written by another thread.
        ///     If the index is being written it can be understood that the insert operation happened before but the item was
        ///     overwritten or removed.
        /// </remarks>
        public bool Insert(int index, T item, out T previous)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            return InsertInternal(index, item, out previous);
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            var found = Interlocked.Exchange(ref _entries[index], value: null);
            if (found == null)
            {
                return false;
            }

            Interlocked.Decrement(ref _count);
            return true;
        }

        /// <summary>
        ///     Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///     <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index, out T previous)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            return RemoveAtInternal(index, out previous);
        }

        /// <summary>
        ///     Removes the item at the specified index if it matches the specified value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="check">The predicate to decide to remove.</param>
        /// <returns>
        ///     <c>true</c> if the item was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool RemoveAt(int index, Predicate<T> check)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }

            var found = Interlocked.CompareExchange(ref _entries[index], value: null, comparand: null);
            if (found == null)
            {
                return false;
            }

            var comparisonItem = found == BucketHelper.Null ? default! : (T)found;
            if (!check(comparisonItem))
            {
                return false;
            }

            var compare = Interlocked.CompareExchange(ref _entries[index], value: null, found);
            if (found != compare)
            {
                return false;
            }

            Interlocked.Decrement(ref _count);
            return true;
        }

        /// <summary>
        ///     Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public void Set(int index, T item, out bool isNew)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            SetInternal(index, item, out isNew);
        }

        /// <summary>
        ///     Tries to retrieve the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool TryGet(int index, out T value)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity");
            }

            return TryGetInternal(index, out value);
        }

        /// <summary>
        ///     Replaces the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="check">The test to update the item.</param>
        /// <param name="isEmpty">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///     <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        ///     The insertion can fail if the index is already used or is being written by another thread.
        ///     If the index is being written it can be understood that the insert operation happened before but the item was
        ///     overwritten or removed.
        /// </remarks>
        public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
        {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be greater or equal to 0 and less than capacity.");
            }

            if (itemUpdateFactory == null)
            {
                throw new ArgumentNullException(nameof(itemUpdateFactory));
            }

            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }

            return UpdateInternal(index, itemUpdateFactory, check, out isEmpty);
        }

        public IEnumerable<T> Where(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }

            return WhereExtracted();

            IEnumerable<T> WhereExtracted()
            {
                foreach (var entry in _entries)
                {
                    if (entry == null)
                    {
                        continue;
                    }

                    var castValue = entry == BucketHelper.Null ? default! : (T)entry;
                    if (check(castValue))
                    {
                        yield return castValue;
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<int, T>> WhereIndexed(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }

            return WhereExtracted();

            IEnumerable<KeyValuePair<int, T>> WhereExtracted()
            {
                var index = 0;
                foreach (var entry in _entries)
                {
                    if (entry == null)
                    {
                        continue;
                    }

                    var castValue = entry == BucketHelper.Null ? default! : (T)entry;
                    if (!check(castValue))
                    {
                        continue;
                    }

                    yield return new KeyValuePair<int, T>(index, castValue);

                    index++;
                }
            }
        }

        internal bool ExchangeInternal(int index, T item, out T previous)
        {
            previous = default!;
            var found = Interlocked.Exchange(ref _entries[index], item ?? BucketHelper.Null);
            if (found == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }

            if (found != BucketHelper.Null)
            {
                previous = (T)found;
            }

            return false;
        }

        internal bool InsertInternal(int index, T item, out T previous)
        {
            previous = default!;
            var found = Interlocked.CompareExchange(ref _entries[index], item ?? BucketHelper.Null, comparand: null);
            if (found == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }

            if (found != BucketHelper.Null)
            {
                previous = (T)found;
            }

            return false;
        }

        internal bool InsertInternal(int index, T item)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var entries = _entries;
            if (entries == null)
            {
                return false;
            }

            var found = Interlocked.CompareExchange(ref entries[index], item ?? BucketHelper.Null, comparand: null);
            if (found != null)
            {
                return false;
            }

            Interlocked.Increment(ref _count);
            return true;
        }

        internal bool RemoveAtInternal(int index, [NotNullWhen(true)] out T previous)
        {
            previous = default!;
            var found = Interlocked.Exchange(ref _entries[index], value: null);
            if (found == null)
            {
                return false;
            }

            Interlocked.Decrement(ref _count);
            if (found != BucketHelper.Null)
            {
                previous = (T)found;
            }

            return true;
        }

        internal void SetInternal(int index, T item, out bool isNew)
        {
            isNew = Interlocked.Exchange(ref _entries[index], item ?? BucketHelper.Null) == null;
            if (isNew)
            {
                Interlocked.Increment(ref _count);
            }
        }

        internal bool TryGetInternal(int index, [NotNullWhen(true)] out T value)
        {
            var found = Interlocked.CompareExchange(ref _entries[index], value: null, comparand: null);
            if (found == null)
            {
                value = default!;
                return false;
            }

            value = found == BucketHelper.Null ? default! : (T)found;
            return true;
        }

        internal bool UpdateInternal(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
        {
            var found = Interlocked.CompareExchange(ref _entries[index], value: null, comparand: null);
            object? compare = BucketHelper.Null;
            var result = false;
            if (found != null)
            {
                var comparisonItem = found == BucketHelper.Null ? default! : (T)found;
                if (check!(comparisonItem))
                {
                    var item = itemUpdateFactory!(comparisonItem);
                    compare = Interlocked.CompareExchange(ref _entries[index], item ?? BucketHelper.Null, found);
                    result = found == compare;
                }
            }

            isEmpty = found == null || compare == null;
            return result;
        }
    }
}