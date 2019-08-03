#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable CA2235 // Mark all non-serializable fields
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Class:  SortedList
**
** Purpose: Represents a collection of key/value pairs
**          that are sorted by the keys and are accessible
**          by key and by index.
**
===========================================================*/

using System.Diagnostics;
using System.Globalization;
using Theraot.Collections.ThreadSafe;

namespace System.Collections
{
    // The SortedList class implements a sorted list of keys and values. Entries in
    // a sorted list are sorted by their keys and are accessible both by key and by
    // index. The keys of a sorted list can be ordered either according to a
    // specific IComparer implementation given when the sorted list is
    // instantiated, or according to the IComparable implementation provided
    // by the keys themselves. In either case, a sorted list does not allow entries
    // with duplicate keys.
    //
    // A sorted list internally maintains two arrays that store the keys and
    // values of the entries. The capacity of a sorted list is the allocated
    // length of these internal arrays. As elements are added to a sorted list, the
    // capacity of the sorted list is automatically increased as required by
    // reallocating the internal arrays.  The capacity is never automatically
    // decreased, but users can call either TrimToSize or
    // Capacity explicitly.
    //
    // The GetKeyList and GetValueList methods of a sorted list
    // provides access to the keys and values of the sorted list in the form of
    // List implementations. The List objects returned by these
    // methods are aliases for the underlying sorted list, so modifications
    // made to those lists are directly reflected in the sorted list, and vice
    // versa.
    //
    // The SortedList class provides a convenient way to create a sorted
    // copy of another dictionary, such as a Hashtable. For example:
    //
    // Hashtable h = new Hashtable();
    // h.Add(...);
    // h.Add(...);
    // ...
    // SortedList s = new SortedList(h);
    //
    // The last line above creates a sorted list that contains a copy of the keys
    // and values stored in the hashtable. In this particular example, the keys
    // will be ordered according to the IComparable interface, which they
    // all must implement. To impose a different ordering, SortedList also
    // has a constructor that allows a specific IComparer implementation to
    // be specified.
    //
    [DebuggerTypeProxy(typeof(SortedListDebugView))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [Serializable]
    public class SortedList : IDictionary, ICloneable
    {
        // Copy of Array.MaxArrayLength
        internal const int MaxArrayLength = 0X7FEFFFFF;

        private IComparer _comparer;
        private KeyList _keyList;
        private object[] _keys;
        private int _size;
        private ValueList _valueList;
        private object[] _values;
        private int _version;

        // Constructs a new sorted list. The sorted list is initially empty and has
        // a capacity of zero. Upon adding the first element to the sorted list the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the sorted list are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the sorted list.
        public SortedList()
        {
            Init();
        }

        // Constructs a new sorted list. The sorted list is initially empty and has
        // a capacity of zero. Upon adding the first element to the sorted list the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the sorted list are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the sorted list.
        //
        public SortedList(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Non-negative number required.");
            }

            _keys = new object[initialCapacity];
            _values = new object[initialCapacity];
            _comparer = new Comparer(CultureInfo.CurrentCulture);
        }

        // Constructs a new sorted list with a given IComparer
        // implementation. The sorted list is initially empty and has a capacity of
        // zero. Upon adding the first element to the sorted list the capacity is
        // increased to 16, and then increased in multiples of two as required. The
        // elements of the sorted list are ordered according to the given
        // IComparer implementation. If comparer is null, the
        // elements are compared to each other using the IComparable
        // interface, which in that case must be implemented by the keys of all
        // entries added to the sorted list.
        //
        public SortedList(IComparer comparer)
            : this()
        {
            if (comparer != null)
            {
                _comparer = comparer;
            }
        }

        // Constructs a new sorted list with a given IComparer
        // implementation and a given initial capacity. The sorted list is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required. The elements of the sorted list
        // are ordered according to the given IComparer implementation. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented
        // by the keys of all entries added to the sorted list.
        //
        public SortedList(IComparer comparer, int capacity)
            : this(comparer)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Capacity = capacity;
        }

