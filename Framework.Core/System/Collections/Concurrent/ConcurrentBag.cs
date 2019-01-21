#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace System.Collections.Concurrent
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    public class ConcurrentBag<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        private SafeQueue<T> _wrapped;

        public ConcurrentBag()
        {
            _wrapped = new SafeQueue<T>();
        }

        public ConcurrentBag(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _wrapped = new SafeQueue<T>(collection);
        }

        public bool IsEmpty => Count == 0;

        public int Count => _wrapped.Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        public void CopyTo(T[] array, int index)
        {
            _wrapped.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public T[] ToArray()
        {
            return _wrapped.ToArray();
        }

        public bool TryTake(out T item)
        {
            return _wrapped.TryTake(out item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            _wrapped.DeprecatedCopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Add(item);
            return true;
        }

        public void Add(T item)
        {
            _wrapped.Add(item);
        }

        public void Clear()
        {
            _wrapped = new SafeQueue<T>();
        }

        public bool TryPeek(out T result)
        {
            return _wrapped.TryPeek(out result);
        }
    }
}

#endif