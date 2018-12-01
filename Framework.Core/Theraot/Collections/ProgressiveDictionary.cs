#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveDictionary<TKey, TValue> : ProgressiveCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _cache;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ProgressiveSet<TKey> _keysReadonly;
        private readonly ProgressiveSet<TValue> _valuesReadonly;

        public ProgressiveDictionary(IEnumerable<KeyValuePair<TKey, TValue>> wrapped)
            : this(wrapped, new Dictionary<TKey, TValue>(), null, null)
        {
            //Empty
        }

        public ProgressiveDictionary(IEnumerable<KeyValuePair<TKey, TValue>> wrapped, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : this(wrapped, new Dictionary<TKey, TValue>(keyComparer), keyComparer, valueComparer)
        {
            //Empty
        }

        protected ProgressiveDictionary(IEnumerable<KeyValuePair<TKey, TValue>> wrapped, IDictionary<TKey, TValue> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : base(wrapped, cache ?? throw new ArgumentNullException(nameof(cache)), new KeyValuePairEqualityComparer<TKey, TValue>(keyComparer, valueComparer))
        {
            _cache = (IDictionary<TKey, TValue>)Cache;
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _valuesReadonly = new ProgressiveSet<TValue>(Progressor.ConvertProgressive(input => input.Value), valueComparer);
            _keysReadonly = new ProgressiveSet<TKey>(Progressor.ConvertProgressive(input => input.Key), keyComparer);
        }

        protected ProgressiveDictionary(IObservable<KeyValuePair<TKey, TValue>> wrapped, IDictionary<TKey, TValue> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : base(wrapped, cache ?? throw new ArgumentNullException(nameof(cache)), new KeyValuePairEqualityComparer<TKey, TValue>(keyComparer, valueComparer))
        {
            _cache = (IDictionary<TKey, TValue>)Cache;
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _valuesReadonly = new ProgressiveSet<TValue>(Progressor.ConvertProgressive(input => input.Value), valueComparer);
            _keysReadonly = new ProgressiveSet<TKey>(Progressor.ConvertProgressive(input => input.Key), keyComparer);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keysReadonly;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _valuesReadonly;

        public IReadOnlyCollection<TKey> Keys => _keysReadonly;

        public IReadOnlyCollection<TValue> Values => _valuesReadonly;

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                try
                {
                    return _cache[key];
                }
                catch (KeyNotFoundException)
                {
                    while (Progressor.TryTake(out var item))
                    {
                        if (_keyComparer.Equals(key, item.Key))
                        {
                            return item.Value;
                        }
                    }
                    throw;
                }
            }

            set => throw new NotSupportedException();
        }

        public TValue this[TKey key]
        {
            get
            {
                try
                {
                    return _cache[key];
                }
                catch (KeyNotFoundException)
                {
                    while (Progressor.TryTake(out var item))
                    {
                        if (_keyComparer.Equals(key, item.Key))
                        {
                            return item.Value;
                        }
                    }
                    throw;
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                return true;
            }
            while (Progressor.TryTake(out var item))
            {
                if (_keyComparer.Equals(key, item.Key))
                {
                    return true;
                }
            }
            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public bool SetEquals(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.SetEquals(this, other);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = this[key];
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = default;
                return false;
            }
        }
    }
}

#endif