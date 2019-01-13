#if LESSTHAN_NET45

namespace System.Reflection
{
    public static class RuntimeReflectionExtensions
    {
        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            return del.Method;
        }
    }
}

#endif