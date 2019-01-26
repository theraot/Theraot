#if FAT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    public sealed class ConvertedValueCollection<TKey, TInput, TValue> : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
    {
        private readonly IDictionary<TKey, TInput> _wrapped;
        private readonly Func<TInput, TValue> _converter;

        internal ConvertedValueCollection(IDictionary<TKey, TInput> wrapped, Func<TInput, TValue> converter)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public int Count => _wrapped.Count;

        bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

        bool ICollection<TValue>.IsReadOnly => true;

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
            Extensions.CopyTo
            (
                _wrapped.ConvertProgressive
                (
                    pair =>
                    {
                        var converter = _converter;
                        return converter(pair.Value);
                    }
                ),
                array,
                arrayIndex
            );
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _wrapped.ConvertProgressive
                (
                    pair =>
                    {
                        var converter = _converter;
                        return converter(pair.Value);
                    }
                ).GetEnumerator();
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
            return _wrapped.Where
                (
                    pair =>
                    {
                        var converter = _converter;
                        return EqualityComparer<TValue>.Default.Equals(item, converter(pair.Value));
                    }
                ).HasAtLeast(1);
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

#endif