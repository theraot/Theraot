// Needed for Workaround

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

namespace Theraot.Core
{
    public static class ComparerExtensions
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            return !(comparer is ReverseComparer<T> originalAsReverse) ? new ReverseComparer<T>(comparer ?? Comparer<T>.Default) : originalAsReverse.Wrapped;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif

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