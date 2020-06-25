using Theraot.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    internal sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public static readonly HashSetEqualityComparer<T> Instance = new HashSetEqualityComparer<T>();

        public bool Equals
        (
            [AllowNull]
            HashSet<T> x,
            [AllowNull]
            HashSet<T> y
        )
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            foreach (var item in x)
            {
                if (!y.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public int GetHashCode(HashSet<T> obj)
        {
            return Extensions.ComputeHashCode(obj);
        }
    }
}