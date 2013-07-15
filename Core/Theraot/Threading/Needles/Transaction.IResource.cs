#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transaction
    {
        private interface IResource
        {
            bool Check();

            bool Commit();

            void Capture(ref Needles.Needle<Thread> thread);

            void Rollback();
        }
    }
}

#endif
