using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    internal sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public static readonly HashSetEqualityComparer<T> Instance = new(equalityComparer: null);

        private readonly IEqualityComparer<T>? _equalityComparer;

        public HashSetEqualityComparer(IEqualityComparer<T>? equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool Equals([AllowNull] HashSet<T> x, [AllowNull] HashSet<T> y)
        {
            return SetEqualityComparer<T>.Equals(x, y, _equalityComparer);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public int GetHashCode(HashSet<T> obj)
        {
            return SetEqualityComparer<T>.GetHashCode(obj, _equalityComparer);
        }
    }
}