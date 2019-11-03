// ReSharper disable HeuristicUnreachableCode

#pragma warning disable CA1043 // Use Integral Or String Argument For Indexers

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public partial class ReadOnlyNullAwareDictionary<TKey, TValue> : IDictionary<ReadOnlyStructNeedle<TKey>, TValue>, IReadOnlyDictionary<ReadOnlyStructNeedle<TKey>, TValue>
    {
        public ReadOnlyNullAwareDictionary(IDictionary<ReadOnlyStructNeedle<TKey>, TValue> dictionary)
        {
            Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            Keys = new KeyCollection(new ProxyCollection<ReadOnlyStructNeedle<TKey>>(() => Dictionary.Keys));
            Values = new ValueCollection(new ProxyCollection<TValue>(() => Dictionary.Values));
        }

        public int Count => Dictionary.Count;

        bool ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.IsReadOnly => true;
        public KeyCollection Keys { get; }
        ICollection<ReadOnlyStructNeedle<TKey>> IDictionary<ReadOnlyStructNeedle<TKey>, TValue>.Keys => Keys.WrapAsReadOnlyICollection();
        IEnumerable<ReadOnlyStructNeedle<TKey>> IReadOnlyDictionary<ReadOnlyStructNeedle<TKey>, TValue>.Keys => Keys;
        public ValueCollection Values { get; }
        ICollection<TValue> IDictionary<ReadOnlyStructNeedle<TKey>, TValue>.Values => Values;
        IEnumerable<TValue> IReadOnlyDictionary<ReadOnlyStructNeedle<TKey>, TValue>.Values => Values;
        protected IDictionary<ReadOnlyStructNeedle<TKey>, TValue> Dictionary { get; }
        public TValue this[ReadOnlyStructNeedle<TKey> key] => Dictionary[key];

        TValue IDictionary<ReadOnlyStructNeedle<TKey>, TValue>.this[ReadOnlyStructNeedle<TKey> key]
        {
            get => this[key];

            set => throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Add(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue> item)
        {
            No.Op(item);
            throw new NotSupportedException();
        }

        void IDictionary<ReadOnlyStructNeedle<TKey>, TValue>.Add(ReadOnlyStructNeedle<TKey> key, TValue value)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Contains(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public bool ContainsKey(ReadOnlyStructNeedle<TKey> key)
        {
            return Dictionary.ContainsKey(key);
        }

        void ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.CopyTo(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue>>.Remove(KeyValuePair<ReadOnlyStructNeedle<TKey>, TValue> item)
        {
            No.Op(item);
            throw new NotSupportedException();
        }

        bool IDictionary<ReadOnlyStructNeedle<TKey>, TValue>.Remove(ReadOnlyStructNeedle<TKey> key)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(ReadOnlyStructNeedle<TKey> key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }
    }
}