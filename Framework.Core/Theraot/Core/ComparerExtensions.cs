// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Theraot.Core
{
    public static class ComparerExtensions
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            return comparer is ReverseComparer<T> originalAsReverse ? originalAsReverse.Wrapped : new ReverseComparer<T>(comparer ?? Comparer<T>.Default);
        }

        private sealed class ReverseComparer<T> : IComparer<T>
        {
            public ReverseComparer(IComparer<T> wrapped)
            {
                Wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            }

            internal IComparer<T> Wrapped { get; }

            public int Compare([AllowNull] T x, [AllowNull] T y)
            {
                return Wrapped.Compare(y!, x!);
            }
        }
    }
}