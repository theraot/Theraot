#if LESSTHAN_NET40 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct, Inherited = false)]
    public sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
        // Empty
    }
}

#endif