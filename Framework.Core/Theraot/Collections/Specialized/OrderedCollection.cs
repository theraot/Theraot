#if FAT

using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    public sealed class OrderedCollection<T> : ICollection<T>
    {
        private AVLTree<T, T> _data;

        public OrderedCollection()
        {
            _data = new AVLTree<T, T>();
        }

        public OrderedCollection(IComparer<T> comparer)
        {
            _data = new AVLTree<T, T>(comparer);
        }

        public int Count { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T item)
        {
            _data.Add(item, item);
            Count++;
        }

        public void Clear()
        {
            _data = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            return _data.Get(item) != null;
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public void CopyTo(T[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.ConvertProgressive(input => input.Key).GetEnumerator();
        }

        public bool Remove(T item)
        {
            if (_data.Remove(item))
            {
                Count--;
                return true;
            }
            return false;
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (Extensions.Remove(this, item, comparer))
            {
                Count--;
                return true;
            }
            return false;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

#endif