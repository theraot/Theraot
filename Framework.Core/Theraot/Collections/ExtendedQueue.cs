using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class QueueEx<T> : Queue<T>, IProducerConsumerCollection<T>
    {
        public QueueEx()
        {
            // Empty
        }

        public QueueEx(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public QueueEx(IEnumerable<T> collection)
            : base(collection)
        {
            // Empty
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.DeprecatedCopyTo(this, array, index);
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Enqueue(item);
            return true;
        }

        public bool TryDequeue(out T item)
        {
            try
            {
                item = Dequeue();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default;
                return false;
            }
        }

        public bool TryPeek(out T item)
        {
            try
            {
                item = Peek();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default;
                return false;
            }
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryDequeue(out item);
        }
    }
}