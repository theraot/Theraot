#if NET20 || NET30

namespace System.Threading
{
    internal static class ReaderWriterLockSlimExtensions
    {
        internal static bool Has(this LockState state, LockState value)
        {
            return (state & value) > 0;
        }

        internal static bool IsSet (this ManualResetEventSlim self)
        {
            return self.IsSet;
        }
    }

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