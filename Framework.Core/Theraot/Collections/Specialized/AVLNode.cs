// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed class AVLNode<TKey, TValue>
    {
        private int _balance;
        private int _depth;
        private AVLNode<TKey, TValue> _left;
        private AVLNode<TKey, TValue> _right;

        private AVLNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            _left = null;
            _right = null;
        }

        public TKey Key { get; }

        public AVLNode<TKey, TValue> Left => _left;

        public AVLNode<TKey, TValue> Right => _right;

        public TValue Value { get; }

        internal static void Add(ref AVLNode<TKey, TValue> node, TKey key, TValue value, IComparer<TKey> comparer)
        {
            var created = new AVLNode<TKey, TValue>(key, value);
            AddExtracted(ref node, key, comparer, created);
        }

        internal static bool AddNonDuplicate(ref AVLNode<TKey, TValue> node, TKey key, TValue value, IComparer<TKey> comparer)
        {
            return AddNonDuplicateExtracted(ref node, key, value, comparer, null);
        }

#if FAT
        internal static void Bound(AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer, out AVLNode<TKey, TValue> lower, out AVLNode<TKey, TValue> upper)
        {
            lower = null;
            upper = null;
            while (node != null)
            {
                var compare = comparer.Compare(key, node.Key);
                if (compare <= 0)
                {
                    upper = node;
                }
                if (compare >= 0)
                {
                    lower = node;
                }
                if (compare == 0)
                {
                    break;
                }
                node = compare < 0 ? node._left : node._right;
            }
        }
#endif

        internal static IEnumerable<AVLNode<TKey, TValue>> EnumerateFrom(AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
            var stack = new Stack<AVLNode<TKey, TValue>>();
            while (node != null)
            {
                var compare = comparer.Compare(key, node.Key);
                if (compare == 0)
                {
                    break;
                }

                if (compare < 0)
                {
                    stack.Push(node);
                    node = node._left;
                }
                else
                {
                    node = node._right;
                }
            }

            while (true)
            {
                if (node != null)
                {
                    yield return node;
                    foreach (var item in EnumerateRoot(node._right))
                    {
                        yield return item;
                    }
                }

                if (stack.Count == 0)
                {
                    break;
                }

                node = stack.Pop();
            }
        }

        internal static IEnumerable<AVLNode<TKey, TValue>> EnumerateRoot(AVLNode<TKey, TValue> node)
        {
            if (node == null)
            {
                yield break;
            }

            var stack = new Stack<AVLNode<TKey, TValue>>();
            while (true)
            {
                while (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }

                if (stack.Count > 0)
                {
                    node = stack.Pop();
                    yield return node;
                    node = node.Right;
                }
                else
                {
                    break;
                }
            }
        }

        internal static AVLNode<TKey, TValue> Get(AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
#endif
            while (node != null)
            {
                var compare = comparer.Compare(key, node.Key);
                if (compare == 0)
                {
                    break;
                }

                node = compare < 0 ? node._left : node._right;
            }

            return node;
        }

        internal static AVLNode<TKey, TValue> GetNearestLeft(AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
            AVLNode<TKey, TValue> result = null;
            while (node != null)
            {
                var compare = comparer.Compare(key, node.Key);
                if (compare >= 0)
                {
                    result = node;
                }

                if (compare == 0)
                {
                    break;
                }

                node = compare < 0 ? node._left : node._right;
            }

            return result;
        }

        internal static AVLNode<TKey, TValue> GetNearestRight(AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
            AVLNode<TKey, TValue> result = null;
            while (node != null)
            {
                var compare = comparer.Compare(key, node.Key);
                if (compare <= 0)
                {
                    result = node;
                }

                if (compare == 0)
                {
                    break;
                }

                node = compare < 0 ? node._left : node._right;
            }

            return result;
        }

#if FAT
        internal static AVLNode<TKey, TValue> GetOrAdd(ref AVLNode<TKey, TValue> node, TKey key, Func<TKey, TValue> factory, IComparer<TKey> comparer, out bool isNew)
        {
            return GetOrAddExtracted(ref node, key, factory, comparer, null, out isNew);
        }
