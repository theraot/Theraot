#if FAT

namespace Theraot.Threading
{
    internal sealed partial class NoReentrantReadWriteLock
    {
        private enum Status
        {
            WriteRequested = -2,
            WriteMode = -1,
            Free = 0,
            ReadMode = 1
        }
    }
}

#endif