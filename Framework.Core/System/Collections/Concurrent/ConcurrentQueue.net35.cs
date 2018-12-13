#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace System.Collections.Concurrent
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ConcurrentQueue<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        private SafeQueue<T> _wrapped;

        public ConcurrentQueue()
        {
            _wrapped = new SafeQueue<T>();
        }

        public ConcurrentQueue(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            _wrapped = new SafeQueue<T>(collection);
        }

        public int Count => _wrapped.Count;

        public bool IsEmpty => _wrapped.Count == 0;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        public void Clear()
        {
            _wrapped = new SafeQueue<T>();
        }

        public void CopyTo(T[] array, int index)
        {
            _wrapped.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        public void Enqueue(T item)
        {
            _wrapped.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            return _wrapped.ToArray();
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Enqueue(item);
            return true;
        }

        public bool TryDequeue(out T result)
        {
            return _wrapped.TryTake(out result);
        }

        public bool TryPeek(out T result)
        {
            return _wrapped.TryPeek(out result);
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryDequeue(out item);
        }
    }
}

#endif