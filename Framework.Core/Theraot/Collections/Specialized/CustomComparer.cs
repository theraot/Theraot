// Needed for Workaround

using System;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class CustomComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparison;

        public CustomComparer(Func<T, T, int> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            _comparison = comparison;
        }

        public CustomComparer(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            _comparison = comparison.Invoke;
        }

        public int Compare(T x, T y)
        {
            return _comparison.Invoke(x, y);
        }
    }
}