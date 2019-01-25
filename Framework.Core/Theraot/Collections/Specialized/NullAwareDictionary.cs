// Needed for NET30

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
// ReSharper disable HeuristicUnreachableCode

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Theraot.Reflection;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public sealed class NullAwareDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TValue> _valueComparer;
        private readonly DictionaryEx<TKey, TValue> _wrapped;
        private bool _hasNull;
        private TValue[] _valueForNull;

        public NullAwareDictionary()
        {
            _wrapped = new DictionaryEx<TKey, TValue>();
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<TKey>(new[] { default(TKey) }, _wrapped.Keys, () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey>(() => _wrapped.Keys).AsReadOnlyICollection;
                Values = new ProxyCollection<TValue>(() => _wrapped.Values).AsReadOnlyICollection;
            }
            AsReadOnly = new ReadOnlyDictionary<TKey, TValue>(this);
        }

        public NullAwareDictionary(IEqualityComparer<TKey> comparer)
        {
            _wrapped = new DictionaryEx<TKey, TValue>(comparer);
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<TKey>(new[] { default(TKey) }, _wrapped.Keys, () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey>(() => _wrapped.Keys).AsReadOnlyICollection;
                Values = new ProxyCollection<TValue>(() => _wrapped.Values).AsReadOnlyICollection;
            }
            AsReadOnly = new ReadOnlyDictionary<TKey, TValue>(this);
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<TKey>(new[] { default(TKey) }, _wrapped.Keys, () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey>(() => _wrapped.Keys).AsReadOnlyICollection;
                Values = new ProxyCollection<TValue>(() => _wrapped.Values).AsReadOnlyICollection;
            }
            AsReadOnly = new ReadOnlyDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public IReadOnlyDictionary<TKey, TValue> AsReadOnly { get; }

        public IEqualityComparer<TKey> Comparer => _wrapped.Comparer;

        public int Count => _hasNull ? _wrapped.Count + 1 : _wrapped.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public ICollection<TKey> Keys { get; }

        public ICollection<TValue> Values { get; }

        public TValue this[TKey key]
        {
            get
            {
                // key could be null
                if (key != null)
                {
                    return _wrapped[key];
                }

                if (_hasNull)
                {
                    return _valueForNull[0];
                }
                throw new KeyNotFoundException();
            }
            set
            {
                // key could be null
                if (key == null)
                {
                    SetForNull(value); // OK
                }
                else
                {
                    _wrapped[key] = value;
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            // key could  be null
            if (key == null)
            {
                if (_hasNull)
                {
                    throw new ArgumentException("An element for the null key already exists.", nameof(key));
                }
                SetForNull(value);
            }
            else
            {
                _wrapped.Add(key, value);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            Add(key, value);
        }

        public void Clear()
        {
            ClearForNull();
            _wrapped.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (key == null)
            {
                return _hasNull && _valueComparer.Equals(_valueForNull[0], value);
            }
            try
            {
                return _valueComparer.Equals(_wrapped[key], value);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            // key could  be null
            return key == null ? _hasNull : _wrapped.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            this.CopyTo(array);
        }

        public void Deconstruct(out KeyValuePair<TKey, TValue>[] dictionary)
        {
            var result = _wrapped as IEnumerable<KeyValuePair<TKey, TValue>>;
            if (_hasNull)
            {
                // if the dictionary has null, TKey can be null, if TKey can be null, the default of TKey is null
                result = new ExtendedEnumerable<KeyValuePair<TKey, TValue>>(new[] { new KeyValuePair<TKey, TValue>(default, _valueForNull[0]) }, result);
            }
            dictionary = result.ToArray();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_hasNull)
            {
                // if the dictionary has null, TKey can be null, if TKey can be null, the default of TKey is null
                yield return new KeyValuePair<TKey, TValue>(default, _valueForNull[0]);
            }
            foreach (var item in _wrapped)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            // key can be null
            if (key != null)
            {
                return _wrapped.Remove(key);
            }

            if (!_hasNull)
            {
                return false;
            }

            ClearForNull();
            return true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (key == null)
            {
                if (!_valueComparer.Equals(_valueForNull[0], value))
                {
                    return false;
                }

                ClearForNull();
                return true;
            }
            try
            {
                return _valueComparer.Equals(_wrapped[key], value) && _wrapped.Remove(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            // key can be null
            if (key != null)
            {
                return _wrapped.TryGetValue(key, out value);
            }

            if (_hasNull)
            {
                value = _valueForNull[0];
                return true;
            }
            value = default;
            return false;
        }

        private void ClearForNull()
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
        }

        private void SetForNull(TValue value)
        {
            _valueForNull[0] = value;
            _hasNull = true;
        }
    }
}