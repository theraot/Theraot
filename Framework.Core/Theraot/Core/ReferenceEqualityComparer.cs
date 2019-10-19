// Needed for NET30

using System;
using System.Collections.Generic;

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
        }

        public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null!)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            return obj.GetHashCode();
        }
    }
}