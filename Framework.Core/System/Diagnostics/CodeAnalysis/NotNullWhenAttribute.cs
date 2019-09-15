#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        public bool ReturnValue { get; }
    }
}

#endif