#if LESSTHAN_NET35

#pragma warning disable RCS1154 // Sort enum members.

namespace System.Threading
{
    [Flags]
    internal enum LockState
    {
        None = 0,
        Upgradable = 1,
        Read = 2,
        Write = 4,
        UpgradedRead = Upgradable | Read,
        UpgradedWrite = Upgradable | Write
    }

    internal class ThreadLockState
    {
        public LockState LockState;
        public int ReaderRecursiveCount;
        public int UpgradeableRecursiveCount;
        public int WriterRecursiveCount;
    }
}

#endif