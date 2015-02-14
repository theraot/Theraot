using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    public sealed class ValueCollection<TKey, TValue> : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
    {
        private readonly IDictionary<TKey, TValue> _wrapped;

        internal ValueCollection(IDictionary<TKey, TValue> wrapped)
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

        bool ICollection<TValue>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
            _wrapped.ConvertProgressive(pair => pair.Value).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _wrapped.ConvertProgressive(pair => pair.Value).GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_wrapped).CopyTo(array, index);
        }

        void ICollection<TValue>.Add(TValue item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TValue>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TValue>.Contains(TValue item)
        {
            return _wrapped.Where(pair => EqualityComparer<TValue>.Default.Equals(item, pair.Value)).HasAtLeast(1);
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}