using System;
using System.Collections.Generic;
using System.Text;

namespace Theraot.Core
{
    public sealed class DelegateEqualityComparer : IEqualityComparer<Delegate>
    {
        private static readonly DelegateEqualityComparer _instance = new DelegateEqualityComparer();

        private DelegateEqualityComparer()
        {
            //Empty
        }

        public static DelegateEqualityComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Equals(Delegate x, Delegate y)
        {
            return CompareInternal(x, y);
        }

        public int GetHashCode(Delegate obj)
        {
            return obj.Method.GetHashCode();
        }

        private static bool CompareInternal(Delegate x, Delegate y)
        {
            if (ReferenceEquals(x, null))
            {
                if (ReferenceEquals(y, null))
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
                        var leftBody = x.Method.GetMethodBody().GetILAsByteArray();
                        var rightBody = y.Method.GetMethodBody().GetILAsByteArray();
                        if (leftBody.Length != rightBody.Length)
                        {
                            return false;
                        }
                        else
                        {
                            for (var index = 0; index < leftBody.Length; index++)
                            {
                                if (leftBody[index] != rightBody[index])
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
