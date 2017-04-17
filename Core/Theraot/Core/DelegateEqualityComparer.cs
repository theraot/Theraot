#if FAT

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Theraot.Core
{
    public sealed class DelegateEqualityComparer : IEqualityComparer<Delegate>
    {
        private static readonly DelegateEqualityComparer _default = new DelegateEqualityComparer();

        private DelegateEqualityComparer()
        {
            // Empty
        }

        public static DelegateEqualityComparer Default
        {
            get { return _default; }
        }

        public bool Equals(Delegate x, Delegate y)
        {
            return CompareInternal(x, y);
        }

        public int GetHashCode(Delegate obj)
        {
            // obj can be null
            return ReferenceEquals(obj, null) ? 0 : ReferenceEquals(obj.Target, null) ? obj.GetMethodInfo().GetHashCode() : obj.GetMethodInfo().GetHashCode() ^ obj.Target.GetHashCode();
        }

        private static bool CompareInternal(Delegate x, Delegate y)
        {
            return ReferenceEquals(x, null) ? ReferenceEquals(y, null) : !ReferenceEquals(y, null) && ReferenceEquals(x.Target, y.Target) && x.GetMethodInfo() == y.GetMethodInfo();
        }
    }
}

#endif