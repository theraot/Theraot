#if FAT

using System;

namespace Theraot.Core
{
    public interface ICloneable<out T>
#if !NETCOREAPP1_0 && !NETCOREAPP1_1
        : ICloneable
#endif
    {
#if !NETCOREAPP1_0 && !NETCOREAPP1_1

        new
#endif

        T Clone();
    }
}

#endif