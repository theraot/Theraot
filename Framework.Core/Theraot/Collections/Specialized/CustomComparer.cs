// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public sealed class CustomComparer<T> : Comparer<T>
    {
        private readonly Func<T, T, int> _comparison;

        public CustomComparer(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            _comparison = comparison.Invoke;
        }

        public override int Compare([AllowNull] T x, [AllowNull] T y)
        {
            return _comparison.Invoke(x!, y!);
        }
    }
}