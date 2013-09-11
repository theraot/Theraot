using System;
using System.Collections.Generic;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class NeedleConversionEqualityComparer<TNeedle, T> : IEqualityComparer<TNeedle>
        where TNeedle : INeedle<T>
    {
        private IEqualityComparer<T> _comparer;

        public NeedleConversionEqualityComparer(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(TNeedle x, TNeedle y)
        {
            return _comparer.Equals(Conversion(x), Conversion(y));
        }

        public int GetHashCode(TNeedle obj)
        {
            return _comparer.GetHashCode(Conversion(obj));
        }

        private static T Conversion(TNeedle needle)
        {
            if (ReferenceEquals(needle, null))
            {
                return default(T);
            }
            else
            {
                return needle.Value;
            }
        }
    }
}
