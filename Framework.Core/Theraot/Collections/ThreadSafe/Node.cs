// Needed for NET40

using System;

namespace Theraot.Collections.ThreadSafe
{
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

    [Serializable]
#endif
    internal class Node<T>
    {
        public Node<T> Link;
        public T Value;

        private static Pool<Node<T>> _pool;

        static Node()
        {
            _pool = new Pool<Node<T>>(64, Recycle);
        }

        private Node()
        {
            // Empty
        }

        public static void Recycle(Node<T> node)
        {
            node.Link = null;
            node.Value = default;
        }

        public void Initialize(Node<T> link, T value)
        {
            Link = link;
            Value = value;
        }

        internal static void Donate(Node<T> node)
        {
            _pool.Donate(node);
        }

        internal static Node<T> GetNode(Node<T> link, T item)
        {
            if (!_pool.TryGet(out Node<T> node))
            {
                node = new Node<T>();
            }
            node.Initialize(link, item);
            return node;
        }
    }
}