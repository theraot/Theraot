#if LESSTHAN_NET45 || NETSTANDARD1_0

namespace System.Collections.Concurrent
{
    [Flags]
    public enum EnumerablePartitionerOptions
    {
        None = 0,
        NoBuffering = 1
    }
}

#endif