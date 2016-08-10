using System.Collections.Generic;

namespace Theraot.Collections.ThreadSafe
{
    public interface IBucket<T>
    {
        int Capacity
        {
            get;
        }
        int Count
        {
            get;
        }

        void CopyTo(T[] array, int arrayIndex);
        bool Exchange(int index, T item, out T previous);
        IEnumerator<T> GetEnumerator();
        bool Insert(int index, T item);
        bool Insert(int index, T item, out T previous);
        bool RemoveAt(int index);
        bool RemoveAt(int index, out T previous);
        bool RemoveValueAt(int index, T value, out T previous);
        void Set(int index, T item, out bool isNew);
        bool TryGet(int index, out T value);
        bool Update(int index, T item, T comparisonItem, out T previous, out bool isNew);
    }
}