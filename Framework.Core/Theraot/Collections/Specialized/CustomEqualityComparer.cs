// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public class CustomEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparison;

        private readonly Func<T, int> _getHashCode;

        public CustomEqualityComparer(Func<T, T, bool> comparison, Func<T, int> getHashCode)
        {
            _comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));
            _getHashCode = getHashCode ?? throw new ArgumentNullException(nameof(getHashCode));
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