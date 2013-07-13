#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public interface ITransactionResource
    {
        bool Commit();

        IDisposable Lock();

        void Rollback();
    }
}

#endif