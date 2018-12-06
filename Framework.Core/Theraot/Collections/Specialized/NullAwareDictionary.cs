// Needed for NET30

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public sealed class NullAwareDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _wrapped;
        private readonly IEqualityComparer<TValue> _valueComparer;
        private bool _hasNull;
        private TValue[] _valueForNull;

        public NullAwareDictionary()
        {
            _wrapped = new Dictionary<TKey, TValue>();
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
            _wrapped = new Dictionary<TKey, TValue>(comparer);
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

        public NullAwareDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }
            _wrapped = new Dictionary<TKey, TValue>(dictionary);
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<TKey>(new[] { default(TKey) }, _wrapped.Keys, () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
                TakeValueForNull(dictionary);
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
                Add(pair);
            }
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }
            _wrapped = new Dictionary<TKey, TValue>(dictionary, comparer);
            _hasNull = false;
            _valueForNull = new[] { default(TValue) };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<TKey>(new[] { default(TKey) }, _wrapped.Keys, () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
                TakeValueForNull(dictionary);
            }
            else
            {
                Keys = new ProxyCollection<TKey>(() => _wrapped.Keys).AsReadOnlyICollection;
                Values = new ProxyCollection<TValue>(() => _wrapped.Values).AsReadOnlyICollection;
            }
            AsReadOnly = new ReadOnlyDictionary<TKey, TValue>(this);
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
                if (key == null)
                {
                    if (_hasNull)
                    // ReSharper disable once HeuristicUnreachableCode
                    {
                        // ReSharper disable once HeuristicUnreachableCode
                        return _valueForNull[0];
                    }
                    throw new KeyNotFoundException();
                }
                return _wrapped[key];
            }
            set
            {
                // key could  be null
                if (key == null)
                // ReSharper disable once HeuristicUnreachableCode
                {
                    // ReSharper disable once HeuristicUnreachableCode
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
            // ReSharper disable once HeuristicUnreachableCode
            {
                // ReSharper disable once HeuristicUnreachableCode
                if (_hasNull)
                // ReSharper disable once HeuristicUnreachableCode
                {
                    // ReSharper disable once HeuristicUnreachableCode
                    throw new ArgumentException();
                }
                SetForNull(value);
            }
            else
            {
                _wrapped.Add(key, value);
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
            _wrapped.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (key == null)
            {
                if (_hasNull)
                {
                    return _valueComparer.Equals(_valueForNull[0], value);
                }
                return false;
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

        public bool Contains(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public bool ContainsKey(TKey key)
        {
            // key could  be null
            if (key == null)
            // ReSharper disable once HeuristicUnreachableCode
            {
                // ReSharper disable once HeuristicUnreachableCode
                return _hasNull; // OK
            }
            return _wrapped.ContainsKey(key);
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

        public void Deconstruct(out KeyValuePair<TKey, TValue>[] dictionary)
        {
            var result = _wrapped as IEnumerable<KeyValuePair<TKey, TValue>>;
            if (_hasNull)
            {
                // if the dictionary has null, TKey can be null, if TKey can be null, the default of TKey is null
                result = result.Prepend(new KeyValuePair<TKey, TValue>(default, _valueForNull[0]));
            }
            dictionary = Enumerable.ToArray(result);
        }

        public void ExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.ExceptWith(this, other);
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
            if (key == null)
            // ReSharper disable once HeuristicUnreachableCode
            {
                // ReSharper disable once HeuristicUnreachableCode
                if (_hasNull) // OK
                // ReSharper disable once HeuristicUnreachableCode
                {
                    // ReSharper disable once HeuristicUnreachableCode
                    ClearForNull();
                    return true;
                }
                return false;
            }
            return _wrapped.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (key == null)
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
                if (_valueComparer.Equals(_wrapped[key], value))
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

        public bool Remove(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            if (item.Key == null)
            {
                if (_hasNull)
                {
                    ClearForNull();
                    return true;
                }
                return false;
            }
            return _wrapped.Remove(item, comparer);
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
            if (key == null)
            // ReSharper disable once HeuristicUnreachableCode
            {
                // ReSharper disable once HeuristicUnreachableCode
                if (_hasNull)
                // ReSharper disable once HeuristicUnreachableCode
                {
                    // ReSharper disable once HeuristicUnreachableCode
                    value = _valueForNull[0];
                    return true;
                }
                value = default;
                return false;
            }
            return _wrapped.TryGetValue(key, out value);
        }

        public void UnionWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            this.AddRange(other);
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

        private void TakeValueForNull(IDictionary<TKey, TValue> dictionary)
        {
            TValue valueForNull = default;
            try
            {
                // if the dictionary has null, TKey can be null, if TKey can be null, the default of TKey is null
                // ReSharper disable once AssignNullToNotNullAttribute
                _hasNull = dictionary.TryGetValue(default, out valueForNull);
            }
            catch (ArgumentNullException exception)
            {
                GC.KeepAlive(exception);
            }
            _valueForNull = new[] { valueForNull };
        }
    }
}