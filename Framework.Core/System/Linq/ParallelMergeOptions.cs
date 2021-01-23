#if LESSTHAN_NET40

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