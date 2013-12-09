using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class EnumerationCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IExtendedReadOnlyCollection<T>, IExtendedCollection<T>, IEnumerable<T>
    {
        private readonly Func<int> _count;
        private readonly IEnumerable<T> _wrapped;

        public EnumerationCollection(IEnumerable<T> wrapped, Func<int> count)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = Check.NotNullArgument(count, "count");
        }

        public EnumerationCollection(IEnumerable<T> wrapped)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _count = () => Enumerable.Count(_wrapped);
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        IReadOnlyCollection<T> IExtendedCollection<T>.AsReadOnly
        {
            get
            {
                return this;
            }
        }

        public bool Contains(T item)
        {
            return System.Linq.Enumerable.Contains(_wrapped, item, EqualityComparer<T>.Default);
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool IExtendedCollection<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
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