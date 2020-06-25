// Needed for NET30

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Theraot.Core
{
    public static class ReferenceEqualityComparer
    {
        public static ReferenceEqualityComparer<object> Default { get; } = ReferenceEqualityComparer<object>.Instance;
    }

    public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        private ReferenceEqualityComparer()
        {
            // Empty
        }

        public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();

        bool IEqualityComparer<T>.Equals
        (
            [AllowNull]
            T x,
            [AllowNull]
            T y
        )
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode
        (
#if GREATERTHAN_NETCOREAPP22
            [DisallowNull]
#endif
            T obj
        )
        {
            if (obj == null!)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }
    }
}