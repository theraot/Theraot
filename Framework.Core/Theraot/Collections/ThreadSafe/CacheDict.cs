// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Provides a dictionary-like object used for caches which holds onto a maximum
    /// number of elements specified at construction time.
    /// </summary>
    public sealed class CacheDict<TKey, TValue>
    {
        private readonly Entry[] _entries;

        /// <summary>
        /// Creates a dictionary-like object used for caches.
        /// </summary>
        /// <param name="size">The maximum number of elements to store will be this number aligned to next ^2.</param>
        public CacheDict(int capacity)
        {
            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _entries = new Entry[capacity];
        }

        /// <summary>
        /// Gets or sets the value associated with the given key.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set => Add(key, value);
        }

        /// <summary>
        /// Adds a new element to the cache, possibly replacing some
        /// element that is already present.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            int hash = key.GetHashCode();
            int index = hash & (_entries.Length - 1);
            var entry = Volatile.Read(ref _entries[index]);
            if (entry == null || entry.Hash != hash || !entry.Key.Equals(key))
            {
                Volatile.Write(ref _entries[index], new Entry(hash, key, value));
            }
        }

        /// <summary>
        /// Tries to get the value associated with 'key', returning true if it's found and
        /// false if it's not present.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int hash = key.GetHashCode();
            int idx = hash & (_entries.Length - 1);
            var entry = Volatile.Read(ref _entries[idx]);
            if (entry != null && entry.Hash == hash && entry.Key.Equals(key))
            {
                value = entry.Value;
                return true;
            }
            value = default;
            return false;
        }

        // class, to ensure atomic updates.
        private sealed class Entry
        {
            internal readonly int Hash;
            internal readonly TKey Key;
            internal readonly TValue Value;

            internal Entry(int hash, TKey key, TValue value)
            {
                Hash = hash;
                Key = key;
                Value = value;
            }
        }
    }
}