#endif

        internal static bool Remove(ref AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
#endif
            if (node == null)
            {
                return false;
            }

            var compare = comparer.Compare(key, node.Key);
            if (compare == 0)
            {
                var result = RemoveExtracted(ref node);
                return result;
            }

            try
            {
                return compare < 0 ? Remove(ref node._left, key, comparer) : Remove(ref node._right, key, comparer);
            }
            finally
            {
                MakeBalanced(ref node);
            }
        }

        internal static AVLNode<TKey, TValue> RemoveNearestLeft(ref AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
            AVLNode<TKey, TValue> result = null;
            return RemoveNearestLeftExtracted(ref node, ref result, key, comparer);
        }

        internal static AVLNode<TKey, TValue> RemoveNearestRight(ref AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer)
        {
            AVLNode<TKey, TValue> result = null;
            return RemoveNearestRightExtracted(ref node, ref result, key, comparer);
        }

        private static void AddExtracted(ref AVLNode<TKey, TValue> node, TKey key, IComparer<TKey> comparer, AVLNode<TKey, TValue> created)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
#endif
            // Ok, it has for node only
            int compare;
            if (node == null || (compare = comparer.Compare(key, node.Key)) == 0)
            {
                if (Interlocked.CompareExchange(ref node, created, null) == null)
                {
                    return;
                }

                compare = -node._balance;
            }

            if (compare < 0)
            {
                AddExtracted(ref node._left, key, comparer, created);
            }
            else
            {
                AddExtracted(ref node._right, key, comparer, created);
            }

            MakeBalanced(ref node);
        }

        private static bool AddNonDuplicateExtracted(ref AVLNode<TKey, TValue> node, TKey key, TValue value, IComparer<TKey> comparer, AVLNode<TKey, TValue> created)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
#endif
            // Ok, it has for node only
            if (node == null)
            {
                if (created == null)
                {
                    created = new AVLNode<TKey, TValue>(key, value);
                }

                var found = Interlocked.CompareExchange(ref node, created, null);
                if (found == null)
                {
                    return true;
                }

                node = found;
            }

            var compare = comparer.Compare(key, node.Key);
            if (compare == 0)
            {
                return false;
            }

            try
            {
                return compare < 0 ? AddNonDuplicateExtracted(ref node._left, key, value, comparer, created) : AddNonDuplicateExtracted(ref node._right, key, value, comparer, created);
            }
            finally
            {
                MakeBalanced(ref node);
            }
        }

        private static void DoubleLeft(ref AVLNode<TKey, TValue> node)
        {
            if (node._right == null)
            {
                return;
            }

            RotateRight(ref node._right);
            RotateLeft(ref node);
        }

        private static void DoubleRight(ref AVLNode<TKey, TValue> node)
        {
            if (node._left == null)
            {
                return;
            }

            RotateLeft(ref node._left);
            RotateRight(ref node);
        }

#if FAT
        private static AVLNode<TKey, TValue> GetOrAddExtracted(ref AVLNode<TKey, TValue> node, TKey key, Func<TKey, TValue> factory, IComparer<TKey> comparer, AVLNode<TKey, TValue> created, out bool isNew)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
#endif
            // Ok, it has for node only
            if (node == null)
            {
                if (created == null)
                {
                    created = new AVLNode<TKey, TValue>(key, factory(key));
                    factory = null;
                }
                var found = Interlocked.CompareExchange(ref node, created, null);
                if (found == null)
                {
                    isNew = true;
                    return created;
                }
                node = found;
            }
            var compare = comparer.Compare(key, node.Key);
            if (compare == 0)
            {
                isNew = false;
                return node;
            }
            try
            {
                return compare < 0
                           ? GetOrAddExtracted(ref node._left, key, factory, comparer, created, out isNew)
                           : GetOrAddExtracted(ref node._right, key, factory, comparer, created, out isNew);
            }
            finally
            {
                MakeBalanced(ref node);
            }
        }
