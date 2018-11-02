#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public sealed partial class ExtendedReadOnlyCollection<T> : IExtendedReadOnlyCollection<T>, IExtendedCollection<T>
    {
        bool IExtendedCollection<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
    }
}

#endif