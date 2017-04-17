#if FAT

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Theraot.Core
{
    public sealed class DelegateDeepEqualityComparer : IEqualityComparer<Delegate>
    {
        private static readonly DelegateDeepEqualityComparer _default = new DelegateDeepEqualityComparer();

        private DelegateDeepEqualityComparer()
        {
            //Empty
        }

        public static DelegateDeepEqualityComparer Default
        {
            get { return _default; }
        }

        public bool Equals(Delegate x, Delegate y)
        {
            return CompareInternal(x, y);
        }

        public int GetHashCode(Delegate obj)
        {
            if (ReferenceEquals(obj, null)) // obj can be null
            {
                return 0;
            }
            var methodBody = obj.GetMethodInfo().GetMethodBody();
            if (ReferenceEquals(methodBody, null))
            {
                return 0;
            }
            var hash = 0;
            var tmp = 0;
            var body = methodBody.GetILAsByteArray();
            for (var index = 0; index < body.Length; index++)
            {
                if (index % 4 == 0)
                {
                    hash = (hash << 5) - hash + tmp;
                    tmp = body[index];
                }
                else
                {
                    tmp = tmp << 8 | body[index];
                }
            }
            if (tmp != 0)
            {
                hash = (hash << 5) - hash + tmp;
            }
            var target = obj.Target;
            if (!ReferenceEquals(target, null))
            {
                hash ^= target.GetHashCode();
            }
            return hash;
        }

        private static bool CompareInternal(Delegate x, Delegate y)
        {
            if (ReferenceEquals(x, null))
            {
                return ReferenceEquals(y, null);
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            if (!ReferenceEquals(x.Target, y.Target))
            {
                return false;
            }
            var leftBody = x.GetMethodInfo().GetMethodBody();
            var rightBody = y.GetMethodInfo().GetMethodBody();
            if (ReferenceEquals(leftBody, null))
            {
                if (ReferenceEquals(rightBody, null))
                {
                    return true;
                }
                return false;
            }
            if (ReferenceEquals(rightBody, null))
            {
                return false;
            }
            var leftBodyCode = leftBody.GetILAsByteArray();
            var rightBodyCode = rightBody.GetILAsByteArray();
            if (leftBodyCode.Length != rightBodyCode.Length)
            {
                return false;
            }
            for (var index = 0; index < leftBodyCode.Length; index++)
            {
                if (leftBodyCode[index] != rightBodyCode[index])
                {
                    return false;
                }
            }
            return true;
        }
    }
}

#endif