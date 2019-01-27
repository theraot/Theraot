#if FAT
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Theraot.Core
{
    public sealed class DelegateEqualityComparer : IEqualityComparer<Delegate>
    {
        private DelegateEqualityComparer()
        {
            // Empty
        }

        public static DelegateEqualityComparer Default { get; } = new DelegateEqualityComparer();

        public bool Equals(Delegate x, Delegate y)
        {
            return CompareInternal(x, y);
        }

        public int GetHashCode(Delegate obj)
        {
            // obj can be null
            return obj == null ? 0 : obj.Target == null ? obj.GetMethodInfo().GetHashCode() : obj.GetMethodInfo().GetHashCode() ^ obj.Target.GetHashCode();
        }

        private static bool CompareInternal(Delegate x, Delegate y)
        {
            return x == null ? y == null : y != null && x.Target == y.Target && x.GetMethodInfo() == y.GetMethodInfo();
        }
    }
}

#endif