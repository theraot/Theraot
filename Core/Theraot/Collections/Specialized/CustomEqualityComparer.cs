#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class CustomEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _comparison;
        private Func<T, int> _getHashCode;

        public CustomEqualityComparer(Func<T, T, bool> comparison, Func<T, int> getHashCode)
        {
            _comparison = Check.NotNullArgument(comparison, "comparison");
            _getHashCode = Check.NotNullArgument(getHashCode, "getHasCode");
        }

        public CustomEqualityComparer(IComparer<T> comparer, Func<T, int> getHashCode)
        {
            var  _comparer = Check.NotNullArgument(comparer, "comparer");
            _comparison = (x, y) => _comparer.Compare(x, y) == 0;
            _getHashCode = Check.NotNullArgument(getHashCode, "getHasCode");
        }

        public CustomEqualityComparer(Func<T, T, int> comparison, Func<T, int> getHashCode)
        {
            var  __comparison = Check.NotNullArgument(comparison, "comparison");
            _comparison = (x, y) => __comparison.Invoke(x, y) == 0;
            _getHashCode = Check.NotNullArgument(getHashCode, "getHasCode");
        }

        public CustomEqualityComparer(Comparison<T> comparison, Func<T, int> getHashCode)
        {
            var  __comparison = Check.NotNullArgument(comparison, "comparison");
            _comparison = (x, y) => __comparison.Invoke(x, y) == 0;
            _getHashCode = Check.NotNullArgument(getHashCode, "getHasCode");
        }

        public bool Equals(T x, T y)
        {
            return _comparison.Invoke(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode.Invoke(obj);
        }
    }
}

#endif