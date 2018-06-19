#if NET20 || NET30 || NET35 || NET40

using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    public partial class ReadOnlyDictionary<TKey, TValue>
    {
        [Serializable]
        public sealed class ValueCollection : ICollection<TValue>, ICollection
        {
            private readonly ICollection<TValue> _wrapped;

            internal ValueCollection(ICollection<TValue> wrapped)
            {
                if (wrapped == null)
                {
                    throw new ArgumentNullException("wrapped");
                }
                _wrapped = wrapped;
            }

            public int Count
            {
                get { return _wrapped.Count; }
            }

            bool ICollection.IsSynchronized
            {
                get { return ((ICollection)_wrapped).IsSynchronized; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_wrapped).SyncRoot; }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                _wrapped.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
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
                return _wrapped.Contains(item);
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

#endif