// Needed for NET40

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a fixed size thread-safe lock-free (loops) stack.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the stack.</typeparam>
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

    [Serializable]
#endif

    public sealed class SafeStack<T> : IProducerConsumerCollection<T>
    {
        private int _count;
        private Node<T> _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStack{T}" /> class.
        /// </summary>
        public SafeStack()
        {
            _root = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStack{T}" /> class.
        /// </summary>
        public SafeStack(IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                _root = Node<T>.GetNode(_root, item);
                _count++;
            }
        }

        /// <summary>
        /// Gets the number of items actually contained.
        /// </summary>
        public int Count => Volatile.Read(ref _count);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => throw new NotSupportedException();

        /// <summary>
        /// Attempts to Adds the specified item at the front.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if the item was added; otherwise, <c>false</c>.
        /// </returns>
        public void Add(T item)
        {
            var root = Volatile.Read(ref _root);
            var node = Node<T>.GetNode(root, item);
            var spinWait = new SpinWait();
            while (true)
            {
                var found = Interlocked.CompareExchange(ref _root, node, root);
                if (found == root)
                {
                    Interlocked.Increment(ref _count);
                    return;
                }
                root = Volatile.Read(ref _root);
                node.Link = root;
                spinWait.SpinOnce();
            }
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.CopyTo(this, array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.DeprecatedCopyTo(this, array, index);
        }

        /// <summary>
        /// Returns an <see cref="System.Collections.Generic.IEnumerator{T}" /> that allows to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = Volatile.Read(ref _root);
            while (current != null)
            {
                yield return current.Value;
                current = current.Link;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the next item to be taken from the back without removing it.
        /// </summary>
        /// <returns>The next item to be taken from the back.</returns>
        /// <exception cref="InvalidOperationException">No more items to be taken.</exception>
        public T Peek()
        {
            var root = Volatile.Read(ref _root);
            if (root == null)
            {
                throw new InvalidOperationException();
            }
            return root.Value;
        }

        public T[] ToArray()
        {
            return Extensions.ToArray(this, Count);
        }

        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Add(item);
            return true;
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
            var root = Volatile.Read(ref _root);
            if (root == null)
            {
                item = default;
                return false;
            }
            item = root.Value;
            return true;
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
            var spinWait = new SpinWait();
            while (true)
            {
                var root = Volatile.Read(ref _root);
                if (root == null)
                {
                    item = default;
                    return false;
                }
                var found = Interlocked.CompareExchange(ref _root, root.Link, root);
                if (found == root)
                {
                    item = root.Value;
                    Node<T>.Donate(root);
                    Interlocked.Decrement(ref _count);
                    return true;
                }
                spinWait.SpinOnce();
            }
        }
    }
}