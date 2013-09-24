#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class SpecialCollection<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly Func<T, bool> _contains;
        private readonly Func<int> _count;
        private readonly Func<IEnumerator<T>> _getEnumeratorOfT;

        private SpecialCollection(Func<IEnumerator<T>> getEnumerator, Func<int> count, Func<T, bool> contains)
        {
            _getEnumeratorOfT = Check.NotNullArgument(getEnumerator, "getEnumerator");
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

        public static SpecialCollection<T> CreateFromArray(T[] array)
        {
            var delegateEnumerable = new EnumerableFromDelegate<T>(array.GetEnumerator);
            return new SpecialCollection<T>
                   (
                       () => delegateEnumerable.GetEnumerator(),
                       () =>
                       {
                           return array.Length;
                       },
            (T item) =>
            {
                return Array.IndexOf(array, item) >= 0;
            }
                   );
        }

        public static SpecialCollection<T> CreateFromCollection(ICollection<T> obj)
        {
            return new SpecialCollection<T>
                   (
                       obj.GetEnumerator,
                       () =>
                       {
                           return obj.Count;
                       },
            obj.Contains
                   );
        }

        public bool Contains(T item)
        {
            return _contains.Invoke(item);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return System.Linq.Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(T[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _getEnumeratorOfT.Invoke();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _getEnumeratorOfT.Invoke();
        }
    }
}

#endif