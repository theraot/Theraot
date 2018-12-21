#if FAT

using System;

namespace Theraot.Core
{
    public interface ICloneable<out T>
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2
        : ICloneable
#endif
    {
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

        new
#endif

        T Clone();
    }
}

#endif