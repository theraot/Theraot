// Needed for Workaround

using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static class ComparerExtensions
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            return !(comparer is ReverseComparer<T> originalAsReverse) ? new ReverseComparer<T>(comparer ?? Comparer<T>.Default) : originalAsReverse.Wrapped;
        }

        private sealed class ReverseComparer<T> : IComparer<T>
        {
            public ReverseComparer(IComparer<T> wrapped)
            {
                Wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            }

            internal IComparer<T> Wrapped { get; }

            public int Compare(T x, T y)
            {
                return Wrapped.Compare(y, x);
            }
        }
    }
}