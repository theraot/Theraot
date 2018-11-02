#if FAT

namespace Theraot.Core
{
    public interface ICloner<T>
    {
        T Clone(T target);
    }
}

#endif