#if NET35

namespace System.Linq
{
    public enum ParallelMergeOptions
    {
        Default = 0,
        NotBuffered,
        AutoBuffered,
        FullyBuffered
    }
}

#endif