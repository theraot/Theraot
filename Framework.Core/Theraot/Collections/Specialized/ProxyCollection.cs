// Needed for NET40

// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public sealed class ProxyCollection<T> : ICollection<T>
    {
        private readonly Func<ICollection<T>> _wrapped;

        public ProxyCollection(Func<ICollection<T>> wrapped)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            var wrapper = EnumerationList<T>.Create(this);
            AsIReadOnlyCollection = wrapper;
            AsReadOnlyICollection = wrapper;
        }

        public IReadOnlyCollection<T> AsIReadOnlyCollection { get; }

        public ICollection<T> AsReadOnlyICollection { get; }

        public int Count => Instance.Count;

        public bool IsReadOnly => Instance.IsReadOnly;

        private ICollection<T> Instance => _wrapped.Invoke() ?? ArrayEx.Empty<T>();

        public void Add(T item)
        {
            Instance.Add(item);
        }

        public void Clear()
        {
            Instance.Clear();
        }

        public bool Contains(T item)
        {
            return Instance.Contains(item);
        }

        public void CopyTo(T[] array)
        {
            Instance.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Instance.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Instance.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var collection = Instance;
            foreach (var item in collection)
            {
                if (!ReferenceEquals(collection, Instance))
                {
                    throw new InvalidOperationException();
                }

                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(T item)
        {
            return Instance.Remove(item);
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            return Instance.RemoveWhereEnumerable(input => comparer.Equals(input, item)).Any();
        }

        public T[] ToArray()
        {
            var array = new T[Instance.Count];
            Instance.CopyTo(array, 0);
            return array;
        }
    }

    [DebuggerNonUserCode]
    public sealed class ProxyCollection<TCovered, TUncovered> : ICollection<TUncovered>
    {
        private readonly Func<TUncovered, TCovered> _cover;
        private readonly Func<TCovered, TUncovered> _uncover;
        private readonly Func<ICollection<TCovered>> _wrapped;

        public ProxyCollection(Func<ICollection<TCovered>> wrapped, Func<TCovered, TUncovered> uncover, Func<TUncovered, TCovered> cover)
        {
            _uncover = uncover ?? throw new ArgumentNullException(nameof(uncover));
            _cover = cover ?? throw new ArgumentNullException(nameof(cover));
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            var wrapper = EnumerationList<TUncovered>.Create(this);
            AsIReadOnlyCollection = wrapper;
            AsReadOnlyICollection = wrapper;
        }

        public IReadOnlyCollection<TUncovered> AsIReadOnlyCollection { get; }

        public ICollection<TUncovered> AsReadOnlyICollection { get; }

        public int Count => Instance.Count;

        public bool IsReadOnly => Instance.IsReadOnly;

        private ICollection<TCovered> Instance => _wrapped.Invoke() ?? ArrayEx.Empty<TCovered>();

        public void Add(TUncovered item)
        {
            Instance.Add(_cover(item));
        }

        public void Clear()
        {
            Instance.Clear();
        }

        public bool Contains(TUncovered item)
        {
            return Instance.Contains(_cover(item));
        }

        public void CopyTo(TUncovered[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Instance.ConvertedCopyTo(_uncover, array, 0);
        }

        public void CopyTo(TUncovered[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Instance.ConvertedCopyTo(_uncover, array, arrayIndex);
        }

        public void CopyTo(TUncovered[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Instance.ConvertedCopyTo(_uncover, array, arrayIndex, countLimit);
        }

        public IEnumerator<TUncovered> GetEnumerator()
        {
            var collection = Instance;
            foreach (var item in collection)
            {
                if (!ReferenceEquals(collection, Instance))
                {
                    throw new InvalidOperationException();
                }

                yield return _uncover(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TUncovered item)
        {
            return Instance.Remove(_cover(item));
        }

        public bool Remove(TUncovered item, IEqualityComparer<TUncovered> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TUncovered>.Default;
            }

            return Instance.RemoveWhereEnumerable(input => comparer.Equals(_uncover(input), item)).Any();
        }

        public TUncovered[] ToArray()
        {
            var array = new TUncovered[Instance.Count];
            Instance.ConvertedCopyTo(_uncover, array, 0);
            return array;
        }
    }
}