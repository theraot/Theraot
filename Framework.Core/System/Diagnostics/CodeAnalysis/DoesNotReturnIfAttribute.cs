#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue)
        {
            ParameterValue = parameterValue;
        }

        public bool ParameterValue { get; }
    }
}

#endif