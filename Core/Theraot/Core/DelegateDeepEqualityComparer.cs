using System;
using System.Collections.Generic;

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
            get
            {
                return _default;
            }
        }

        public bool Equals(Delegate x, Delegate y)
        {
            return CompareInternal(x, y);
        }

        public int GetHashCode(Delegate obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }
            else
            {
                var methodBody = obj.Method.GetMethodBody();
                if (ReferenceEquals(methodBody, null))
                {
                    return 0;
                }
                else
                {
                    int hash = 0;
                    int tmp = 0;
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
                        hash = hash ^ target.GetHashCode();
                    }
                    return hash;
                }
            }
        }

        private static bool CompareInternal(Delegate x, Delegate y)
        {
            if (ReferenceEquals(x, null))
            {
                return ReferenceEquals(y, null);
            }
            else
            {
                if (ReferenceEquals(y, null))
                {
                    return false;
                }
                else
                {
                    if (!ReferenceEquals(x.Target, y.Target))
                    {
                        return false;
                    }
                    else
                    {
                        var leftBody = x.Method.GetMethodBody();
                        var rightBody = y.Method.GetMethodBody();
                        if (ReferenceEquals(leftBody, null))
                        {
                            if (ReferenceEquals(rightBody, null))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (ReferenceEquals(rightBody, null))
                            {
                                return false;
                            }
                            else
                            {
                                var leftBodyCode = leftBody.GetILAsByteArray();
                                var rightBodyCode = rightBody.GetILAsByteArray();
                                if (leftBodyCode.Length != rightBodyCode.Length)
                                {
                                    return false;
                                }
                                else
                                {
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
                    }
                }
            }
        }
    }
}