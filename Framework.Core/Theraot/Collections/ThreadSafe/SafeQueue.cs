// Needed for NET40

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe lock-free (read may loop) queue.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the queue.</typeparam>
    [Serializable]
    public sealed class SafeQueue<T> : IProducerConsumerCollection<T>
    {
        private int _count;
        private Node<FixedSizeQueue<T>> _root;
        private Node<FixedSizeQueue<T>> _tail;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeQueue{T}" /> class.
        /// </summary>
        public SafeQueue()
        {
            _root = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(64));
            _tail = _root;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeQueue{T}" /> class.
        /// </summary>
        public SafeQueue(IEnumerable<T> source)
        {
            _root = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(source));
            _count = _root.Value.Count;
            _tail = _root;
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count => Volatile.Read(ref _count);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        /// <summary>
        /// Attempts to Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var tail = Volatile.Read(ref _tail);
                if (tail.Value.TryAdd(item))
                {
                    Interlocked.Increment(ref _count);
                    return;
                }
                var node = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(64));
                var found = Interlocked.CompareExchange(ref tail.Link, node, null);
                if (found == null)
                {
                    Volatile.Write(ref _tail, node);
                }
                spinWait.SpinOnce();
            }
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            var root = _root;
            do
            {
                foreach (var item in root.Value)
                {
                    yield return item;
                }
                root = root.Link;
            } while (root != null);
        }

        public T[] ToArray()
        {
            return this.ToArray(Count);
        }

        /// <summary>
        /// Attempts to retrieve the next item to be taken without removing it.
        /// </summary>
        /// <param name="item">The item retrieved.</param>
        /// <returns>
        ///   <c>true</c> if an item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryPeek(out T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var root = Volatile.Read(ref _root);
                if (root.Value.TryPeek(out item))
                {
                    return true;
                }
                if (root.Link == null)
                {
                    return false;
                }
                var found = Interlocked.CompareExchange(ref _root, root.Link, root);
                if (found == root)
                {
                    Node<FixedSizeQueue<T>>.Donate(root);
                }
                spinWait.SpinOnce();
            }
        }

        /// <summary>
        /// Attempts to retrieve and remove the next item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was taken; otherwise, <c>false</c>.
        /// </returns>
        public bool TryTake(out T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var root = Volatile.Read(ref _root);
                if (root.Value.TryTake(out item))
                {
                    Interlocked.Decrement(ref _count);
                    return true;
                }
                if (root.Link == null)
                {
                    return false;
                }
                var found = Interlocked.CompareExchange(ref _root, root.Link, root);
                if (found == root)
                {
                    Node<FixedSizeQueue<T>>.Donate(root);
                }
                spinWait.SpinOnce();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.DeprecatedCopyTo(this, array, index);
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
    }
}