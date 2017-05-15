#if NET20 || NET30 || NET35 || NET40

using System.Collections.Generic;

using Theraot.Collections.Specialized;

namespace System.Collections.ObjectModel
{
    [Serializable]
    public partial class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly KeyCollection _keys;
        private readonly ValueCollection _values;
        private readonly IDictionary<TKey, TValue> _wrapped;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            _wrapped = dictionary;
            _keys = new KeyCollection(new DelegatedCollection<TKey>(() => _wrapped.Keys));
            _values = new ValueCollection(new DelegatedCollection<TValue>(() => _wrapped.Values));
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        public IDictionary<TKey, TValue> Dictionary
        {
            get { return _wrapped; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_wrapped).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_wrapped).SyncRoot; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)_wrapped).IsFixedSize; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return true; }
        }

        ICollection IDictionary.Keys
        {
            get { return _keys; }
        }

        ICollection IDictionary.Values
        {
            get { return _values; }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return _keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return _values; }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return _keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return _values; }
        }

        public KeyCollection Keys
        {
            get { return _keys; }
        }

        public ValueCollection Values
        {
            get { return _values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (ReferenceEquals(key, null))
                {
                    throw new ArgumentNullException("key");
                }
                if (key is TKey)
                {
                    return this[(TKey)key];
                }
                return null;
            }

            set { throw new NotSupportedException(); }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return this[key]; }

            set { throw new NotSupportedException(); }
        }

        public TValue this[TKey key]
        {
            get { return _wrapped[key]; }
        }

        public bool ContainsKey(TKey key)
        {
            return _wrapped.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_wrapped).CopyTo(array, index);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            GC.KeepAlive(item);
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _wrapped.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            GC.KeepAlive(item);
            throw new NotSupportedException();
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
            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException("key");
            }
            return key is TKey && ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_wrapped).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _wrapped.TryGetValue(key, out value);
        }
    }
}

#endif