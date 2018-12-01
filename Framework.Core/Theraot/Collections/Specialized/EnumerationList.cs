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
        private readonly IEnumerable<T> _wrapped;

        public EnumerationList(IEnumerable<T> wrapped)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            if (wrapped is ICollection<T> collection)
            {
                _count = () => collection.Count;
            }
            else
            {
                _count = _wrapped.Count;
            }
            _contains = item => _wrapped.Contains(item, EqualityComparer<T>.Default);
        }

        public EnumerationList(T[] wrapped)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            _count = () => wrapped.Length;
            _contains = item => Array.IndexOf(wrapped, item) >= 0;
        }

        public EnumerationList(ICollection<T> wrapped)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            _count = () => wrapped.Count;
            _contains = wrapped.Contains;
        }

        public EnumerationList(IEnumerable<T> wrapped, Func<int> count)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            _count = count ?? throw new ArgumentNullException(nameof(count));
            _contains = item => _wrapped.Contains(item, EqualityComparer<T>.Default);
        }

        public EnumerationList(IEnumerable<T> wrapped, Func<T, bool> contains)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            _count = _wrapped.Count;
            _contains = contains ?? throw new ArgumentNullException(nameof(contains));
        }

        public EnumerationList(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> contains)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            _count = count ?? throw new ArgumentNullException(nameof(count));
            _contains = contains ?? throw new ArgumentNullException(nameof(contains));
        }

        public int Count => _count.Invoke();

        bool ICollection<T>.IsReadOnly => true;

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                if (_wrapped is IList<T> list)
                {
                    return list[index];
                }
                if (index < Count)
                {
                    using (var enumerator = _wrapped.SkipItems(index).GetEnumerator())
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
            return _contains.Invoke(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public void CopyTo(T[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _wrapped.IndexOf(item);
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

        public T[] ToArray()
        {
            var array = new T[_count.Invoke()];
            CopyTo(array);
            return array;
        }
    }
}