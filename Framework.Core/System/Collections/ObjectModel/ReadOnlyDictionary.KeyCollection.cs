#if LESSTHAN_NET45

#pragma warning disable RECS0096 // Type parameter is never used
#pragma warning disable CA1034 // Nested types should not be visible
// ReSharper disable UnusedTypeParameter

using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    public partial class ReadOnlyDictionary<TKey, TValue> // TValue is used in another file, this is a partial class
    {
        [Serializable]
        public sealed class KeyCollection : ICollection<TKey>, ICollection
        {
            private readonly ICollection<TKey> _wrapped;

            internal KeyCollection(ICollection<TKey> wrapped)
            {
                _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            }

            public int Count => _wrapped.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

            object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                _wrapped.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
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
                return _wrapped.Contains(item);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_wrapped).CopyTo(array, index);
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
}

#endif