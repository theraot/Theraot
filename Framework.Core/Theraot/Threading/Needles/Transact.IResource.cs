#if FAT

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        private interface IResource
        {
            bool Capture();

            bool CheckCapture();

            bool CheckValue();

            bool Commit();

            void Release();
        }
    }
}

#endif