#if FAT

using System.Collections.Generic;

namespace Theraot.Core
{
    public class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        private static readonly ReferenceEqualityComparer _default = new ReferenceEqualityComparer();

        private ReferenceEqualityComparer()
        {
            // Empty
        }

        public static ReferenceEqualityComparer Default
        {
            get
            {
                return _default;
            }
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
    }
}

#endif