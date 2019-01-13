#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
#pragma warning disable CA2235 // Mark all non-serializable fields
#pragma warning disable CC0021 // Use nameof
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor
// ReSharper disable once VirtualMemberCallInConstructor

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Class:  Hashtable
**
** Purpose: Represents a collection of key/value pairs
**          that are organized based on the hash code
**          of the key.
**
===========================================================*/

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Collections
{
    // The Hashtable class represents a dictionary of associated keys and values
    // with constant lookup time.
    //
    // Objects used as keys in a hashtable must implement the GetHashCode
    // and Equals methods (or they can rely on the default implementations
    // inherited from Object if key equality is simply reference
    // equality). Furthermore, the GetHashCode and Equals methods of
    // a key object must produce the same results given the same parameters for the
    // entire time the key is present in the hashtable. In practical terms, this
    // means that key objects should be immutable, at least for the time they are
    // used as keys in a hashtable.
    //
    // When entries are added to a hashtable, they are placed into
    // buckets based on the hashcode of their keys. Subsequent lookups of
    // keys will use the hashcode of the keys to only search a particular bucket,
    // thus substantially reducing the number of key comparisons required to find
    // an entry. A hashtable's maximum load factor, which can be specified
    // when the hashtable is instantiated, determines the maximum ratio of
    // hashtable entries to hashtable buckets. Smaller load factors cause faster
    // average lookup times at the cost of increased memory consumption. The
    // default maximum load factor of 1.0 generally provides the best balance
    // between speed and size. As entries are added to a hashtable, the hashtable's
    // actual load factor increases, and when the actual load factor reaches the
    // maximum load factor value, the number of buckets in the hashtable is
    // automatically increased by approximately a factor of two (to be precise, the
    // number of hashtable buckets is increased to the smallest prime number that
    // is larger than twice the current number of hashtable buckets).
    //
    // Each object provides their own hash function, accessed by calling
    // GetHashCode().  However, one can write their own object
    // implementing IEqualityComparer and pass it to a constructor on
    // the Hashtable.  That hash function (and the equals method on the
    // IEqualityComparer) would be used for all objects in the table.
    //
    [DebuggerTypeProxy(typeof(HashtableDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class Hashtable : IDictionary, ISerializable, IDeserializationCallback, ICloneable
    {
        /*
          This Hashtable uses double hashing.  There are hashsize buckets in the
          table, and each bucket can contain 0 or 1 element.  We use a bit to mark
          whether there's been a collision when we inserted multiple elements
          (ie, an inserted item was hashed at least a second time and we probed
          this bucket, but it was already in use).  Using the collision bit, we
          can terminate lookups & removes for elements that aren't in the hash
          table more quickly.  We steal the most significant bit from the hash code
          to store the collision bit.

          Our hash function is of the following form:

          h(key, n) = h1(key) + n*h2(key)

          where n is the number of times we've hit a collided bucket and rehashed
          (on this particular lookup).  Here are our hash functions:

          h1(key) = GetHash(key);  // default implementation calls key.GetHashCode();
          h2(key) = 1 + (((h1(key) >> 5) + 1) % (hashsize - 1));

          The h1 can return any number.  h2 must return a number between 1 and
          hashsize - 1 that is relatively prime to hashsize (not a problem if
          hashsize is prime).  (Knuth's Art of Computer Programming, Vol. 3, p. 528-9)
          If this is true, then we are guaranteed to visit every bucket in exactly
          hashsize probes, since the least common multiple of hashsize and h2(key)
          will be hashsize * h2(key).  (This is the first number where adding h2 to
          h1 mod hashsize will be 0 and we will search the same bucket twice).

          We previously used a different h2(key, n) that was not constant.  That is a
          horrifically bad idea, unless you can prove that series will never produce
          any identical numbers that overlap when you mod them by hashsize, for all
          subranges from i to i+hashsize, for all i.  It's not worth investigating,
          since there was no clear benefit from using that hash function, and it was
          broken.

          For efficiency reasons, we've implemented this by storing h1 and h2 in a
          temporary, and setting a variable called seed equal to h1.  We do a probe,
          and if we collided, we simply add h2 to seed each time through the loop.

          A good test for h2() is to subclass Hashtable, provide your own implementation
          of GetHash() that returns a constant, then add many items to the hash table.
          Make sure Count equals the number of items you inserted.

          Note that when we remove an item from the hash table, we set the key
          equal to buckets, if there was a collision in this bucket.  Otherwise
          we'd either wipe out the collision bit, or we'd still have an item in
          the hash table.

           --
        */

        private const string _comparerName = "Comparer";
        private const string _hashCodeProviderName = "HashCodeProvider";
        private const string _hashSizeName = "HashSize";
        private const int _initialSize = 3;

        private const string _keyComparerName = "KeyComparer";
        private const string _keysName = "Keys";
        private const string _loadFactorName = "LoadFactor";
        private const string _valuesName = "Values";
        private const string _versionName = "Version";
        // Deleted entries have their key set to buckets

        private Bucket[] _buckets;

        // The total number of entries in the hash table.
        private int _count;

        private volatile bool _isWriterInProgress;

        private ICollection _keys;

        private float _loadFactor;

        private int _loadSize;

        // The total number of collision bits set in the hashtable
        private int _occupancy;

        private ICollection _values;

        private volatile int _version;

        // Constructs a new hashtable. The hashtable is created with an initial
        // capacity of zero and a load factor of 1.0.
        public Hashtable()
            : this(0, 1.0f)
        {
            // Empty
        }

        // Constructs a new hashtable with the given initial capacity and a load
        // factor of 1.0. The capacity argument serves as an indication of
        // the number of entries the hashtable will contain. When this number (or
        // an approximation) is known, specifying it in the constructor can
        // eliminate a number of resizing operations that would otherwise be
        // performed when elements are added to the hashtable.
        //
        public Hashtable(int capacity) : this(capacity, 1.0f)
        {
        }

        // Constructs a new hashtable with the given initial capacity and load
        // factor. The capacity argument serves as an indication of the
        // number of entries the hashtable will contain. When this number (or an
        // approximation) is known, specifying it in the constructor can eliminate
        // a number of resizing operations that would otherwise be performed when
        // elements are added to the hashtable. The loadFactor argument
        // indicates the maximum ratio of hashtable entries to hashtable buckets.
        // Smaller load factors cause faster average lookup times at the cost of
        // increased memory consumption. A load factor of 1.0 generally provides
        // the best balance between speed and size.
        //
        public Hashtable(int capacity, float loadFactor)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");
            }

            if (!(loadFactor >= 0.1f && loadFactor <= 1.0f))
            {
                throw new ArgumentOutOfRangeException(nameof(loadFactor));
            }

            // Based on perf work, .72 is the optimal load factor for this table.
            _loadFactor = 0.72f * loadFactor;

            double rawSize = capacity / _loadFactor;
            if (rawSize > int.MaxValue)
            {
                throw new ArgumentException("Hashtable's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.", nameof(capacity));
            }

            // Avoid awfully small sizes
            var hashsize = rawSize > _initialSize ? HashHelpers.GetPrime((int)rawSize) : _initialSize;
            _buckets = new Bucket[hashsize];

            _loadSize = (int)(_loadFactor * hashsize);
            _isWriterInProgress = false;
            // Based on the current algorithm, loadSize must be less than hashsize.
            Debug.Assert(_loadSize < hashsize, "Invalid hashtable loadSize!");
        }

        public Hashtable(int capacity, float loadFactor, IEqualityComparer equalityComparer) : this(capacity, loadFactor)
        {
            EqualityComparer = equalityComparer;
        }

        [Obsolete("Please use Hashtable(IEqualityComparer) instead.")]
        public Hashtable(IHashCodeProvider hcp, IComparer comparer)
                    : this(0, 1.0f, hcp, comparer)
        {
        }

        public Hashtable(IEqualityComparer equalityComparer) : this(0, 1.0f, equalityComparer)
        {
        }

        [Obsolete("Please use Hashtable(int, IEqualityComparer) instead.")]
        public Hashtable(int capacity, IHashCodeProvider hcp, IComparer comparer)
                    : this(capacity, 1.0f, hcp, comparer)
        {
        }

        public Hashtable(int capacity, IEqualityComparer equalityComparer)
                    : this(capacity, 1.0f, equalityComparer)
        {
        }

        // Constructs a new hashtable containing a copy of the entries in the given
        // dictionary. The hashtable is created with a load factor of 1.0.
        //
        public Hashtable(IDictionary d) : this(d, 1.0f)
        {
        }

        // Constructs a new hashtable containing a copy of the entries in the given
        // dictionary. The hashtable is created with the given load factor.
        //
        public Hashtable(IDictionary d, float loadFactor)
            : this(d, loadFactor, null)
        {
        }

        [Obsolete("Please use Hashtable(IDictionary, IEqualityComparer) instead.")]
        public Hashtable(IDictionary d, IHashCodeProvider hcp, IComparer comparer)
                    : this(d, 1.0f, hcp, comparer)
        {
        }

        public Hashtable(IDictionary d, IEqualityComparer equalityComparer)
                    : this(d, 1.0f, equalityComparer)
        {
        }

        [Obsolete("Please use Hashtable(int, float, IEqualityComparer) instead.")]
        public Hashtable(int capacity, float loadFactor, IHashCodeProvider hcp, IComparer comparer)
                    : this(capacity, loadFactor)
        {
            if (hcp != null || comparer != null)
            {
                EqualityComparer = new CompatibleComparer(hcp, comparer);
            }
        }

        [Obsolete("Please use Hashtable(IDictionary, float, IEqualityComparer) instead.")]
        public Hashtable(IDictionary d, float loadFactor, IHashCodeProvider hcp, IComparer comparer)
                    : this(d?.Count ?? 0, loadFactor, hcp, comparer)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d), "Dictionary cannot be null.");
            }

            var e = d.GetEnumerator();
            while (e.MoveNext())
            {
                Add(e.Key, e.Value);
            }
        }

        public Hashtable(IDictionary d, float loadFactor, IEqualityComparer equalityComparer)
                    : this(d?.Count ?? 0, loadFactor, equalityComparer)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d), "Dictionary cannot be null.");
            }

            var e = d.GetEnumerator();
            while (e.MoveNext())
            {
                Add(e.Key, e.Value);
            }
        }

        // Note: this constructor is a bogus constructor that does nothing
        // and is for use only with SyncHashtable.
        internal Hashtable(bool trash)
        {
            Theraot.No.Op(trash);
        }

        protected Hashtable(SerializationInfo info, StreamingContext context)
        {
            //We can't do anything with the keys and values until the entire graph has been deserialized
            //and we have a reasonable estimate that GetHashCode is not going to fail.  For the time being,
            //we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            HashHelpers.SerializationInfoTable.Add(this, info);
        }

        // Returns the number of associations in this hashtable.
        //
        public virtual int Count => _count;

        public virtual bool IsFixedSize => false;

        // Is this Hashtable read-only?
        public virtual bool IsReadOnly => false;

        // Is this Hashtable synchronized?  See SyncRoot property
        public virtual bool IsSynchronized => false;

        // Returns a collection representing the keys of this hashtable. The order
        // in which the returned collection represents the keys is unspecified, but
        // it is guaranteed to be          buckets = newBuckets; the same order in which a collection returned by
        // GetValues represents the values of the hashtable.
        //
        // The returned collection is live in the sense that any changes
        // to the hash table are reflected in this collection.  It is not
        // a static copy of all the keys in the hash table.
        //
        public virtual ICollection Keys => _keys ?? (_keys = new KeyCollection(this));

        // Returns the object to synchronize on for this hash table.
        public virtual object SyncRoot => this;

        // Returns a collection representing the values of this hashtable. The
        // order in which the returned collection represents the values is
        // unspecified, but it is guaranteed to be the same order in which a
        // collection returned by GetKeys represents the keys of the
        // hashtable.
        //
        // The returned collection is live in the sense that any changes
        // to the hash table are reflected in this collection.  It is not
        // a static copy of all the keys in the hash table.
        //
        public virtual ICollection Values => _values ?? (_values = new ValueCollection(this));

        [Obsolete("Please use KeyComparer properties.")]
        protected IComparer comparer
        {
            get
            {
                if (EqualityComparer is CompatibleComparer compatibleComparer)
                {
                    return compatibleComparer.Comparer;
                }

                if (EqualityComparer == null)
                {
                    return null;
                }
                throw new ArgumentException();
            }
            set
            {
                if (EqualityComparer is CompatibleComparer keyComparer)
                {
                    EqualityComparer = new CompatibleComparer(keyComparer.HashCodeProvider, value);
                }
                else if (EqualityComparer == null)
                {
                    EqualityComparer = new CompatibleComparer(null, value);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        protected IEqualityComparer EqualityComparer { get; private set; }

        [Obsolete("Please use EqualityComparer property.")]
        protected IHashCodeProvider hcp
        {
            get
            {
                if (EqualityComparer is CompatibleComparer compatbileComparer)
                {
                    return compatbileComparer.HashCodeProvider;
                }

                if (EqualityComparer == null)
                {
                    return null;
                }
                throw new ArgumentException();
            }
            set
            {
                if (EqualityComparer is CompatibleComparer keyComparer)
                {
                    EqualityComparer = new CompatibleComparer(value, keyComparer.Comparer);
                }
                else if (EqualityComparer == null)
                {
                    EqualityComparer = new CompatibleComparer(value, null);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        // Returns the value associated with the given key. If an entry with the
        // given key is not found, the returned value is null.
        //
        public virtual object this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Key cannot be null.");
                }

                // Take a snapshot of buckets, in case another thread does a resize
                var buckets = _buckets;
                var hashcode = InitHash(key, buckets.Length, out var seed, out var incr);
                var attempt = 0;

                Bucket b;
                var bucketNumber = (int)(seed % (uint)buckets.Length);
                do
                {
                    //     A read operation on hashtable has three steps:
                    //        (1) calculate the hash and find the slot number.
                    //        (2) compare the hashcode, if equal, go to step 3. Otherwise end.
                    //        (3) compare the key, if equal, go to step 4. Otherwise end.
                    //        (4) return the value contained in the bucket.
                    //     After step 3 and before step 4. A writer can kick in a remove the old item and add a new one
                    //     in the same bucket. So in the reader we need to check if the hash table is modified during above steps.
                    //
                    // Writers (Insert, Remove, Clear) will set 'isWriterInProgress' flag before it starts modifying
                    // the hashtable and will clear the flag when it is done.  When the flag is cleared, the 'version'
                    // will be increased.  We will repeat the reading if a writer is in progress or done with the modification
                    // during the read.
                    //
                    // Our memory model guarantee if we pick up the change in bucket from another processor,
                    // we will see the 'isWriterProgress' flag to be true or 'version' is changed in the reader.
                    //
                    var spin = new SpinWait();
                    while (true)
                    {
                        // this is volatile read, following memory accesses can not be moved ahead of it.
                        var currentVersion = _version;
                        b = buckets[bucketNumber];

                        if (!_isWriterInProgress && currentVersion == _version)
                        {
                            break;
                        }

                        spin.SpinOnce();
                    }

                    if (b.Key == null)
                    {
                        return null;
                    }
                    if ((b.HashColl & 0x7FFFFFFF) == hashcode && KeyEquals(b.Key, key))
                    {
                        return b.Val;
                    }

                    bucketNumber = (int)((bucketNumber + incr) % (uint)buckets.Length);
                } while (b.HashColl < 0 && ++attempt < buckets.Length);
                return null;
            }

            set => Insert(key, value, false);
        }

        // Returns a thread-safe wrapper for a Hashtable.
        //
        public static Hashtable Synchronized(Hashtable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            return new SyncHashtable(table);
        }

        // Adds an entry with the given key and value to this hashtable. An
        // ArgumentException is thrown if the key is null or if the key is already
        // present in the hashtable.
        //
        public virtual void Add(object key, object value)
        {
            Insert(key, value, true);
        }

        // Removes all entries from this hashtable.
        public virtual void Clear()
        {
            Debug.Assert(!_isWriterInProgress, "Race condition detected in usages of Hashtable - multiple threads appear to be writing to a Hashtable instance simultaneously!  Don't do that - use Hashtable.Synchronized.");

            if (_count == 0 && _occupancy == 0)
            {
                return;
            }

            _isWriterInProgress = true;
            for (var i = 0; i < _buckets.Length; i++)
            {
                _buckets[i].HashColl = 0;
                _buckets[i].Key = null;
                _buckets[i].Val = null;
            }

            _count = 0;
            _occupancy = 0;
            UpdateVersion();
            _isWriterInProgress = false;
        }

        // Clone returns a virtually identical copy of this hash table.  This does
        // a shallow copy - the Objects in the table aren't cloned, only the references
        // to those Objects.
        public virtual object Clone()
        {
            var buckets = _buckets;
            var ht = new Hashtable(_count, EqualityComparer) { _version = _version, _loadFactor = _loadFactor, _count = 0 };

            var bucket = buckets.Length;
            while (bucket > 0)
            {
                bucket--;
                var key = buckets[bucket].Key;
                if (key != null && key != buckets)
                {
                    ht[key] = buckets[bucket].Val;
                }
            }

            return ht;
        }

        // Checks if this hashtable contains the given key.
        public virtual bool Contains(object key)
        {
            return ContainsKey(key);
        }

        // Checks if this hashtable contains an entry with the given key.  This is
        // an O(1) operation.
        //
        public virtual bool ContainsKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            // Take a snapshot of buckets, in case another thread resizes table
            var buckets = _buckets;
            var hashcode = InitHash(key, buckets.Length, out var seed, out var incr);
            var attempt = 0;

            Bucket b;
            var bucketNumber = (int)(seed % (uint)buckets.Length);
            do
            {
                b = buckets[bucketNumber];
                if (b.Key == null)
                {
                    return false;
                }
                if ((b.HashColl & 0x7FFFFFFF) == hashcode && KeyEquals(b.Key, key))
                {
                    return true;
                }

                bucketNumber = (int)((bucketNumber + incr) % (uint)buckets.Length);
            } while (b.HashColl < 0 && ++attempt < buckets.Length);
            return false;
        }

        // Checks if this hashtable contains an entry with the given value. The
        // values of the entries of the hashtable are compared to the given value
        // using the Object.Equals method. This method performs a linear
        // search and is thus be substantially slower than the ContainsKey
        // method.
        //
        public virtual bool ContainsValue(object value)
        {
            if (value == null)
            {
                for (var i = _buckets.Length; --i >= 0;)
                {
                    if (_buckets[i].Key != null && _buckets[i].Key != _buckets && _buckets[i].Val == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (var i = _buckets.Length; --i >= 0;)
                {
                    var val = _buckets[i].Val;
                    if (val?.Equals(value) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Copies the values in this hash table to an array at
        // a given index.  Note that this only copies values, and not keys.
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

            CopyEntries(array, arrayIndex);
        }

        // Returns an enumerator for this hashtable.
        // If modifications made to the hashtable while an enumeration is
        // in progress, the MoveNext and Current methods of the
        // enumerator will throw an exception.
        //
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new HashtableEnumerator(this, HashtableEnumerator.DictEntry);
        }

        // Returns a dictionary enumerator for this hashtable.
        // If modifications made to the hashtable while an enumeration is
        // in progress, the MoveNext and Current methods of the
        // enumerator will throw an exception.
        //
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return new HashtableEnumerator(this, HashtableEnumerator.DictEntry);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            // This is imperfect - it only works well if all other writes are
            // also using our synchronized wrapper.  But it's still a good idea.
            lock (SyncRoot)
            {
                // This method hasn't been fully tweaked to be safe for a concurrent writer.
                var oldVersion = _version;
                info.AddValue(_loadFactorName, _loadFactor);
                info.AddValue(_versionName, _version);

                //
                // We need to maintain serialization compatibility with Everett and RTM.
                // If the comparer is null or a compatible comparer, serialize Hashtable
                // in a format that can be deserialized on Everett and RTM.
                //
                // Also, if the Hashtable is using randomized hashing, serialize the old
                // view of the _keyComparer so previous frameworks don't see the new types
                var keyComparerForSerialization = EqualityComparer;

                if (keyComparerForSerialization == null)
                {
                    info.AddValue(_comparerName, null, typeof(IComparer));
                    info.AddValue(_hashCodeProviderName, null, typeof(IHashCodeProvider));
                }
                else if (keyComparerForSerialization is CompatibleComparer c)
                {
                    info.AddValue(_comparerName, c.Comparer, typeof(IComparer));
                    info.AddValue(_hashCodeProviderName, c.HashCodeProvider, typeof(IHashCodeProvider));
                }
                else
                {
                    info.AddValue(_keyComparerName, keyComparerForSerialization, typeof(IEqualityComparer));
                }

                info.AddValue(_hashSizeName, _buckets.Length); //This is the length of the bucket array.
                var serKeys = new object[_count];
                var serValues = new object[_count];
                CopyKeys(serKeys, 0);
                CopyValues(serValues, 0);
                info.AddValue(_keysName, serKeys, typeof(object[]));
                info.AddValue(_valuesName, serValues, typeof(object[]));

                // Explicitly check to see if anyone changed the Hashtable while we
                // were serializing it.  That's a race in their code.
                if (_version != oldVersion)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }
            }
        }

        //
        // DeserializationEvent Listener
        //
        public virtual void OnDeserialization(object sender)
        {
            if (_buckets != null)
            {
                // Somebody had a dependency on this hashtable and fixed us up before the ObjectManager got to it.
                return;
            }

            HashHelpers.SerializationInfoTable.TryGetValue(this, out var siInfo);

            if (siInfo == null)
            {
                throw new SerializationException("OnDeserialization method was called while the object was not being deserialized.");
            }

            var hashsize = 0;
            IComparer c = null;

            IHashCodeProvider hashCodeProvider = null;

            object[] serKeys = null;
            object[] serValues = null;

            var enumerator = siInfo.GetEnumerator();

            while (enumerator.MoveNext())
            {
                switch (enumerator.Name)
                {
                    case _loadFactorName:
                        _loadFactor = siInfo.GetSingle(_loadFactorName);
                        break;

                    case _hashSizeName:
                        hashsize = siInfo.GetInt32(_hashSizeName);
                        break;

                    case _keyComparerName:
                        EqualityComparer = (IEqualityComparer)siInfo.GetValue(_keyComparerName, typeof(IEqualityComparer));
                        break;

                    case _comparerName:
                        c = (IComparer)siInfo.GetValue(_comparerName, typeof(IComparer));
                        break;

                    case _hashCodeProviderName:
                        hashCodeProvider = (IHashCodeProvider)siInfo.GetValue(_hashCodeProviderName, typeof(IHashCodeProvider));
                        break;

                    case _keysName:
                        serKeys = (object[])siInfo.GetValue(_keysName, typeof(object[]));
                        break;

                    case _valuesName:
                        serValues = (object[])siInfo.GetValue(_valuesName, typeof(object[]));
                        break;

                    default:
                        // Unknown, skip
                        break;
                }
            }

            _loadSize = (int)(_loadFactor * hashsize);

            // V1 object doesn't has _keyComparer field.
            if (EqualityComparer == null && (c != null || hashCodeProvider != null))
            {
                EqualityComparer = new CompatibleComparer(hashCodeProvider, c);
            }

            _buckets = new Bucket[hashsize];

            if (serKeys == null)
            {
                throw new SerializationException("The keys for this dictionary are missing.");
            }
            if (serValues == null)
            {
                throw new SerializationException("The values for this dictionary are missing.");
            }
            if (serKeys.Length != serValues.Length)
            {
                throw new SerializationException("Deserialization data is corrupt. The keys and values arrays have different sizes.");
            }
            for (var i = 0; i < serKeys.Length; i++)
            {
                if (serKeys[i] == null)
                {
                    throw new SerializationException();
                }
                Insert(serKeys[i], serValues[i], true);
            }

            _version = siInfo.GetInt32(_versionName);

            HashHelpers.SerializationInfoTable.Remove(this);
        }

        // Removes an entry from this hashtable. If an entry with the specified
        // key exists in the hashtable, it is removed. An ArgumentException is
        // thrown if the key is null.
        //
        public virtual void Remove(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            Debug.Assert(!_isWriterInProgress, "Race condition detected in usages of Hashtable - multiple threads appear to be writing to a Hashtable instance simultaneously!  Don't do that - use Hashtable.Synchronized.");

            // Assuming only one concurrent writer, write directly into buckets.
            var hashcode = InitHash(key, _buckets.Length, out var seed, out var incr);
            var attempt = 0;

            Bucket b;
            var bn = (int)(seed % (uint)_buckets.Length);  // bucketNumber
            do
            {
                b = _buckets[bn];
                if ((b.HashColl & 0x7FFFFFFF) == hashcode && KeyEquals(b.Key, key))
                {
                    _isWriterInProgress = true;
                    // Clear hash_coll field, then key, then value
                    _buckets[bn].HashColl &= unchecked((int)0x80000000);
                    if (_buckets[bn].HashColl != 0)
                    {
                        _buckets[bn].Key = _buckets;
                    }
                    else
                    {
                        _buckets[bn].Key = null;
                    }
                    _buckets[bn].Val = null;  // Free object references sooner & simplify ContainsValue.
                    _count--;
                    UpdateVersion();
                    _isWriterInProgress = false;
                    return;
                }
                bn = (int)((bn + incr) % (uint)_buckets.Length);
            } while (b.HashColl < 0 && ++attempt < _buckets.Length);
        }

        internal virtual KeyValuePairs[] ToKeyValuePairsArray()
        {
            var array = new KeyValuePairs[_count];
            var index = 0;
            var buckets = _buckets;
            for (var i = buckets.Length; --i >= 0;)
            {
                var key = buckets[i].Key;
                if (key != null && key != _buckets)
                {
                    array[index++] = new KeyValuePairs(key, buckets[i].Val);
                }
            }

            return array;
        }

        // Internal method to get the hash code for an Object.  This will call
        // GetHashCode() on each object if you haven't provided an IHashCodeProvider
        // instance.  Otherwise, it calls hcp.GetHashCode(obj).
        protected virtual int GetHash(object key)
        {
            if (EqualityComparer != null)
            {
                return EqualityComparer.GetHashCode(key);
            }

            return key.GetHashCode();
        }

        // Internal method to compare two keys.  If you have provided an IComparer
        // instance in the constructor, this method will call comparer.Compare(item, key).
        // Otherwise, it will call item.Equals(key).
        //
        protected virtual bool KeyEquals(object item, object key)
        {
            Debug.Assert(key != null, "key can't be null here!");
            if (ReferenceEquals(_buckets, item))
            {
                return false;
            }

            if (ReferenceEquals(item, key))
            {
                return true;
            }

            if (EqualityComparer != null)
            {
                return EqualityComparer.Equals(item, key);
            }

            return item?.Equals(key) == true;
        }

        // Copies the keys of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the KeyCollection class.
        private void CopyEntries(Array array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(array.Rank == 1);

            var buckets = _buckets;
            for (var i = buckets.Length; --i >= 0;)
            {
                var key = buckets[i].Key;
                if (key != null && key != _buckets)
                {
                    var entry = new DictionaryEntry(key, buckets[i].Val);
                    array.SetValue(entry, arrayIndex++);
                }
            }
        }

        // Copies the keys of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the KeyCollection class.
        private void CopyKeys(Array array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(array.Rank == 1);

            var buckets = _buckets;
            for (var i = buckets.Length; --i >= 0;)
            {
                var key = buckets[i].Key;
                if (key != null && key != _buckets)
                {
                    array.SetValue(key, arrayIndex++);
                }
            }
        }

        // Copies the values in this Hashtable to an KeyValuePairs array.
        // KeyValuePairs is different from Dictionary Entry in that it has special
        // debugger attributes on its fields.
        // Copies the values of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the ValueCollection class.
        private void CopyValues(Array array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(array.Rank == 1);

            var buckets = _buckets;
            for (var i = buckets.Length; --i >= 0;)
            {
                var key = buckets[i].Key;
                if (key != null && key != _buckets)
                {
                    array.SetValue(buckets[i].Val, arrayIndex++);
                }
            }
        }

        // Increases the bucket count of this hashtable. This method is called from
        // the Insert method when the actual load factor of the hashtable reaches
        // the upper limit specified when the hashtable was constructed. The number
        // of buckets in the hashtable is increased to the smallest prime number
        // that is larger than twice the current number of buckets, and the entries
        // in the hashtable are redistributed into the new buckets using the cached
        // hashcodes.
        private void Expand()
        {
            var rawSize = HashHelpers.ExpandPrime(_buckets.Length);
            Rehash(rawSize);
        }

        // ?InitHash? is basically an implementation of classic DoubleHashing (see http://en.wikipedia.org/wiki/Double_hashing)
        //
        // 1) The only ?correctness? requirement is that the ?increment? used to probe
        //    a. Be non-zero
        //    b. Be relatively prime to the table size ?hashSize?. (This is needed to insure you probe all entries in the table before you ?wrap? and visit entries already probed)
        // 2) Because we choose table sizes to be primes, we just need to insure that the increment is 0 < incr < hashSize
        //
        // Thus this function would work: Incr = 1 + (seed % (hashSize-1))
        //
        // While this works well for ?uniformly distributed? keys, in practice, non-uniformity is common.
        // In particular in practice we can see ?mostly sequential? where you get long clusters of keys that ?pack?.
        // To avoid bad behavior you want it to be the case that the increment is ?large? even for ?small? values (because small
        // values tend to happen more in practice). Thus we multiply ?seed? by a number that will make these small values
        // bigger (and not hurt large values). We picked HashPrime (101) because it was prime, and if ?hashSize-1? is not a multiple of HashPrime
        // (enforced in GetPrime), then incr has the potential of being every value from 1 to hashSize-1. The choice was largely arbitrary.
        //
        // Computes the hash function:  H(key, i) = h1(key) + i*h2(key, hashSize).
        // The out parameter seed is h1(key), while the out parameter
        // incr is h2(key, hashSize).  Callers of this function should
        // add incr each time through a loop.
        private uint InitHash(object key, int hashsize, out uint seed, out uint incr)
        {
            // Hashcode must be positive.  Also, we must not use the sign bit, since
            // that is used for the collision bit.
            var hashcode = (uint)GetHash(key) & 0x7FFFFFFF;
            seed = hashcode;
            // Restriction: incr MUST be between 1 and hashsize - 1, inclusive for
            // the modular arithmetic to work correctly.  This guarantees you'll
            // visit every bucket in the table exactly once within hashsize
            // iterations.  Violate this and it'll cause obscure bugs forever.
            // If you change this calculation for h2(key), update putEntry too!
            incr = 1 + (seed * HashHelpers.HashPrime % ((uint)hashsize - 1));
            return hashcode;
        }

        // Inserts an entry into this hashtable. This method is called from the Set
        // and Add methods. If the add parameter is true and the given key already
        // exists in the hashtable, an exception is thrown.
        private void Insert(object key, object value, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            if (_count >= _loadSize)
            {
                Expand();
            }
            else if (_occupancy > _loadSize && _count > 100)
            {
                Rehash();
            }

            // Assume we only have one thread writing concurrently.  Modify
            // buckets to contain new data, as long as we insert in the right order.
            var hashcode = InitHash(key, _buckets.Length, out var seed, out var incr);
            var attempt = 0;
            var emptySlotNumber = -1; // We use the empty slot number to cache the first empty slot. We chose to reuse slots
            // create by remove that have the collision bit set over using up new slots.
            var bucketNumber = (int)(seed % (uint)_buckets.Length);
            do
            {
                // Set emptySlot number to current bucket if it is the first available bucket that we have seen
                // that once contained an entry and also has had a collision.
                // We need to search this entire collision chain because we have to ensure that there are no
                // duplicate entries in the table.
                if (emptySlotNumber == -1 && _buckets[bucketNumber].Key == _buckets && _buckets[bucketNumber].HashColl < 0)//(((buckets[bucketNumber].hash_coll & unchecked(0x80000000))!=0)))
                {
                    emptySlotNumber = bucketNumber;
                }

                // Insert the key/value pair into this bucket if this bucket is empty and has never contained an entry
                // OR
                // This bucket once contained an entry but there has never been a collision
                if (_buckets[bucketNumber].Key == null || (_buckets[bucketNumber].Key == _buckets && (_buckets[bucketNumber].HashColl & 0x80000000) == 0))
                {
                    // If we have found an available bucket that has never had a collision, but we've seen an available
                    // bucket in the past that has the collision bit set, use the previous bucket instead
                    if (emptySlotNumber != -1) // Reuse slot
                    {
                        bucketNumber = emptySlotNumber;
                    }

                    // We pretty much have to insert in this order.  Don't set hash
                    // code until the value & key are set appropriately.
                    _isWriterInProgress = true;
                    _buckets[bucketNumber].Val = value;
                    _buckets[bucketNumber].Key = key;
                    _buckets[bucketNumber].HashColl |= (int)hashcode;
                    _count++;
                    UpdateVersion();
                    _isWriterInProgress = false;

                    return;
                }

                // The current bucket is in use
                // OR
                // it is available, but has had the collision bit set and we have already found an available bucket
                if ((_buckets[bucketNumber].HashColl & 0x7FFFFFFF) == hashcode && KeyEquals(_buckets[bucketNumber].Key, key))
                {
                    if (add)
                    {
                        throw new ArgumentException();
                    }
                    _isWriterInProgress = true;
                    _buckets[bucketNumber].Val = value;
                    UpdateVersion();
                    _isWriterInProgress = false;

                    return;
                }

                // The current bucket is full, and we have therefore collided.  We need to set the collision bit
                // unless we have remembered an available slot previously.
                if (emptySlotNumber == -1)
                {// We don't need to set the collision bit here since we already have an empty slot
                    if (_buckets[bucketNumber].HashColl >= 0)
                    {
                        _buckets[bucketNumber].HashColl |= unchecked((int)0x80000000);
                        _occupancy++;
                    }
                }

                bucketNumber = (int)((bucketNumber + incr) % (uint)_buckets.Length);
            } while (++attempt < _buckets.Length);

            // This code is here if and only if there were no buckets without a collision bit set in the entire table
            if (emptySlotNumber != -1)
            {
                // We pretty much have to insert in this order.  Don't set hash
                // code until the value & key are set appropriately.
                _isWriterInProgress = true;
                _buckets[emptySlotNumber].Val = value;
                _buckets[emptySlotNumber].Key = key;
                _buckets[emptySlotNumber].HashColl |= (int)hashcode;
                _count++;
                UpdateVersion();
                _isWriterInProgress = false;

                return;
            }

            // If you see this assert, make sure load factor & count are reasonable.
            // Then verify that our double hash function (h2, described at top of file)
            // meets the requirements described above. You should never see this assert.
            Debug.Assert(false, "hash table insert failed!  Load factor too high, or our double hashing function is incorrect.");
            throw new InvalidOperationException();
        }

        private void PutEntry(Bucket[] newBuckets, object key, object value, int hashcode)
        {
            Debug.Assert(hashcode >= 0, "hashcode >= 0");  // make sure collision bit (sign bit) wasn't set.

            var seed = (uint)hashcode;
            var incr = unchecked(1 + (seed * HashHelpers.HashPrime % ((uint)newBuckets.Length - 1)));
            var bucketNumber = (int)(seed % (uint)newBuckets.Length);
            while (true)
            {
                if (newBuckets[bucketNumber].Key == null || newBuckets[bucketNumber].Key == _buckets)
                {
                    newBuckets[bucketNumber].Val = value;
                    newBuckets[bucketNumber].Key = key;
                    newBuckets[bucketNumber].HashColl |= hashcode;
                    return;
                }

                if (newBuckets[bucketNumber].HashColl >= 0)
                {
                    newBuckets[bucketNumber].HashColl |= unchecked((int)0x80000000);
                    _occupancy++;
                }
                bucketNumber = (int)((bucketNumber + incr) % (uint)newBuckets.Length);
            }
        }

        // We occasionally need to rehash the table to clean up the collision bits.
        private void Rehash()
        {
            Rehash(_buckets.Length);
        }

        private void Rehash(int newSize)
        {
            // reset occupancy
            _occupancy = 0;

            // Don't replace any internal state until we've finished adding to the
            // new bucket[].  This serves two purposes:
            //   1) Allow concurrent readers to see valid hashtable contents
            //      at all times
            //   2) Protect against an OutOfMemoryException while allocating this
            //      new bucket[].
            var newBuckets = new Bucket[newSize];

            // rehash table into new buckets
            int index;
            for (index = 0; index < _buckets.Length; index++)
            {
                var old = _buckets[index];
                if (old.Key != null && old.Key != _buckets)
                {
                    var hashcode = old.HashColl & 0x7FFFFFFF;
                    PutEntry(newBuckets, old.Key, old.Val, hashcode);
                }
            }

            // New bucket[] is good to go - replace buckets and other internal state.
            _isWriterInProgress = true;
            _buckets = newBuckets;
            _loadSize = (int)(_loadFactor * newSize);
            UpdateVersion();
            _isWriterInProgress = false;
            // minimum size of hashtable is 3 now and maximum loadFactor is 0.72 now.
            Debug.Assert(_loadSize < newSize, "Our current implementation means this is not possible.");
        }

        private void UpdateVersion()
        {
            // Version might become negative when version is int.MaxValue, but the oddity will be still be correct.
            // So we don't need to special case this.
            _version++;
        }

        // The hash table data.
        // This cannot be serialized
        private struct Bucket
        {
            public int HashColl;
            public object Key;
            public object Val;
            // Store hash code; sign bit means there was a collision.
        }
        // internal debug view class for hashtable
        internal class HashtableDebugView
        {
            private readonly Hashtable _hashtable;

            public HashtableDebugView(Hashtable hashtable)
            {
                _hashtable = hashtable ?? throw new ArgumentNullException(nameof(hashtable));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePairs[] Items => _hashtable.ToKeyValuePairsArray();
        }

        // Implements an enumerator for a hashtable. The enumerator uses the
        // internal version number of the hashtable to ensure that no modifications
        // are made to the hashtable while an enumeration is in progress.
        private sealed class HashtableEnumerator : IDictionaryEnumerator, ICloneable
        {
            internal const int DictEntry = 3;
            internal const int Keys = 1;
            internal const int Values = 2;
            private readonly int _getObjectRetType;
            private readonly Hashtable _hashtable;
            private readonly int _version;
            private int _bucket;
            private bool _current;
            // What should GetObject return?
            private object _currentKey;
            private object _currentValue;

            internal HashtableEnumerator(Hashtable hashtable, int getObjRetType)
            {
                _hashtable = hashtable;
                _bucket = hashtable._buckets.Length;
                _version = hashtable._version;
                _current = false;
                _getObjectRetType = getObjRetType;
            }

            public object Current
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    if (_getObjectRetType == Keys)
                    {
                        return _currentKey;
                    }

                    if (_getObjectRetType == Values)
                    {
                        return _currentValue;
                    }

                    return new DictionaryEntry(_currentKey, _currentValue);
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return new DictionaryEntry(_currentKey, _currentValue);
                }
            }

            public object Key
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    }

                    return _currentKey;
                }
            }

            public object Value
            {
                get
                {
                    if (!_current)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }

                    return _currentValue;
                }
            }

            public object Clone() => MemberwiseClone();

            public bool MoveNext()
            {
                if (_version != _hashtable._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                while (_bucket > 0)
                {
                    _bucket--;
                    var key = _hashtable._buckets[_bucket].Key;
                    if (key != null && key != _hashtable._buckets)
                    {
                        _currentKey = key;
                        _currentValue = _hashtable._buckets[_bucket].Val;
                        _current = true;
                        return true;
                    }
                }
                _current = false;
                return false;
            }

            public void Reset()
            {
                if (_version != _hashtable._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                _current = false;
                _bucket = _hashtable._buckets.Length;
                _currentKey = null;
                _currentValue = null;
            }
        }

        // Implements a Collection for the keys of a hashtable. An instance of this
        // class is created by the GetKeys method of a hashtable.
        private sealed class KeyCollection : ICollection
        {
            private readonly Hashtable _hashtable;

            internal KeyCollection(Hashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public int Count => _hashtable._count;

            public bool IsSynchronized => _hashtable.IsSynchronized;

            public object SyncRoot => _hashtable.SyncRoot;

            public void CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
                }

                if (array.Length - index < _hashtable._count)
                {
                    throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
                }

                _hashtable.CopyKeys(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return new HashtableEnumerator(_hashtable, HashtableEnumerator.Keys);
            }
        }

        // Synchronized wrapper for hashtable
        private class SyncHashtable : Hashtable, IEnumerable
        {
            private readonly Hashtable _table;

            internal SyncHashtable(Hashtable table) : base(false)
            {
                _table = table;
            }

            public override int Count => _table.Count;

            public override bool IsFixedSize => _table.IsFixedSize;

            public override bool IsReadOnly => _table.IsReadOnly;

            public override bool IsSynchronized => true;

            public override ICollection Keys
            {
                get
                {
                    lock (_table.SyncRoot)
                    {
                        return _table.Keys;
                    }
                }
            }

            public override object SyncRoot => _table.SyncRoot;

            public override ICollection Values
            {
                get
                {
                    lock (_table.SyncRoot)
                    {
                        return _table.Values;
                    }
                }
            }

            public override object this[object key]
            {
                get => _table[key];
                set
                {
                    lock (_table.SyncRoot)
                    {
                        _table[key] = value;
                    }
                }
            }

            public override void Add(object key, object value)
            {
                lock (_table.SyncRoot)
                {
                    _table.Add(key, value);
                }
            }

            public override void Clear()
            {
                lock (_table.SyncRoot)
                {
                    _table.Clear();
                }
            }

            public override object Clone()
            {
                lock (_table.SyncRoot)
                {
                    return Synchronized((Hashtable)_table.Clone());
                }
            }

            public override bool Contains(object key)
            {
                return _table.Contains(key);
            }

            public override bool ContainsKey(object key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Key cannot be null.");
                }
                return _table.ContainsKey(key);
            }

            public override bool ContainsValue(object value)
            {
                lock (_table.SyncRoot)
                {
                    return _table.ContainsValue(value);
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (_table.SyncRoot)
                {
                    _table.CopyTo(array, arrayIndex);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _table.GetEnumerator();
            }

            public override IDictionaryEnumerator GetEnumerator()
            {
                return _table.GetEnumerator();
            }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new PlatformNotSupportedException();
            }

            public override void OnDeserialization(object sender)
            {
                // Does nothing.  We have to implement this because our parent HT implements it,
                // but it doesn't do anything meaningful.  The real work will be done when we
                // call OnDeserialization on our parent table.
            }

            public override void Remove(object key)
            {
                lock (_table.SyncRoot)
                {
                    _table.Remove(key);
                }
            }

            internal override KeyValuePairs[] ToKeyValuePairsArray()
            {
                return _table.ToKeyValuePairsArray();
            }
        }

        // Implements a Collection for the values of a hashtable. An instance of
        // this class is created by the GetValues method of a hashtable.
        private sealed class ValueCollection : ICollection
        {
            private readonly Hashtable _hashtable;

            internal ValueCollection(Hashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public int Count => _hashtable._count;

            public bool IsSynchronized => _hashtable.IsSynchronized;

            public object SyncRoot => _hashtable.SyncRoot;

            public void CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
                }

                if (array.Length - index < _hashtable._count)
                {
                    throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
                }

                _hashtable.CopyValues(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return new HashtableEnumerator(_hashtable, HashtableEnumerator.Values);
            }
        }
    }

    internal static class HashHelpers
    {
        public const int HashCollisionThreshold = 100;

        public const int HashPrime = 101;

        // This is the maximum prime smaller than Array.MaxArrayLength
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;
        // Table of prime numbers to use as hash table sizes.
        // A typical resize algorithm would pick the smallest prime number in this array
        // that is larger than twice the previous capacity.
        // Suppose our Hashtable currently has capacity x and enough elements are added
        // such that a resize needs to occur. Resizing first computes 2x then finds the
        // first prime in the table greater than 2x, i.e. if primes are ordered
        // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n.
        // Doubling is important for preserving the asymptotic complexity of the
        // hashtable operations such as add.  Having a prime guarantees that double
        // hashing does not lead to infinite loops.  IE, your hash function will be
        // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
        // We prefer the low computation costs of higher prime numbers over the increased
        // memory allocation of a fixed prime number i.e. when right sizing a HashSet.
        public static readonly int[] Primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369 };

        private static ConditionalWeakTable<object, SerializationInfo> _serializationInfoTable;

        public static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
        {
            get
            {
                if (_serializationInfoTable == null)
                {
                    Interlocked.CompareExchange(ref _serializationInfoTable, new ConditionalWeakTable<object, SerializationInfo>(), null);
                }

                return _serializationInfoTable;
            }
        }

        // Returns size of hashtable to grow to.
        public static int ExpandPrime(int oldSize)
        {
            var newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encountering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                Debug.Assert(MaxPrimeArrayLength == GetPrime(MaxPrimeArrayLength), "Invalid MaxPrimeArrayLength");
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }

        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException("Hashtable's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.");
            }

            foreach (var prime in Primes)
            {
                if (prime >= min)
                {
                    return prime;
                }
            }

            //outside of our predefined table.
            //compute the hard way.
            for (var i = min | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && (i - 1) % HashPrime != 0)
                {
                    return i;
                }
            }
            return min;
        }

        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                var limit = (int)Math.Sqrt(candidate);
                for (var divisor = 3; divisor <= limit; divisor += 2)
                {
                    if (candidate % divisor == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return candidate == 2;
        }
    }

    internal sealed class CompatibleComparer : IEqualityComparer
    {
        internal CompatibleComparer(IHashCodeProvider hashCodeProvider, IComparer comparer)
        {
            HashCodeProvider = hashCodeProvider;
            Comparer = comparer;
        }

        internal IComparer Comparer { get; }
        internal IHashCodeProvider HashCodeProvider { get; }

        public int Compare(object a, object b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a == null)
            {
                return -1;
            }

            if (b == null)
            {
                return 1;
            }

            if (Comparer != null)
            {
                return Comparer.Compare(a, b);
            }

            if (a is IComparable ia)
            {
                return ia.CompareTo(b);
            }

            throw new ArgumentException("At least one object must implement IComparable.");
        }

        public new bool Equals(object x, object y) => Compare(x, y) == 0;

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return HashCodeProvider != null ?
                HashCodeProvider.GetHashCode(obj) :
                obj.GetHashCode();
        }
    }

    [DebuggerDisplay("{_value}", Name = "[{_key}]")]
    internal class KeyValuePairs
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _key;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly object _value;

        public KeyValuePairs(object key, object value)
        {
            _value = value;
            _key = key;
        }
    }
}

#endif