// Needed for NET40

#pragma warning disable RCS1169 // Make field read-only.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    ///     Represent a fixed size thread-safe lock-free (read may loop) queue.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the queue.</typeparam>
    [Serializable]
    public sealed class ThreadSafeQueue<T> : IProducerConsumerCollection<T>
    {
        private int _count;

        private Node<FixedSizeQueue<T>> _root;

        private Node<FixedSizeQueue<T>> _tail;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadSafeQueue{T}" /> class.
        /// </summary>
        public ThreadSafeQueue()
        {
            _root = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(64));
            _tail = _root;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadSafeQueue{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source for the initial contents of the <see cref="ThreadSafeQueue{T}" />.
        /// </param>
        public ThreadSafeQueue(IEnumerable<T> source)
        {
            _root = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(source));
            _count = _root.Value.Count;
            _tail = _root;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the number of items actually contained.
        /// </summary>
        public int Count => Volatile.Read(ref _count);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        /// <summary>
        ///     Attempts to Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var tail = Volatile.Read(ref _tail);
                if (tail!.Value.TryAdd(item))
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

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns an <see cref="IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var root in SequenceHelper.ExploreSequenceUntilNull(_root, found => found.Link))
            {
                foreach (var item in root.Value)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            return this.ToArray(Count);
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Add(item);
            return true;
        }

        /// <summary>
        ///     Attempts to retrieve the next item to be taken without removing it.
        /// </summary>
        /// <param name="item">The item retrieved.</param>
        /// <returns>
        ///     <c>true</c> if an item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryPeek(out T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var root = Volatile.Read(ref _root);
                if (root!.Value.TryPeek(out item))
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
        ///     Attempts to retrieve and remove the next item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <c>true</c> if the item was taken; otherwise, <c>false</c>.
        /// </returns>
        public bool TryTake(out T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var root = Volatile.Read(ref _root);
                if (root!.Value.TryTake(out item))
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
    }
}