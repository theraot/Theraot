#if NET20 || NET30 || NET35

namespace System.Linq
{
    public enum ParallelExecutionMode
    {
        Default = 0,
        ForceParallelism = 1
    }
}

#endif