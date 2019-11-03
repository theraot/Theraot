// Needed for NET30

#pragma warning disable CA1043 // Use Integral Or String Argument For Indexers
#pragma warning disable CS8714 // Nullability of type argument doesn't match 'notnull' constraint

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Theraot.Reflection;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public sealed class NullAwareDictionary<TKey, TValue> : IDictionary<ReadOnlyStructNeedle<TKey>, TValue>, IDictionary<TKey, TValue>, IHasComparer<TKey>
    {
        private readonly ICollection<TKey> _keys;
        private readonly IEqualityComparer<TValue> _valueComparer;
        private readonly IDictionary<TKey, TValue> _wrapped;
        private bool _hasNull;
        private TValue[] _valueForNull;

        public NullAwareDictionary()
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
        }

        public NullAwareDictionary(IEqualityComparer<TKey> comparer)
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = comparer;
            _wrapped = new DictionaryEx<TKey, TValue>(comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary, IEqualityComparer<TKey>? comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary, TValue valueForNullKey)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
            ValueForNullKey = valueForNullKey;
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary, TValue valueForNullKey, IEqualityComparer<TKey>? comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
            ValueForNullKey = valueForNullKey;
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> wrapped, TValue valueForNullKey)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (wrapped.TryGetComparer(out var comparer))
            {
                Comparer = comparer;
                _wrapped = wrapped;
            }
            else
            {
                Comparer = EqualityComparer<TKey>.Default;
                _wrapped = wrapped.WithComparer(Comparer);
            }
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            ValueForNullKey = valueForNullKey;
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> wrapped, TValue valueForNullKey, IEqualityComparer<TKey>? comparer)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (comparer == null)
            {
                if (wrapped.TryGetComparer(out comparer))
                {
                    Comparer = comparer;
                    _wrapped = wrapped;
                }
                else
                {
                    Comparer = EqualityComparer<TKey>.Default;
                    _wrapped = wrapped.WithComparer(Comparer);
                }
            }
            else
            {
                Comparer = comparer;
                _wrapped = wrapped.WithComparer(Comparer);
            }
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            ValueForNullKey = valueForNullKey;
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary, ReadOnlyStructNeedle<TValue> valueForNullKey)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
            if (valueForNullKey.IsAlive)
            {
                ValueForNullKey = valueForNullKey.Value;
            }
        }

        public NullAwareDictionary(KeyValuePair<TKey, TValue>[] dictionary, ReadOnlyStructNeedle<TValue> valueForNullKey, IEqualityComparer<TKey>? comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
            if (valueForNullKey.IsAlive)
            {
                ValueForNullKey = valueForNullKey.Value;
            }
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> wrapped, ReadOnlyStructNeedle<TValue> valueForNullKey)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (wrapped.TryGetComparer(out var comparer))
            {
                Comparer = comparer;
                _wrapped = wrapped;
            }
            else
            {
                Comparer = EqualityComparer<TKey>.Default;
                _wrapped = wrapped.WithComparer(Comparer);
            }
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            if (valueForNullKey.IsAlive)
            {
                ValueForNullKey = valueForNullKey.Value;
            }
        }

        public NullAwareDictionary(IDictionary<TKey, TValue> wrapped, ReadOnlyStructNeedle<TValue> valueForNullKey, IEqualityComparer<TKey>? comparer)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            if (comparer == null)
            {
                if (wrapped.TryGetComparer(out comparer))
                {
                    Comparer = comparer;
                    _wrapped = wrapped;
                }
                else
                {
                    Comparer = EqualityComparer<TKey>.Default;
                    _wrapped = wrapped.WithComparer(Comparer);
                }
            }
            else
            {
                Comparer = comparer;
                _wrapped = wrapped.WithComparer(Comparer);
            }
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            if (valueForNullKey.IsAlive)
            {
                ValueForNullKey = valueForNullKey.Value;
            }
        }

        public NullAwareDictionary(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>[] dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public NullAwareDictionary(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>[] dictionary, IEqualityComparer<TKey>? comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "dictionary is null.");
            }

            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
            _valueComparer = EqualityComparer<TValue>.Default;
            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            _wrapped = new DictionaryEx<TKey, TValue>(Comparer);
            if (typeof(TKey).CanBeNull())
            {
                Keys = new ConditionalExtendedList<ReadOnlyStructNeedle<TKey>>(new[] { default(ReadOnlyStructNeedle<TKey>) }, _wrapped.Keys.AsNeedleEnumerable(), () => _hasNull, null);
                Values = new ConditionalExtendedList<TValue>(_valueForNull, _wrapped.Values, () => _hasNull, null);
            }
            else
            {
                Keys = new ProxyCollection<TKey, ReadOnlyStructNeedle<TKey>>(_wrapped.Keys.AsICollection<TKey>, key => new ReadOnlyStructNeedle<TKey>(key), needle => needle.Value);
                Values = _wrapped.Values.WrapAsReadOnlyICollection();
            }
            _keys = new ProxyCollection<ReadOnlyStructNeedle<TKey>, TKey>(() => Keys, needle => needle.Value, value => new ReadOnlyStructNeedle<TKey>(value));
            AsReadOnly = new ReadOnlyNullAwareDictionary<TKey, TValue>(this);
            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public IReadOnlyDictionary<ReadOnlyStructNeedle<TKey>, TValue> AsReadOnly { get; }

        public IEqualityComparer<TKey> Comparer { get; }

        public int Count => _hasNull ? _wrapped.Count + 1 : _wrapped.Count;

        bool ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.IsReadOnly => false;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public ICollection<ReadOnlyStructNeedle<TKey>> Keys { get; }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return _keys; }
        }

        public TValue ValueForNullKey
        {
            get
            {
                if (_hasNull)
                {
                    return _valueForNull[0];
                }

                throw new KeyNotFoundException();
            }

            set
            {
                _valueForNull[0] = value;
                _hasNull = true;
            }
        }

        public ICollection<TValue> Values { get; }

        public TValue this[[AllowNull] TKey key]
        {
            get
            {
                if (key == null)
                {
                    return ValueForNullKey;
                }
                return _wrapped[key];
            }
            set
            {
                if (key == null)
                {
                    ValueForNullKey = value;
                    return;
                }
                _wrapped[key] = value;
            }
        }

        public TValue this[ReadOnlyStructNeedle<TKey> key]
        {
            get
            {
                if (key.IsAlive)
                {
                    return _wrapped[key.Value];
                }
                return ValueForNullKey;
            }
            set
            {
                if (key.IsAlive)
                {
                    this[key.Value] = value;
                }
                else
                {
                    ValueForNullKey = value;
                }
            }
        }

        public void Add([AllowNull] TKey key, TValue value)
        {
            if (key == null)
            {
                AddForNullKey(value);
                return;
            }
            _wrapped.Add(key, value);
        }

        public void Add(ReadOnlyStructNeedle<TKey> key, TValue value)
        {
            if (key.IsAlive)
            {
                _wrapped.Add(key.Value, value);
                return;
            }
            AddForNullKey(value);
        }

        void ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Add(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            Add(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            Add(key, value);
        }

        public void AddForNullKey(TValue value)
        {
            if (_hasNull)
            {
                throw new ArgumentException("An element for the null key already exists.");
            }

            ValueForNullKey = value;
        }

        public void Clear()
        {
            ClearForNullKey();
            _wrapped.Clear();
        }

        public void ClearForNullKey()
        {
            _hasNull = false;
            _valueForNull = new[] { default(TValue)! };
        }

        bool ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Contains(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (!key.IsAlive)
            {
                return _hasNull && _valueComparer.Equals(_valueForNull[0], value);
            }

            try
            {
                return _valueComparer.Equals(_wrapped[key.Value], value);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
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

        public bool ContainsKey([AllowNull] TKey key)
        {
            if (key == null)
            {
                return ContainsNullKey();
            }
            return _wrapped.ContainsKey(key);
        }

        public bool ContainsKey(ReadOnlyStructNeedle<TKey> key)
        {
            if (key.IsAlive)
            {
                return _wrapped.ContainsKey(key.Value);
            }
            return _hasNull;
        }

        public bool ContainsNullKey()
        {
            return _hasNull;
        }

        public void CopyTo(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            _wrapped.ConvertedCopyTo(pair => new KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value), array, arrayIndex);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            this.CopyTo(array, arrayIndex);
        }

        public void Deconstruct(out KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>[] dictionary, out IEqualityComparer<TKey> comparer)
        {
            var output = new List<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>();
            if (_hasNull)
            {
                output.Add(new KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>(default, _valueForNull[0]));
            }
            foreach (var pair in _wrapped)
            {
                output.Add(new KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value));
            }
            dictionary = output.ToArray();
            comparer = Comparer;
        }

        public IEnumerator<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>> GetEnumerator()
        {
            if (_hasNull)
            {
                // if the dictionary has null, TKey can be null, if TKey can be null, the default of TKey is null
                yield return new KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>(default, _valueForNull[0]);
            }

            foreach (var pair in _wrapped)
            {
                yield return new KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>(new ReadOnlyStructNeedle<TKey>(pair.Key), pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            if (_hasNull)
            {
                // if the dictionary has null, TKey can be null, if TKey can be null, the default of TKey is null
                yield return new KeyValuePair<TKey, TValue>(default!, _valueForNull[0]);
            }

            foreach (var pair in _wrapped)
            {
                yield return pair;
            }
        }

        public bool Remove([AllowNull] TKey key)
        {
            if (key == null)
            {
                return RemoveNullKey();
            }
            return _wrapped.Remove(key);
        }

        public bool Remove(ReadOnlyStructNeedle<TKey> key)
        {
            if (key.IsAlive)
            {
                return _wrapped.Remove(key.Value);
            }
            return RemoveNullKey();
        }

        bool ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Remove(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            if (!key.IsAlive)
            {
                if (!_valueComparer.Equals(_valueForNull[0], value))
                {
                    return false;
                }

                ClearForNullKey();
                return true;
            }

            try
            {
                return _valueComparer.Equals(_wrapped[key.Value], value) && _wrapped.Remove(key.Value);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
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

                ClearForNullKey();
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

        public bool RemoveNullKey()
        {
            if (!_hasNull)
            {
                return false;
            }

            ClearForNullKey();
            return true;
        }

        public bool TryAdd([AllowNull] TKey key, TValue value)
        {
            if (key == null)
            {
                return TryAddNullKey(value);
            }
            return _wrapped.TryAdd(key, value);
        }

        public bool TryAdd(ReadOnlyStructNeedle<TKey> key, TValue value)
        {
            if (key.IsAlive)
            {
                return _wrapped.TryAdd(key.Value, value);
            }
            return TryAddNullKey(value);
        }

        public bool TryAddNullKey(TValue value)
        {
            if (_hasNull)
            {
                return false;
            }
            ValueForNullKey = value;
            return true;
        }

        public bool TryGetValue([AllowNull] TKey key, out TValue value)
        {
            if (key == null)
            {
                return TryGetValueForNullKey(out value);
            }
            return _wrapped.TryGetValue(key, out value);
        }

        public bool TryGetValue(ReadOnlyStructNeedle<TKey> key, out TValue value)
        {
            if (key.IsAlive)
            {
                return _wrapped.TryGetValue(key.Value, out value);
            }
            return TryGetValueForNullKey(out value);
        }

        public bool TryGetValueForNullKey(out TValue value)
        {
            if (_hasNull)
            {
                value = _valueForNull[0];
                return true;
            }
            value = default!;
            return false;
        }
    }
}