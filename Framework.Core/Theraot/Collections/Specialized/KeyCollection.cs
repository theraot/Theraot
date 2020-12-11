// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    public sealed class KeyCollection<TKey, TValue> : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
    {
        private readonly IDictionary<TKey, TValue> _wrapped;

        internal KeyCollection(IDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
        }

        public int Count => _wrapped.Count;

        bool ICollection<TKey>.IsReadOnly => true;

        bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

        void ICollection<TKey>.Add(TKey item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TKey>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TKey>.Contains(TKey item)
        {
            return _wrapped.ContainsKey(item);
        }

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
            _wrapped.ConvertProgressive(pair => pair.Key).CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_wrapped).CopyTo(array, index);
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return _wrapped.ConvertProgressive(pair => pair.Key).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TKey>.Remove(TKey item)
        {
            throw new NotSupportedException();
        }
    }
}