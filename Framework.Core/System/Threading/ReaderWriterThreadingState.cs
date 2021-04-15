#if LESSTHAN_NET35

namespace System.Threading
{
    internal class ReaderWriterThreadingState
    {
        public ReaderWriterLockState LockState;
        public int ReaderRecursiveCount;
        public int UpgradeableRecursiveCount;
        public int WriterRecursiveCount;
    }
}

#endif