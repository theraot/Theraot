// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Theraot.Core;

namespace System.Dynamic.Utils
{
    internal class CacheDict<TKey, TValue>
    {
        // cache size is always ^2.
        // items are placed at [hash ^ mask]
        // new item will displace previous one at the same location.
        private readonly int _mask;

        private readonly Entry[] _entries;

        // class, to ensure atomic updates.
        private sealed class Entry
        {
            internal readonly int _hash;
            internal readonly TKey _key;
            internal readonly TValue _value;

            internal Entry(int hash, TKey key, TValue value)
            {
                _hash = hash;
                _key = key;
                _value = value;
            }
        }

        /// <summary>
        /// Creates a dictionary-like object used for caches.
        /// </summary>
        /// <param name="size">The maximum number of elements to store will be this number aligned to next ^2.</param>
        internal CacheDict(int size)
        {
            var alignedSize = NumericHelper.NextPowerOf2(size - 1);
            _mask = alignedSize - 1;
            _entries = new Entry[alignedSize];
        }

        internal bool TryGetValue(TKey key, out TValue value)
        {
            var hash = key.GetHashCode();
            var idx = hash & _mask;

            var entry = Volatile.Read(ref _entries[idx]);
            if (entry != null && entry._hash == hash && entry._key.Equals(key))
            {
                value = entry._value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        internal void Add(TKey key, TValue value)
        {
            var hash = key.GetHashCode();
            var idx = hash & _mask;

            var entry = Volatile.Read(ref _entries[idx]);
            if (entry == null || entry._hash != hash || !entry._key.Equals(key))
            {
                Volatile.Write(ref _entries[idx], new Entry(hash, key, value));
            }
        }

        internal TValue this[TKey key]
        {
            set { Add(key, value); }
        }
    }
}