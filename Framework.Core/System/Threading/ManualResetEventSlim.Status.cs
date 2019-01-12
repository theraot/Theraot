#if LESSTHAN_NET40

namespace System.Threading
{
    public partial class ManualResetEventSlim
    {
        private enum Status
        {
            Disposed = -1,
            NotSet = 0,
            Set = 1,
            HandleRequestedNotSet = 2,
            HandleRequestedSet = 3,
            HandleReadyNotSet = 4,
            HandleReadySet = 5
        }
    }
}

#endif