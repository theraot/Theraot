#if FAT
using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Collections.Specialized;
using Theraot.Reflection;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveDictionary<TKey, TValue> : ProgressiveCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _cache;
        private readonly IEqualityComparer<TKey> _keyComparer;
        [NonSerialized]
        private KeyCollection<TKey, TValue> _keyCollection;
        [NonSerialized]
        private ValueCollection<TKey, TValue> _valueCollection;

        public ProgressiveDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
            : this(Progressor<KeyValuePair<TKey, TValue>>.CreateFromIEnumerable(enumerable), new Dictionary<TKey, TValue>(), null, null)
        {
            // Empty
        }

        public ProgressiveDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : this(Progressor<KeyValuePair<TKey, TValue>>.CreateFromIEnumerable(enumerable), new Dictionary<TKey, TValue>(keyComparer), keyComparer, valueComparer)
        {
            // Empty
        }

        public ProgressiveDictionary(IObservable<KeyValuePair<TKey, TValue>> observable)
            : this(Progressor<KeyValuePair<TKey, TValue>>.CreateFromIObservable(observable), new Dictionary<TKey, TValue>(), null, null)
        {
            // Empty
        }

        public ProgressiveDictionary(IObservable<KeyValuePair<TKey, TValue>> observable, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : this(Progressor<KeyValuePair<TKey, TValue>>.CreateFromIObservable(observable), new Dictionary<TKey, TValue>(keyComparer), keyComparer, valueComparer)
        {
            // Empty
        }

        protected ProgressiveDictionary(Progressor<KeyValuePair<TKey, TValue>> progressor, IDictionary<TKey, TValue> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : base(progressor, cache ?? throw new ArgumentNullException(nameof(cache)), new KeyValuePairEqualityComparer<TKey, TValue>(keyComparer, valueComparer))
        {
            _cache = (IDictionary<TKey, TValue>)Cache;
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => TypeHelper.LazyCreate(ref _keyCollection, () => new KeyCollection<TKey, TValue>(this));

        public IReadOnlyCollection<TKey> Keys => TypeHelper.LazyCreate(ref _keyCollection, () => new KeyCollection<TKey, TValue>(this));

        ICollection<TValue> IDictionary<TKey, TValue>.Values => TypeHelper.LazyCreate(ref _valueCollection, () => new ValueCollection<TKey, TValue>(this));

        public IReadOnlyCollection<TValue> Values => TypeHelper.LazyCreate(ref _valueCollection, () => new ValueCollection<TKey, TValue>(this));

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];

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
                    foreach (var found in ProgressorWhere(Check))
                    {
                        return found.Value;
                    }
                    throw;
                }
                bool Check(KeyValuePair<TKey, TValue> pair)
                {
                    return _keyComparer.Equals(key, pair.Key);
                }
            }
        }

        public static ProgressiveDictionary<TKey, TValue> Create<TDictionary>(Progressor<KeyValuePair<TKey, TValue>> progressor, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            where TDictionary : IDictionary<TKey, TValue>, new()
        {
            return new ProgressiveDictionary<TKey, TValue>(progressor, new TDictionary(), keyComparer, valueComparer);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool ContainsKey(TKey key)
        {
            return _cache.ContainsKey(key) || ProgressorWhere(Check).Any();

            bool Check(KeyValuePair<TKey, TValue> pair)
            {
                return _keyComparer.Equals(key, pair.Key);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool SetEquals(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.SetEquals(this, other);
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