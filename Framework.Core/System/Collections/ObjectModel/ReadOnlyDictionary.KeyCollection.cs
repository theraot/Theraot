#if LESSTHAN_NET45

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

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                _wrapped.CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_wrapped).CopyTo(array, index);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
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