#if FAT

using System;

namespace Theraot.Core
{
#if NETCF

    public interface ICloneable<T>
#if !NETCOREAPP1_1
    : ICloneable
#endif
#else

    public interface ICloneable<out T>
#if !NETCOREAPP1_1
        : ICloneable
#endif
#endif
    {
#if !NETCOREAPP1_1
        new
#endif

        T Clone();
    }
}

#endif