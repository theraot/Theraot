﻿#if LESSTHAN_NET40 || NETSTANDARD1_0

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable S2743 // Static fields in generic types
#pragma warning disable MA0015 // Specify the parameter name in ArgumentException

// ReSharper disable InvertIf
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable StaticMemberInGenericType
// ReSharper disable MergeConditionalExpression
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable JoinNullCheckWithUsage
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

#if LESSTHAN_NET40

using System.Runtime.Serialization;
using System.Security.Permissions;

#endif

using System.Threading;

namespace System.Collections.Concurrent
{
    /// <summary>Represents a thread-safe collection of keys and values.</summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// All public and protected members of <see cref="ConcurrentDictionary{TKey,TValue}"/> are thread-safe and may be used
    /// concurrently from multiple threads.
    /// </remarks>
    [DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public
#if LESSTHAN_NET40
        partial
#endif
        class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        /// <summary>The default capacity, i.e. the initial # of buckets.</summary>
        /// <remarks>
        /// When choosing this value, we are making a trade-off between the size of a very small dictionary,
        /// and the number of resizes when constructing a large dictionary. Also, the capacity should not be
        /// divisible by a small prime.
        /// </remarks>
        private const int DefaultCapacity = 31;

        /// <summary>
        /// The maximum size of the striped lock that will not be exceeded when locks are automatically
        /// added as the dictionary grows.
        /// </summary>
        /// <remarks>
        /// The user is allowed to exceed this limit by passing
        /// a concurrency level larger than MaxLockNumber into the constructor.
        /// </remarks>
        private const int MaxLockNumber = 1024;

        /// <summary>Whether TValue is a type that can be written atomically (i.e., with no danger of torn reads).</summary>
        private static readonly bool _isValueWriteAtomic = IsValueWriteAtomic();

        /// <summary>Key equality comparer.</summary>
        private readonly IEqualityComparer<TKey>? _comparer;

        /// <summary>Default comparer for TKey.</summary>
        /// <remarks>
        /// Used to avoid repeatedly accessing the shared default generic static, in particular for reference types where it's
        /// currently not devirtualized: https://github.com/dotnet/runtime/issues/10050.
        /// </remarks>
        private readonly EqualityComparer<TKey> _defaultComparer;

        /// <summary>Whether to dynamically increase the size of the striped lock.</summary>
        [NonSerialized]
        private readonly bool _growLockArray;

        /// <summary>The maximum number of elements per lock before a resize operation is triggered.</summary>
        [NonSerialized]
        private int _budget;

        /// <summary>Internal tables of the dictionary.</summary>
        [NonSerialized]
        private volatile Tables _tables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the default concurrency level, has the default initial capacity, and
        /// uses the default comparer for the key type.
        /// </summary>
        public ConcurrentDictionary() : this(DefaultConcurrencyLevel, DefaultCapacity, growLockArray: true, comparer: null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the specified concurrency level and capacity, and uses the default
        /// comparer for the key type.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the
        /// <see cref="ConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
        /// <param name="capacity">The initial number of elements that the <see cref="ConcurrentDictionary{TKey,TValue}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity"/> is less than 0.</exception>
        public ConcurrentDictionary(int concurrencyLevel, int capacity) : this(concurrencyLevel, capacity, growLockArray: false, comparer: null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that contains elements copied from the specified <see cref="IEnumerable{T}"/>, has the default concurrency
        /// level, has the default initial capacity, and uses the default comparer for the key type.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="ConcurrentDictionary{TKey,TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, comparer: null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the specified concurrency level and capacity, and uses the specified
        /// <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey>? comparer) : this(DefaultConcurrencyLevel, DefaultCapacity, growLockArray: true, comparer) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that contains elements copied from the specified <see cref="IEnumerable"/>, has the default concurrency
        /// level, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="ConcurrentDictionary{TKey,TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is a null reference (Nothing in Visual Basic).</exception>
        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
            : this(comparer)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            InitializeFromCollection(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that contains elements copied from the specified <see cref="IEnumerable"/>,
        /// has the specified concurrency level, has the specified initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the <see cref="ConcurrentDictionary{TKey,TValue}"/> concurrently.
        /// </param>
        /// <param name="collection">The <see cref="IEnumerable{T}"/> whose elements are copied to the new
        /// <see cref="ConcurrentDictionary{TKey,TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than 1.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
            : this(concurrencyLevel, DefaultCapacity, growLockArray: false, comparer)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            InitializeFromCollection(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the specified concurrency level, has the specified initial capacity, and
        /// uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="ConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
        /// <param name="capacity">The initial number of elements that the <see cref="ConcurrentDictionary{TKey,TValue}"/> can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than 1. -or- <paramref name="capacity"/> is less than 0.</exception>
        public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey>? comparer)
            : this(concurrencyLevel, capacity, growLockArray: false, comparer)
        {
        }

        internal ConcurrentDictionary(int concurrencyLevel, int capacity, bool growLockArray, IEqualityComparer<TKey>? comparer)
        {
            if (concurrencyLevel < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(concurrencyLevel), "ConcurrencyLevelMustBePositive");
            }

            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "CapacityMustNotBeNegative");
            }

            // The capacity should be at least as large as the concurrency level. Otherwise, we would have locks that don't guard
            // any buckets.
            if (capacity < concurrencyLevel)
            {
                capacity = concurrencyLevel;
            }

            var locks = new object[concurrencyLevel];
            locks[0] = locks; // reuse array as the first lock object just to avoid an additional allocation
            for (var i = 1; i < locks.Length; i++)
            {
                locks[i] = new object();
            }

            var countPerLock = new int[locks.Length];
            var buckets = new Node[capacity];
            _tables = new Tables(buckets, locks, countPerLock);

            _defaultComparer = EqualityComparer<TKey>.Default;
            if (comparer != null &&
                !ReferenceEquals(comparer, _defaultComparer) && // if this is the default comparer, take the optimized path
                !ReferenceEquals(comparer, StringComparer.Ordinal)) // strings as keys are extremely common, so special-case StringComparer.Ordinal, which is the same as the default comparer
            {
                _comparer = comparer;
            }

            _growLockArray = growLockArray;
            _budget = buckets.Length / locks.Length;
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="ConcurrentDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <value>The number of key/value pairs contained in the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</value>
        /// <remarks>Count has snapshot semantics and represents the number of items in the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// at the moment when Count was accessed.</remarks>
        public int Count
        {
            get
            {
                var acquiredLocks = 0;
                try
                {
                    // Acquire all locks
                    AcquireAllLocks(ref acquiredLocks);

                    return GetCountInternal();
                }
                finally
                {
                    // Release locks that have been acquired earlier
                    ReleaseLocks(0, acquiredLocks);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="ConcurrentDictionary{TKey,TValue}"/> is empty.
        /// </summary>
        /// <value>true if the <see cref="ConcurrentDictionary{TKey,TValue}"/> is empty; otherwise,
        /// false.</value>
        public bool IsEmpty
        {
            get
            {
                // Check if any buckets are non-empty, without acquiring any locks.
                // This fast path should generally suffice as collections are usually not empty.
                if (!AreAllBucketsEmpty())
                {
                    return false;
                }

                // We didn't see any buckets containing items, however we can't be sure
                // the collection was actually empty at any point in time as items may have been
                // added and removed while iterating over the buckets such that we never saw an
                // empty bucket, but there was always an item present in at least one bucket.
                var acquiredLocks = 0;
                try
                {
                    // Acquire all locks
                    AcquireAllLocks(ref acquiredLocks);

                    return AreAllBucketsEmpty();
                }
                finally
                {
                    // Release locks that have been acquired earlier
                    ReleaseLocks(0, acquiredLocks);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> has a fixed size.
        /// </summary>
        /// <value>true if the <see cref="IDictionary"/> has a
        /// fixed size; otherwise, false. For <see cref="ConcurrentDictionary{TKey,TValue}"/>, this property always
        /// returns false.</value>
        bool IDictionary.IsFixedSize => false;

        /// <summary>
        /// Gets a value indicating whether the dictionary is read-only.
        /// </summary>
        /// <value>true if the <see cref="ICollection{T}"/> is
        /// read-only; otherwise, false. For <see cref="Dictionary{TKey,TValue}"/>, this property always returns
        /// false.</value>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> is read-only.
        /// </summary>
        /// <value>true if the <see cref="IDictionary"/> is
        /// read-only; otherwise, false. For <see cref="ConcurrentDictionary{TKey,TValue}"/>, this property always
        /// returns false.</value>
        bool IDictionary.IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection"/> is
        /// synchronized with the SyncRoot.
        /// </summary>
        /// <value>true if access to the <see cref="ICollection"/> is synchronized
        /// (thread safe); otherwise, false. For <see cref="ConcurrentDictionary{TKey,TValue}"/>, this property always
        /// returns false.</value>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        /// <value>An <see cref="ICollection{TKey}"/> containing the keys in the
        /// <see cref="Dictionary{TKey,TValue}"/>.</value>
        public ICollection<TKey> Keys => GetKeys();

        /// <summary>
        /// Gets an <see cref="IEnumerable{TKey}"/> containing the keys of
        /// the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <value>An <see cref="IEnumerable{TKey}"/> containing the keys of
        /// the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</value>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => GetKeys();

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the keys of the <see cref="IDictionary"/>.
        /// </summary>
        /// <value>An <see cref="ICollection"/> containing the keys of the <see cref="IDictionary"/>.</value>
        ICollection IDictionary.Keys => GetKeys();

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ICollection"/>. This property is not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">The SyncRoot property is not supported.</exception>
        object ICollection.SyncRoot => throw new NotSupportedException("SyncRoot_NotSupported");

        /// <summary>
        /// Gets a collection containing the values in the <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        /// <value>An <see cref="ICollection{TValue}"/> containing the values in
        /// the
        /// <see cref="Dictionary{TKey,TValue}"/>.</value>
        public ICollection<TValue> Values => GetValues();

        /// <summary>
        /// Gets an <see cref="IEnumerable{TValue}"/> containing the values
        /// in the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <value>An <see cref="IEnumerable{TValue}"/> containing the
        /// values in the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</value>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => GetValues();

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the values in the <see cref="IDictionary"/>.
        /// </summary>
        /// <value>An <see cref="ICollection"/> containing the values in the <see cref="IDictionary"/>.</value>
        ICollection IDictionary.Values => GetValues();

        /// <summary>The number of concurrent writes for which to optimize by default.</summary>
        private static int DefaultConcurrencyLevel => Environment.ProcessorCount;

        /// <summary>Gets or sets the value associated with the specified key.</summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>
        /// The value associated with the specified key. If the specified key is not found, a get operation throws a
        /// <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and <paramref name="key"/> does not exist in the collection.
        /// </exception>
        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                {
                    ThrowKeyNotFoundException(key);
                }

                return value;
            }
            set
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                TryAddInternal(key, nullableHashcode: null, value, updateIfExists: true, acquireLock: true, out _);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>The value associated with the specified key, or a null reference (Nothing in Visual Basic)
        /// if <paramref name="key"/> is not in the dictionary or <paramref name="key"/> is of a type that is
        /// not assignable to the key type <typeparamref name="TKey"/> of the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</value>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException">
        /// A value is being assigned, and <paramref name="key"/> is of a type that is not assignable to the
        /// key type <typeparamref name="TKey"/> of the <see cref="ConcurrentDictionary{TKey,TValue}"/>. -or- A value is being
        /// assigned, and <paramref name="key"/> is of a type that is not assignable to the value type
        /// <typeparamref name="TValue"/> of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// </exception>
        object? IDictionary.this[object key]
        {
            get
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (key is TKey tkey && TryGetValue(tkey, out var value))
                {
                    return value;
                }

                return null;
            }
            set
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (!(key is TKey))
                {
                    throw new ArgumentException("TypeOfKeyIncorrect");
                }

                ThrowIfInvalidObjectValue(value);

                this[(TKey)key] = (TValue)value!;
            }
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</exception>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
            {
                throw new ArgumentException("KeyAlreadyExisted");
            }
        }

        /// <summary>
        /// Adds the specified value to the <see cref="ICollection{TValue}"/>
        /// with the specified key.
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/>
        /// structure representing the key and value to add to the <see cref="Dictionary{TKey,TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="item"/> of <paramref name="item"/> is null.</exception>
        /// <exception cref="OverflowException">The <see cref="Dictionary{TKey,TValue}"/>
        /// contains too many elements.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the
        /// <see cref="Dictionary{TKey,TValue}"/></exception>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)this).Add(item.Key, item.Value);

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The object to use as the key.</param>
        /// <param name="value">The object to use as the value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is of a type that is not assignable to the key type <typeparamref name="TKey"/> of the <see cref="Dictionary{TKey,TValue}"/>. -or-
        /// <paramref name="value"/> is of a type that is not assignable to <typeparamref name="TValue"/>,
        /// the type of values in the <see cref="Dictionary{TKey,TValue}"/>.
        /// -or- A value with the same key already exists in the <see cref="Dictionary{TKey,TValue}"/>.
        /// </exception>
        void IDictionary.Add(object key, object? value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!(key is TKey))
            {
                throw new ArgumentException("TypeOfKeyIncorrect");
            }

            ThrowIfInvalidObjectValue(value);

            ((IDictionary<TKey, TValue>)this).Add((TKey)key, (TValue)value!);
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key does not already
        /// exist, or updates a key/value pair in the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key
        /// already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key
        /// based on the key's existing value</param>
        /// <param name="factoryArgument">An argument to pass into <paramref name="addValueFactory"/> and <paramref name="updateValueFactory"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="addValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="updateValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The new value for the key.  This will be either be the result of addValueFactory (if the key was
        /// absent) or the result of updateValueFactory (if the key was present).</returns>
        public TValue AddOrUpdate<TArg>(
            TKey key, Func<TKey, TArg, TValue> addValueFactory, Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (addValueFactory is null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }

            if (updateValueFactory is null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);

            while (true)
            {
                if (TryGetValueInternal(key, hashcode, out var oldValue))
                {
                    // key exists, try to update
                    var newValue = updateValueFactory(key, oldValue, factoryArgument);
                    if (TryUpdateInternal(key, hashcode, newValue, oldValue))
                    {
                        return newValue;
                    }
                }
                else
                {
                    // key doesn't exist, try to add
                    if (TryAddInternal(key, hashcode, addValueFactory(key, factoryArgument), updateIfExists: false, acquireLock: true, out var resultingValue))
                    {
                        return resultingValue;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key does not already
        /// exist, or updates a key/value pair in the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key
        /// already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key
        /// based on the key's existing value</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="addValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="updateValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The new value for the key.  This will be either the result of addValueFactory (if the key was
        /// absent) or the result of updateValueFactory (if the key was present).</returns>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (addValueFactory is null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }

            if (updateValueFactory is null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);

            while (true)
            {
                if (TryGetValueInternal(key, hashcode, out var oldValue))
                {
                    // key exists, try to update
                    var newValue = updateValueFactory(key, oldValue);
                    if (TryUpdateInternal(key, hashcode, newValue, oldValue))
                    {
                        return newValue;
                    }
                }
                else
                {
                    // key doesn't exist, try to add
                    if (TryAddInternal(key, hashcode, addValueFactory(key), updateIfExists: false, acquireLock: true, out var resultingValue))
                    {
                        return resultingValue;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key does not already
        /// exist, or updates a key/value pair in the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key
        /// already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on
        /// the key's existing value</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="updateValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The new value for the key.  This will be either the value of addValue (if the key was
        /// absent) or the result of updateValueFactory (if the key was present).</returns>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (updateValueFactory is null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);

            while (true)
            {
                if (TryGetValueInternal(key, hashcode, out var oldValue))
                {
                    // key exists, try to update
                    var newValue = updateValueFactory(key, oldValue);
                    if (TryUpdateInternal(key, hashcode, newValue, oldValue))
                    {
                        return newValue;
                    }
                }
                else
                {
                    // key doesn't exist, try to add
                    if (TryAddInternal(key, hashcode, addValue, updateIfExists: false, acquireLock: true, out var resultingValue))
                    {
                        return resultingValue;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="ConcurrentDictionary{TKey,TValue}"/>.
        /// </summary>
        public void Clear()
        {
            var locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);

                // If the dictionary is already empty, then there's nothing to clear.
                if (AreAllBucketsEmpty())
                {
                    return;
                }

                var tables = _tables;
                var newTables = new Tables(new Node[DefaultCapacity], tables._locks, new int[tables._countPerLock.Length]);
                _tables = newTables;
                _budget = Math.Max(1, newTables._buckets.Length / newTables._locks.Length);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/>
        /// contains a specific key and value.
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/>
        /// structure to locate in the <see cref="ICollection{TValue}"/>.</param>
        /// <returns>true if the <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, false.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!TryGetValue(item.Key, out var value))
            {
                return false;
            }

            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <summary>
        /// Gets whether the <see cref="IDictionary"/> contains an
        /// element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary"/>.</param>
        /// <returns>true if the <see cref="IDictionary"/> contains
        /// an element with the specified key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"> <paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        bool IDictionary.Contains(object key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return key is TKey tkey && ContainsKey(tkey);
        }

        /// <summary>
        /// Determines whether the <see cref="ConcurrentDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="ConcurrentDictionary{TKey, TValue}"/>.</param>
        /// <returns>true if the <see cref="ConcurrentDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference (Nothing in Visual Basic).</exception>
        public bool ContainsKey(TKey key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return TryGetValue(key, out _);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an array of type <see cref="KeyValuePair{TKey,TValue}"/>,
        /// starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array of type <see cref="KeyValuePair{TKey,TValue}"/> that is the destination of the <see cref="KeyValuePair{TKey,TValue}"/> elements copied from the <see  cref="ICollection"/>. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of the <paramref name="array"/>. -or- The number of
        /// elements in the source <see cref="ICollection"/> is greater than the available space from <paramref name="arrayIndex"/> to
        /// the end of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "IndexIsNegative");
            }

            var locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);

                var count = 0;
                var countPerLock = _tables._countPerLock;
                for (var i = 0; i < countPerLock.Length && count >= 0; i++)
                {
                    count += countPerLock[i];
                }

                if (array.Length - count < arrayIndex || count < 0) //"count" itself or "count + index" can overflow
                {
                    throw new ArgumentException("ArrayNotLargeEnough");
                }

                CopyToPairs(array, arrayIndex);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an array, starting
        /// at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from
        /// the <see cref="ICollection"/>. The array must have zero-based
        /// indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying
        /// begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than
        /// 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> is equal to or greater than
        /// the length of the <paramref name="array"/>. -or- The number of elements in the source <see cref="ICollection"/>
        /// is greater than the available space from <paramref name="index"/> to the end of the destination
        /// <paramref name="array"/>.</exception>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "IndexIsNegative");
            }

            var locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);
                var tables = _tables;

                var count = 0;
                var countPerLock = tables._countPerLock;
                for (var i = 0; i < countPerLock.Length && count >= 0; i++)
                {
                    count += countPerLock[i];
                }

                if (array.Length - count < index || count < 0) //"count" itself or "count + index" can overflow
                {
                    throw new ArgumentException("ArrayNotLargeEnough");
                }

                // To be consistent with the behavior of ICollection.CopyTo() in Dictionary<TKey,TValue>,
                // we recognize three types of target arrays:
                //    - an array of KeyValuePair<TKey, TValue> structs
                //    - an array of DictionaryEntry structs
                //    - an array of objects

                if (array is KeyValuePair<TKey, TValue>[] pairs)
                {
                    CopyToPairs(pairs, index);
                    return;
                }

                if (array is DictionaryEntry[] entries)
                {
                    CopyToEntries(entries, index);
                    return;
                }

                if (array is object[] objects)
                {
                    CopyToObjects(objects, index);
                    return;
                }

                throw new ArgumentException("ArrayIncorrectType", nameof(array));
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</summary>
        /// <returns>An enumerator for the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</returns>
        /// <remarks>
        /// The enumerator returned from the dictionary is safe to use concurrently with
        /// reads and writes to the dictionary, however it does not represent a moment-in-time snapshot
        /// of the dictionary.  The contents exposed through the enumerator may contain modifications
        /// made to the dictionary after <see cref="GetEnumerator"/> was called.
        /// </remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

        /// <summary>Returns an enumerator that iterates through the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</summary>
        /// <returns>An enumerator for the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</returns>
        /// <remarks>
        /// The enumerator returned from the dictionary is safe to use concurrently with
        /// reads and writes to the dictionary, however it does not represent a moment-in-time snapshot
        /// of the dictionary.  The contents exposed through the enumerator may contain modifications
        /// made to the dictionary after <see cref="GetEnumerator"/> was called.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Provides an <see cref="IDictionaryEnumerator"/> for the
        /// <see cref="IDictionary"/>.</summary>
        /// <returns>An <see cref="IDictionaryEnumerator"/> for the <see cref="IDictionary"/>.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(this);

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The value for the key.  This will be either the existing value for the key if the
        /// key is already in the dictionary, or the new value for the key as returned by valueFactory
        /// if the key was not in the dictionary.</returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (valueFactory is null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);

            if (!TryGetValueInternal(key, hashcode, out var resultingValue))
            {
                TryAddInternal(key, hashcode, valueFactory(key), updateIfExists: false, acquireLock: true, out resultingValue);
            }

            return resultingValue;
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <param name="factoryArgument">An argument value to pass into <paramref name="valueFactory"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The value for the key.  This will be either the existing value for the key if the
        /// key is already in the dictionary, or the new value for the key as returned by valueFactory
        /// if the key was not in the dictionary.</returns>
        public TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (valueFactory is null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);

            if (!TryGetValueInternal(key, hashcode, out var resultingValue))
            {
                TryAddInternal(key, hashcode, valueFactory(key, factoryArgument), updateIfExists: false, acquireLock: true, out resultingValue);
            }

            return resultingValue;
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">the value to be added, if the key does not already exist</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The value for the key.  This will be either the existing value for the key if the
        /// key is already in the dictionary, or the new value if the key was not in the dictionary.</returns>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);

            if (!TryGetValueInternal(key, hashcode, out var resultingValue))
            {
                TryAddInternal(key, hashcode, value, updateIfExists: false, acquireLock: true, out resultingValue);
            }

            return resultingValue;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully remove; otherwise false. This method also returns
        /// false if
        /// <paramref name="key"/> was not found in the original <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        bool IDictionary<TKey, TValue>.Remove(TKey key) => TryRemove(key, out _);

        /// <summary>
        /// Removes a key and value from the dictionary.
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/>
        /// structure representing the key and value to remove from the <see cref="Dictionary{TKey,TValue}"/>.</param>
        /// <returns>true if the key and value represented by <paramref name="item"/> is successfully
        /// found and removed; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">The Key property of <paramref name="item"/> is a null reference (Nothing in Visual Basic).</exception>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) =>
            TryRemove(item);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        void IDictionary.Remove(object key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key is TKey tkey)
            {
                TryRemove(tkey, out _);
            }
        }

        /// <summary>
        /// Copies the key and value pairs stored in the <see cref="ConcurrentDictionary{TKey,TValue}"/> to a
        /// new array.
        /// </summary>
        /// <returns>A new array containing a snapshot of key and value pairs copied from the <see cref="ConcurrentDictionary{TKey,TValue}"/>.
        /// </returns>
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            var locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);

                var count = 0;
                var countPerLock = _tables._countPerLock;
                for (var i = 0; i < countPerLock.Length; i++)
                {
                    checked
                    {
                        count += countPerLock[i];
                    }
                }

                if (count == 0)
                {
                    return ArrayEx.Empty<KeyValuePair<TKey, TValue>>();
                }

                var array = new KeyValuePair<TKey, TValue>[count];
                CopyToPairs(array, 0);
                return array;
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        /// <summary>
        /// Attempts to add the specified key and value to the <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be a null reference (Nothing
        /// in Visual Basic) for reference types.</param>
        /// <returns>
        /// true if the key/value pair was added to the <see cref="ConcurrentDictionary{TKey, TValue}"/> successfully; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The <see cref="ConcurrentDictionary{TKey, TValue}"/> contains too many elements.</exception>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return TryAddInternal(key, nullableHashcode: null, value, updateIfExists: false, acquireLock: true, out _);
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key from the <see cref="ConcurrentDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, <paramref name="value"/> contains the object from
        /// the <see cref="ConcurrentDictionary{TKey,TValue}"/> with the specified key or the default value of
        /// <typeparamref name="TValue"/>, if the operation failed.
        /// </param>
        /// <returns>true if the key was found in the <see cref="ConcurrentDictionary{TKey,TValue}"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference (Nothing in Visual Basic).</exception>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // We must capture the volatile _tables field into a local variable: it is set to a new table on each table resize.
            // The Volatile.Read on the array element then ensures that we have a copy of the reference to tables._buckets[bucketNo]:
            // this protects us from reading fields ('_hashcode', '_key', '_value' and '_next') of different instances.
            var tables = _tables;

            var comparer = _comparer;
            if (comparer is null)
            {
                var hashcode = key.GetHashCode();
                if (typeof(TKey).GetTypeInfo().IsValueType)
                {
                    for (var n = Volatile.Read(ref tables.GetBucket(hashcode)); n != null; n = n._next)
                    {
                        if (hashcode == n._hashcode && EqualityComparer<TKey>.Default.Equals(n._key, key))
                        {
                            value = n._value;
                            return true;
                        }
                    }
                }
                else
                {
                    for (var n = Volatile.Read(ref tables.GetBucket(hashcode)); n != null; n = n._next)
                    {
                        if (hashcode == n._hashcode && _defaultComparer.Equals(n._key, key))
                        {
                            value = n._value;
                            return true;
                        }
                    }
                }
            }
            else
            {
                var hashcode = comparer.GetHashCode(key);
                for (var n = Volatile.Read(ref tables.GetBucket(hashcode)); n != null; n = n._next)
                {
                    if (hashcode == n._hashcode && comparer.Equals(n._key, key))
                    {
                        value = n._value;
                        return true;
                    }
                }
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Attempts to remove and return the value with the specified key from the <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove and return.</param>
        /// <param name="value">
        /// When this method returns, <paramref name="value"/> contains the object removed from the
        /// <see cref="ConcurrentDictionary{TKey,TValue}"/> or the default value of <typeparamref name="TValue"/> if the operation failed.
        /// </param>
        /// <returns>true if an object was removed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference (Nothing in Visual Basic).</exception>
        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return TryRemoveInternal(key, out value, matchValue: false, default);
        }

        /// <summary>Removes a key and value from the dictionary.</summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/> representing the key and value to remove.</param>
        /// <returns>
        /// true if the key and value represented by <paramref name="item"/> are successfully
        /// found and removed; otherwise, false.
        /// </returns>
        /// <remarks>
        /// Both the specified key and value must match the entry in the dictionary for it to be removed.
        /// The key is compared using the dictionary's comparer (or the default comparer for <typeparamref name="TKey"/>
        /// if no comparer was provided to the dictionary when it was constructed).  The value is compared using the
        /// default comparer for <typeparamref name="TValue"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> property of <paramref name="item"/> is a null reference.
        /// </exception>
        public bool TryRemove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key is null)
            {
                throw new ArgumentNullException(nameof(item), "ItemKeyIsNull");
            }

            return TryRemoveInternal(item.Key, out _, matchValue: true, item.Value);
        }

        /// <summary>
        /// Updates the value associated with <paramref name="key"/> to <paramref name="newValue"/> if the existing value is equal
        /// to <paramref name="comparisonValue"/>.
        /// </summary>
        /// <param name="key">The key whose value is compared with <paramref name="comparisonValue"/> and
        /// possibly replaced.</param>
        /// <param name="newValue">The value that replaces the value of the element with <paramref name="key"/> if the comparison results in equality.</param>
        /// <param name="comparisonValue">The value that is compared to the value of the element with
        /// <paramref name="key"/>.</param>
        /// <returns>
        /// true if the value with <paramref name="key"/> was equal to <paramref name="comparisonValue"/> and
        /// replaced with <paramref name="newValue"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference.</exception>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return TryUpdateInternal(key, nullableHashcode: null, newValue, comparisonValue);
        }

        /// <summary>Determines whether type TValue can be written atomically.</summary>
        private static bool IsValueWriteAtomic()
        {
            // Section 12.6.6 of ECMA CLI explains which types can be read and written atomically without
            // the risk of tearing. See https://www.ecma-international.org/publications/files/ECMA-ST/ECMA-335.pdf

            if (!typeof(TValue).GetTypeInfo().IsValueType ||
                typeof(TValue) == typeof(IntPtr) ||
                typeof(TValue) == typeof(UIntPtr))
            {
                return true;
            }

            switch (TypeEx.GetTypeCode(typeof(TValue)))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    return true;

                case TypeCode.Double:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return IntPtr.Size == 8;

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private static void ThrowIfInvalidObjectValue(object? value)
        {
            if (value != null)
            {
                if (!(value is TValue))
                {
                    throw new ArgumentException("Type of value is incorrect");
                }
            }
            else if (default(TValue) is not null)
            {
                throw new ArgumentException("Type of value is incorrect");
            }
        }

        [DoesNotReturn]
        private static void ThrowKeyNotFoundException(TKey key) => throw new KeyNotFoundException(key.ToString());

        private void AcquireAllLocks(ref int locksAcquired)
        {
            // First, acquire lock 0
            AcquireLocks(0, 1, ref locksAcquired);

            // Now that we have lock 0, the _locks array will not change (i.e., grow),
            // and so we can safely read _locks.Length.
            AcquireLocks(1, _tables._locks.Length, ref locksAcquired);
            Debug.Assert(locksAcquired == _tables._locks.Length);
        }

        private void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
        {
            Debug.Assert(fromInclusive <= toExclusive);
            var locks = _tables._locks;

            for (var i = fromInclusive; i < toExclusive; i++)
            {
                var lockTaken = false;
                try
                {
                    MonitorEx.Enter(locks[i], ref lockTaken);
                }
                finally
                {
                    if (lockTaken)
                    {
                        locksAcquired++;
                    }
                }
            }
        }

        private bool AreAllBucketsEmpty()
        {
            var countPerLock = _tables._countPerLock;

            for (var i = 0; i < countPerLock.Length; i++)
            {
                if (countPerLock[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void CopyToEntries(DictionaryEntry[] array, int index)
        {
            var buckets = _tables._buckets;
            for (var i = 0; i < buckets.Length; i++)
            {
                for (var current = buckets[i]; current != null; current = current._next)
                {
                    array[index] = new DictionaryEntry(current._key, current._value);
                    index++; // this should never flow, CopyToEntries is only called when there's no overflow risk
                }
            }
        }

        private void CopyToObjects(object[] array, int index)
        {
            var buckets = _tables._buckets;
            for (var i = 0; i < buckets.Length; i++)
            {
                for (var current = buckets[i]; current != null; current = current._next)
                {
                    array[index] = new KeyValuePair<TKey, TValue>(current._key, current._value);
                    index++; // this should never overflow, CopyToObjects is only called when there's no overflow risk
                }
            }
        }

        private void CopyToPairs(KeyValuePair<TKey, TValue>[] array, int index)
        {
            var buckets = _tables._buckets;
            for (var i = 0; i < buckets.Length; i++)
            {
                for (var current = buckets[i]; current != null; current = current._next)
                {
                    array[index] = new KeyValuePair<TKey, TValue>(current._key, current._value);
                    index++; // this should never overflow, CopyToPairs is only called when there's no overflow risk
                }
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="ConcurrentDictionary{TKey,TValue}"/>. Should only be used after all locks
        /// have been acquired.
        /// </summary>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <value>The number of key/value pairs contained in the <see cref="ConcurrentDictionary{TKey,TValue}"/>.</value>
        /// <remarks>Count has snapshot semantics and represents the number of items in the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// at the moment when Count was accessed.</remarks>
        private int GetCountInternal()
        {
            var count = 0;
            var countPerLocks = _tables._countPerLock;

            // Compute the count, we allow overflow
            for (var i = 0; i < countPerLocks.Length; i++)
            {
                count += countPerLocks[i];
            }

            return count;
        }

        /// <summary>
        /// Gets a collection containing the keys in the dictionary.
        /// </summary>
        private ReadOnlyCollection<TKey> GetKeys()
        {
            var locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);

                var count = GetCountInternal();
                if (count < 0)
                {
                    throw new OutOfMemoryException();
                }

                var keys = new List<TKey>(count);
                var buckets = _tables._buckets;
                for (var i = 0; i < buckets.Length; i++)
                {
                    for (var current = buckets[i]; current != null; current = current._next)
                    {
                        keys.Add(current._key);
                    }
                }

                return new ReadOnlyCollection<TKey>(keys);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the dictionary.
        /// </summary>
        private ReadOnlyCollection<TValue> GetValues()
        {
            var locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);

                var count = GetCountInternal();
                if (count < 0)
                {
                    throw new OutOfMemoryException();
                }

                var values = new List<TValue>(count);
                var buckets = _tables._buckets;
                for (var i = 0; i < buckets.Length; i++)
                {
                    for (var current = buckets[i]; current != null; current = current._next)
                    {
                        values.Add(current._value);
                    }
                }

                return new ReadOnlyCollection<TValue>(values);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }

        private void GrowTable(Tables tables)
        {
            const int MaxArrayLength = 0X7FEFFFFF;
            var locksAcquired = 0;
            try
            {
                // The thread that first obtains _locks[0] will be the one doing the resize operation
                AcquireLocks(0, 1, ref locksAcquired);

                // Make sure nobody resized the table while we were waiting for lock 0:
                if (tables != _tables)
                {
                    // We assume that since the table reference is different, it was already resized (or the budget
                    // was adjusted). If we ever decide to do table shrinking, or replace the table for other reasons,
                    // we will have to revisit this logic.
                    return;
                }

                // Compute the (approx.) total size. Use an Int64 accumulation variable to avoid an overflow.
                long approxCount = 0;
                for (var i = 0; i < tables._countPerLock.Length; i++)
                {
                    approxCount += tables._countPerLock[i];
                }

                //
                // If the bucket array is too empty, double the budget instead of resizing the table
                //
                if (approxCount < tables._buckets.Length / 4)
                {
                    _budget = 2 * _budget;
                    if (_budget < 0)
                    {
                        _budget = int.MaxValue;
                    }

                    return;
                }

                // Compute the new table size. We find the smallest integer larger than twice the previous table size, and not divisible by
                // 2,3,5 or 7. We can consider a different table-sizing policy in the future.
                var newLength = 0;
                var maximizeTableSize = false;
                try
                {
                    checked
                    {
                        // Double the size of the buckets table and add one, so that we have an odd integer.
                        newLength = (tables._buckets.Length * 2) + 1;

                        // Now, we only need to check odd integers, and find the first that is not divisible
                        // by 3, 5 or 7.
                        while (newLength % 3 == 0 || newLength % 5 == 0 || newLength % 7 == 0)
                        {
                            newLength += 2;
                        }

                        Debug.Assert(newLength % 2 != 0);

                        if (newLength > MaxArrayLength)
                        {
                            maximizeTableSize = true;
                        }
                    }
                }
                catch (OverflowException)
                {
                    maximizeTableSize = true;
                }

                if (maximizeTableSize)
                {
                    newLength = MaxArrayLength;

                    // We want to make sure that GrowTable will not be called again, since table is at the maximum size.
                    // To achieve that, we set the budget to int.MaxValue.
                    //
                    // (There is one special case that would allow GrowTable() to be called in the future:
                    // calling Clear() on the ConcurrentDictionary will shrink the table and lower the budget.)
                    _budget = int.MaxValue;
                }

                // Now acquire all other locks for the table
                AcquireLocks(1, tables._locks.Length, ref locksAcquired);

                var newLocks = tables._locks;

                // Add more locks
                if (_growLockArray && tables._locks.Length < MaxLockNumber)
                {
                    newLocks = new object[tables._locks.Length * 2];
                    Array.Copy(tables._locks, newLocks, tables._locks.Length);
                    for (var i = tables._locks.Length; i < newLocks.Length; i++)
                    {
                        newLocks[i] = new object();
                    }
                }

                var newBuckets = new Node[newLength];
                var newCountPerLock = new int[newLocks.Length];
                var newTables = new Tables(newBuckets, newLocks, newCountPerLock);

                // Copy all data into a new table, creating new nodes for all elements
                foreach (var bucket in tables._buckets)
                {
                    var current = bucket;
                    while (current != null)
                    {
                        var next = current._next;
                        ref var newBucket = ref newTables.GetBucketAndLock(current._hashcode, out var newLockNo);
                        Volatile.Write(ref newBucket, new Node(current._key, current._value, current._hashcode, newBucket));

                        checked
                        {
                            newCountPerLock[newLockNo]++;
                        }

                        current = next;
                    }
                }

                // Adjust the budget
                _budget = Math.Max(1, newBuckets.Length / newLocks.Length);

                // Replace tables with the new versions
                _tables = newTables;
            }
            finally
            {
                // Release all locks that we took earlier
                ReleaseLocks(0, locksAcquired);
            }
        }

        private void InitializeFromCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
            {
                if (pair.Key is null)
                {
                    throw new ArgumentNullException(nameof(pair.Key));
                }

                if (!TryAddInternal(pair.Key, nullableHashcode: null, pair.Value, updateIfExists: false, acquireLock: false, out _))
                {
                    throw new ArgumentException("SourceContainsDuplicateKeys");
                }
            }

            if (_budget == 0)
            {
                var tables = _tables;
                _budget = tables._buckets.Length / tables._locks.Length;
            }
        }

        private void ReleaseLocks(int fromInclusive, int toExclusive)
        {
            Debug.Assert(fromInclusive <= toExclusive);

            var tables = _tables;
            for (var i = fromInclusive; i < toExclusive; i++)
            {
                Monitor.Exit(tables._locks[i]);
            }
        }

        private bool TryAddInternal(TKey key, int? nullableHashcode, TValue value, bool updateIfExists, bool acquireLock, out TValue resultingValue)
        {
            var comparer = _comparer;

            Debug.Assert(
                nullableHashcode is null ||
                (comparer is null && key.GetHashCode() == nullableHashcode) ||
                (comparer != null && comparer.GetHashCode(key) == nullableHashcode));

            var hashcode =
                nullableHashcode ??
                (comparer is null ? key.GetHashCode() : comparer.GetHashCode(key));

            while (true)
            {
                var tables = _tables;
                var locks = tables._locks;
                ref var bucket = ref tables.GetBucketAndLock(hashcode, out var lockNo);

                var resizeDesired = false;
                var lockTaken = false;
                try
                {
                    if (acquireLock)
                    {
                        MonitorEx.Enter(locks[lockNo], ref lockTaken);
                    }

                    // If the table just got resized, we may not be holding the right lock, and must retry.
                    // This should be a rare occurrence.
                    if (tables != _tables)
                    {
                        continue;
                    }

                    // Try to find this key in the bucket
                    Node? prev = null;
                    for (var node = bucket; node != null; node = node._next)
                    {
                        Debug.Assert((prev is null && node == bucket) || prev!._next == node);
                        if (hashcode == node._hashcode && (comparer is null ? _defaultComparer.Equals(node._key, key) : comparer.Equals(node._key, key)))
                        {
                            // The key was found in the dictionary. If updates are allowed, update the value for that key.
                            // We need to create a new node for the update, in order to support TValue types that cannot
                            // be written atomically, since lock-free reads may be happening concurrently.
                            if (updateIfExists)
                            {
                                if (_isValueWriteAtomic)
                                {
                                    node._value = value;
                                }
                                else
                                {
                                    var newNode = new Node(node._key, value, hashcode, node._next);
                                    if (prev is null)
                                    {
                                        Volatile.Write(ref bucket, newNode);
                                    }
                                    else
                                    {
                                        prev._next = newNode;
                                    }
                                }

                                resultingValue = value;
                            }
                            else
                            {
                                resultingValue = node._value;
                            }

                            return false;
                        }

                        prev = node;
                    }

                    // The key was not found in the bucket. Insert the key-value pair.
                    var resultNode = new Node(key, value, hashcode, bucket);
                    Volatile.Write(ref bucket, resultNode);
                    checked
                    {
                        tables._countPerLock[lockNo]++;
                    }

                    //
                    // If the number of elements guarded by this lock has exceeded the budget, resize the bucket table.
                    // It is also possible that GrowTable will increase the budget but won't resize the bucket table.
                    // That happens if the bucket table is found to be poorly utilized due to a bad hash function.
                    //
                    if (tables._countPerLock[lockNo] > _budget)
                    {
                        resizeDesired = true;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(locks[lockNo]);
                    }
                }

                //
                // The fact that we got here means that we just performed an insertion. If necessary, we will grow the table.
                //
                // Concurrency notes:
                // - Notice that we are not holding any locks at when calling GrowTable. This is necessary to prevent deadlocks.
                // - As a result, it is possible that GrowTable will be called unnecessarily. But, GrowTable will obtain lock 0
                //   and then verify that the table we passed to it as the argument is still the current table.
                //
                if (resizeDesired)
                {
                    GrowTable(tables);
                }

                resultingValue = value;
                return true;
            }
        }

        private bool TryGetValueInternal(TKey key, int hashcode, [MaybeNullWhen(false)] out TValue value)
        {
            Debug.Assert((_comparer is null ? key.GetHashCode() : _comparer.GetHashCode(key)) == hashcode);

            // We must capture the volatile _tables field into a local variable: it is set to a new table on each table resize.
            // The Volatile.Read on the array element then ensures that we have a copy of the reference to tables._buckets[bucketNo]:
            // this protects us from reading fields ('_hashcode', '_key', '_value' and '_next') of different instances.
            var tables = _tables;

            var comparer = _comparer;
            if (comparer is null)
            {
                if (typeof(TKey).GetTypeInfo().IsValueType)
                {
                    for (var n = Volatile.Read(ref tables.GetBucket(hashcode)); n != null; n = n._next)
                    {
                        if (hashcode == n._hashcode && EqualityComparer<TKey>.Default.Equals(n._key, key))
                        {
                            value = n._value;
                            return true;
                        }
                    }
                }
                else
                {
                    for (var n = Volatile.Read(ref tables.GetBucket(hashcode)); n != null; n = n._next)
                    {
                        if (hashcode == n._hashcode && _defaultComparer.Equals(n._key, key))
                        {
                            value = n._value;
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (var n = Volatile.Read(ref tables.GetBucket(hashcode)); n != null; n = n._next)
                {
                    if (hashcode == n._hashcode && comparer.Equals(n._key, key))
                    {
                        value = n._value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Removes the specified key from the dictionary if it exists and returns its associated value.
        /// If matchValue flag is set, the key will be removed only if is associated with a particular
        /// value.
        /// </summary>
        /// <param name="key">The key to search for and remove if it exists.</param>
        /// <param name="value">The variable into which the removed value, if found, is stored.</param>
        /// <param name="matchValue">Whether removal of the key is conditional on its value.</param>
        /// <param name="oldValue">The conditional value to compare against if <paramref name="matchValue"/> is true</param>
        private bool TryRemoveInternal(TKey key, [MaybeNullWhen(false)] out TValue value, bool matchValue, TValue? oldValue)
        {
            var comparer = _comparer;
            var hashcode = comparer is null ? key.GetHashCode() : comparer.GetHashCode(key);
            while (true)
            {
                var tables = _tables;
                var locks = tables._locks;
                ref var bucket = ref tables.GetBucketAndLock(hashcode, out var lockNo);

                lock (locks[lockNo])
                {
                    // If the table just got resized, we may not be holding the right lock, and must retry.
                    // This should be a rare occurrence.
                    if (tables != _tables)
                    {
                        continue;
                    }

                    Node? prev = null;
                    for (var curr = bucket; curr != null; curr = curr._next)
                    {
                        Debug.Assert((prev is null && curr == bucket) || prev!._next == curr);

                        if (hashcode == curr._hashcode && (comparer is null ? _defaultComparer.Equals(curr._key, key) : comparer.Equals(curr._key, key)))
                        {
                            if (matchValue)
                            {
                                var valuesMatch = EqualityComparer<TValue>.Default.Equals(oldValue!, curr._value);
                                if (!valuesMatch)
                                {
                                    value = default;
                                    return false;
                                }
                            }

                            if (prev is null)
                            {
                                Volatile.Write(ref bucket, curr._next);
                            }
                            else
                            {
                                prev._next = curr._next;
                            }

                            value = curr._value;
                            tables._countPerLock[lockNo]--;
                            return true;
                        }

                        prev = curr;
                    }
                }

                value = default;
                return false;
            }
        }

        /// <summary>
        /// Updates the value associated with <paramref name="key"/> to <paramref name="newValue"/> if the existing value is equal
        /// to <paramref name="comparisonValue"/>.
        /// </summary>
        /// <param name="key">The key whose value is compared with <paramref name="comparisonValue"/> and
        /// possibly replaced.</param>
        /// <param name="nullableHashcode">The hashcode computed for <paramref name="key"/>.</param>
        /// <param name="newValue">The value that replaces the value of the element with <paramref name="key"/> if the comparison results in equality.</param>
        /// <param name="comparisonValue">The value that is compared to the value of the element with
        /// <paramref name="key"/>.</param>
        /// <returns>
        /// true if the value with <paramref name="key"/> was equal to <paramref name="comparisonValue"/> and
        /// replaced with <paramref name="newValue"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference.</exception>
        private bool TryUpdateInternal(TKey key, int? nullableHashcode, TValue newValue, TValue comparisonValue)
        {
            var comparer = _comparer;

            Debug.Assert(
                nullableHashcode is null ||
                (comparer is null ? key.GetHashCode() : comparer.GetHashCode(key)) == nullableHashcode);

            var hashcode =
                nullableHashcode ??
                (comparer is null ? key.GetHashCode() : comparer.GetHashCode(key));

            var valueComparer = EqualityComparer<TValue>.Default;

            while (true)
            {
                var tables = _tables;
                var locks = tables._locks;
                ref var bucket = ref tables.GetBucketAndLock(hashcode, out var lockNo);

                lock (locks[lockNo])
                {
                    // If the table just got resized, we may not be holding the right lock, and must retry.
                    // This should be a rare occurrence.
                    if (tables != _tables)
                    {
                        continue;
                    }

                    // Try to find this key in the bucket
                    Node? prev = null;
                    for (var node = bucket; node != null; node = node._next)
                    {
                        Debug.Assert((prev is null && node == bucket) || prev!._next == node);
                        if (hashcode == node._hashcode && (comparer is null ? _defaultComparer.Equals(node._key, key) : comparer.Equals(node._key, key)))
                        {
                            if (valueComparer.Equals(node._value, comparisonValue))
                            {
                                if (_isValueWriteAtomic)
                                {
                                    node._value = newValue;
                                }
                                else
                                {
                                    var newNode = new Node(node._key, newValue, hashcode, node._next);

                                    if (prev is null)
                                    {
                                        Volatile.Write(ref bucket, newNode);
                                    }
                                    else
                                    {
                                        prev._next = newNode;
                                    }
                                }

                                return true;
                            }

                            return false;
                        }

                        prev = node;
                    }

                    // didn't find the key
                    return false;
                }
            }
        }

        /// <summary>
        /// A private class to represent enumeration over the dictionary that implements the
        /// IDictionaryEnumerator interface.
        /// </summary>
        private sealed class DictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator; // Enumerator over the dictionary.

            internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> dictionary)
            {
                _enumerator = dictionary.GetEnumerator();
            }

            public object Current => Entry;

            public DictionaryEntry Entry => new(_enumerator.Current.Key, _enumerator.Current.Value);

            public object Key => _enumerator.Current.Key;

            public object? Value => _enumerator.Current.Value;

            public bool MoveNext() => _enumerator.MoveNext();

            public void Reset() => _enumerator.Reset();
        }

        /// <summary>Provides an enumerator implementation for the dictionary.</summary>
        private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            // Provides a manually-implemented version of (approximately) this iterator:
#pragma warning disable S125 // Sections of code should not be commented out
            //     Node?[] buckets = _tables._buckets;
            //     for (int i = 0; i < buckets.Length; i++)
            //         for (Node? current = Volatile.Read(ref buckets[i]); current != null; current = current._next)
            //             yield return new KeyValuePair<TKey, TValue>(current._key, current._value);
#pragma warning restore S125 // Sections of code should not be commented out

            private const int StateDone = 3;
            private const int StateInnerLoop = 2;
            private const int StateOuterloop = 1;
            private const int StateUninitialized = 0;
            private readonly ConcurrentDictionary<TKey, TValue> _dictionary;
            private Node?[]? _buckets;
            private int _i;
            private Node? _node;
            private int _state;

            public Enumerator(ConcurrentDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _i = -1;
            }

            public KeyValuePair<TKey, TValue> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                // Empty
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case StateUninitialized:
                        _buckets = _dictionary._tables._buckets;
                        _i = -1;
                        goto case StateOuterloop;

                    case StateOuterloop:
                        var buckets = _buckets;
                        Debug.Assert(buckets != null);

                        var i = ++_i;
                        if ((uint)i < (uint)buckets!.Length)
                        {
                            // The Volatile.Read ensures that we have a copy of the reference to buckets[i]:
                            // this protects us from reading fields ('_key', '_value' and '_next') of different instances.
                            _node = Volatile.Read(ref buckets[i]);
                            _state = StateInnerLoop;
                            goto case StateInnerLoop;
                        }

                        goto default;

                    case StateInnerLoop:
                        var node = _node;
                        if (node != null)
                        {
                            Current = new KeyValuePair<TKey, TValue>(node._key, node._value);
                            _node = node._next;
                            return true;
                        }

                        goto case StateOuterloop;

                    default:
                        _state = StateDone;
                        return false;
                }
            }

            public void Reset()
            {
                _buckets = null;
                _node = null;
                Current = default;
                _i = -1;
                _state = StateUninitialized;
            }
        }

        /// <summary>
        /// A node in a singly-linked list representing a particular hash table bucket.
        /// </summary>
        private sealed class Node
        {
            internal readonly int _hashcode;
            internal readonly TKey _key;
            internal volatile Node? _next;
            internal TValue _value;

            internal Node(TKey key, TValue value, int hashcode, Node? next)
            {
                _key = key;
                _value = value;
                _next = next;
                _hashcode = hashcode;
            }
        }

        /// <summary>Tables that hold the internal state of the ConcurrentDictionary</summary>
        /// <remarks>
        /// Wrapping the three tables in a single object allows us to atomically
        /// replace all tables at once.
        /// </remarks>
        private sealed class Tables
        {
            /// <summary>A singly-linked list for each bucket.</summary>
            internal readonly Node?[] _buckets;

            /// <summary>The number of elements guarded by each lock.</summary>
            internal readonly int[] _countPerLock;

            /// <summary>Pre-computed multiplier for use on 64-bit performing faster modulo operations.</summary>
            internal readonly ulong _fastModBucketsMultiplier;

            /// <summary>A set of locks, each guarding a section of the table.</summary>
            internal readonly object[] _locks;

            internal Tables(Node?[] buckets, object[] locks, int[] countPerLock)
            {
                _buckets = buckets;
                _locks = locks;
                _countPerLock = countPerLock;
                if (IntPtr.Size == 8)
                {
                    _fastModBucketsMultiplier = FastModHelper.GetFastModMultiplier((uint)buckets.Length);
                }
            }

            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            internal ref Node? GetBucket(int hashcode)
            {
                var buckets = _buckets;
                if (IntPtr.Size == 8)
                {
                    return ref buckets[FastModHelper.FastMod((uint)hashcode, (uint)buckets.Length, _fastModBucketsMultiplier)];
                }

                return ref buckets[(uint)hashcode % (uint)buckets.Length];
            }

            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            internal ref Node? GetBucketAndLock(int hashcode, out uint lockNo)
            {
                var buckets = _buckets;
                uint bucketNo;
                if (IntPtr.Size == 8)
                {
                    bucketNo = FastModHelper.FastMod((uint)hashcode, (uint)buckets.Length, _fastModBucketsMultiplier);
                }
                else
                {
                    bucketNo = (uint)hashcode % (uint)buckets.Length;
                }

                lockNo = bucketNo % (uint)_locks.Length; // doesn't use FastMod, as it would require maintaining a different multiplier
                return ref buckets[bucketNo];
            }
        }
    }

#if LESSTHAN_NET40

    [Serializable]
    [HostProtection(Synchronization = true, ExternalThreading = true)]
    public partial class ConcurrentDictionary<TKey, TValue> where TKey : notnull
    {
        private KeyValuePair<TKey, TValue>[]? _serializationArray; // Used for custom serialization
        private int _serializationCapacity; // used to save the capacity in serialization
        private int _serializationConcurrencyLevel; // used to save the concurrency level in serialization

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _ = context;
            var buckets = new Node[_serializationCapacity];
            var countPerLock = new int[_serializationConcurrencyLevel];

            var locks = new object[_serializationConcurrencyLevel];
            for (var i = 0; i < locks.Length; i++)
            {
                locks[i] = new object();
            }

            _tables = new Tables(buckets, locks, countPerLock);

            var array = _serializationArray!;
            InitializeFromCollection(array);
            _serializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _ = context;
            var tables = _tables;

            // save the data into the serialization array to be saved
            _serializationArray = ToArray();
            _serializationConcurrencyLevel = tables._locks.Length;
            _serializationCapacity = tables._buckets.Length;
        }
    }

#endif
}

#endif