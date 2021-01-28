using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    internal sealed class SortedSetEqualityComparer<T> : IEqualityComparer<SortedSet<T>>
    {
        public static readonly SortedSetEqualityComparer<T> Instance = new(null);

        private readonly IEqualityComparer<T>? _equalityComparer;

        public SortedSetEqualityComparer(IEqualityComparer<T>? equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool Equals([AllowNull] SortedSet<T> x, [AllowNull] SortedSet<T> y)
        {
            return SetEqualityComparer<T>.Equals(x, y, _equalityComparer);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public int GetHashCode(SortedSet<T> obj)
        {
            return SetEqualityComparer<T>.GetHashCode(obj, _equalityComparer);
        }
    }
}