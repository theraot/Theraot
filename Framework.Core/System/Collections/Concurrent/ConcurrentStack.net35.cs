#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace System.Collections.Concurrent
{
    [SerializableAttribute]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ConcurrentStack<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
    {
        private SafeStack<T> _wrapped;

        public ConcurrentStack()
        {
            _wrapped = new SafeStack<T>();
        }

        public ConcurrentStack(IEnumerable<T> collection)
        {
            _wrapped = new SafeStack<T>(collection);
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        public bool IsEmpty
        {
            get { return _wrapped.Count == 0; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public void Clear()
        {
            Volatile.Write(ref _wrapped, new SafeStack<T>());
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.CopyTo(this, array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                throw new ArgumentException();
            }
            for (int index = 0; index < count; index++)
            {
                _wrapped.Add(items[index + startIndex]);
            }
        }

        public T[] ToArray()
        {
            return _wrapped.ToArray();
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            _wrapped.Add(item);
            return true;
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
            int index = 0;
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
                throw new ArgumentException();
            }
            int index = 0;
            for (; index < count; index++)
            {
                if (!TryPop(out items[index + startIndex]))
                {
                    break;
                }
            }
            return index;
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return _wrapped.TryTake(out item);
        }
    }
}

#endif