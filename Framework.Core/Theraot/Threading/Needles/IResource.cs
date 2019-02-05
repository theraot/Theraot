#if FAT
namespace Theraot.Threading.Needles
{
    internal interface IResource
    {
        bool Capture();

        bool CheckCapture();

        bool CheckValue();

        bool Commit();

        void Release();
    }
}

#endif