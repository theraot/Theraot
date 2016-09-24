#if FAT

using System;

namespace Theraot.Core
{
#if NETCF
    public interface ICloneable<T> : ICloneable
#else
    public interface ICloneable<out T> : ICloneable
#endif
    {
        new T Clone();
    }
}

#endif