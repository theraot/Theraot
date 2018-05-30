#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed partial class ReentrantReadWriteLock : IReadWriteLock
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