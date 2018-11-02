// Needed for NET40

using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe wait-free queue.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the queue.</typeparam>
    [Serializable]
    public sealed class SafeQueue<T> : IEnumerable<T>
    {
        private int _count;
        private Node _root;
        private Node _tail;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeQueue{T}" /> class.
        /// </summary>
        public SafeQueue()
        {
            _root = new Node();
            _tail = _root;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeQueue{T}" /> class.
        /// </summary>
        public SafeQueue(IEnumerable<T> source)
        {
            _root = new Node(source);
            _count = _root.Queue.Count;
            _tail = _root;
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count
        {
            get { return Volatile.Read(ref _count); }
        }

        /// <summary>
        /// Attempts to Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            loop:
            if (_tail.Queue.Add(item))
            {
                Interlocked.Increment(ref _count);
            }
            else
            {
                var created = new Node();
                var found = Interlocked.CompareExchange(ref _tail.Next, created, null);
                _tail = found ?? created;
                goto loop;
            }
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
                foreach (var item in root.Queue)
                {
                    yield return item;
                }
                root = root.Next;
            } while (root != null);
        }

        /// <summary>
        /// Attempts to retrieve the next item to be taken from the back without removing it.
        /// </summary>
        /// <param name="item">The item retrieved.</param>
        /// <returns>
        ///   <c>true</c> if an item was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryPeek(out T item)
        {
            var root = _root;
            while (true)
            {
                if (_root.Queue.TryPeek(out item))
                {
                    return true;
                }
                if (root.Next != null)
                {
                    var found = Interlocked.CompareExchange(ref _root, root.Next, root);
                    root = found == root ? root.Next : found;
                }
                else
                {
                    break;
                }
            }
            item = default(T);
            return false;
        }

        /// <summary>
        /// Attempts to retrieve and remove the next item from the back.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was taken; otherwise, <c>false</c>.
        /// </returns>
        public bool TryTake(out T item)
        {
            var root = _root;
            while (true)
            {
                if (_root.Queue.TryTake(out item))
                {
                    Interlocked.Decrement(ref _count);
                    return true;
                }
                if (root.Next != null)
                {
                    var found = Interlocked.CompareExchange(ref _root, root.Next, root);
                    root = found == root ? root.Next : found;
                }
                else
                {
                    break;
                }
            }
            item = default(T);
            return false;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        private class Node
        {
            internal readonly FixedSizeQueue<T> Queue;

            internal Node Next;

            public Node()
            {
                Queue = new FixedSizeQueue<T>(64);
            }

            public Node(IEnumerable<T> source)
            {
                Queue = new FixedSizeQueue<T>(source);
            }
        }
    }
}