// Needed for NET40

using System;

namespace Theraot.Collections.ThreadSafe
{
    [Serializable]
    internal sealed class Node<T>
    {
        public Node<T>? Link;

        // ReSharper disable once RedundantDefaultMemberInitializer
        public T Value = default!;

        private static readonly Pool<Node<T>> _pool = new(64, Recycle);

        private Node()
        {
            // Empty
        }

        public static void Recycle(Node<T> node)
        {
            node.Link = null;
            node.Value = default!;
        }

        public void Initialize(Node<T>? link, T value)
        {
            Link = link;
            Value = value;
        }

        internal static void Donate(Node<T> node)
        {
            _pool.Donate(node);
        }

        internal static Node<T> GetNode(Node<T>? link, T item)
        {
            if (!_pool.TryGet(out var node))
            {
                node = new Node<T>();
            }

            node.Initialize(link, item);
            return node;
        }
    }
}