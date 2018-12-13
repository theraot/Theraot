// Needed for Workaround

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.Core
{
    public static class ComparerExtensions
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            return !(comparer is ReverseComparer<T> originalAsReverse) ? new ReverseComparer<T>(comparer ?? Comparer<T>.Default) : originalAsReverse.Wrapped;
        }

        public static IComparer<T> ToComparer<T>(this Comparison<T> comparison)
        {
            // Replacement for Comparer.Create(Comparison<T>) added in .NET 4.5
            return new CustomComparer<T>(comparison);
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