#if LESSTHAN_NET45

#pragma warning disable S4144 // Methods should not have identical implementations
// ReSharper disable HeuristicUnreachableCode

using System.Collections.Generic;
using Theraot;
using Theraot.Collections.Specialized;

namespace System.Collections.ObjectModel
{
    [Serializable]
    public partial class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            Keys = new KeyCollection(new ProxyCollection<TKey>(() => Dictionary.Keys));
            Values = new ValueCollection(new ProxyCollection<TValue>(() => Dictionary.Values));
        }

        public int Count => Dictionary.Count;

        bool IDictionary.IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;
        bool IDictionary.IsReadOnly => true;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
        bool ICollection.IsSynchronized => ((ICollection)Dictionary).IsSynchronized;
        public KeyCollection Keys { get; }
        ICollection IDictionary.Keys => Keys;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        object ICollection.SyncRoot => ((ICollection)Dictionary).SyncRoot;
        public ValueCollection Values { get; }
        ICollection IDictionary.Values => Values;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
        protected IDictionary<TKey, TValue> Dictionary { get; }
        public TValue this[TKey key] => Dictionary[key];

        object? IDictionary.this[object key]
        {
            get
            {
                switch (key)
                {
                    case null:
                        throw new ArgumentNullException(nameof(key));
                    case TKey keyAsTKey:
                        return this[keyAsTKey];

                    default:
                        return null;
                }
            }

            set => throw new NotSupportedException();
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];

            set => throw new NotSupportedException();
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            No.Op(item);
            throw new NotSupportedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        void IDictionary.Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool IDictionary.Contains(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return key is TKey keyAsTKey && ContainsKey(keyAsTKey);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)Dictionary).CopyTo(array, index);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            No.Op(item);
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }
    }
}

#endif