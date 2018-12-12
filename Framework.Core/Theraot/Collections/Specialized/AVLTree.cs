// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.Specialized
{
    public sealed class AVLTree<TKey, TValue> : IEnumerable<AVLTree<TKey, TValue>.AVLNode>
    {
        private readonly IComparer<TKey> _comparer;
        private AVLNode _root;

        public AVLTree()
        {
            _root = null;
            _comparer = Comparer<TKey>.Default;
        }

        public AVLTree(IComparer<TKey> comparer)
        {
            _root = null;
            _comparer = comparer ?? Comparer<TKey>.Default;
        }

        public int Count { get; private set; }

        public AVLNode Root => _root;

        public void Add(TKey key, TValue value)
        {
            AVLNode.Add(ref _root, key, value, _comparer);
            Count++;
        }

        public bool AddNonDuplicate(TKey key, TValue value)
        {
            if (AVLNode.AddNonDuplicate(ref _root, key, value, _comparer))
            {
                Count++;
                return true;
            }
            return false;
        }

#if FAT
        public void Bound(TKey key, out AVLNode lower, out AVLNode upper)
        {
            AVLNode.Bound(_root, key, _comparer, out lower, out upper);
        }
#endif

        public void Clear()
        {
            _root = null;
            Count = 0;
        }

        public AVLNode Get(TKey key)
        {
            return AVLNode.Get(_root, key, _comparer);
        }

        public IEnumerator<AVLNode> GetEnumerator()
        {
            return AVLNode.EnumerateRoot(_root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public AVLNode GetNearestLeft(TKey key)
        {
            return AVLNode.GetNearestLeft(_root, key, _comparer);
        }

        public AVLNode GetNearestRight(TKey key)
        {
            return AVLNode.GetNearestRight(_root, key, _comparer);
        }

#if FAT
        public AVLNode GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            var result = AVLNode.GetOrAdd(ref _root, key, factory, _comparer, out var isNew);
            if (isNew)
            {
                Count++;
            }
            return result;
        }
#endif

        public IEnumerable<AVLNode> Range(TKey lower, TKey upper)
        {
            foreach (var item in AVLNode.EnumerateFrom(_root, lower, _comparer))
            {
                var comparer = _comparer;
                if (comparer.Compare(item.Key, upper) > 0)
                {
                    break;
                }
                yield return item;
            }
        }

        public bool Remove(TKey key)
        {
            if (AVLNode.Remove(ref _root, key, _comparer))
            {
                Count--;
                return true;
            }
            return false;
        }

        public AVLNode RemoveNearestLeft(TKey key)
        {
            return AVLNode.RemoveNearestLeft(ref _root, key, _comparer);
        }

        public AVLNode RemoveNearestRight(TKey key)
        {
            return AVLNode.RemoveNearestRight(ref _root, key, _comparer);
        }

        public sealed class AVLNode
        {
            private int _balance;
            private int _depth;
            private AVLNode _left;
            private AVLNode _right;

            private AVLNode(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                _left = null;
                _right = null;
            }

            public TKey Key { get; }

            public AVLNode Left => _left;

            public AVLNode Right => _right;

            public TValue Value { get; }

            internal static void Add(ref AVLNode node, TKey key, TValue value, IComparer<TKey> comparer)
            {
                var created = new AVLNode(key, value);
                AddExtracted(ref node, key, comparer, created);
            }

            internal static bool AddNonDuplicate(ref AVLNode node, TKey key, TValue value, IComparer<TKey> comparer)
            {
                return AddNonDuplicateExtracted(ref node, key, value, comparer, null);
            }

#if FAT
            internal static void Bound(AVLNode node, TKey key, IComparer<TKey> comparer, out AVLNode lower, out AVLNode upper)
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

            internal static IEnumerable<AVLNode> EnumerateFrom(AVLNode node, TKey key, IComparer<TKey> comparer)
            {
                var stack = new Stack<AVLNode>();
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

            internal static IEnumerable<AVLNode> EnumerateRoot(AVLNode node)
            {
                if (node != null)
                {
                    var stack = new Stack<AVLNode>();
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
            }

            internal static AVLNode Get(AVLNode node, TKey key, IComparer<TKey> comparer)
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

            internal static AVLNode GetNearestLeft(AVLNode node, TKey key, IComparer<TKey> comparer)
            {
                AVLNode result = null;
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

            internal static AVLNode GetNearestRight(AVLNode node, TKey key, IComparer<TKey> comparer)
            {
                AVLNode result = null;
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
            internal static AVLNode GetOrAdd(ref AVLNode node, TKey key, Func<TKey, TValue> factory, IComparer<TKey> comparer, out bool isNew)
            {
                return GetOrAddExtracted(ref node, key, factory, comparer, null, out isNew);
            }
#endif

            internal static bool Remove(ref AVLNode node, TKey key, IComparer<TKey> comparer)
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
                    if (compare < 0)
                    {
                        return Remove(ref node._left, key, comparer);
                    }
                    return Remove(ref node._right, key, comparer);
                }
                finally
                {
                    MakeBalanced(ref node);
                }
            }

            internal static AVLNode RemoveNearestLeft(ref AVLNode node, TKey key, IComparer<TKey> comparer)
            {
                AVLNode result = null;
                return RemoveNearestLeftExtracted(ref node, ref result, key, comparer);
            }

            internal static AVLNode RemoveNearestRight(ref AVLNode node, TKey key, IComparer<TKey> comparer)
            {
                AVLNode result = null;
                return RemoveNearestRightExtracted(ref node, ref result, key, comparer);
            }

            private static void AddExtracted(ref AVLNode node, TKey key, IComparer<TKey> comparer, AVLNode created)
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

            private static bool AddNonDuplicateExtracted(ref AVLNode node, TKey key, TValue value, IComparer<TKey> comparer, AVLNode created)
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
                        created = new AVLNode(key, value);
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
                    if (compare < 0)
                    {
                        return AddNonDuplicateExtracted(ref node._left, key, value, comparer, created);
                    }
                    return AddNonDuplicateExtracted(ref node._right, key, value, comparer, created);
                }
                finally
                {
                    MakeBalanced(ref node);
                }
            }

            private static void DoubleLeft(ref AVLNode node)
            {
                if (node._right != null)
                {
                    RotateRight(ref node._right);
                    RotateLeft(ref node);
                }
            }

            private static void DoubleRight(ref AVLNode node)
            {
                if (node._left != null)
                {
                    RotateLeft(ref node._left);
                    RotateRight(ref node);
                }
            }

#if FAT
            private static AVLNode GetOrAddExtracted(ref AVLNode node, TKey key, Func<TKey, TValue> factory, IComparer<TKey> comparer, AVLNode created, out bool isNew)
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
                        created = new AVLNode(key, factory(key));
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
                    if (compare < 0)
                    {
                        return GetOrAddExtracted(ref node._left, key, factory, comparer, created, out isNew);
                    }
                    return GetOrAddExtracted(ref node._right, key, factory, comparer, created, out isNew);
                }
                finally
                {
                    MakeBalanced(ref node);
                }
            }
#endif

            private static void MakeBalanced(ref AVLNode node)
            {
                AVLNode current;
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

            private static bool RemoveExtracted(ref AVLNode node)
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
                        node = new AVLNode(successor.Key, successor.Value)
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

            private static AVLNode RemoveNearestLeftExtracted(ref AVLNode node, ref AVLNode result, TKey key, IComparer<TKey> comparer)
            {
                if (node == null)
                {
                    return null;
                }
                var compare = comparer.Compare(key, node.Key);
                AVLNode tmp;
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
                    if (tmp == null)
                    {
                        tmp = node;
                        RemoveExtracted(ref node);
                    }
                }
                return tmp;
            }

            private static AVLNode RemoveNearestRightExtracted(ref AVLNode node, ref AVLNode result, TKey key, IComparer<TKey> comparer)
            {
                if (node == null)
                {
                    return null;
                }
                var compare = comparer.Compare(key, node.Key);
                AVLNode tmp;
                if (compare == 0)
                {
                    tmp = node;
                    RemoveExtracted(ref node);
                    return tmp;
                }
                if (compare < 0)
                {
                    tmp = RemoveNearestRightExtracted(ref node._left, ref node, key, comparer);
                    if (tmp == null)
                    {
                        tmp = node;
                        RemoveExtracted(ref node);
                    }
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

            private static void RotateLeft(ref AVLNode node)
            {
                var root = node;
                var right = node._right;
                if (right != null)
                {
                    var rightLeft = right._left;
                    node._right = rightLeft;
                    right._left = root;
                    node = right;
                    Update(root);
                    Update(right);
                }
            }

            private static void RotateRight(ref AVLNode node)
            {
                var root = node;
                var left = node._left;
                if (left != null)
                {
                    var leftRight = left._right;
                    node._left = leftRight;
                    left._right = root;
                    node = left;
                    Update(root);
                    Update(left);
                }
            }

            private static void Update(AVLNode node)
            {
                var right = node._right;
                var left = node._left;
                node._depth = Math.Max(right?._depth + 1 ?? 0, left?._depth + 1 ?? 0);
                node._balance = (right?._depth + 1 ?? 0) - (left?._depth + 1 ?? 0);
            }
        }
    }
}