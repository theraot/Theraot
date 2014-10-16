#if FAT

using System.Threading;

namespace Theraot.Threading.Needles
{
    public interface ILockable
    {
        bool HasOwner { get; }

        bool Capture();

        bool CheckAccess(Thread thread);

        void Uncapture();
    }
}

#endif