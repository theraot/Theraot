// Needed for NET30

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
            // Empty
        }

        public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }
    }
}