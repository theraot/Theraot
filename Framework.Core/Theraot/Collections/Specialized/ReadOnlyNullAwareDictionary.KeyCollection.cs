#pragma warning disable RECS0096 // Type parameter is never used
#pragma warning disable CA1034 // Nested types should not be visible

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized
{
    // ReSharper disable once UnusedTypeParameter
    public sealed partial class ReadOnlyNullAwareDictionary<TKey, TValue> // TValue is used in another file, this is a partial class
    {
        [Serializable]
        public sealed class KeyCollection : ICollection<ReadOnlyStructNeedle<TKey>>, ICollection
        {
            private readonly ICollection<ReadOnlyStructNeedle<TKey>> _wrapped;

            internal KeyCollection(ICollection<ReadOnlyStructNeedle<TKey>> wrapped)
            {
                _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            }

            public int Count => _wrapped.Count;

            bool ICollection<ReadOnlyStructNeedle<TKey>>.IsReadOnly => true;

            bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

            object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

            void ICollection<ReadOnlyStructNeedle<TKey>>.Add(ReadOnlyStructNeedle<TKey> item)
            {
                throw new NotSupportedException();
            }

            void ICollection<ReadOnlyStructNeedle<TKey>>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<ReadOnlyStructNeedle<TKey>>.Contains(ReadOnlyStructNeedle<TKey> item)
            {
                return _wrapped.Contains(item);
            }

            public void CopyTo(ReadOnlyStructNeedle<TKey>[] array, int arrayIndex)
            {
                _wrapped.CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_wrapped).CopyTo(array, index);
            }

            public IEnumerator<ReadOnlyStructNeedle<TKey>> GetEnumerator()
            {
                return _wrapped.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool ICollection<ReadOnlyStructNeedle<TKey>>.Remove(ReadOnlyStructNeedle<TKey> item)
            {
                throw new NotSupportedException();
            }
        }
    }
}