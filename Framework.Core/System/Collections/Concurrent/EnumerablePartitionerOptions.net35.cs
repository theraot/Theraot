#if NET20 || NET30 || NET35

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