#endif

        private static void MakeBalanced(ref AVLNode<TKey, TValue> node)
        {
            AVLNode<TKey, TValue> current;
            do
            {
                current = node;
                Update(node);
                if (node._balance >= 2)
                {
                    if (node._right._balance <= 1)
                    {
                        DoubleLeft(ref node);
                    }
                    else
                    {
                        RotateLeft(ref node);
                    }
                }
                else if (node._balance <= -2)
                {
                    if (node._left._balance >= 1)
                    {
                        DoubleRight(ref node);
                    }
                    else
                    {
                        RotateRight(ref node);
                    }
                }
            } while (node != current);
        }

        private static bool RemoveExtracted(ref AVLNode<TKey, TValue> node)
        {
            if (node == null)
            {
                return false;
            }

            if (node._right == null)
            {
                node = node._left;
            }
            else
            {
                if (node._left == null)
                {
                    node = node._right;
                }
                else
                {
                    var trunk = node._right;
                    var successor = trunk;
                    while (successor._left != null)
                    {
                        trunk = successor;
                        successor = trunk._left;
                    }

                    if (trunk == successor)
                    {
                        node._right = successor._right;
                    }
                    else
                    {
                        trunk._left = successor._right;
                    }

                    var tmpLeft = node._left;
                    var tmpRight = node._right;
                    var tmpBalance = node._balance;
                    node = new AVLNode<TKey, TValue>(successor.Key, successor.Value)
                    {
                        _left = tmpLeft,
                        _right = tmpRight,
                        _balance = tmpBalance
                    };
                }
            }

            if (node != null)
            {
                MakeBalanced(ref node);
            }

            return true;
        }

        private static AVLNode<TKey, TValue> RemoveNearestLeftExtracted(ref AVLNode<TKey, TValue> node, ref AVLNode<TKey, TValue> result, TKey key, IComparer<TKey> comparer)
        {
            if (node == null)
            {
                return null;
            }

            var compare = comparer.Compare(key, node.Key);
            AVLNode<TKey, TValue> tmp;
            if (compare == 0)
            {
                tmp = node;
                RemoveExtracted(ref node);
                return tmp;
            }

            if (compare < 0)
            {
                tmp = RemoveNearestLeftExtracted(ref node._left, ref result, key, comparer);
                if (tmp == null)
                {
                    tmp = result;
                    RemoveExtracted(ref result);
                }

                MakeBalanced(ref node);
            }
            else
            {
                tmp = RemoveNearestLeftExtracted(ref node._right, ref node, key, comparer);
                if (tmp != null)
                {
                    return tmp;
                }

                tmp = node;
                RemoveExtracted(ref node);
            }

            return tmp;
        }

        private static AVLNode<TKey, TValue> RemoveNearestRightExtracted(ref AVLNode<TKey, TValue> node, ref AVLNode<TKey, TValue> result, TKey key, IComparer<TKey> comparer)
        {
            if (node == null)
            {
                return null;
            }

            var compare = comparer.Compare(key, node.Key);
            AVLNode<TKey, TValue> tmp;
            if (compare == 0)
            {
                tmp = node;
                RemoveExtracted(ref node);
                return tmp;
            }

            if (compare < 0)
            {
                tmp = RemoveNearestRightExtracted(ref node._left, ref node, key, comparer);
                if (tmp != null)
                {
                    return tmp;
                }

                tmp = node;
                RemoveExtracted(ref node);
            }
            else
            {
                tmp = RemoveNearestRightExtracted(ref node._right, ref result, key, comparer);
                if (tmp == null)
                {
                    tmp = result;
                    RemoveExtracted(ref result);
                }

                MakeBalanced(ref node);
            }

            return tmp;
        }

        private static void RotateLeft(ref AVLNode<TKey, TValue> node)
        {
            var root = node;
            var right = node._right;
            if (right == null)
            {
                return;
            }

            var rightLeft = right._left;
            node._right = rightLeft;
            right._left = root;
            node = right;
            Update(root);
            Update(right);
        }

        private static void RotateRight(ref AVLNode<TKey, TValue> node)
        {
            var root = node;
            var left = node._left;
            if (left == null)
            {
                return;
            }

            var leftRight = left._right;
            node._left = leftRight;
            left._right = root;
            node = left;
            Update(root);
            Update(left);
        }

        private static void Update(AVLNode<TKey, TValue> node)
        {
            var right = node._right;
            var left = node._left;
            node._depth = Math.Max(right?._depth + 1 ?? 0, left?._depth + 1 ?? 0);
            node._balance = (right?._depth + 1 ?? 0) - (left?._depth + 1 ?? 0);
        }
    }
}