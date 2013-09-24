#if FAT

namespace Theraot.Core
{
    public static class Reference
    {
        public static bool Equals<T>(T objA, T objB)
        {
            if (typeof(T).IsValueType)
            {
                return false;
            }
            else
            {
                return ReferenceEquals(objA, objB);
            }
        }

        public static bool IsNull<T>(T objA)
        {
            if (typeof(T).IsValueType)
            {
                return false;
            }
            else
            {
                return objA == null;
            }
        }
    }
}

#endif