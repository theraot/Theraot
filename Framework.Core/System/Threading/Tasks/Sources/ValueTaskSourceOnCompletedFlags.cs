#if LESSTHAN_NET45

#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes

namespace System.Threading.Tasks.Sources
{
    [Flags]
    public enum ValueTaskSourceOnCompletedFlags
    {
        None,
        UseSchedulingContext,
        FlowExecutionContext
    }
}

#endif