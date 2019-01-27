#if LESSTHAN_NET40
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names

namespace System.Linq
{
    public enum ParallelMergeOptions
    {
        Default = 0,
        NotBuffered = 1,
        AutoBuffered = 2,
        FullyBuffered = 3
    }
}

#endif