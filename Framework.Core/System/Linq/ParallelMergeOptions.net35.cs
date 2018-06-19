#if NET20 || NET30 || NET35

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