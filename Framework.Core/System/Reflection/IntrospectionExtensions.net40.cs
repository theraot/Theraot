#if NET20 || NET30 || NET35 || NET40

namespace System.Reflection
{
    public static class IntrospectionExtensions
    {
        public static Type GetTypeInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type;
        }
    }
}

#endif