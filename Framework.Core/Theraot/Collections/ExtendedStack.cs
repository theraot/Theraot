using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class StackEx<T> : Stack<T>, IProducerConsumerCollection<T>
    {
        public StackEx()
        {
            // Empty
        }

        public StackEx(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public StackEx(IEnumerable<T> collection)
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
            Push(item);
            return true;
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

        public bool TryPop(out T item)
        {
            try
            {
                item = Pop();
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
            return TryPop(out item);
        }
    }
}