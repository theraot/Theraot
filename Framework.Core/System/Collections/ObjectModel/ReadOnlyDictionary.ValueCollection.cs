#if LESSTHAN_NET45

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable RECS0096 // Type parameter is never used

using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    // ReSharper disable once UnusedTypeParameter
    public partial class ReadOnlyDictionary<TKey, TValue> // TKey is used in another file, this is a partial class
    {
        [Serializable]
        public sealed class ValueCollection : ICollection<TValue>, ICollection
        {
            private readonly ICollection<TValue> _wrapped;

            internal ValueCollection(ICollection<TValue> wrapped)
            {
                _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            }

            public int Count => _wrapped.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

            object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

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
                return _wrapped.Contains(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                _wrapped.CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_wrapped).CopyTo(array, index);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
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
}

#endif