// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public class ProxyCollection<T> : ICollection<T>
    {
        private readonly Func<ICollection<T>> _wrapped;

        public ProxyCollection(Func<ICollection<T>> wrapped)
        {
            _wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            var wrapper = new EnumerationList<T>(this);
            AsIReadOnlyCollection = wrapper;
            AsReadOnlyICollection = wrapper;
        }

        public IReadOnlyCollection<T> AsIReadOnlyCollection { get; }

        public ICollection<T> AsReadOnlyICollection { get; }

        public int Count => Instance.Count;

        public bool IsReadOnly => Instance.IsReadOnly;

        private ICollection<T> Instance => _wrapped.Invoke() ?? ArrayReservoir<T>.EmptyArray;

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

        public void CopyTo(T[] array, int arrayIndex)
        {
            Instance.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array)
        {
            Instance.CopyTo(array, 0);
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
            foreach (var foundItem in Instance.RemoveWhereEnumerable(input => comparer.Equals(input, item)))
            {
                GC.KeepAlive(foundItem);
                return true;
            }
            return false;
        }

        public T[] ToArray()
        {
            var array = new T[Instance.Count];
            Instance.CopyTo(array, 0);
            return array;
        }
    }
}