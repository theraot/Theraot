#if LESSTHAN_NET45

using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class PropertyInfoTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static object GetValue(this PropertyInfo info, object obj)
        {
            return info.GetValue(obj, null);
        }
    }
}

#elif LESSTHAN_NETSTANDARD15

using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class PropertyInfoTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod;
        }
    }
}

#endif
