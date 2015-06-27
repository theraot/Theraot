#if FAT

namespace Theraot.Collections.ThreadSafe
{
    internal enum BucketStatus
    {
        Free = 0,
        GrowRequested = 1,
        Waiting = 2,
        Copy = 3,
        CopyCleanup = 4
    }
}

#endif