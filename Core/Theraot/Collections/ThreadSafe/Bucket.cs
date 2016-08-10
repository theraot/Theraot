using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe wait-free bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    [Serializable]
    public sealed class Bucket<T> : IEnumerable<T>, IBucket<T>
    {
        public int Capacity { get; private set; }
        public int Count { get; private set; }
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Exchange(int index, T item, out T previous)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IBucket<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Insert(int index, T item, out T previous)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAt(int index, out T previous)
        {
            throw new NotImplementedException();
        }

        public bool RemoveValueAt(int index, T value, out T previous)
        {
            throw new NotImplementedException();
        }

        public void Set(int index, T item, out bool isNew)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(int index, out T value)
        {
            throw new NotImplementedException();
        }

        public bool Update(int index, T item, T comparisonItem, out T previous, out bool isNew)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Where(Predicate<T> predicate)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}