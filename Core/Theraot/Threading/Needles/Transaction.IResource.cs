#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transaction
    {
        private interface IResource
        {
            bool Commit();

            IDisposable Lock();

            void Rollback();
        }
    }
}

#endif
