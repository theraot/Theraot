#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable CA2214 // Do not call overridable methods in constructors
#pragma warning disable CA2235 // Mark all non-serializable fields
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occurring in the constructor
// ReSharper disable VirtualMemberCallInConstructor

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Class:  ArrayList
**
** Purpose: Implements a dynamically sized List as an array,
**          and provides many convenience methods for treating
**          an array as an IList.
**
===========================================================*/

using System.Diagnostics;

namespace System.Collections
{
    // Implements a variable-size List that uses an array of objects to store the
    // elements. A ArrayList has a capacity, which is the allocated length
    // of the internal array. As elements are added to a ArrayList, the capacity
    // of the ArrayList is automatically increased as required by reallocating the
    // internal array.
    //
    [DebuggerTypeProxy(typeof(ArrayListDebugView))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [Serializable]
    public class ArrayList : IList, ICloneable
    {
        // Copy of Array.MaxArrayLength
        private const int _maxArrayLength = 0X7FEFFFFF;

        private const int _defaultCapacity = 4;
        private object?[] _items;
        private int _size;
        private int _version;

        // Constructs a ArrayList. The list is initially empty and has a capacity
        // of zero. Upon adding the first element to the list the capacity is
        // increased to _defaultCapacity, and then increased in multiples of two as required.
        public ArrayList()
        {
            _items = ArrayEx.Empty<object>();
        }

        // Constructs a ArrayList with a given initial capacity. The list is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required.
        //
        public ArrayList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"'{nameof(capacity)}' must be non-negative.");
            }

