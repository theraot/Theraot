#if NET20 || NET30 || NET35 || NET40

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