#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class AllowNullAttribute : Attribute
    {
        // Empty
    }
}

#endif