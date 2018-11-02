#if FAT

using System.Reflection;

namespace Theraot.Core
{
    public static class Reference
    {
        public static bool Equals<T>(T objA, T objB)
        {
            var info = typeof(T).GetTypeInfo();
            return info.IsValueType ? false : ReferenceEquals(objA, objB);
        }

        public static bool IsNull<T>(T objA)
        {
            var info = typeof(T).GetTypeInfo();
            return info.IsValueType ? false : ReferenceEquals(objA, null);
        }
    }
}

#endif