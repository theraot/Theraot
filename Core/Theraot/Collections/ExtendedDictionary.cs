#if FAT

using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    [System.Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedDictionary<TKey, TValue> : IExtendedDictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
    {
        private readonly IReadOnlyCollection<TKey> _keysReadonly;
        private readonly IExtendedReadOnlyDictionary<TKey, TValue> _readOnly;
        private readonly IEqualityComparer<TValue> _valueComparer;
        private readonly IReadOnlyCollection<TValue> _valuesReadonly;
        private readonly Dictionary<TKey, TValue> _wrapped;

        public ExtendedDictionary()
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>();
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new Specialized.DelegatedCollection<TKey>(() => _wrapped.Keys).AsReadOnly;
            _valuesReadonly = new Specialized.DelegatedCollection<TValue>(() => _wrapped.Values).AsReadOnly;
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype)
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>();
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }
            this.AddRange(prototype);
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> keyComparer)
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            if (keyComparer == null)
            {
                throw new ArgumentNullException("keyComparer");
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }
            this.AddRange(prototype);
        }

        public ExtendedDictionary(IEqualityComparer<TKey> keyComparer)
        {
            _valueComparer = EqualityComparer<TValue>.Default;
            if (keyComparer == null)
            {
                throw new ArgumentNullException("keyComparer");
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            if (valueComparer == null)
            {
                throw new ArgumentNullException("valueComparer");
            }
            _valueComparer = valueComparer;
            if (keyComparer == null)
            {
                throw new ArgumentNullException("keyComparer");
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }
            this.AddRange(prototype);
        }

        public ExtendedDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            if (valueComparer == null)
            {
                throw new ArgumentNullException("valueComparer");
            }
            _valueComparer = valueComparer;
            if (keyComparer == null)
            {
                throw new ArgumentNullException("keyComparer");
            }
            _wrapped = new Dictionary<TKey, TValue>(keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
        }

        public IReadOnlyDictionary<TKey, TValue> AsReadOnly
        {
            get { return _readOnly; }
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return _wrapped.Keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return _wrapped.Values; }
        }

        IReadOnlyCollection<KeyValuePair<TKey, TValue>> IExtendedCollection<KeyValuePair<TKey, TValue>>.AsReadOnly
        {
            get { return _readOnly; }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return _keysReadonly; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return _valuesReadonly; }
        }

        public IReadOnlyCollection<TKey> Keys
        {
            get { return _keysReadonly; }
        }

        public IReadOnlyCollection<TValue> Values
        {
            get { return _valuesReadonly; }
        }

        public TValue this[TKey key]
        {
            get { return _wrapped[key]; }

            set { _wrapped[key] = value; }
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
                    throw new ArgumentNullException("comparer");
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
            TKey key = item.Key;
            try
            {
                if (_valueComparer.Equals(_wrapped[key], item.Value))
                {
                    return _wrapped.Remove(key);
                }
                else
                {
                    return false;
                }
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