#if FAT

namespace Theraot.Collections
{
    public interface IExtendedReadOnlySet<T> : IReadOnlySet<T>, IExtendedReadOnlyCollection<T>
    {
        //Empty
    }
}

#endif