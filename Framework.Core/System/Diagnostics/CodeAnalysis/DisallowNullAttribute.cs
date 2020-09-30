#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

// ReSharper disable once RedundantAttributeUsageProperty

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    public sealed class DisallowNullAttribute : Attribute
    {
        // Empty
    }
}

#endif