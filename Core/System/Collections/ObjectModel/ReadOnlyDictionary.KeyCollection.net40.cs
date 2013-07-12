#if NET20 || NET30 || NET35 || NET40

using System.Collections.Generic;

using Theraot.Core;

namespace System.Collections.ObjectModel
{
    public partial class ReadOnlyDictionary<TKey, TValue>
    {
        [System.Serializable]
        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
        {
            private readonly ICollection<TKey> _wrapped;

            internal KeyCollection(ICollection<TKey> wrapped)
            {
                _wrapped = Check.NotNullArgument(wrapped, "wrapped");
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
                _wrapped.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
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
                return _wrapped.Contains(item);
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
}

#endif