// Needed for Workaround

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class CustomEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparison;
        private readonly Func<T, int> _getHashCode;

        public CustomEqualityComparer(Func<T, T, bool> comparison, Func<T, int> getHashCode)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            if (getHashCode == null)
            {
                throw new ArgumentNullException("getHashCode");
            }
            _comparison = comparison;
            _getHashCode = getHashCode;
        }

        public CustomEqualityComparer(IComparer<T> comparer, Func<T, int> getHashCode)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (getHashCode == null)
            {
                throw new ArgumentNullException("getHashCode");
            }
            _comparison = (x, y) => comparer.Compare(x, y) == 0;
            _getHashCode = getHashCode;
        }

        public CustomEqualityComparer(Func<T, T, int> comparison, Func<T, int> getHashCode)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            if (getHashCode == null)
            {
                throw new ArgumentNullException("getHashCode");
            }
            _comparison = (x, y) => comparison.Invoke(x, y) == 0;
            _getHashCode = getHashCode;
        }

        public CustomEqualityComparer(Comparison<T> comparison, Func<T, int> getHashCode)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            if (getHashCode == null)
            {
                throw new ArgumentNullException("getHashCode");
            }
            _comparison = (x, y) => comparison.Invoke(x, y) == 0;
            _getHashCode = getHashCode;
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