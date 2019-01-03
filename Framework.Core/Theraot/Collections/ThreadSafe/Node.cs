// Needed for NET40

namespace Theraot.Collections.ThreadSafe
{
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

    [System.Serializable]
#endif

    internal class Node<T>
    {
        public Node<T> Link;
        public T Value;

        private static readonly Pool<Node<T>> _pool;

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
            if (!_pool.TryGet(out var node))
            {
                node = new Node<T>();
            }
            node.Initialize(link, item);
            return node;
        }
    }
}