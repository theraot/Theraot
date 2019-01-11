#if FAT

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Theraot.Collections
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
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

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            Extensions.CanCopyTo(Count - index, array, count);
            Extensions.CopyTo(this, index, array, arrayIndex, count);
        }

#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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

#endif

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Push(item);
            return true;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryPop(out item);
        }
    }
}

#endif