// Needed for NET40

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
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<TValue>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_wrapped).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_wrapped).SyncRoot; }
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

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
            Extensions.CopyTo(_wrapped.ConvertProgressive(pair => pair.Value), array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_wrapped).CopyTo(array, index);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _wrapped.ConvertProgressive(pair => pair.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            throw new NotSupportedException();
        }
    }
}