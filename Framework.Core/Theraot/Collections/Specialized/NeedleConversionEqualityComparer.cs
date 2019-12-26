// Needed for NET35 (ConditionalWeakTable)

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public sealed class NeedleConversionEqualityComparer<TNeedle, T> : ConversionEqualityComparer<TNeedle, T>
        where TNeedle : INeedle<T>
    {
        public NeedleConversionEqualityComparer(IEqualityComparer<T> comparer)
            : base(comparer, Conversion)
        {
            // Empty
        }

        [return: NotNullIfNotNull("needle")]
        private static T Conversion(TNeedle needle)
        {
            if (needle == null)
            {
                return default!;
            }

            if (needle is ICacheNeedle<T> cacheNeedle && cacheNeedle.TryGetValue(out var value))
            {
                return value;
            }

            return needle.Value;
        }
    }
}