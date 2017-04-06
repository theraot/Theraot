#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IReadOnlySet<T> : IReadOnlyCollection<T>
    {
        bool IsProperSubsetOf(IEnumerable<T> other);

        bool IsProperSupersetOf(IEnumerable<T> other);

        bool IsSubsetOf(IEnumerable<T> other);

        bool IsSupersetOf(IEnumerable<T> other);

        bool Overlaps(IEnumerable<T> other);

        bool SetEquals(IEnumerable<T> other);
    }
}

#endif