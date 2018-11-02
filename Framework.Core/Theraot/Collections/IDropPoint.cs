#if FAT

namespace Theraot.Collections
{
    public interface IDropPoint<T> : IReadOnlyDropPoint<T>
    {
        bool Add(T item);

        void Clear();

        bool TryTake(out T item);
    }
}

#endif