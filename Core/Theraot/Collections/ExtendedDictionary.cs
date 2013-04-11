#if FAT
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    //TODO
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedDictionary<TKey, TValue> : IExtendedDictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
    {
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ExtendedReadOnlyCollection<TKey> _keysReadonly;
        private readonly IExtendedReadOnlyDictionary<TKey, TValue> _readOnly;
        private readonly IEqualityComparer<TValue> _valueComparer;
        private readonly ExtendedReadOnlyCollection<TValue> _valuesReadonly;
        private readonly Dictionary<TKey, TValue> _wrapped;

        public ExtendedDictionary()
        {
            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>();
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype)
        {
            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>();
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> keyComparer)
        {
            _keyComparer = Check.NotNullArgument(keyComparer, "keyComparer");
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>(_keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public ExtendedDictionary(IEqualityComparer<TKey> keyComparer)
        {
            _keyComparer = Check.NotNullArgument(keyComparer, "keyComparer");
            _valueComparer = EqualityComparer<TValue>.Default;
            _wrapped = new Dictionary<TKey, TValue>(_keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
        }

        public ExtendedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _keyComparer = Check.NotNullArgument(keyComparer, "keyComparer");
            _valueComparer = Check.NotNullArgument(valueComparer, "valueComparer");
            _wrapped = new Dictionary<TKey, TValue>(_keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public ExtendedDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _keyComparer = Check.NotNullArgument(keyComparer, "keyComparer");
            _valueComparer = Check.NotNullArgument(valueComparer, "valueComparer");
            _wrapped = new Dictionary<TKey, TValue>(_keyComparer);
            _readOnly = new ExtendedReadOnlyDictionary<TKey, TValue>(this);
            _keysReadonly = new ExtendedReadOnlyCollection<TKey>(_wrapped.Keys);
            _valuesReadonly = new ExtendedReadOnlyCollection<TValue>(_wrapped.Values);
        }

        public IReadOnlyDictionary<TKey, TValue> AsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        public int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        public IReadOnlyCollection<TKey> Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        public IReadOnlyCollection<TValue> Values
        {
            get
            {
                return _valuesReadonly;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _wrapped.Keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return _wrapped.Values;
            }
        }

        IReadOnlyCollection<KeyValuePair<TKey, TValue>> IExtendedCollection<KeyValuePair<TKey, TValue>>.AsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return _valuesReadonly;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _wrapped[key];
            }
            set
            {
                _wrapped[key] = value;
            }
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
                return Check.NotNullArgument(comparer, "comparer").Equals(new KeyValuePair<TKey, TValue>(item.Key, _wrapped[item.Key]), item);
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
            Extensions.CopyTo<KeyValuePair<TKey, TValue>>(this, array);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo<KeyValuePair<TKey, TValue>>(this, array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo<KeyValuePair<TKey, TValue>>(this, array, arrayIndex, countLimit);
        }

        public void ExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.ExceptWith(this, other);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.IntersectWith(this, other);
        }

        public bool IsProperSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            throw new System.NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            throw new System.NotImplementedException();
        }

        public bool Overlaps(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            throw new System.NotImplementedException();
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
            return Enumerable.Any
                   (
                       this.RemoveWhereEnumerable
                       (
                           input => comparer.Equals(input, item)
                       )
                   );
        }

        public bool SetEquals(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            throw new System.NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.SymmetricExceptWith(this, other);
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