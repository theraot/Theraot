#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IExtendedList<T> : IExtendedCollection<T>, IExtendedReadOnlyList<T>, IList<T>
    {
        new IReadOnlyList<T> AsReadOnly { get; }

        new int Count { get; }

        new T this[int index] { get; set; }

        void Move(int oldIndex, int newIndex);

        void RemoveRange(int index, int count);

        void Reverse();

        void Reverse(int index, int count);

        void Sort(IComparer<T> comparer);

        void Sort(int index, int count, IComparer<T> comparer);

        void Swap(int indexA, int indexB);
    }
}

#endif