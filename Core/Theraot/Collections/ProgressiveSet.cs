using System;
using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Collections
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveSet<T> : ProgressiveCollection<T>, IExtendedReadOnlySet<T>, IExtendedSet<T>, ISet<T>
    {
        public ProgressiveSet(IEnumerable<T> wrapped)
            : this(wrapped, new ExtendedSet<T>(), null)
        {
            //Empty
        }

        public ProgressiveSet(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new ExtendedSet<T>(comparer), null)
        {
            //Empty
        }

        internal ProgressiveSet(IProgressor<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new ExtendedSet<T>(comparer), comparer)
        {
            //Empty
        }

        protected ProgressiveSet(IEnumerable<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
            : this(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), cache, comparer)
        {
            //Empty
        }

        protected ProgressiveSet(IProgressor<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
            : base((out T value) =>
            {
            again:
                if (wrapped.TryTake(out value))
                {
                    if (cache.Contains(value))
                    {
                        goto again;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }, cache, comparer)
        {
            //Empty
        }

        private ProgressiveSet(IEnumerator<T> enumerator, ISet<T> cache, IEqualityComparer<T> comparer)
            : base((out T value) =>
            {
            again:
                if (enumerator.MoveNext())
                {
                    value = enumerator.Current;
                    if (cache.Contains(value))
                    {
                        goto again;
                    }
                    return true;
                }
                else
                {
                    enumerator.Dispose();
                    value = default(T);
                    return false;
                }
            }, cache, comparer)
        {
            //Empty
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns True")]
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        IReadOnlySet<T> IExtendedSet<T>.AsReadOnly
        {
            get
            {
                return this;
            }
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
        bool IExtendedSet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool IExtendedSet<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}