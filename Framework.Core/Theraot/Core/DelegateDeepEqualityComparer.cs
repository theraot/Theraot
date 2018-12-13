#if FAT

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Theraot.Core
{
    public sealed class DelegateDeepEqualityComparer : IEqualityComparer<Delegate>
    {
        private DelegateDeepEqualityComparer()
        {
            //Empty
        }

        public static DelegateDeepEqualityComparer Default { get; } = new DelegateDeepEqualityComparer();

        public bool Equals(Delegate x, Delegate y)
        {
            return CompareInternal(x, y);
        }

        public int GetHashCode(Delegate obj)
        {
            // ReSharper disable once UseNullPropagation
            if (obj == null) // obj can be null
            {
                return 0; // TODO: Test coverage?
            }
            var methodBody = obj.GetMethodInfo().GetMethodBody();
            if (methodBody == null)
            {
                return 0;
            }
            var hash = 0;
            var tmp = 0;
            var body = methodBody.GetILAsByteArray();
            for (var index = 0; index < body.Length; index++)
            {
                ref var current = ref body[index];
                if (index % 4 == 0)
                {
                    hash = (hash << 5) - hash + tmp;
                    tmp = current;
                }
                else
                {
                    tmp = tmp << 8 | current;
                }
            }
            if (tmp != 0)
            {
                hash = (hash << 5) - hash + tmp;
            }
            var target = obj.Target;
            if (target != null)
            {
                hash ^= target.GetHashCode();
            }
            return hash;
        }

        private static bool CompareInternal(Delegate x, Delegate y)
        {
            if (x == null)
            {
                return y == null;
            }
            if (y == null)
            {
                return false;
            }
            if (x.Target != y.Target)
            {
                return false;
            }
            var leftBody = x.GetMethodInfo().GetMethodBody();
            var rightBody = y.GetMethodInfo().GetMethodBody();
            if (leftBody == null)
            {
                if (rightBody == null)
                {
                    return true;
                }
                return false;
            }
            if (rightBody == null)
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