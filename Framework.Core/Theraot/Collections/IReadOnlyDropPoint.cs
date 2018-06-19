#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IReadOnlyDropPoint<T> : IReadOnlyCollection<T>
    {
        T Item { get; }
    }
}

#endif