// Needed for NET30

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public sealed class EnumerationList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly Func<T, bool> _contains;

        private readonly Action<T[], int> _copyTo;

        private readonly Func<int> _count;

        private readonly Func<IEnumerator<T>> _getEnumerator;

        private readonly Func<int, T> _index;

        private readonly Func<T, int> _indexOf;

        private EnumerationList(IEnumerable<T> wrapped)
        {
            switch (wrapped)
            {
                case T[] array:
                    _count = () => array.Length;
                    _contains = item => Array.IndexOf(array, item) != -1;
                    _index = index => array[index];
                    _indexOf = item => Array.IndexOf(array, item);
                    _getEnumerator = ((IList<T>)array).GetEnumerator;
                    _copyTo = array.CopyTo;
                    break;

                case IList<T> list:
                    _count = () => list.Count;
                    _contains = list.Contains;
                    _index = index => list[index];
                    _indexOf = list.IndexOf;
                    _getEnumerator = list.GetEnumerator;
                    _copyTo = list.CopyTo;
                    break;

                case IReadOnlyList<T> readOnlyList:
                    _count = () => readOnlyList.Count;
                    _contains = value => readOnlyList.Contains(value);
                    _index = index => readOnlyList[index];
                    _indexOf = item => readOnlyList.IndexOf(item);
                    _getEnumerator = readOnlyList.GetEnumerator;
                    _copyTo = (array, arrayIndex) => readOnlyList.CopyTo(array, arrayIndex);
                    break;

                case ICollection<T> collection:
                    _count = () => collection.Count;
                    _contains = collection.Contains;
                    _index = Index;
                    _indexOf = item => collection.IndexOf(item);
                    _getEnumerator = collection.GetEnumerator;
                    _copyTo = collection.CopyTo;
                    break;

                case IReadOnlyCollection<T> readOnlyCollection:
                    _count = () => readOnlyCollection.Count;
                    _contains = value => readOnlyCollection.Contains(value);
                    _index = Index;
                    _indexOf = item => readOnlyCollection.IndexOf(item);
                    _getEnumerator = readOnlyCollection.GetEnumerator;
                    _copyTo = (array, arrayIndex) => readOnlyCollection.CopyTo(array, arrayIndex);
                    break;

                default:
                    _count = wrapped.Count;
                    _contains = wrapped.Contains!;
                    _index = Index;
                    _indexOf = wrapped.IndexOf!;
                    _getEnumerator = wrapped.GetEnumerator;
                    _copyTo = wrapped.CopyTo!;
                    break;
            }

            T Index(int index)
            {
                if (index >= _count())
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                using (var enumerator = wrapped.Skip(index).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public int Count => _count();

        bool ICollection<T>.IsReadOnly => true;

        public T this[int index] => _index(index);

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        public static EnumerationList<T> Create(IEnumerable<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped));
            }

            return new EnumerationList<T>(wrapped);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return _contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _copyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _getEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _indexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
}