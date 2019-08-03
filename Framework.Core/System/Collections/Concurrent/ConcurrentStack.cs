#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace System.Collections.Concurrent
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class ConcurrentStack<T> : IProducerConsumerCollection<T>, IReadOnlyCollection<T>
    {
        private ThreadSafeStack<T> _wrapped;

        public ConcurrentStack()
        {
            _wrapped = new ThreadSafeStack<T>();
        }

        public ConcurrentStack(IEnumerable<T> collection)
        {
            _wrapped = new ThreadSafeStack<T>(collection);
        }

        public int Count => _wrapped.Count;

        public bool IsEmpty => _wrapped.Count == 0;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        public void Clear()
        {
            Volatile.Write(ref _wrapped, new ThreadSafeStack<T>());
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.CopyTo(this, array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public void Push(T item)
        {
            _wrapped.Add(item);
        }

        public void PushRange(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                _wrapped.Add(item);
            }
        }

        public void PushRange(T[] items, int startIndex, int count)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (startIndex + count > items.Length)
            {
                throw new ArgumentException("The sum of the startIndex and count arguments must be less than or equal to the collection's Count.");
            }

            for (var index = 0; index < count; index++)
            {
                _wrapped.Add(items[index + startIndex]);
            }
        }

        public T[] ToArray()
        {
            return _wrapped.ToArray();
        }

        public bool TryPeek(out T result)
        {
            return _wrapped.TryPeek(out result);
        }

        public bool TryPop(out T result)
        {
            return _wrapped.TryTake(out result);
        }

        public int TryPopRange(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var index = 0;
            for (; index < items.Length; index++)
            {
                if (!TryPop(out items[index]))
                {
                    break;
                }
            }

            return index;
        }

        public int TryPopRange(T[] items, int startIndex, int count)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (startIndex + count > items.Length)
            {
                throw new ArgumentException("The sum of the startIndex and count arguments must be less than or equal to the collection's Count.");
            }

            var index = 0;
            for (; index < count; index++)
            {
                if (!TryPop(out items[index + startIndex]))
                {
                    break;
                }
            }

            return index;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Push(item);
            return true;
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryPop(out item);
        }
    }
}

#endif