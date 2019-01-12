#if LESSTHAN_NET40 || NETSTANDARD1_0

namespace System.Collections.Concurrent
{
    [Flags]
    public enum EnumerablePartitionerOptions
    {
        None = 0x0,
        NoBuffering = 0x1
    }
}

#endif