using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    public class EnumerationCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IExtendedReadOnlyCollection<T>, IExtendedCollection<T>, IEnumerable<T>
    {
        private IEnumerable<T> _wrapped;

        public EnumerationCollection(IEnumerable<T> wrapped)
        {
            _wrapped = Check.NotNullArgument(wrapped, "wrapped");
        }

        public int Count
        {
            get
            {
                return Enumerable.Count(_wrapped);
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

        protected IEnumerable<T> Wrapped
        {
            get
            {
                return _wrapped;
            }
        }

        public bool Contains(T item)
        {
            return Enumerable.Contains(_wrapped, item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return _wrapped.Contains(item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CopyTo(_wrapped, array);
        }

        public void CopyTo(T[] array)
        {
            Extensions.CopyTo(_wrapped, array);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(_wrapped, array, arrayIndex, countLimit);
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
    }
}