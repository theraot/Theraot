#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public partial class ProgressiveSet<T> : IExtendedReadOnlySet<T>, IExtendedSet<T>
    {
        IReadOnlySet<T> IExtendedSet<T>.AsReadOnly
        {
            get { return this; }
        }

        bool IExtendedSet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool IExtendedSet<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
    }
}

#endif