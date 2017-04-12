#if FAT

using System;
using System.Collections.Generic;

using Theraot.Collections.Specialized;
using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class ExtendedReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, IExtendedDictionary<TKey, TValue>
    {
        private readonly ExtendedReadOnlyCollection<TKey> _keys;
        private readonly ExtendedReadOnlyCollection<TValue> _values;
        private readonly IDictionary<TKey, TValue> _wrapped;

        public ExtendedReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            _wrapped = dictionary;
            _keys = new ExtendedReadOnlyCollection<TKey>(new DelegatedCollection<TKey>(() => _wrapped.Keys));
            _values = new ExtendedReadOnlyCollection<TValue>(new DelegatedCollection<TValue>(() => _wrapped.Values));
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return _keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return _values; }
        }

        IReadOnlyCollection<KeyValuePair<TKey, TValue>> IExtendedCollection<KeyValuePair<TKey, TValue>>.AsReadOnly
        {
            get { return this; }
        }

        IReadOnlyDictionary<TKey, TValue> IExtendedDictionary<TKey, TValue>.AsReadOnly
        {
            get { return this; }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return _keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return _values; }
        }

        public IReadOnlyCollection<TKey> Keys
        {
            get { return _keys; }
        }

        public IReadOnlyCollection<TValue> Values
        {
            get { return _values; }
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

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _wrapped.Contains(item);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            return System.Linq.Enumerable.Contains(this, item, comparer);
        }

        public bool ContainsKey(TKey key)
        {
            return _wrapped.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array)
        {
            _wrapped.CopyTo(array, 0);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
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

        bool IExtendedCollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item, IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            throw new NotSupportedException();
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

        public bool SetEquals(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.SetEquals(this, other);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            var array = new KeyValuePair<TKey, TValue>[_wrapped.Count];
            CopyTo(array);
            return array;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _wrapped.TryGetValue(key, out value);
        }
    }
}

#endif