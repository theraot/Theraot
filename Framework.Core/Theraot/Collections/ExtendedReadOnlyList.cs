// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class ExtendedReadOnlyList<T> : ReadOnlyCollection<T>, IReadOnlyList<T>, ICollection<T>
    {
        private readonly IList<T> _wrapped;

        public ExtendedReadOnlyList(IList<T> wrapped)
            : base(wrapped)
        {
            _wrapped = wrapped;
        }

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(T[] array)
        {
            _wrapped.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public T[] ToArray()
        {
            var array = new T[_wrapped.Count];
            CopyTo(array);
            return array;
        }
    }
}