// Needed for NET30

using System.Collections.Generic;

namespace Theraot.Core
{
    public static class ReferenceEqualityComparer
    {
        private static readonly ReferenceEqualityComparer<object> _default = ReferenceEqualityComparer<object>.Instance;

        public static ReferenceEqualityComparer<object> Default
        {
            get { return _default; }
        }
    }

    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        private static readonly ReferenceEqualityComparer<T> _instance = new ReferenceEqualityComparer<T>();

        private ReferenceEqualityComparer()
        {
            // Empty
        }

        public static ReferenceEqualityComparer<T> Instance
        {
            get { return _instance; }
        }

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