#if LESSTHAN_NET45

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