using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed class KeyCollection<TKey, TValue> : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
    {
        private readonly IDictionary<TKey, TValue> _wrapped;

        internal KeyCollection(IDictionary<TKey, TValue> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            _wrapped = wrapped;
        }

        public int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)_wrapped).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)_wrapped).SyncRoot;
            }
        }

        bool ICollection<TKey>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
            _wrapped.ConvertProgressive(pair => pair.Key).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return _wrapped.ConvertProgressive(pair => pair.Key).GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_wrapped).CopyTo(array, index);
        }

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

        bool ICollection<TKey>.Remove(TKey item)
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}