#if LESSTHAN_NET45

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

        public KeyCollection Keys { get; }
        public ValueCollection Values { get; }
        protected IDictionary<TKey, TValue> Dictionary { get; }

        bool IDictionary.IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;
        bool IDictionary.IsReadOnly => true;
        bool ICollection.IsSynchronized => ((ICollection)Dictionary).IsSynchronized;

        ICollection IDictionary.Keys => Keys;
        object ICollection.SyncRoot => ((ICollection)Dictionary).SyncRoot;

        ICollection IDictionary.Values => Values;

        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (key is TKey keyAsTKey)
                {
                    return this[keyAsTKey];
                }

                return null;
            }

            set => throw new NotSupportedException();
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotSupportedException();
        }

        void IDictionary.Clear()
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

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)Dictionary).CopyTo(array, index);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException();
        }

        public int Count => Dictionary.Count;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];

            set => throw new NotSupportedException();
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
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

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
        public TValue this[TKey key] => Dictionary[key];
    }
}

#endif