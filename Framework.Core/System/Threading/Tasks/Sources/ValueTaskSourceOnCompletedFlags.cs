#if LESSTHAN_NET45

#pragma warning disable S2344 // Rename this enumeration to remove the 'Flags' suffix.

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