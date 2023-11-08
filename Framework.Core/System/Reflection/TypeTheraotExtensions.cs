using System.Diagnostics.CodeAnalysis;

namespace System.Reflection;

public static class TypeTheraotExtensions
{
#if NETFRAMEWORK && !NET45_OR_GREATER
    public static Type[] GenericTypeArguments(this Type @this) => @this.GetGenericArguments();
#endif
#if !NET5_0_OR_GREATER
    // [Intrinsic]
    public static bool IsAssignableTo(this Type @this, [NotNullWhen(true)]Type? targetType)
        => targetType?.IsAssignableFrom(@this) ?? false;
#endif
}
