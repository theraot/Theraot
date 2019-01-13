// Needed for NET30

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public class EnumerationList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly Func<T, bool> _contains;
        private readonly Func<int> _count;
        private readonly Func<int, T> _index;
        private readonly Func<T, int> _indexOf;
        private readonly Func<IEnumerator<T>> _getEnumerator;

        public EnumerationList(IEnumerable<T> wrapped)
        {
            switch (wrapped)
            {
                case null:
                    throw new ArgumentNullException(nameof(wrapped));
                case T[] array:
                    _count = () => array.Length;
                    _contains = item => Array.IndexOf(array, item) != -1;
                    _index = index => array[index];
                    _indexOf = item => Array.IndexOf(array, item);
                    _getEnumerator = array.Cast<T>().GetEnumerator;
                    break;

                case IList<T> list:
                    _count = () => list.Count;
                    _contains = list.Contains;
                    _index = index => list[index];
                    _indexOf = list.IndexOf;
                    _getEnumerator = list.GetEnumerator;
                    break;

                case IReadOnlyList<T> readOnlyList:
                    _count = () => readOnlyList.Count;
                    _contains = readOnlyList.Contains;
                    _index = index => readOnlyList[index];
                    _indexOf = readOnlyList.IndexOf;
                    _getEnumerator = readOnlyList.GetEnumerator;
                    break;

                case ICollection<T> collection:
                    _count = () => collection.Count;
                    _contains = collection.Contains;
                    _index = Index;
                    _indexOf = collection.IndexOf;
                    _getEnumerator = collection.GetEnumerator;
                    break;

                case IReadOnlyCollection<T> readOnlyCollection:
                    _count = () => readOnlyCollection.Count;
                    _contains = readOnlyCollection.Contains;
                    _index = Index;
                    _indexOf = readOnlyCollection.IndexOf;
                    _getEnumerator = readOnlyCollection.GetEnumerator;
                    break;

                default:
                    _count = wrapped.Count;
                    _contains = wrapped.Contains;
                    _index = Index;
                    _indexOf = wrapped.IndexOf;
                    _getEnumerator = wrapped.GetEnumerator;
                    break;
            }

            T Index(int index)
            {
                if (index < _count())
                {
                    using (var enumerator = wrapped.Skip(index).GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            return enumerator.Current;
                        }
                    }
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public int Count => _count();

        bool ICollection<T>.IsReadOnly => true;

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        public T this[int index] => _index(index);

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
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
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