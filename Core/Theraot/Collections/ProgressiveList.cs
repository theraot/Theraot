// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public partial class ProgressiveList<T> : ProgressiveCollection<T>, IReadOnlyList<T>, IList<T>
    {
        private readonly IList<T> _cache;

        public ProgressiveList(IEnumerable<T> wrapped)
            : this(wrapped, new List<T>(), null)
        {
            // Empty
        }

        public ProgressiveList(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new List<T>(), comparer)
        {
            // Empty
        }

        protected ProgressiveList(IEnumerable<T> wrapped, IList<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped, cache, comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
        }

        protected ProgressiveList(Progressor<T> wrapped, IList<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped, cache, comparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
        }

        public T this[int index]
        {
            get
            {
                if (index >= _cache.Count)
                {
                    Progressor.While(() => _cache.Count < index + 1).Consume();
                }
                return _cache[index];
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        T IList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public int IndexOf(T item)
        {
            int cacheIndex = _cache.IndexOf(item, Comparer);
            if (cacheIndex >= 0)
            {
                return cacheIndex;
            }
            else
            {
                int index = _cache.Count - 1;
                bool found = false;
                Progressor.While
                (
                    input =>
                    {
                        index++;
                        if (Comparer.Equals(input, item))
                        {
                            found = true;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                ).Consume();
                if (found)
                {
                    return index;
                }
                else
                {
                    return -1;
                }
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
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        protected override bool CacheContains(T item)
        {
            return _cache.Contains(item, Comparer);
        }
    }
}