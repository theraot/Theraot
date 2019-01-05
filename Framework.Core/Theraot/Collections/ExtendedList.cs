using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedList<T> : IList<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly List<T> _wrapped;

        public ExtendedList()
        {
            _comparer = EqualityComparer<T>.Default;
            _wrapped = new List<T>();
        }

        public ExtendedList(IEnumerable<T> prototype)
        {
            _comparer = EqualityComparer<T>.Default;
            _wrapped = new List<T>();
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }
            this.AddRange(prototype);
        }

        public ExtendedList(IEnumerable<T> prototype, IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _wrapped = new List<T>();
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }
            this.AddRange(prototype);
        }

        public ExtendedList(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _wrapped = new List<T>();
        }

        public int Count => _wrapped.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => _wrapped[index];

            set => _wrapped[index] = value;
        }

        public void Add(T item)
        {
            _wrapped.Add(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public ExtendedList<T> Clone()
        {
            return new ExtendedList<T>(this);
        }

        public bool Contains(T item)
        {
            return _wrapped.Exists(input => _comparer.Equals(input, item));
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return _wrapped.Contains(item, comparer);
        }

        public void CopyTo(T[] array)
        {
            _wrapped.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _wrapped.IndexOf(item, _comparer);
        }

        public void Insert(int index, T item)
        {
            _wrapped.Insert(index, item);
        }

        public void Move(int oldIndex, int newIndex)
        {
            _wrapped.Move(oldIndex, newIndex);
        }

        public bool Remove(T item)
        {
            foreach (var foundItem in _wrapped.RemoveWhereEnumerable(input => _comparer.Equals(input, item)))
            {
                GC.KeepAlive(foundItem);
                return true;
            }
            return false;
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }
            foreach (var foundItem in _wrapped.RemoveWhereEnumerable(input => comparer.Equals(input, item)))
            {
                GC.KeepAlive(foundItem);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            _wrapped.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            for (; count > 0; count--)
            {
                _wrapped.RemoveAt(index);
            }
        }

        public void Reverse()
        {
            _wrapped.Reverse();
        }

        public void Reverse(int index, int count)
        {
            _wrapped.Reverse(index, count);
        }

        public void Sort(IComparer<T> comparer)
        {
            _wrapped.Sort(comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _wrapped.Sort(index, count, comparer);
        }

        public void Swap(int indexA, int indexB)
        {
            _wrapped.Swap(indexA, indexB);
        }
    }
}