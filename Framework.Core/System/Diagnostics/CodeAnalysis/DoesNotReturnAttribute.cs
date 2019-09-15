#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DoesNotReturnAttribute : Attribute
    {
        // Empty
    }
}

#endif