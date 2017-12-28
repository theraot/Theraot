// Needed for NET30

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed partial class NullAwareDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializable
    {
        private static readonly TKey _typedNull = TypeHelper.Cast<TKey>(null);

        private readonly Dictionary<TKey, TValue> _dictionary;
        private bool _hasNull;

        [NonSerialized]
        private ExtendedReadOnlyCollection<TKey> _keys;

        [NonSerialized]
        private IReadOnlyDictionary<TKey, TValue> _readOnly;

        [NonSerialized]
        private IEqualityComparer<TValue> _valueComparer;

        private TValue[] _valueForNull;

        [NonSerialized]
        private ExtendedReadOnlyCollection<TValue> _values;

        public NullAwareDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
            if (typeof(TKey).CanBeNull())
            {
                InitializeNullable();
            }
            else
            {
                InitializeNotNullable();
            }
        }

        public NullAwareDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
            if (typeof(TKey).CanBeNull())
            {
                InitializeNullable();
            }
            else
            {
                InitializeNotNullable();
            }
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary", "dictionary is null.");
            }
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
            if (typeof(TKey).CanBeNull())
            {
                InitializeNullable();
                TakeValueForNull(dictionary);
            }
            else
            {
                InitializeNotNullable();
            }
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary", "dictionary is null.");
            }
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
            if (typeof(TKey).CanBeNull())
            {
                InitializeNullable();
                TakeValueForNull(dictionary);
            }
            else
            {
                InitializeNotNullable();
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        private NullAwareDictionary(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _dictionary = (Dictionary<TKey, TValue>)info.GetValue("dictionary", typeof(Dictionary<TKey, TValue>));
            _hasNull = info.GetBoolean("_hasNull");
            _valueForNull[0] = (TValue)info.GetValue("valueForNull", typeof(TValue));
        }

        public IReadOnlyDictionary<TKey, TValue> AsReadOnly
        {
            get { return _readOnly; }
        }

        public IEqualityComparer<TKey> Comparer
        {
            get { return _dictionary.Comparer; }
        }

        public int Count
        {
            get { return _hasNull ? _dictionary.Count + 1 : _dictionary.Count; }
        }

        public ICollection<TKey> Keys
        {
            get { return _keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _values; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public TValue this[TKey key]
        {
            get
            {
                // key can be null
                if (ReferenceEquals(key, null))
                {
                    if (_hasNull)
                    {
                        return _valueForNull[0];
                    }
                    throw new KeyNotFoundException();
                }
                return _dictionary[key];
            }
            set
            {
                // key can be null
                if (ReferenceEquals(key, null))
                {
                    SetForNull(value);
                }
                else
                {
                    _dictionary[key] = value;
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            // key can be null
            if (ReferenceEquals(key, null))
            {
                if (_hasNull)
                {
                    throw new ArgumentException();
                }
                SetForNull(value);
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            Add(key, value);
        }

        public void Clear()
        {
            ClearForNull();
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (ReferenceEquals(key, null))
            {
                if (_hasNull)
                {
                    return _valueComparer.Equals(_valueForNull[0], value);
                }
                return false;
            }
            try
            {
                return _valueComparer.Equals(_dictionary[key], value);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public bool ContainsKey(TKey key)
        {
            // key can be null
            if (ReferenceEquals(key, null))
            {
                return _hasNull;
            }
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, countLimit);
        }

        public void ExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.ExceptWith(this, other);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_hasNull)
            {
                yield return new KeyValuePair<TKey, TValue>(
                    _typedNull,
                    _valueForNull[0]
                );
            }
            foreach (var item in _dictionary)
            {
                yield return item;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dictionary", _dictionary, typeof(Dictionary<TKey, TValue>));
            info.AddValue("hasNull", _hasNull);
            info.AddValue("valueForNull", _valueForNull[0], typeof(TValue));
        }

        public void IntersectWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.IntersectWith(this, other);
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

        public bool Remove(TKey key)
        {
            // key can be null
            if (ReferenceEquals(key, null))
            {
                if (_hasNull)
                {
                    ClearForNull();
                    return true;
                }
                return false;
            }
            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (ReferenceEquals(key, null))
            {
                if (_valueComparer.Equals(_valueForNull[0], value))
                {
                    ClearForNull();
                    return true;
                }
                return false;
            }
            try
            {
                if (_valueComparer.Equals(_dictionary[key], value))
                {
                    return _dictionary.Remove(key);
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            if (ReferenceEquals(item.Key, null))
            {
                if (_hasNull)
                {
                    ClearForNull();
                    return true;
                }
                return false;
            }
            return _dictionary.Remove(item, comparer);
        }

        public bool SetEquals(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.SetEquals(this, other);
        }

        public void SymmetricExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.SymmetricExceptWith(this, other);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            // key can be null
            if (ReferenceEquals(key, null))
            {
                if (_hasNull)
                {
                    value = _valueForNull[0];
                    return true;
                }
                value = default(TValue);
                return false;
            }
            return _dictionary.TryGetValue(key, out value);
        }

        public void UnionWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            this.AddRange(other);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ClearForNull()
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
        }

        private void InitializeNotNullable()
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            _keys = new ExtendedReadOnlyCollection<TKey>(_dictionary.Keys);
            _values = new ExtendedReadOnlyCollection<TValue>(_dictionary.Values);
            _readOnly = new ReadOnlyDictionary<TKey, TValue>(this);
        }

        private void InitializeNullable()
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            _keys = new ExtendedReadOnlyCollection<TKey>(
                new EnumerationCollection<TKey>(
                    new ConditionalExtendedEnumerable<TKey>(
                        new[] { _typedNull },
                        _dictionary.Keys,
                        () => _hasNull,
                        null
                    )
                )
            );
            _values = new ExtendedReadOnlyCollection<TValue>(
                new EnumerationCollection<TValue>(
                    new ConditionalExtendedEnumerable<TValue>(
                        _valueForNull,
                        _dictionary.Values,
                        () => _hasNull,
                        null
                    )
                )
            );
            _readOnly = new ReadOnlyDictionary<TKey, TValue>(this);
        }

        private void SetForNull(TValue value)
        {
            _valueForNull[0] = value;
            _hasNull = true;
        }

        private void TakeValueForNull(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary.ContainsKey(_typedNull))
            {
                _valueForNull = new[] { dictionary[_typedNull] };
                _hasNull = true;
            }
            else
            {
                _valueForNull = new[] { default(TValue) };
                _hasNull = false;
            }
        }
    }
}