        // Constructs a new sorted list containing a copy of the entries in the
        // given dictionary. The elements of the sorted list are ordered according
        // to the IComparable interface, which must be implemented by the
        // keys of all entries in the given dictionary as well as keys
        // subsequently added to the sorted list.
        //
        public SortedList(IDictionary d)
            : this(d, null)
        {
            // Empty
        }

        // Constructs a new sorted list containing a copy of the entries in the
        // given dictionary. The elements of the sorted list are ordered according
        // to the given IComparer implementation. If comparer is
        // null, the elements are compared to each other using the
        // IComparable interface, which in that case must be implemented
        // by the keys of all entries in the given dictionary as well as keys
        // subsequently added to the sorted list.
        //
        public SortedList(IDictionary d, IComparer comparer)
            : this(comparer, d?.Count ?? 0)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d), "Dictionary cannot be null.");
            }

            d.Keys.CopyTo(_keys, 0);
            d.Values.CopyTo(_values, 0);

            // Array.Sort(Array keys, Array values, IComparer comparer) does not exist in System.Runtime contract v4.0.10.0.
            // This works around that by sorting only on the keys and then assigning values accordingly.
            Array.Sort(_keys, comparer);
            for (var i = 0; i < _keys.Length; i++)
            {
                _values[i] = d[_keys[i]];
            }
            _size = d.Count;
        }

        // Returns the capacity of this sorted list. The capacity of a sorted list
        // represents the allocated length of the internal arrays used to store the
        // keys and values of the list, and thus also indicates the maximum number
        // of entries the list can contain before a reallocation of the internal
        // arrays is required.
        //
        public virtual int Capacity
        {
            get => _keys.Length;
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");
                }

                if (value != _keys.Length)
                {
                    if (value > 0)
                    {
                        var newKeys = new object[value];
                        var newValues = new object[value];
                        if (_size > 0)
                        {
                            Array.Copy(_keys, 0, newKeys, 0, _size);
                            Array.Copy(_values, 0, newValues, 0, _size);
                        }
                        _keys = newKeys;
                        _values = newValues;
                    }
                    else
                    {
                        // size can only be zero here.
                        Debug.Assert(_size == 0, "Size is not zero");
                        _keys = ArrayEx.Empty<object>();
                        _values = ArrayEx.Empty<object>();
                    }
                }
            }
        }

        // Returns the number of entries in this sorted list.
        //
        public virtual int Count => _size;

        public virtual bool IsFixedSize => false;

        // Is this SortedList read-only?
        public virtual bool IsReadOnly => false;

        // Is this SortedList synchronized (thread-safe)?
        public virtual bool IsSynchronized => false;

        // Returns a collection representing the keys of this sorted list. This
        // method returns the same object as GetKeyList, but typed as an
        // ICollection instead of an IList.
        //
        public virtual ICollection Keys => GetKeyList();

        // Synchronization root for this object.
        public virtual object SyncRoot => this;

        // Returns a collection representing the values of this sorted list. This
        // method returns the same object as GetValueList, but typed as an
        // ICollection instead of an IList.
        //
        public virtual ICollection Values => GetValueList();

        // Returns the value associated with the given key. If an entry with the
        // given key is not found, the returned value is null.
        //
        public virtual object this[object key]
        {
            get
            {
                var i = IndexOfKey(key);
                if (i >= 0)
                {
                    return _values[i];
                }

                return null;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Key cannot be null.");
                }

                var i = Array.BinarySearch(_keys, 0, _size, key, _comparer);
                if (i >= 0)
                {
                    _values[i] = value;
                    _version++;
                    return;
                }
                Insert(~i, key, value);
            }
        }

        // Returns a thread-safe SortedList.
        //
        public static SortedList Synchronized(SortedList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return new SyncSortedList(list);
        }

        // Adds an entry with the given key and value to this sorted list. An
        // ArgumentException is thrown if the key is already present in the sorted list.
        //
        public virtual void Add(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var i = Array.BinarySearch(_keys, 0, _size, key, _comparer);
            if (i >= 0)
            {
                throw new ArgumentException("Item has already been added. Key in dictionary: '{GetKey(i)}'  Key being added: '{key}'");
            }

            Insert(~i, key, value);
        }

        // Removes all entries from this sorted list.
        public virtual void Clear()
        {
            // clear does not change the capacity
            _version++;
            Array.Clear(_keys, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            Array.Clear(_values, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            _size = 0;
        }

        // Makes a virtually identical copy of this SortedList.  This is a shallow
        // copy.  IE, the Objects in the SortedList are not cloned - we copy the
        // references to those objects.
        public virtual object Clone()
        {
            var sl = new SortedList(_size);
            Array.Copy(_keys, 0, sl._keys, 0, _size);
            Array.Copy(_values, 0, sl._values, 0, _size);
            sl._size = _size;
            sl._version = _version;
            sl._comparer = _comparer;
            // Don't copy keyList nor valueList.
            return sl;
        }

        // Checks if this sorted list contains an entry with the given key.
        //
        public virtual bool Contains(object key)
        {
            return IndexOfKey(key) >= 0;
        }

        // Checks if this sorted list contains an entry with the given key.
        //
        public virtual bool ContainsKey(object key)
        {
            // Yes, this is a SPEC'ed duplicate of Contains().
            return IndexOfKey(key) >= 0;
        }

        // Checks if this sorted list contains an entry with the given value. The
        // values of the entries of the sorted list are compared to the given value
        // using the Object.Equals method. This method performs a linear
        // search and is substantially slower than the Contains
        // method.
        //
        public virtual bool ContainsValue(object value)
        {
            return IndexOfValue(value) >= 0;
        }

        // Copies the values in this SortedList to an array.
        public virtual void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Array cannot be null.");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number required.");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
            }

            for (var i = 0; i < Count; i++)
            {
                var entry = new DictionaryEntry(_keys[i], _values[i]);
                array.SetValue(entry, i + arrayIndex);
            }
        }

        // Returns the value of the entry at the given index.
        //
        public virtual object GetByIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            return _values[index];
        }

        // Returns an IDictionaryEnumerator for this sorted list.  If modifications
        // made to the sorted list while an enumeration is in progress,
        // the MoveNext and Remove methods
        // of the enumerator will throw an exception.
        //
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return new SortedListEnumerator(this, 0, _size, SortedListEnumerator.DictEntry);
        }

        // Returns the key of the entry at the given index.
        //
        public virtual object GetKey(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            return _keys[index];
        }

        // Returns an IList representing the keys of this sorted list. The
        // returned list is an alias for the keys of this sorted list, so
        // modifications made to the returned list are directly reflected in the
        // underlying sorted list, and vice versa. The elements of the returned
        // list are ordered in the same way as the elements of the sorted list. The
        // returned list does not support adding, inserting, or modifying elements
        // (the Add, AddRange, Insert, InsertRange,
        // Reverse, Set, SetRange, and Sort methods
        // throw exceptions), but it does allow removal of elements (through the
        // Remove and RemoveRange methods or through an enumerator).
        // Null is an invalid key value.
        //
        public virtual IList GetKeyList()
        {
            return _keyList ?? (_keyList = new KeyList(this));
        }

        // Returns an IList representing the values of this sorted list. The
        // returned list is an alias for the values of this sorted list, so
        // modifications made to the returned list are directly reflected in the
        // underlying sorted list, and vice versa. The elements of the returned
        // list are ordered in the same way as the elements of the sorted list. The
        // returned list does not support adding or inserting elements (the
        // Add, AddRange, Insert and InsertRange
        // methods throw exceptions), but it does allow modification and removal of
        // elements (through the Remove, RemoveRange, Set and
        // SetRange methods or through an enumerator).
        //
        public virtual IList GetValueList()
        {
            return _valueList ?? (_valueList = new ValueList(this));
        }

        // Returns the index of the entry with a given key in this sorted list. The
        // key is located through a binary search, and thus the average execution
        // time of this method is proportional to Log2(size), where
        // size is the size of this sorted list. The returned value is -1 if
        // the given key does not occur in this sorted list. Null is an invalid
        // key value.
        //
        public virtual int IndexOfKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var ret = Array.BinarySearch(_keys, 0, _size, key, _comparer);
            return ret >= 0 ? ret : -1;
        }

        // Returns the index of the first occurrence of an entry with a given value
        // in this sorted list. The entry is located through a linear search, and
        // thus the average execution time of this method is proportional to the
        // size of this sorted list. The elements of the list are compared to the
        // given value using the Object.Equals method.
        //
        public virtual int IndexOfValue(object value)
        {
            return Array.IndexOf(_values, value, 0, _size);
        }

        // Removes an entry from this sorted list. If an entry with the specified
        // key exists in the sorted list, it is removed. An ArgumentException is
        // thrown if the key is null.
        //
        public virtual void Remove(object key)
        {
            var i = IndexOfKey(key);
            if (i >= 0)
            {
                RemoveAt(i);
            }
        }

        // Removes the entry at the given index. The size of the sorted list is
        // decreased by one.
        //
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            _size--;
            if (index < _size)
            {
                Array.Copy(_keys, index + 1, _keys, index, _size - index);
                Array.Copy(_values, index + 1, _values, index, _size - index);
            }
            _keys[_size] = null;
            _values[_size] = null;
            _version++;
        }

        // Sets the value at an index to a given value.  The previous value of
        // the given entry is overwritten.
        //
        public virtual void SetByIndex(int index, object value)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            _values[index] = value;
            _version++;
        }

        // Sets the capacity of this sorted list to the size of the sorted list.
        // This method can be used to minimize a sorted list's memory overhead once
        // it is known that no new elements will be added to the sorted list. To
        // completely clear a sorted list and release all memory referenced by the
        // sorted list, execute the following statements:
        //
        // sortedList.Clear();
        // sortedList.TrimToSize();
        //
        public virtual void TrimToSize()
        {
            Capacity = _size;
        }

        internal virtual KeyValuePairs[] ToKeyValuePairsArray()
        {
            var array = new KeyValuePairs[Count];
            for (var i = 0; i < Count; i++)
            {
                array[i] = new KeyValuePairs(_keys[i], _values[i]);
            }
            return array;
        }

        // Copies the values in this SortedList to an KeyValuePairs array.
        // KeyValuePairs is different from Dictionary Entry in that it has special
        // debugger attributes on its fields.
        // Ensures that the capacity of this sorted list is at least the given
        // minimum value. If the current capacity of the list is less than
        // min, the capacity is increased to twice the current capacity or
        // to min, whichever is larger.
        private void EnsureCapacity(int min)
        {
            var newCapacity = _keys.Length == 0 ? 16 : _keys.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > MaxArrayLength)
            {
                newCapacity = MaxArrayLength;
            }

            if (newCapacity < min)
            {
                newCapacity = min;
            }

            Capacity = newCapacity;
        }

        // Returns an IEnumerator for this sorted list.  If modifications
        // made to the sorted list while an enumeration is in progress,
        // the MoveNext and Remove methods
        // of the enumerator will throw an exception.
        //
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SortedListEnumerator(this, 0, _size, SortedListEnumerator.DictEntry);
        }

        private void Init()
        {
            _keys = ArrayEx.Empty<object>();
            _values = ArrayEx.Empty<object>();
            _size = 0;
            _comparer = new Comparer(CultureInfo.CurrentCulture);
        }

        // Inserts an entry with a given key and value at a given index.
        private void Insert(int index, object key, object value)
        {
            if (_size == _keys.Length)
            {
                EnsureCapacity(_size + 1);
            }

            if (index < _size)
            {
                Array.Copy(_keys, index, _keys, index + 1, _size - index);
                Array.Copy(_values, index, _values, index + 1, _size - index);
            }
            _keys[index] = key;
            _values[index] = value;
            _size++;
            _version++;
        }

        [Serializable]
        private sealed class KeyList : IList
        {
            private readonly SortedList _sortedList;

            internal KeyList(SortedList sortedList)
            {
                _sortedList = sortedList;
            }

            public int Count => _sortedList._size;

            public bool IsFixedSize => true;
            public bool IsReadOnly => true;
            public bool IsSynchronized => _sortedList.IsSynchronized;

            public object SyncRoot => _sortedList.SyncRoot;

            public object this[int index]
            {
                get => _sortedList.GetKey(index);
                set => throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
            }

            public int Add(object value)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void Clear()
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public bool Contains(object value)
            {
                return _sortedList.Contains(value);
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                // defer error checking to Array.Copy
                Array.Copy(_sortedList._keys, 0, array, arrayIndex, _sortedList.Count);
            }

            public IEnumerator GetEnumerator()
            {
                return new SortedListEnumerator(_sortedList, 0, _sortedList.Count, SortedListEnumerator.Keys);
            }

            public int IndexOf(object value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Key cannot be null.");
                }

                var i = Array.BinarySearch(_sortedList._keys, 0,
                                           _sortedList.Count, value, _sortedList._comparer);
                if (i >= 0)
                {
                    return i;
                }

                return -1;
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void Remove(object value)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }
        }

        // internal debug view class for sorted list
        internal class SortedListDebugView
        {
            private readonly SortedList _sortedList;

            public SortedListDebugView(SortedList sortedList)
            {
                _sortedList = sortedList ?? throw new ArgumentNullException(nameof(sortedList));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePairs[] Items => _sortedList.ToKeyValuePairsArray();
        }

        private sealed class SortedListEnumerator : IDictionaryEnumerator, ICloneable
        {
            internal const int DictEntry = 3;
            internal const int Keys = 1;
            internal const int Values = 2;

            private bool _current;
            private readonly int _endIndex;
            private readonly int _getObjectRetType;
            private int _index;
            private object _key;
            private readonly SortedList _sortedList;
            private readonly int _startIndex;
            private object _value;

            // Store for Reset.
            private readonly int _version;

            // Is the current element valid?
            // What should GetObject return?
            internal SortedListEnumerator(SortedList sortedList, int index, int count, int getObjRetType)
            {
                _sortedList = sortedList;
                _index = index;
                _startIndex = index;
                _endIndex = index + count;
                _version = sortedList._version;
                _getObjectRetType = getObjRetType;
                _current = false;
            }

            public object Current
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return _getObjectRetType == Keys ? _key : _getObjectRetType == Values ? _value : new DictionaryEntry(_key, _value);
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    if (_version != _sortedList._version)
                    {
                        throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                    }

                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return new DictionaryEntry(_key, _value);
                }
            }

            public object Key
            {
                get
                {
                    if (_version != _sortedList._version)
                    {
                        throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                    }

                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return _key;
                }
            }

            public object Value
            {
                get
                {
                    if (_version != _sortedList._version)
                    {
                        throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                    }

                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return _value;
                }
            }

            public object Clone() => MemberwiseClone();

            public bool MoveNext()
            {
                if (_version != _sortedList._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                if (_index < _endIndex)
                {
                    _key = _sortedList._keys[_index];
                    _value = _sortedList._values[_index];
                    _index++;
                    _current = true;
                    return true;
                }
                _key = null;
                _value = null;
                _current = false;
                return false;
            }

            public void Reset()
            {
                if (_version != _sortedList._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                _index = _startIndex;
                _current = false;
                _key = null;
                _value = null;
            }
        }

        [Serializable]
        private class SyncSortedList : SortedList
        {
            private readonly SortedList _list;
            private readonly object _root;

            internal SyncSortedList(SortedList list)
            {
                _list = list;
                _root = list.SyncRoot;
                IsReadOnly = list.IsReadOnly;
                IsFixedSize = list.IsFixedSize;
            }

            public override int Capacity
            {
                get { lock (_root) { return _list.Capacity; } }
            }

            public override int Count
            {
                get { lock (_root) { return _list.Count; } }
            }

            public override bool IsFixedSize { get; }
            public override bool IsReadOnly { get; }
            public override bool IsSynchronized => true;
            public override object SyncRoot => _root;

            public override object this[object key]
            {
                get
                {
                    lock (_root)
                    {
                        return _list[key];
                    }
                }
                set
                {
                    lock (_root)
                    {
                        _list[key] = value;
                    }
                }
            }

            public override void Add(object key, object value)
            {
                lock (_root)
                {
                    _list.Add(key, value);
                }
            }

            public override void Clear()
            {
                lock (_root)
                {
                    _list.Clear();
                }
            }

            public override object Clone()
            {
                lock (_root)
                {
                    return _list.Clone();
                }
            }

            public override bool Contains(object key)
            {
                lock (_root)
                {
                    return _list.Contains(key);
                }
            }

            public override bool ContainsKey(object key)
            {
                lock (_root)
                {
                    return _list.ContainsKey(key);
                }
            }

            public override bool ContainsValue(object value)
            {
                lock (_root)
                {
                    return _list.ContainsValue(value);
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (_root)
                {
                    _list.CopyTo(array, arrayIndex);
                }
            }

            public override object GetByIndex(int index)
            {
                lock (_root)
                {
                    return _list.GetByIndex(index);
                }
            }

            public override IDictionaryEnumerator GetEnumerator()
            {
                lock (_root)
                {
                    return _list.GetEnumerator();
                }
            }

            public override object GetKey(int index)
            {
                lock (_root)
                {
                    return _list.GetKey(index);
                }
            }

            public override IList GetKeyList()
            {
                lock (_root)
                {
                    return _list.GetKeyList();
                }
            }

            public override IList GetValueList()
            {
                lock (_root)
                {
                    return _list.GetValueList();
                }
            }

            public override int IndexOfKey(object key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Key cannot be null.");
                }

                lock (_root)
                {
                    return _list.IndexOfKey(key);
                }
            }

            public override int IndexOfValue(object value)
            {
                lock (_root)
                {
                    return _list.IndexOfValue(value);
                }
            }

            public override void Remove(object key)
            {
                lock (_root)
                {
                    _list.Remove(key);
                }
            }

            public override void RemoveAt(int index)
            {
                lock (_root)
                {
                    _list.RemoveAt(index);
                }
            }

            public override void SetByIndex(int index, object value)
            {
                lock (_root)
                {
                    _list.SetByIndex(index, value);
                }
            }

            public override void TrimToSize()
            {
                lock (_root)
                {
                    _list.TrimToSize();
                }
            }
        }

        [Serializable]
        private sealed class ValueList : IList
        {
            private readonly SortedList _sortedList;

            internal ValueList(SortedList sortedList)
            {
                _sortedList = sortedList;
            }

            public int Count => _sortedList._size;

            public bool IsFixedSize => true;
            public bool IsReadOnly => true;
            public bool IsSynchronized => _sortedList.IsSynchronized;

            public object SyncRoot => _sortedList.SyncRoot;

            public object this[int index]
            {
                get => _sortedList.GetByIndex(index);
                set => throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public int Add(object value)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void Clear()
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public bool Contains(object value)
            {
                return _sortedList.ContainsValue(value);
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                if (array != null && array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                // defer error checking to Array.Copy
                Array.Copy(_sortedList._values, 0, array, arrayIndex, _sortedList.Count);
            }

            public IEnumerator GetEnumerator()
            {
                return new SortedListEnumerator(_sortedList, 0, _sortedList.Count, SortedListEnumerator.Values);
            }

            public int IndexOf(object value)
            {
                return Array.IndexOf(_sortedList._values, value, 0, _sortedList.Count);
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void Remove(object value)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("This operation is not supported on SortedList nested types because they require modifying the original SortedList.");
            }
        }
    }
}

#endif