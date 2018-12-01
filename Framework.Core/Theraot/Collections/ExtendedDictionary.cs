#if FAT

using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TValue> _valueComparer;
        private readonly Dictionary<TKey, TValue> _wrapped;

        public ExtendedDictionary()
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>();
            Keys = new ProxyCollection<TKey>(() => _wrapped.Keys).AsIReadOnlyCollection;
            Values = new ProxyCollection<TValue>(() => _wrapped.Values).AsIReadOnlyCollection;
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype)
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>();
            Keys = new ProxyCollection<TKey>(() => _wrapped.Keys).AsIReadOnlyCollection;
            Values = new ProxyCollection<TValue>(() => _wrapped.Values).AsIReadOnlyCollection;
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }
            this.AddRange(prototype);
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> keyComparer)
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            if (keyComparer == null)
            {
                throw new ArgumentNullException(nameof(keyComparer));
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            Keys = Extensions.WrapAsIReadOnlyCollection(_wrapped.Keys);
            Values = Extensions.WrapAsIReadOnlyCollection(_wrapped.Values);
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }
            this.AddRange(prototype);
        }

        public ExtendedDictionary(IEqualityComparer<TKey> keyComparer)
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            if (keyComparer == null)
            {
                throw new ArgumentNullException(nameof(keyComparer));
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            Keys = Extensions.WrapAsIReadOnlyCollection(_wrapped.Keys);
            Values = Extensions.WrapAsIReadOnlyCollection(_wrapped.Values);
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
            if (keyComparer == null)
            {
                throw new ArgumentNullException(nameof(keyComparer));
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            Keys = Extensions.WrapAsIReadOnlyCollection(_wrapped.Keys);
            Values = Extensions.WrapAsIReadOnlyCollection(_wrapped.Values);
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }
            this.AddRange(prototype);
        }

        public ExtendedDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
            if (keyComparer == null)
            {
                throw new ArgumentNullException(nameof(keyComparer));
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            Keys = Extensions.WrapAsIReadOnlyCollection(_wrapped.Keys);
            Values = Extensions.WrapAsIReadOnlyCollection(_wrapped.Values);
        }

        public int Count => _wrapped.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _wrapped.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _wrapped.Values;

        public IReadOnlyCollection<TKey> Keys { get; }

        public IReadOnlyCollection<TValue> Values { get; }

        public TValue this[TKey key]
        {
            get => _wrapped[key];

            set => _wrapped[key] = value;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _wrapped.Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            _wrapped.Add(key, value);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                return _valueComparer.Equals(_wrapped[item.Key], item.Value);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            try
            {
                if (comparer == null)
                {
                    throw new ArgumentNullException(nameof(comparer));
                }
                return comparer.Equals(new KeyValuePair<TKey, TValue>(item.Key, _wrapped[item.Key]), item);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _wrapped.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            try
            {
                if (_valueComparer.Equals(_wrapped[key], item.Value))
                {
                    return _wrapped.Remove(key);
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool Remove(TKey key)
        {
            return _wrapped.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            return this.RemoveWhereEnumerable(input => comparer.Equals(input, item)).Any();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _wrapped.TryGetValue(key, out value);
        }

        public void UnionWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.UnionWith(this, other);
        }
    }
}

#endif