#if FAT

namespace Theraot.Core
{
    public static class Reference
    {
        public static bool Equals<T>(T objA, T objB)
        {
            return typeof(T).IsValueType ? false : ReferenceEquals(objA, objB);
        }

        public static bool IsNull<T>(T objA)
        {
            return typeof(T).IsValueType ? false : ReferenceEquals(objA, null);
        }
    }
}

#endif