// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public class ProxyCollection<T> : ICollection<T>
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
    public class ProxyCollection<TUnderlying, TExposed> : ICollection<TExposed>
    {
        private readonly Func<TUnderlying, TExposed> _expose;
        private readonly Func<TExposed, TUnderlying> _unexpose;
        private readonly Func<ICollection<TUnderlying>> _wrapped;

        public ProxyCollection(Func<ICollection<TUnderlying>> wrapped, Func<TUnderlying, TExposed> expose, Func<TExposed, TUnderlying> unexpose)
        {
            _expose = expose ?? throw new ArgumentNullException(nameof(expose));
            _unexpose = unexpose ?? throw new ArgumentNullException(nameof(unexpose));
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            var wrapper = EnumerationList<TExposed>.Create(this);
            AsIReadOnlyCollection = wrapper;
            AsReadOnlyICollection = wrapper;
        }

        public IReadOnlyCollection<TExposed> AsIReadOnlyCollection { get; }

        public ICollection<TExposed> AsReadOnlyICollection { get; }

        public int Count => Instance.Count;

        public bool IsReadOnly => Instance.IsReadOnly;

        private ICollection<TUnderlying> Instance => _wrapped.Invoke() ?? ArrayEx.Empty<TUnderlying>();

        public void Add(TExposed item)
        {
            Instance.Add(_unexpose(item));
        }

        public void Clear()
        {
            Instance.Clear();
        }

        public bool Contains(TExposed item)
        {
            return Instance.Contains(_unexpose(item));
        }

        public void CopyTo(TExposed[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Instance.ConvertedCopyTo(_expose, array, 0);
        }

        public void CopyTo(TExposed[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Instance.ConvertedCopyTo(_expose, array, arrayIndex);
        }

        public void CopyTo(TExposed[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Instance.ConvertedCopyTo(_expose, array, arrayIndex, countLimit);
        }

        public IEnumerator<TExposed> GetEnumerator()
        {
            var collection = Instance;
            foreach (var item in collection)
            {
                if (!ReferenceEquals(collection, Instance))
                {
                    throw new InvalidOperationException();
                }

                yield return _expose(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TExposed item)
        {
            return Instance.Remove(_unexpose(item));
        }

        public bool Remove(TExposed item, IEqualityComparer<TExposed> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TExposed>.Default;
            }

            return Instance.RemoveWhereEnumerable(input => comparer.Equals(_expose(input), item)).Any();
        }

        public TExposed[] ToArray()
        {
            var array = new TExposed[Instance.Count];
            Instance.ConvertedCopyTo(_expose, array, 0);
            return array;
        }
    }
}