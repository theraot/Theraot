#if FAT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Core;
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
    public sealed class NeedleBucket<T, TNeedle> : IEnumerable<T>, IBucket<T>
        where TNeedle : class, IRecyclableNeedle<T>
    {
        private readonly FixedSizeBucket<TNeedle> _entries;
        private readonly Func<T, TNeedle> _needleFactory;
        private readonly Func<int, TNeedle> _needleIndexFactory;

        [NonSerialized]
        private NeedleReservoir<T, TNeedle> _reservoir;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeedleBucket{T, TNeedle}" /> class.
        /// </summary>
        /// <param name = "valueFactory">The delegate that is invoked to do the lazy initialization of the items given their index.</param>
        /// <param name="needleFactory">The delegate that is invoked to create a needle</param>
        /// <param name="capacity">The capacity.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is <c>null</c>.</exception>
        public NeedleBucket(Func<int, T> valueFactory, Func<T, TNeedle> needleFactory, int capacity)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            if (needleFactory == null)
            {
                throw new ArgumentNullException("needleFactory");
            }
            _needleFactory = needleFactory;
            _reservoir = new NeedleReservoir<T, TNeedle>(_needleFactory);
            _needleIndexFactory = index => Reservoir.GetNeedle(new ValueFuncClosure<int, T>(valueFactory, index).InvokeReturn());
            _entries = new FixedSizeBucket<TNeedle>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeedleBucket{T, TNeedle}" /> class.
        /// </summary>
        /// <param name = "valueFactory">The delegate that is invoked to do the lazy initialization of the items.</param>
        /// <param name="needleFactory">The delegate that is invoked to create a needle</param>
        /// <param name="capacity">The capacity.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is <c>null</c>.</exception>
        public NeedleBucket(Func<T> valueFactory, Func<T, TNeedle> needleFactory, int capacity)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            if (needleFactory == null)
            {
                throw new ArgumentNullException("needleFactory");
            }
            _needleFactory = needleFactory;
            _reservoir = new NeedleReservoir<T, TNeedle>(_needleFactory);
            _needleIndexFactory = index => Reservoir.GetNeedle(new ValueFuncClosure<T>(valueFactory).InvokeReturn());
            _entries = new FixedSizeBucket<TNeedle>(capacity);
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get { return _entries.Capacity; }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        public NeedleReservoir<T, TNeedle> Reservoir
        {
            get
            {
                var found = Interlocked.CompareExchange(ref _reservoir, null, null);
                if (found != null)
                {
                    return found;
                }
                var created = new NeedleReservoir<T, TNeedle>(_needleFactory);
                found = Interlocked.CompareExchange(ref _reservoir, created, null);
                return found ?? created;
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
            _entries.ToArray().CopyTo(array, arrayIndex);
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
            TNeedle found;
            if (_entries.Exchange(index, Reservoir.GetNeedle(item), out found))
            {
                previous = default(T);
                return true;
            }
            // TryGetValue is null resistant
            found.TryGetValue(out previous);
            // This is a needle that is no longer referenced, we donate it
            Reservoir.DonateNeedle(found);
            return false;
        }

        /// <summary>
        /// Sets the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="needle">The needle.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was new; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public bool ExchangeNeedle(int index, TNeedle needle, out TNeedle previous)
        {
            // This may allow null to enter, this may also give null as previous if null was allowed to enter
            // We don't donate because we return the needle
            return _entries.Exchange(index, needle, out previous);
        }

        /// <summary>
        /// Retrieve or creates a new item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public T Get(int index)
        {
            if (index < 0 || index >= _entries.Capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            // Using TryGetValue first just avoid wasting a needle
            TNeedle found;
            if (_entries.TryGetInternal(index, out found))
            {
                // Null resistant
                T previous;
                found.TryGetValue(out previous);
                return previous;
                // We don't donate because we didn't remove
            }
            var newNeedle = _needleIndexFactory(index);
            if (_entries.InsertInternal(index, newNeedle, out found))
            {
                return newNeedle.Value;
            }
            // we just failed to insert, meaning that we created a usless needle
            // donate it
            Reservoir.DonateNeedle(newNeedle);
            return found.Value;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var needle in _entries)
            {
                T item;
                if (needle.TryGetValue(out item))
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Retrieve or creates a new needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The needle.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public TNeedle GetNeedle(int index)
        {
            var newNeedle = _needleIndexFactory(index);
            TNeedle found;
            if (_entries.Insert(index, newNeedle, out found))
            {
                return newNeedle;
            }
            // we just failed to insert, meaning that we created a usless needle
            // donate it
            Reservoir.DonateNeedle(newNeedle);
            return found;
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
            // Only succeeds if there was nothing there before
            // meaning that if this succeeds it replaced nothing
            // If this fails whatever was there is still there
            var newNeedle = Reservoir.GetNeedle(item);
            if (_entries.Insert(index, newNeedle))
            {
                return true;
            }
            Reservoir.DonateNeedle(newNeedle);
            return false;
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
            // Only succeeds if there was nothing there before
            // meaning that if this succeeds it replaced nothing
            // If this fails whatever was there is still there
            var newNeedle = Reservoir.GetNeedle(item);
            TNeedle found;
            if (_entries.Insert(index, newNeedle, out found))
            {
                previous = default(T);
                return true;
            }
            Reservoir.DonateNeedle(newNeedle);
            // TryGetValue is null resistant
            found.TryGetValue(out previous);
            return false;
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
            // This may allow null to enter
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
            // This may allow null to enter, this may also give null as previous if null was allowed to enter
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
            TNeedle found;
            if (_entries.RemoveAt(index, out found))
            {
                // TryGetValue is null resistant
                found.TryGetValue(out previous);
                // Donate it
                Reservoir.DonateNeedle(found);
                return true;
            }
            previous = default(T);
            return false;
        }

        public bool RemoveAt(int index, Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            TNeedle found = null;
            Predicate<TNeedle> replacementCheck = needle =>
            {
                found = needle;
                return check(needle.Value);
            };
            if (_entries.RemoveAt(index, replacementCheck))
            {
                // Donate it
                Reservoir.DonateNeedle(found);
                return true;
            }
            return false;
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
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public void Set(int index, T item, out bool isNew)
        {
            // This may have replaced something
            _entries.Set(index, Reservoir.GetNeedle(item), out isNew);
        }

        /// <summary>
        /// Sets the needle at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="needle">The needle.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        public void SetNeedle(int index, TNeedle needle, out bool isNew)
        {
            // This may allow null to enter
            _entries.Set(index, needle, out isNew);
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
            TNeedle found;
            if (_entries.TryGet(index, out found))
            {
                // Null resistant
                found.TryGetValue(out value);
                // We don't donate because we didn't remove
                return true;
            }
            value = default(T);
            return false;
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

        public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
        {
            if (itemUpdateFactory == null)
            {
                throw new ArgumentNullException("check");
            }
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            TNeedle newNeedle = null;
            Func<TNeedle, TNeedle> replacementFactory = needle => newNeedle = Reservoir.GetNeedle(itemUpdateFactory(needle.Value));
            Predicate<TNeedle> replacementCheck = needle => check(needle.Value);
            if (_entries.Update(index, replacementFactory, replacementCheck, out isEmpty))
            {
                return true;
            }
            if (newNeedle != null)
            {
                Reservoir.DonateNeedle(newNeedle);
            }
            return false;
        }

        public IEnumerable<T> Where(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            foreach (var needle in _entries.Where(needle => check(needle.Value)))
            {
                yield return needle.Value;
            }
        }
    }
}

#endif