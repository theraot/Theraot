#if LESSTHAN_NET35

namespace System.Threading
{
    [Flags]
    internal enum ReaderWriterLockState
    {
        None = 0,
        Upgradable = 1,
        Read = 2,
        Write = 4
    }
}

#endif