            _items = capacity == 0 ? ArrayEx.Empty<object>() : new object[capacity];
        }

        // Constructs a ArrayList, copying the contents of the given collection. The
        // size and capacity of the new list will both be equal to the size of the
        // given collection.
        //
        public ArrayList(ICollection c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c), "Collection cannot be null.");
            }

            var count = c.Count;
            if (count == 0)
            {
                _items = ArrayEx.Empty<object>();
            }
            else
            {
                _items = new object[count];
                AddRange(c);
            }
        }

        // Note: this constructor is a bogus constructor that does nothing
        // and is for use only with SyncArrayList.
        private ArrayList(bool trash)
        {
            _ = trash;
            _items = null!;
        }

        // Gets and sets the capacity of this list.  The capacity is the size of
        // the internal array used to hold items.  When set, the internal
        // array of the list is reallocated to the given capacity.
        //
        public virtual int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");
                }

                // We don't want to update the version number when we change the capacity.
                // Some existing applications have dependency on this.
                if (value == _items.Length)
                {
                    return;
                }

                if (value > 0)
                {
                    var newItems = new object[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, 0, newItems, 0, _size);
                    }

                    _items = newItems;
                }
                else
                {
                    _items = new object[_defaultCapacity];
                }
            }
        }

        // Read-only property describing how many elements are in the List.
        public virtual int Count => _size;

        public virtual bool IsFixedSize => false;

        // Is this ArrayList read-only?
        public virtual bool IsReadOnly => false;

        // Is this ArrayList synchronized (thread-safe)?
        public virtual bool IsSynchronized => false;

        // Synchronization root for this object.
        public virtual object SyncRoot => this;

        // Sets or Gets the element at the given index.
        //
        public virtual object? this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                _items[index] = value;
                _version++;
            }
        }

        // Creates a ArrayList wrapper for a particular IList.  This does not
        // copy the contents of the IList, but only wraps the IList.  So any
        // changes to the underlying list will affect the ArrayList.  This would
        // be useful if you want to Reverse a subrange of an IList, or want to
        // use a generic BinarySearch or Sort method without implementing one yourself.
        // However, since these methods are generic, the performance may not be
        // nearly as good for some operations as they would be on the IList itself.
        //
        public static ArrayList Adapter(IList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new ListWrapper(list);
        }

        // Returns a list wrapper that is fixed at the current size.  Operations
        // that add or remove items will fail, however, replacing items is allowed.
        //
        public static IList FixedSize(IList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new FixedSizeList(list);
        }

        // Returns a list wrapper that is fixed at the current size.  Operations
        // that add or remove items will fail, however, replacing items is allowed.
        //
        public static ArrayList FixedSize(ArrayList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new FixedSizeArrayList(list);
        }

        // Returns a read-only IList wrapper for the given IList.
        //
        public static IList ReadOnly(IList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new ReadOnlyList(list);
        }

        // Returns a read-only ArrayList wrapper for the given ArrayList.
        //
        public static ArrayList ReadOnly(ArrayList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new ReadOnlyArrayList(list);
        }

        // Returns an IList that contains count copies of value.
        //
        public static ArrayList Repeat(object? value, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            var list = new ArrayList(count > _defaultCapacity ? count : _defaultCapacity);
            for (var i = 0; i < count; i++)
            {
                list.Add(value);
            }

            return list;
        }

        // Returns a thread-safe wrapper around an IList.
        //
        public static IList Synchronized(IList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new SyncIList(list);
        }

        // Returns a thread-safe wrapper around a ArrayList.
        //
        public static ArrayList Synchronized(ArrayList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new SyncArrayList(list);
        }

        // Adds the given object to the end of this list. The size of the list is
        // increased by one. If required, the capacity of the list is doubled
        // before adding the new element.
        //
        public virtual int Add(object? value)
        {
            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }

            _items[_size] = value;
            _version++;
            return _size++;
        }

        // Adds the elements of the given collection to the end of this list. If
        // required, the capacity of the list is increased to twice the previous
        // capacity or the new size, whichever is larger.
        //
        public virtual void AddRange(ICollection c)
        {
            InsertRange(_size, c);
        }

        // Searches a section of the list for a given element using a binary search
        // algorithm. Elements of the list are compared to the search value using
        // the given IComparer interface. If comparer is null, elements of
        // the list are compared to the search value using the IComparable
        // interface, which in that case must be implemented by all elements of the
        // list and the given search value. This method assumes that the given
        // section of the list is already sorted; if this is not the case, the
        // result will be incorrect.
        //
        // The method returns the index of the given value in the list. If the
        // list does not contain the given value, the method returns a negative
        // integer. The bitwise complement operator (~) can be applied to a
        // negative result to produce the index of the first element (if any) that
        // is larger than the given search value. This is also the index at which
        // the search value should be inserted into the list in order for the list
        // to remain sorted.
        //
        // The method uses the Array.BinarySearch method to perform the
        // search.
        //
        public virtual int BinarySearch(int index, int count, object? value, IComparer? comparer)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            return Array.BinarySearch(_items, index, count, value, comparer);
        }

        public virtual int BinarySearch(object? value)
        {
            return BinarySearch(0, Count, value, null);
        }

        public virtual int BinarySearch(object? value, IComparer comparer)
        {
            return BinarySearch(0, Count, value, comparer);
        }

        // Clears the contents of ArrayList.
        public virtual void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
                _size = 0;
            }

            _version++;
        }

        // Clones this ArrayList, doing a shallow copy.  (A copy is made of all
        // Object references in the ArrayList, but the Objects pointed to
        // are not cloned).
        public virtual object Clone()
        {
            var la = new ArrayList(_size)
            {
                _size = _size,
                _version = _version
            };
            Array.Copy(_items, 0, la._items, 0, _size);
            return la;
        }

        // Contains returns true if the specified element is in the ArrayList.
        // It does a linear, O(n) search.  Equality is determined by calling
        // item.Equals().
        //
        public virtual bool Contains(object? value)
        {
            if (value == null)
            {
                for (var i = 0; i < _size; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }

                return false;
            }

            for (var i = 0; i < _size; i++)
            {
                if (_items[i]?.Equals(value) == true)
                {
                    return true;
                }
            }

            return false;
        }

        // Copies this ArrayList into array, which must be of a
        // compatible array type.
        //
        public virtual void CopyTo(Array array, int arrayIndex)
        {
            if (array != null && array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
            }

            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        // Copies this ArrayList into array, which must be of a
        // compatible array type.
        //
        public virtual void CopyTo(Array array)
        {
            CopyTo(array, 0);
        }

        // Copies a section of this list to the given array at the given index.
        //
        // The method uses the Array.Copy method to copy the elements.
        //
        public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
        {
            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            if (array != null && array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
            }

            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, index, array, arrayIndex, count);
        }

        // Returns an enumerator for this list with the given
        // permission for removal of elements. If modifications made to the list
        // while an enumeration is in progress, the MoveNext and
        // GetObject methods of the enumerator will throw an exception.
        //
        public virtual IEnumerator GetEnumerator()
        {
            return new ArrayListEnumeratorSimple(this);
        }

        // Returns an enumerator for a section of this list with the given
        // permission for removal of elements. If modifications made to the list
        // while an enumeration is in progress, the MoveNext and
        // GetObject methods of the enumerator will throw an exception.
        //
        public virtual IEnumerator GetEnumerator(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            return new ArrayListEnumerator(this, index, count);
        }

        public virtual ArrayList GetRange(int index, int count)
        {
            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
            }

            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            return new Range(this, index, count);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards from beginning to end.
        // The elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.IndexOf method to perform the
        // search.
        //
        public virtual int IndexOf(object? value)
        {
            return Array.IndexOf((Array)_items, value, 0, _size);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards, starting at index
        // startIndex and ending at count number of elements. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.IndexOf method to perform the
        // search.
        //
        public virtual int IndexOf(object? value, int startIndex)
        {
            if (startIndex > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            return Array.IndexOf((Array)_items, value, startIndex, _size - startIndex);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards, starting at index
        // startIndex and up to count number of elements. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.IndexOf method to perform the
        // search.
        //
        public virtual int IndexOf(object? value, int startIndex, int count)
        {
            if (startIndex > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            if (count < 0 || startIndex > _size - count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
            }

            return Array.IndexOf((Array)_items, value, startIndex, count);
        }

        // Inserts an element into this list at a given index. The size of the list
        // is increased by one. If required, the capacity of the list is doubled
        // before inserting the new element.
        //
        public virtual void Insert(int index, object? value)
        {
            // Note that insertions at the end are legal.
            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Insertion index was out of range. Must be non-negative and less than or equal to size.");
            }

            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }

            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }

            _items[index] = value;
            _size++;
            _version++;
        }

        // Inserts the elements of the given collection at a given index. If
        // required, the capacity of the list is increased to twice the previous
        // capacity or the new size, whichever is larger.  Ranges may be added
        // to the end of the list by setting index to the ArrayList's size.
        //
        public virtual void InsertRange(int index, ICollection c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c), "Collection cannot be null.");
            }

            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            var count = c.Count;
            if (count <= 0)
            {
                return;
            }

            EnsureCapacity(_size + count);
            // shift existing items
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + count, _size - index);
            }

            var itemsToInsert = new object[count];
            c.CopyTo(itemsToInsert, 0);
            itemsToInsert.CopyTo(_items, index);
            _size += count;
            _version++;
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at the end
        // and ending at the first element in the list. The elements of the list
        // are compared to the given value using the Object.Equals method.
        //
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        //
        public virtual int LastIndexOf(object? value)
        {
            return LastIndexOf(value, _size - 1, _size);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at index
        // startIndex and ending at the first element in the list. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        //
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        //
        public virtual int LastIndexOf(object? value, int startIndex)
        {
            if (startIndex >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            return LastIndexOf(value, startIndex, startIndex + 1);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at index
        // startIndex and up to count elements. The elements of
        // the list are compared to the given value using the Object.Equals
        // method.
        //
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        //
        public virtual int LastIndexOf(object? value, int startIndex, int count)
        {
            if (Count != 0 && (startIndex < 0 || count < 0))
            {
                throw new ArgumentOutOfRangeException(startIndex < 0 ? nameof(startIndex) : nameof(count), "Non-negative number required.");
            }

            if (_size == 0) // Special case for an empty list
            {
                return -1;
            }

            if (startIndex >= _size || count > startIndex + 1)
            {
                throw new ArgumentOutOfRangeException(startIndex >= _size ? nameof(startIndex) : nameof(count), "Must be less than or equal to the size of the collection.");
            }

            return Array.LastIndexOf((Array)_items, value, startIndex, count);
        }

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        //
        public virtual void Remove(object? value)
        {
            var index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        //
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }

            _items[_size] = null;
            _version++;
        }

        // Removes a range of elements from this list.
        //
        public virtual void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            if (count <= 0)
            {
                return;
            }

            var i = _size;
            _size -= count;
            if (index < _size)
            {
                Array.Copy(_items, index + count, _items, index, _size - index);
            }

            while (i > _size)
            {
                _items[--i] = null;
            }

            _version++;
        }

        // Reverses the elements in this list.
        public virtual void Reverse()
        {
            Reverse(0, Count);
        }

        // Reverses the elements in a range of this list. Following a call to this
        // method, an element in the range given by index and count
        // which was previously located at index i will now be located at
        // index index + (index + count - i - 1).
        //
        // This method uses the Array.Reverse method to reverse the
        // elements.
        //
        public virtual void Reverse(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            Array.Reverse(_items, index, count);
            _version++;
        }

        // Sets the elements starting at the given index to the elements of the
        // given collection.
        //
        public virtual void SetRange(int index, ICollection c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c), "Collection cannot be null.");
            }

            var count = c.Count;
            if (index < 0 || index > _size - count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            if (count <= 0)
            {
                return;
            }

            c.CopyTo(_items, index);
            _version++;
        }

        // Sorts the elements in this list.  Uses the default comparer and
        // Array.Sort.
        public virtual void Sort()
        {
            Sort(0, Count, Comparer.Default);
        }

        // Sorts the elements in this list.  Uses Array.Sort with the
        // provided comparer.
        public virtual void Sort(IComparer comparer)
        {
            Sort(0, Count, comparer);
        }

        // Sorts the elements in a section of this list. The sort compares the
        // elements to each other using the given IComparer interface. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented by all
        // elements of the list.
        //
        // This method uses the Array.Sort method to sort the elements.
        //
        public virtual void Sort(int index, int count, IComparer comparer)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            if (_size - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            Array.Sort(_items, index, count, comparer);
            _version++;
        }

        // ToArray returns a new Object array containing the contents of the ArrayList.
        // This requires copying the ArrayList, which is an O(n) operation.
        public virtual object[] ToArray()
        {
            if (_size == 0)
            {
                return ArrayEx.Empty<object>();
            }

            var array = new object[_size];
            Array.Copy(_items, 0, array, 0, _size);
            return array;
        }

        // ToArray returns a new array of a particular type containing the contents
        // of the ArrayList.  This requires copying the ArrayList and potentially
        // downcasting all elements.  This copy may fail and is an O(n) operation.
        // Internally, this implementation calls Array.Copy.
        //
        public virtual Array ToArray(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var array = Array.CreateInstance(type, _size);
            Array.Copy(_items, 0, array, 0, _size);
            return array;
        }

        // Sets the capacity of this list to the size of the list. This method can
        // be used to minimize a list's memory overhead once it is known that no
        // new elements will be added to the list.
        public virtual void TrimToSize()
        {
            Capacity = _size;
        }

        // Ensures that the capacity of this list is at least the given minimum
        // value. If the current capacity of the list is less than min, the
        // capacity is increased to twice the current capacity or to min,
        // whichever is larger.
        private void EnsureCapacity(int min)
        {
            if (_items.Length >= min)
            {
                return;
            }

            var newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > _maxArrayLength)
            {
                newCapacity = _maxArrayLength;
            }

            if (newCapacity < min)
            {
                newCapacity = min;
            }

            Capacity = newCapacity;
        }

        internal class ArrayListDebugView
        {
            private readonly ArrayList _arrayList;

            public ArrayListDebugView(ArrayList arrayList)
            {
                _arrayList = arrayList ?? throw new ArgumentNullException(nameof(arrayList));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object[] Items => _arrayList.ToArray();
        }

        // Implements an enumerator for a ArrayList. The enumerator uses the
        // internal version number of the list to ensure that no modifications are
        // made to the list while an enumeration is in progress.
        private sealed class ArrayListEnumerator : IEnumerator, ICloneable
        {
            private readonly int _endIndex; // Where to stop.
            private readonly ArrayList _list;
            private readonly int _startIndex; // Save this for Reset.
            private readonly int _version;
            private object? _currentElement;
            private int _index;

            internal ArrayListEnumerator(ArrayList list, int index, int count)
            {
                _list = list;
                _startIndex = index;
                _index = index - 1;
                _endIndex = _index + count; // last valid index
                _version = list._version;
                _currentElement = null;
            }

            public object? Current
            {
                get
                {
                    if (_index < _startIndex)
                    {
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    }

                    if (_index > _endIndex)
                    {
                        throw new InvalidOperationException("Enumeration already finished.");
                    }

                    return _currentElement;
                }
            }

            public object Clone() => MemberwiseClone();

            public bool MoveNext()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                if (_index < _endIndex)
                {
                    _currentElement = _list[++_index];
                    return true;
                }

                _index = _endIndex + 1;

                return false;
            }

            public void Reset()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                _index = _startIndex - 1;
            }
        }

        private sealed class ArrayListEnumeratorSimple : IEnumerator, ICloneable
        {
            // this object is used to indicate enumeration has not started or has terminated
            private static readonly object _dummyObject = new object();

            private readonly bool _isArrayList;
            private readonly ArrayList _list;
            private readonly int _version;
            private object? _currentElement;
            private int _index;

            internal ArrayListEnumeratorSimple(ArrayList list)
            {
                _list = list;
                _index = -1;
                _version = list._version;
                _isArrayList = list.GetType() == typeof(ArrayList);
                _currentElement = _dummyObject;
            }

            public object? Current
            {
                get
                {
                    var temp = _currentElement;
                    if (_dummyObject != temp)
                    {
                        return temp;
                    }

                    // check if enumeration has not started or has terminated
                    if (_index == -1)
                    {
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    }

                    throw new InvalidOperationException("Enumeration already finished.");
                }
            }

            public object Clone() => MemberwiseClone();

            public bool MoveNext()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                if (_isArrayList)
                {
                    // avoid calling virtual methods if we are operating on ArrayList to improve performance
                    if (_index < _list._size - 1)
                    {
                        _currentElement = _list._items[++_index];
                        return true;
                    }

                    _currentElement = _dummyObject;
                    _index = _list._size;
                    return false;
                }

                if (_index < _list.Count - 1)
                {
                    _currentElement = _list[++_index];
                    return true;
                }

                _index = _list.Count;
                _currentElement = _dummyObject;
                return false;
            }

            public void Reset()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                _currentElement = _dummyObject;
                _index = -1;
            }
        }

        private sealed class FixedSizeArrayList : ArrayList
        {
            private ArrayList _list;

            internal FixedSizeArrayList(ArrayList l)
            {
                _list = l;
                _version = _list._version;
            }

            public override int Capacity
            {
                get => _list.Capacity;
                set => throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override int Count => _list.Count;

            public override bool IsFixedSize => true;

            public override bool IsReadOnly => _list.IsReadOnly;

            public override bool IsSynchronized => _list.IsSynchronized;

            public override object SyncRoot => _list.SyncRoot;

            public override object? this[int index]
            {
                get => _list[index];
                set
                {
                    _list[index] = value;
                    _version = _list._version;
                }
            }

            public override int Add(object? value)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override void AddRange(ICollection c)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override int BinarySearch(int index, int count, object? value, IComparer? comparer)
            {
                return _list.BinarySearch(index, count, value, comparer);
            }

            public override void Clear()
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override object Clone()
            {
                return new FixedSizeArrayList(_list)
                {
                    _list = (ArrayList)_list.Clone()
                };
            }

            public override bool Contains(object? value)
            {
                return _list.Contains(value);
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public override void CopyTo(int index, Array array, int arrayIndex, int count)
            {
                _list.CopyTo(index, array, arrayIndex, count);
            }

            public override IEnumerator GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public override IEnumerator GetEnumerator(int index, int count)
            {
                return _list.GetEnumerator(index, count);
            }

            public override ArrayList GetRange(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                return new Range(this, index, count);
            }

            public override int IndexOf(object? value)
            {
                return _list.IndexOf(value);
            }

            public override int IndexOf(object? value, int startIndex)
            {
                return _list.IndexOf(value, startIndex);
            }

            public override int IndexOf(object? value, int startIndex, int count)
            {
                return _list.IndexOf(value, startIndex, count);
            }

            public override void Insert(int index, object? value)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override void InsertRange(int index, ICollection c)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override int LastIndexOf(object? value)
            {
                return _list.LastIndexOf(value);
            }

            public override int LastIndexOf(object? value, int startIndex)
            {
                return _list.LastIndexOf(value, startIndex);
            }

            public override int LastIndexOf(object? value, int startIndex, int count)
            {
                return _list.LastIndexOf(value, startIndex, count);
            }

            public override void Remove(object? value)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override void RemoveRange(int index, int count)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public override void Reverse(int index, int count)
            {
                _list.Reverse(index, count);
                _version = _list._version;
            }

            public override void SetRange(int index, ICollection c)
            {
                _list.SetRange(index, c);
                _version = _list._version;
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                _list.Sort(index, count, comparer);
                _version = _list._version;
            }

            public override object[] ToArray()
            {
                return _list.ToArray();
            }

            public override Array ToArray(Type type)
            {
                return _list.ToArray(type);
            }

            public override void TrimToSize()
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }
        }

        private sealed class FixedSizeList : IList
        {
            private readonly IList _list;

            internal FixedSizeList(IList l)
            {
                _list = l;
            }

            public int Count => _list.Count;

            public bool IsFixedSize => true;

            public bool IsReadOnly => _list.IsReadOnly;

            public bool IsSynchronized => _list.IsSynchronized;

            public object SyncRoot => _list.SyncRoot;

            public object this[int index]
            {
                get => _list[index];
                set => _list[index] = value;
            }

            public int Add(object value)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public bool Contains(object value)
            {
                return _list.Contains(value);
            }

            public void CopyTo(Array array, int index)
            {
                _list.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public int IndexOf(object value)
            {
                return _list.IndexOf(value);
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public void Remove(object value)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("Collection was of a fixed size.");
            }
        }

        // This class wraps an IList, exposing it as a ArrayList
        // Note this requires reimplementing half of ArrayList...
        private sealed class ListWrapper : ArrayList
        {
            private readonly IList _list;

            internal ListWrapper(IList list)
            {
                _list = list;
                _version = 0; // list doesn't not contain a version number
            }

            public override int Capacity
            {
                get => _list.Count;
                set
                {
                    if (value < Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");
                    }
                }
            }

            public override int Count => _list.Count;

            public override bool IsFixedSize => _list.IsFixedSize;

            public override bool IsReadOnly => _list.IsReadOnly;

            public override bool IsSynchronized => _list.IsSynchronized;

            public override object SyncRoot => _list.SyncRoot;

            public override object? this[int index]
            {
                get => _list[index];
                set
                {
                    _list[index] = value;
                    _version++;
                }
            }

            public override int Add(object? value)
            {
                var i = _list.Add(value);
                _version++;
                return i;
            }

            public override void AddRange(ICollection c)
            {
                InsertRange(Count, c);
            }

            // Other overloads with automatically work
            public override int BinarySearch(int index, int count, object? value, IComparer? comparer)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                if (comparer == null)
                {
                    comparer = Comparer.Default;
                }

                var lo = index;
                var hi = index + count - 1;
                while (lo <= hi)
                {
                    var mid = (lo + hi) / 2;
                    var r = comparer.Compare(value, _list[mid]);
                    if (r == 0)
                    {
                        return mid;
                    }

                    if (r < 0)
                    {
                        hi = mid - 1;
                    }
                    else
                    {
                        lo = mid + 1;
                    }
                }

                // return bitwise complement of the first element greater than value.
                // Since hi is less than lo now, ~lo is the correct item.
                return ~lo;
            }

            public override void Clear()
            {
                // If _list is an array, it will support Clear method.
                // We shouldn't allow clear operation on a FixedSized ArrayList
                if (_list.IsFixedSize)
                {
                    throw new NotSupportedException("Collection was of a fixed size.");
                }

                _list.Clear();
                _version++;
            }

            public override object Clone()
            {
                // This does not do a shallow copy of _list into a ArrayList!
                // This clones the IListWrapper, creating another wrapper class!
                return new ListWrapper(_list);
            }

            public override bool Contains(object? value)
            {
                return _list.Contains(value);
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public override void CopyTo(int index, Array array, int arrayIndex, int count)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (index < 0 || arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(arrayIndex), "Non-negative number required.");
                }

                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
                }

                if (array.Length - arrayIndex < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                if (_list.Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                for (var i = index; i < index + count; i++)
                {
                    array.SetValue(_list[i], arrayIndex++);
                }
            }

            public override IEnumerator GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public override IEnumerator GetEnumerator(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_list.Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                return new ListWrapperEnumWrapper(this, index, count);
            }

            public override ArrayList GetRange(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_list.Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                return new Range(this, index, count);
            }

            public override int IndexOf(object? value)
            {
                return _list.IndexOf(value);
            }

            public override int IndexOf(object? value, int startIndex)
            {
                return IndexOf(value, startIndex, _list.Count - startIndex);
            }

            public override int IndexOf(object? value, int startIndex, int count)
            {
                if (startIndex < 0 || startIndex > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (count < 0 || startIndex > Count - count)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
                }

                var endIndex = startIndex + count;
                if (value == null)
                {
                    for (var i = startIndex; i < endIndex; i++)
                    {
                        if (_list[i] == null)
                        {
                            return i;
                        }
                    }

                    return -1;
                }

                for (var i = startIndex; i < endIndex; i++)
                {
                    if (_list[i]?.Equals(value) == true)
                    {
                        return i;
                    }
                }

                return -1;
            }

            public override void Insert(int index, object? value)
            {
                _list.Insert(index, value);
                _version++;
            }

            public override void InsertRange(int index, ICollection c)
            {
                if (c == null)
                {
                    throw new ArgumentNullException(nameof(c), "Collection cannot be null.");
                }

                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (c.Count <= 0)
                {
                    return;
                }

                if (_list is ArrayList al)
                {
                    // We need to special case ArrayList.
                    // When c is a range of _list, we need to handle this in a special way.
                    // See ArrayList.InsertRange for details.
                    al.InsertRange(index, c);
                }
                else
                {
                    var en = c.GetEnumerator();
                    while (en.MoveNext())
                    {
                        _list.Insert(index++, en.Current);
                    }
                }

                _version++;
            }

            public override int LastIndexOf(object? value)
            {
                return LastIndexOf(value, _list.Count - 1, _list.Count);
            }

            public override int LastIndexOf(object? value, int startIndex)
            {
                return LastIndexOf(value, startIndex, startIndex + 1);
            }

            public override int LastIndexOf(object? value, int startIndex, int count)
            {
                if (_list.Count == 0)
                {
                    return -1;
                }

                if (startIndex < 0 || startIndex >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (count < 0 || count > startIndex + 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
                }

                var endIndex = startIndex - count + 1;
                if (value == null)
                {
                    for (var i = startIndex; i >= endIndex; i--)
                    {
                        if (_list[i] == null)
                        {
                            return i;
                        }
                    }

                    return -1;
                }

                for (var i = startIndex; i >= endIndex; i--)
                {
                    if (_list[i]?.Equals(value) == true)
                    {
                        return i;
                    }
                }

                return -1;
            }

            public override void Remove(object? value)
            {
                var index = IndexOf(value);
                if (index >= 0)
                {
                    RemoveAt(index);
                }
            }

            public override void RemoveAt(int index)
            {
                _list.RemoveAt(index);
                _version++;
            }

            public override void RemoveRange(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_list.Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                if (count > 0) // be consistent with ArrayList
                {
                    _version++;
                }

                while (count > 0)
                {
                    _list.RemoveAt(index);
                    count--;
                }
            }

            public override void Reverse(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_list.Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                var i = index;
                var j = index + count - 1;
                while (i < j)
                {
                    var tmp = _list[i];
                    _list[i++] = _list[j];
                    _list[j--] = tmp;
                }

                _version++;
            }

            public override void SetRange(int index, ICollection c)
            {
                if (c == null)
                {
                    throw new ArgumentNullException(nameof(c), "Collection cannot be null.");
                }

                if (index < 0 || index > _list.Count - c.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (c.Count <= 0)
                {
                    return;
                }

                var en = c.GetEnumerator();
                while (en.MoveNext())
                {
                    _list[index++] = en.Current;
                }

                _version++;
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_list.Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                var array = new object[count];
                CopyTo(index, array, 0, count);
                Array.Sort(array, 0, count, comparer);
                for (var i = 0; i < count; i++)
                {
                    _list[i + index] = array[i];
                }

                _version++;
            }

            public override object[] ToArray()
            {
                if (Count == 0)
                {
                    return ArrayEx.Empty<object>();
                }

                var array = new object[Count];
                _list.CopyTo(array, 0);
                return array;
            }

            public override Array ToArray(Type type)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                var array = Array.CreateInstance(type, _list.Count);
                _list.CopyTo(array, 0);
                return array;
            }

            public override void TrimToSize()
            {
                // Can't really do much here...
            }

            // This is the enumerator for an IList that's been wrapped in another
            // class that implements all of ArrayList's methods.
            private sealed class ListWrapperEnumWrapper : IEnumerator, ICloneable
            {
                private readonly IEnumerator _enumerator;
                private readonly int _initialCount;

                // for reset
                private readonly int _initialStartIndex;

                private bool _firstCall; // firstCall to MoveNext

                // for reset
                private int _remaining;

                internal ListWrapperEnumWrapper(ListWrapper listWrapper, int startIndex, int count)
                {
                    _enumerator = listWrapper.GetEnumerator();
                    _initialStartIndex = startIndex;
                    _initialCount = count;
                    while (startIndex-- > 0 && _enumerator.MoveNext())
                    {
                        // Empty
                    }

                    _remaining = count;
                    _firstCall = true;
                }

                private ListWrapperEnumWrapper(ListWrapperEnumWrapper prototype)
                {
                    _enumerator = (IEnumerator)((ICloneable)prototype._enumerator).Clone();
                    _initialStartIndex = prototype._initialStartIndex;
                    _initialCount = prototype._initialCount;
                    _remaining = prototype._remaining;
                    _firstCall = prototype._firstCall;
                }

                public object Current
                {
                    get
                    {
                        if (_firstCall)
                        {
                            throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                        }

                        if (_remaining < 0)
                        {
                            throw new InvalidOperationException("Enumeration already finished.");
                        }

                        return _enumerator.Current;
                    }
                }

                public object Clone()
                {
                    return new ListWrapperEnumWrapper(this);
                }

                public bool MoveNext()
                {
                    if (_firstCall)
                    {
                        _firstCall = false;
                        return _remaining-- > 0 && _enumerator.MoveNext();
                    }

                    if (_remaining < 0)
                    {
                        return false;
                    }

                    var r = _enumerator.MoveNext();
                    return r && _remaining-- > 0;
                }

                public void Reset()
                {
                    _enumerator.Reset();
                    var startIndex = _initialStartIndex;
                    while (startIndex-- > 0 && _enumerator.MoveNext())
                    {
                        // Empty
                    }

                    _remaining = _initialCount;
                    _firstCall = true;
                }
            }
        }

        // Implementation of a generic list subrange. An instance of this class
        // is returned by the default implementation of List.GetRange.
        private sealed class Range : ArrayList
        {
            private readonly int _baseIndex;
            private ArrayList _baseList;
            private int _baseSize;
            private int _baseVersion;

            internal Range(ArrayList list, int index, int count)
                : base(false)
            {
                _baseList = list;
                _baseIndex = index;
                _baseSize = count;
                _baseVersion = list._version;
                // we also need to update _version field to make Range of Range work
                _version = list._version;
            }

            public override int Capacity
            {
                get => _baseList.Capacity;

                set
                {
                    if (value < Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");
                    }
                }
            }

            public override int Count
            {
                get
                {
                    InternalUpdateRange();
                    return _baseSize;
                }
            }

            public override bool IsFixedSize => _baseList.IsFixedSize;

            public override bool IsReadOnly => _baseList.IsReadOnly;

            public override bool IsSynchronized => _baseList.IsSynchronized;

            public override object SyncRoot => _baseList.SyncRoot;

            public override object? this[int index]
            {
                get
                {
                    InternalUpdateRange();
                    if (index < 0 || index >= _baseSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                    }

                    return _baseList[_baseIndex + index];
                }
                set
                {
                    InternalUpdateRange();
                    if (index < 0 || index >= _baseSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                    }

                    _baseList[_baseIndex + index] = value;
                    InternalUpdateVersion();
                }
            }

            public override int Add(object? value)
            {
                InternalUpdateRange();
                _baseList.Insert(_baseIndex + _baseSize, value);
                InternalUpdateVersion();
                return _baseSize++;
            }

            public override void AddRange(ICollection c)
            {
                if (c == null)
                {
                    throw new ArgumentNullException(nameof(c));
                }

                InternalUpdateRange();
                var count = c.Count;
                if (count <= 0)
                {
                    return;
                }

                _baseList.InsertRange(_baseIndex + _baseSize, c);
                InternalUpdateVersion();
                _baseSize += count;
            }

            public override int BinarySearch(int index, int count, object? value, IComparer? comparer)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();

                var i = _baseList.BinarySearch(_baseIndex + index, count, value, comparer);
                if (i >= 0)
                {
                    return i - _baseIndex;
                }

                return i + _baseIndex;
            }

            public override void Clear()
            {
                InternalUpdateRange();
                if (_baseSize == 0)
                {
                    return;
                }

                _baseList.RemoveRange(_baseIndex, _baseSize);
                InternalUpdateVersion();
                _baseSize = 0;
            }

            public override object Clone()
            {
                InternalUpdateRange();
                return new Range(_baseList, _baseIndex, _baseSize)
                {
                    _baseList = (ArrayList)_baseList.Clone()
                };
            }

            public override bool Contains(object? value)
            {
                InternalUpdateRange();
                if (value == null)
                {
                    for (var i = 0; i < _baseSize; i++)
                    {
                        if (_baseList[_baseIndex + i] == null)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                for (var i = 0; i < _baseSize; i++)
                {
                    if (_baseList[_baseIndex + i]?.Equals(value) == true)
                    {
                        return true;
                    }
                }

                return false;
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number required.");
                }

                if (array.Length - arrayIndex < _baseSize)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                _baseList.CopyTo(_baseIndex, array, arrayIndex, _baseSize);
            }

            public override void CopyTo(int index, Array array, int arrayIndex, int count)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (array.Length - arrayIndex < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                _baseList.CopyTo(_baseIndex + index, array, arrayIndex, count);
            }

            public override IEnumerator GetEnumerator()
            {
                return GetEnumerator(0, _baseSize);
            }

            public override IEnumerator GetEnumerator(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                return _baseList.GetEnumerator(_baseIndex + index, count);
            }

            public override ArrayList GetRange(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                return new Range(this, index, count);
            }

            public override int IndexOf(object? value)
            {
                InternalUpdateRange();
                var i = _baseList.IndexOf(value, _baseIndex, _baseSize);
                if (i >= 0)
                {
                    return i - _baseIndex;
                }

                return -1;
            }

            public override int IndexOf(object? value, int startIndex)
            {
                if (startIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative number required.");
                }

                if (startIndex > _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                InternalUpdateRange();
                var i = _baseList.IndexOf(value, _baseIndex + startIndex, _baseSize - startIndex);
                if (i >= 0)
                {
                    return i - _baseIndex;
                }

                return -1;
            }

            public override int IndexOf(object? value, int startIndex, int count)
            {
                if (startIndex < 0 || startIndex > _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (count < 0 || startIndex > _baseSize - count)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
                }

                InternalUpdateRange();
                var i = _baseList.IndexOf(value, _baseIndex + startIndex, count);
                if (i >= 0)
                {
                    return i - _baseIndex;
                }

                return -1;
            }

            public override void Insert(int index, object? value)
            {
                if (index < 0 || index > _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                InternalUpdateRange();
                _baseList.Insert(_baseIndex + index, value);
                InternalUpdateVersion();
                _baseSize++;
            }

            public override void InsertRange(int index, ICollection c)
            {
                if (index < 0 || index > _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (c == null)
                {
                    throw new ArgumentNullException(nameof(c));
                }

                InternalUpdateRange();
                var count = c.Count;
                if (count <= 0)
                {
                    return;
                }

                _baseList.InsertRange(_baseIndex + index, c);
                _baseSize += count;
                InternalUpdateVersion();
            }

            public override int LastIndexOf(object? value)
            {
                InternalUpdateRange();
                var i = _baseList.LastIndexOf(value, _baseIndex + _baseSize - 1, _baseSize);
                if (i >= 0)
                {
                    return i - _baseIndex;
                }

                return -1;
            }

            public override int LastIndexOf(object? value, int startIndex)
            {
                return LastIndexOf(value, startIndex, startIndex + 1);
            }

            public override int LastIndexOf(object? value, int startIndex, int count)
            {
                InternalUpdateRange();
                if (_baseSize == 0)
                {
                    return -1;
                }

                if (startIndex >= _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                if (startIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative number required.");
                }

                var i = _baseList.LastIndexOf(value, _baseIndex + startIndex, count);
                if (i >= 0)
                {
                    return i - _baseIndex;
                }

                return -1;
            }

            // Don't need to override Remove

            public override void RemoveAt(int index)
            {
                if (index < 0 || index >= _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                InternalUpdateRange();
                _baseList.RemoveAt(_baseIndex + index);
                InternalUpdateVersion();
                _baseSize--;
            }

            public override void RemoveRange(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                // No need to call _bastList.RemoveRange if count is 0.
                // In addition, _baseList won't change the version number if count is 0.
                if (count <= 0)
                {
                    return;
                }

                _baseList.RemoveRange(_baseIndex + index, count);
                InternalUpdateVersion();
                _baseSize -= count;
            }

            public override void Reverse(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                _baseList.Reverse(_baseIndex + index, count);
                InternalUpdateVersion();
            }

            public override void SetRange(int index, ICollection c)
            {
                InternalUpdateRange();
                if (index < 0 || index >= _baseSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                _baseList.SetRange(_baseIndex + index, c);
                if (c.Count > 0)
                {
                    InternalUpdateVersion();
                }
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (_baseSize - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                InternalUpdateRange();
                _baseList.Sort(_baseIndex + index, count, comparer);
                InternalUpdateVersion();
            }

            public override object[] ToArray()
            {
                InternalUpdateRange();
                if (_baseSize == 0)
                {
                    return ArrayEx.Empty<object>();
                }

                var array = new object[_baseSize];
                Array.Copy(_baseList._items, _baseIndex, array, 0, _baseSize);
                return array;
            }

            public override Array ToArray(Type type)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                InternalUpdateRange();
                var array = Array.CreateInstance(type, _baseSize);
                _baseList.CopyTo(_baseIndex, array, 0, _baseSize);
                return array;
            }

            public override void TrimToSize()
            {
                throw new NotSupportedException("The specified operation is not supported on Ranges.");
            }

            private void InternalUpdateRange()
            {
                if (_baseVersion != _baseList._version)
                {
                    throw new InvalidOperationException("This range in the underlying list is invalid. A possible cause is that elements were removed.");
                }
            }

            private void InternalUpdateVersion()
            {
                _baseVersion++;
                _version++;
            }
        }

        private sealed class ReadOnlyArrayList : ArrayList
        {
            private ArrayList _list;

            internal ReadOnlyArrayList(ArrayList l)
            {
                _list = l;
            }

            public override int Capacity
            {
                get => _list.Capacity;
                set => throw new NotSupportedException("Collection is read-only.");
            }

            public override int Count => _list.Count;

            public override bool IsFixedSize => true;

            public override bool IsReadOnly => true;

            public override bool IsSynchronized => _list.IsSynchronized;

            public override object SyncRoot => _list.SyncRoot;

            public override object? this[int index]
            {
                get => _list[index];
                set => throw new NotSupportedException("Collection is read-only.");
            }

            public override int Add(object? value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void AddRange(ICollection c)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override int BinarySearch(int index, int count, object? value, IComparer? comparer)
            {
                return _list.BinarySearch(index, count, value, comparer);
            }

            public override void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override object Clone()
            {
                return new ReadOnlyArrayList(_list)
                {
                    _list = (ArrayList)_list.Clone()
                };
            }

            public override bool Contains(object? value)
            {
                return _list.Contains(value);
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public override void CopyTo(int index, Array array, int arrayIndex, int count)
            {
                _list.CopyTo(index, array, arrayIndex, count);
            }

            public override IEnumerator GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public override IEnumerator GetEnumerator(int index, int count)
            {
                return _list.GetEnumerator(index, count);
            }

            public override ArrayList GetRange(int index, int count)
            {
                if (index < 0 || count < 0)
                {
                    throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "Non-negative number required.");
                }

                if (Count - index < count)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }

                return new Range(this, index, count);
            }

            public override int IndexOf(object? value)
            {
                return _list.IndexOf(value);
            }

            public override int IndexOf(object? value, int startIndex)
            {
                return _list.IndexOf(value, startIndex);
            }

            public override int IndexOf(object? value, int startIndex, int count)
            {
                return _list.IndexOf(value, startIndex, count);
            }

            public override void Insert(int index, object? value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void InsertRange(int index, ICollection c)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override int LastIndexOf(object? value)
            {
                return _list.LastIndexOf(value);
            }

            public override int LastIndexOf(object? value, int startIndex)
            {
                return _list.LastIndexOf(value, startIndex);
            }

            public override int LastIndexOf(object? value, int startIndex, int count)
            {
                return _list.LastIndexOf(value, startIndex, count);
            }

            public override void Remove(object? value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void RemoveRange(int index, int count)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void Reverse(int index, int count)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void SetRange(int index, ICollection c)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public override object[] ToArray()
            {
                return _list.ToArray();
            }

            public override Array ToArray(Type type)
            {
                return _list.ToArray(type);
            }

            public override void TrimToSize()
            {
                throw new NotSupportedException("Collection is read-only.");
            }
        }

        private sealed class ReadOnlyList : IList
        {
            private readonly IList _list;

            internal ReadOnlyList(IList l)
            {
                _list = l;
            }

            public int Count => _list.Count;

            public bool IsFixedSize => true;

            public bool IsReadOnly => true;

            public bool IsSynchronized => _list.IsSynchronized;

            public object SyncRoot => _list.SyncRoot;

            public object this[int index]
            {
                get => _list[index];
                set => throw new NotSupportedException("Collection is read-only.");
            }

            public int Add(object value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool Contains(object value)
            {
                return _list.Contains(value);
            }

            public void CopyTo(Array array, int index)
            {
                _list.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public int IndexOf(object value)
            {
                return _list.IndexOf(value);
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Remove(object value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
        }

        private sealed class SyncArrayList : ArrayList
        {
            private readonly ArrayList _list;

            internal SyncArrayList(ArrayList list)
                : base(false)
            {
                _list = list;
                SyncRoot = list.SyncRoot;
                IsReadOnly = _list.IsReadOnly;
                IsFixedSize = _list.IsFixedSize;
            }

            public override int Capacity
            {
                get
                {
                    lock (SyncRoot)
                    {
                        return _list.Capacity;
                    }
                }
                set
                {
                    lock (SyncRoot)
                    {
                        _list.Capacity = value;
                    }
                }
            }

            public override int Count
            {
                get
                {
                    lock (SyncRoot)
                    {
                        return _list.Count;
                    }
                }
            }

            public override bool IsFixedSize { get; }

            public override bool IsReadOnly { get; }

            public override bool IsSynchronized => true;

            public override object SyncRoot { get; }

            public override object? this[int index]
            {
                get
                {
                    lock (SyncRoot)
                    {
                        return _list[index];
                    }
                }
                set
                {
                    lock (SyncRoot)
                    {
                        _list[index] = value;
                    }
                }
            }

            public override int Add(object? value)
            {
                lock (SyncRoot)
                {
                    return _list.Add(value);
                }
            }

            public override void AddRange(ICollection c)
            {
                lock (SyncRoot)
                {
                    _list.AddRange(c);
                }
            }

            public override int BinarySearch(object? value)
            {
                lock (SyncRoot)
                {
                    return _list.BinarySearch(value);
                }
            }

            public override int BinarySearch(object? value, IComparer comparer)
            {
                lock (SyncRoot)
                {
                    return _list.BinarySearch(value, comparer);
                }
            }

            public override int BinarySearch(int index, int count, object? value, IComparer? comparer)
            {
                lock (SyncRoot)
                {
                    return _list.BinarySearch(index, count, value, comparer);
                }
            }

            public override void Clear()
            {
                lock (SyncRoot)
                {
                    _list.Clear();
                }
            }

            public override object Clone()
            {
                lock (SyncRoot)
                {
                    return new SyncArrayList((ArrayList)_list.Clone());
                }
            }

            public override bool Contains(object? value)
            {
                lock (SyncRoot)
                {
                    return _list.Contains(value);
                }
            }

            public override void CopyTo(Array array)
            {
                lock (SyncRoot)
                {
                    _list.CopyTo(array);
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (SyncRoot)
                {
                    _list.CopyTo(array, arrayIndex);
                }
            }

            public override void CopyTo(int index, Array array, int arrayIndex, int count)
            {
                lock (SyncRoot)
                {
                    _list.CopyTo(index, array, arrayIndex, count);
                }
            }

            public override IEnumerator GetEnumerator()
            {
                lock (SyncRoot)
                {
                    return _list.GetEnumerator();
                }
            }

            public override IEnumerator GetEnumerator(int index, int count)
            {
                lock (SyncRoot)
                {
                    return _list.GetEnumerator(index, count);
                }
            }

            public override ArrayList GetRange(int index, int count)
            {
                lock (SyncRoot)
                {
                    return _list.GetRange(index, count);
                }
            }

            public override int IndexOf(object? value)
            {
                lock (SyncRoot)
                {
                    return _list.IndexOf(value);
                }
            }

            public override int IndexOf(object? value, int startIndex)
            {
                lock (SyncRoot)
                {
                    return _list.IndexOf(value, startIndex);
                }
            }

            public override int IndexOf(object? value, int startIndex, int count)
            {
                lock (SyncRoot)
                {
                    return _list.IndexOf(value, startIndex, count);
                }
            }

            public override void Insert(int index, object? value)
            {
                lock (SyncRoot)
                {
                    _list.Insert(index, value);
                }
            }

            public override void InsertRange(int index, ICollection c)
            {
                lock (SyncRoot)
                {
                    _list.InsertRange(index, c);
                }
            }

            public override int LastIndexOf(object? value)
            {
                lock (SyncRoot)
                {
                    return _list.LastIndexOf(value);
                }
            }

            public override int LastIndexOf(object? value, int startIndex)
            {
                lock (SyncRoot)
                {
                    return _list.LastIndexOf(value, startIndex);
                }
            }

            public override int LastIndexOf(object? value, int startIndex, int count)
            {
                lock (SyncRoot)
                {
                    return _list.LastIndexOf(value, startIndex, count);
                }
            }

            public override void Remove(object? value)
            {
                lock (SyncRoot)
                {
                    _list.Remove(value);
                }
            }

            public override void RemoveAt(int index)
            {
                lock (SyncRoot)
                {
                    _list.RemoveAt(index);
                }
            }

            public override void RemoveRange(int index, int count)
            {
                lock (SyncRoot)
                {
                    _list.RemoveRange(index, count);
                }
            }

            public override void Reverse(int index, int count)
            {
                lock (SyncRoot)
                {
                    _list.Reverse(index, count);
                }
            }

            public override void SetRange(int index, ICollection c)
            {
                lock (SyncRoot)
                {
                    _list.SetRange(index, c);
                }
            }

            public override void Sort()
            {
                lock (SyncRoot)
                {
                    _list.Sort();
                }
            }

            public override void Sort(IComparer comparer)
            {
                lock (SyncRoot)
                {
                    _list.Sort(comparer);
                }
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                lock (SyncRoot)
                {
                    _list.Sort(index, count, comparer);
                }
            }

            public override object[] ToArray()
            {
                lock (SyncRoot)
                {
                    return _list.ToArray();
                }
            }

            public override Array ToArray(Type type)
            {
                lock (SyncRoot)
                {
                    return _list.ToArray(type);
                }
            }

            public override void TrimToSize()
            {
                lock (SyncRoot)
                {
                    _list.TrimToSize();
                }
            }
        }

        private sealed class SyncIList : IList
        {
            private readonly IList _list;

            internal SyncIList(IList list)
            {
                _list = list;
                SyncRoot = list.SyncRoot;
                IsReadOnly = _list.IsReadOnly;
                IsFixedSize = _list.IsFixedSize;
            }

            public int Count
            {
                get
                {
                    lock (SyncRoot)
                    {
                        return _list.Count;
                    }
                }
            }

            public bool IsFixedSize { get; }

            public bool IsReadOnly { get; }

            public bool IsSynchronized => true;

            public object SyncRoot { get; }

            public object this[int index]
            {
                get
                {
                    lock (SyncRoot)
                    {
                        return _list[index];
                    }
                }
                set
                {
                    lock (SyncRoot)
                    {
                        _list[index] = value;
                    }
                }
            }

            public int Add(object value)
            {
                lock (SyncRoot)
                {
                    return _list.Add(value);
                }
            }

            public void Clear()
            {
                lock (SyncRoot)
                {
                    _list.Clear();
                }
            }

            public bool Contains(object value)
            {
                lock (SyncRoot)
                {
                    return _list.Contains(value);
                }
            }

            public void CopyTo(Array array, int index)
            {
                lock (SyncRoot)
                {
                    _list.CopyTo(array, index);
                }
            }

            public IEnumerator GetEnumerator()
            {
                lock (SyncRoot)
                {
                    return _list.GetEnumerator();
                }
            }

            public int IndexOf(object value)
            {
                lock (SyncRoot)
                {
                    return _list.IndexOf(value);
                }
            }

            public void Insert(int index, object value)
            {
                lock (SyncRoot)
                {
                    _list.Insert(index, value);
                }
            }

            public void Remove(object value)
            {
                lock (SyncRoot)
                {
                    _list.Remove(value);
                }
            }

            public void RemoveAt(int index)
            {
                lock (SyncRoot)
                {
                    _list.RemoveAt(index);
                }
            }
        }
    }
}

#endif