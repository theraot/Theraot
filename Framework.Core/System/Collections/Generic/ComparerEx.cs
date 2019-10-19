using System.Runtime.CompilerServices;

#if LESSTHAN_NET45

using Theraot.Collections.Specialized;

#endif

namespace System.Collections.Generic
{
    /// <summary>
    /// Defines the <see cref="ComparerEx" />
    /// </summary>
    public static class ComparerEx
    {
        /// <summary>
        /// Creates a comparer by using the specified comparison.
        /// </summary>
        /// <typeparam name="T">The type of items compared.</typeparam>
        /// <param name="comparison">The comparison<see cref="Comparison{T}"/></param>
        /// <returns>The new <see cref="Comparer{T}"/>.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Comparer<T> Create<T>(Comparison<T> comparison)
        {
#if LESSTHAN_NET45
            return new CustomComparer<T>(comparison);
#else
            return Comparer<T>.Create(comparison);
#endif
        }
    }
}