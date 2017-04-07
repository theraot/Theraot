#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    public partial class EnumerationCollection<T> : IExtendedCollection<T>
    {
        IReadOnlyCollection<T> IExtendedCollection<T>.AsReadOnly
        {
            get { return this; }
        }

        bool IExtendedCollection<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
    }
}

#endif