#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IExtendedReadOnlyList<T> : IReadOnlyList<T>
    {
        int IndexOf(T item);
    }
}

#endif