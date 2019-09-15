#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    public sealed class MaybeNullAttribute : Attribute
    {
        // Empty
    }
}

#endif