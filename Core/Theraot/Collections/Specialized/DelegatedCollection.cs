// Needed for NET40

using System;
using System.Collections.Generic;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [System.Diagnostics.DebuggerNonUserCode]
    public partial class DelegatedCollection<T> : ICollection<T>
    {
        private readonly IReadOnlyCollection<T> _readOnly;
        private readonly Func<ICollection<T>> _wrapped;

        public DelegatedCollection(Func<ICollection<T>> wrapped)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _readOnly = new ExtendedReadOnlyCollection<T>(this);
        }

        public IReadOnlyCollection<T> AsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        public int Count
        {
            get
            {
                return Instance.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return Instance.IsReadOnly;
            }
        }

        private ICollection<T> Instance
        {
            get
            {
                return _wrapped.Invoke() ?? ArrayReservoir<T>.EmptyArray;
            }
        }

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

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return System.Linq.Enumerable.Contains(Instance, item, comparer);
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
            ICollection<T> collection = Instance;
            foreach (var item in collection)
            {
                if (!ReferenceEquals(collection, Instance))
                {
                    throw new InvalidOperationException();
                }
                yield return item;
            }
        }

        public bool Remove(T item)
        {
            return Instance.Remove(item);
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            foreach (var _item in Instance.RemoveWhereEnumerable(input => _comparer.Equals(input, item)))
            {
                GC.KeepAlive(_item);
                return true;
            }
            return false;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            var array = new T[Instance.Count];
            Instance.CopyTo(array, 0);
            return array;
        }
    }
}