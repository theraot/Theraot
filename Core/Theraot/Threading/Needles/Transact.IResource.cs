#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        private interface IResource
        {
            void Capture(ref Needles.Needle<Thread> thread);

            bool Check();

            bool Commit();

            void Rollback();
        }
    }
}

#endif
