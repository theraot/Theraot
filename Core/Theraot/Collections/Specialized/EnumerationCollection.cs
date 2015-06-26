// Needed for NET30

using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class EnumerationCollection<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private readonly Func<T, bool> _contains;
        private readonly Func<int> _count;
        private readonly IEnumerable<T> _wrapped;

        public EnumerationCollection(IEnumerable<T> wrapped)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = () => _wrapped.Count();
            _contains = item => _wrapped.Contains(item, EqualityComparer<T>.Default);
        }

        public EnumerationCollection(T[] wrapped)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = () => wrapped.Length;
            _contains = item => Array.IndexOf(wrapped, item) >= 0;
        }

        public EnumerationCollection(ICollection<T> wrapped)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = () => wrapped.Count;
            _contains = wrapped.Contains;
        }

        public EnumerationCollection(IEnumerable<T> wrapped, Func<int> count)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = Check.NotNullArgument(count, "count");
            _contains = item => _wrapped.Contains(item, EqualityComparer<T>.Default);
        }

        public EnumerationCollection(IEnumerable<T> wrapped, Func<T, bool> contains)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = () => _wrapped.Count();
            _contains = Check.NotNullArgument(contains, "contains");
        }

        public EnumerationCollection(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> contains)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = Check.NotNullArgument(count, "count");
            _contains = Check.NotNullArgument(contains, "contains");
        }

        public int Count
        {
            get
            {
                return _count.Invoke();
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns true")]
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Contains(T item)
        {
            return _contains.Invoke(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return System.Linq.Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
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

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(T item)
        {
            return _wrapped.IndexOf(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            var array = new T[_count.Invoke()];
            CopyTo(array);
            return array;
        }
    }
}