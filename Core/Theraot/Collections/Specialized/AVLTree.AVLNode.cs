// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.Specialized
{
    public sealed partial class AVLTree<TKey, TValue> : IEnumerable<AVLTree<TKey, TValue>.AVLNode>
    {
        [Serializable]
        public sealed class AVLNode
        {
            private readonly TKey _key;
            private readonly TValue _value;

            private int _balance;
            private int _depth;
            private AVLNode _left;
            private AVLNode _right;

            private AVLNode(TKey key, TValue value)
            {
                _key = key;
                _value = value;
                _left = null;
                _right = null;
            }

            public TKey Key
            {
                get
                {
                    return _key;
                }
            }

            public AVLNode Left
            {
                get
                {
                    return _left;
                }
            }

            public AVLNode Right
            {
                get
                {
                    return _right;
                }
            }

            public TValue Value
            {
                get
                {
                    return _value;
                }
            }

            public int CompareTo(AVLNode other, IComparer<TKey> comparer)
            {
                if (comparer == null)
                {
                    throw new ArgumentNullException("comparer");
                }
                if (other == null)
                {
                    return 1;
                }
                return comparer.Compare(_key, other._key);
            }

            internal static void Add(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var created = new AVLNode(key, value);
                AddExtracted(ref node, key, comparison, created);
            }

            internal static bool AddNonDuplicate(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                return AddNonDuplicateExtracted(ref node, key, value, comparison, null);
            }

            internal static void Bound(AVLNode node, TKey key, Comparison<TKey> comparison, out AVLNode lower, out AVLNode upper)
            {
                lower = null;
                upper = null;
                while (node != null)
                {
                    var compare = comparison.Invoke(key, node._key);
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
                    node = compare > 0 ? node._right : node._left;
                }
            }

            internal static IEnumerable<AVLNode> EnumerateFrom(AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<AVLNode>();
                while (node != null)
                {
                    var compare = comparison.Invoke(key, node._key);
                    if (compare == 0)
                    {
                        break;
                    }
                    if (compare > 0)
                    {
                        node = node._right;
                    }
                    else
                    {
                        stack.Push(node);
                        node = node._left;
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

            internal static AVLNode Get(AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                while (node != null)
                {
                    var compare = comparison(key, node._key);
                    if (compare == 0)
                    {
                        break;
                    }
                    node = compare > 0 ? node._right : node._left;
                }
                return node;
            }

            internal static AVLNode GetFirst(AVLNode node)
            {
                AVLNode result = null;
                while (node != null)
                {
                    result = node;
                    node = node._left;
                }
                return result;
            }

            internal static AVLNode GetLast(AVLNode node)
            {
                AVLNode result = null;
                while (node != null)
                {
                    result = node;
                    node = node._right;
                }
                return result;
            }

            internal static AVLNode GetNearestLeft(AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                AVLNode result = null;
                while (node != null)
                {
                    var compare = comparison.Invoke(key, node._key);
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

            internal static AVLNode GetNearestRight(AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                AVLNode result = null;
                while (node != null)
                {
                    var compare = comparison.Invoke(key, node._key);
                    if (compare <= 0)
                    {
                        result = node;
                    }
                    if (compare == 0)
                    {
                        break;
                    }
                    node = compare > 0 ? node._right : node._left;
                }
                return result;
            }

            internal static AVLNode GetOrAdd(ref AVLNode node, TKey key, Func<TKey, TValue> factory, Comparison<TKey> comparison, out bool isNew)
            {
                return GetOrAddExtracted(ref node, key, factory, comparison, null, out isNew);
            }

            internal static bool Remove(ref AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                if (node == null)
                {
                    return false;
                }
                var compare = comparison(key, node._key);
                if (compare == 0)
                {
                    var result = RemoveExtracted(ref node);
                    return result;
                }
                try
                {
                    if (compare < 0)
                    {
                        return Remove(ref node._left, key, comparison);
                    }
                    return Remove(ref node._right, key, comparison);
                }
                finally
                {
                    MakeBalanced(ref node);
                }
            }

            internal static AVLNode RemoveNearestLeft(ref AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                AVLNode result = null;
                return RemoveNearestLeftExtracted(ref node, ref result, key, comparison);
            }

            internal static AVLNode RemoveNearestRight(ref AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                AVLNode result = null;
                return RemoveNearestRightExtracted(ref node, ref result, key, comparison);
            }

            private static void AddExtracted(ref AVLNode node, TKey key, Comparison<TKey> comparison, AVLNode created)
            {
                int compare;
                if (node == null || (compare = comparison(key, node._key)) == 0)
                {
                    if (Interlocked.CompareExchange(ref node, created, null) == null)
                    {
                        return;
                    }
                    compare = -node._balance;
                }
                if (compare < 0)
                {
                    AddExtracted(ref node._left, key, comparison, created);
                }
                else
                {
                    AddExtracted(ref node._right, key, comparison, created);
                }
                MakeBalanced(ref node);
            }

            private static bool AddNonDuplicateExtracted(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison, AVLNode created)
            {
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
                var compare = comparison(key, node._key);
                if (compare == 0)
                {
                    return false;
                }
                try
                {
                    if (compare < 0)
                    {
                        return AddNonDuplicateExtracted(ref node._left, key, value, comparison, created);
                    }
                    return AddNonDuplicateExtracted(ref node._right, key, value, comparison, created);
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

            private static AVLNode GetOrAddExtracted(ref AVLNode node, TKey key, Func<TKey, TValue> factory, Comparison<TKey> comparison, AVLNode created, out bool isNew)
            {
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
                var compare = comparison(key, node._key);
                if (compare == 0)
                {
                    isNew = false;
                    return node;
                }
                try
                {
                    if (compare < 0)
                    {
                        return GetOrAddExtracted(ref node._left, key, factory, comparison, created, out isNew);
                    }
                    return GetOrAddExtracted(ref node._right, key, factory, comparison, created, out isNew);
                }
                finally
                {
                    MakeBalanced(ref node);
                }
            }

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
                        if (ReferenceEquals(trunk, successor))
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
                        node = new AVLNode(successor._key, successor._value)
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

            private static AVLNode RemoveNearestLeftExtracted(ref AVLNode node, ref AVLNode result, TKey key, Comparison<TKey> comparison)
            {
                if (node == null)
                {
                    return null;
                }
                var compare = comparison.Invoke(key, node._key);
                AVLNode tmp;
                if (compare == 0)
                {
                    tmp = node;
                    RemoveExtracted(ref node);
                    return tmp;
                }
                if (compare < 0)
                {
                    tmp = RemoveNearestLeftExtracted(ref node._left, ref result, key, comparison);
                    if (tmp == null)
                    {
                        tmp = result;
                        RemoveExtracted(ref result);
                    }
                    MakeBalanced(ref node);
                }
                else
                {
                    tmp = RemoveNearestLeftExtracted(ref node._right, ref node, key, comparison);
                    if (tmp == null)
                    {
                        tmp = node;
                        RemoveExtracted(ref node);
                    }
                }
                return tmp;
            }

            private static AVLNode RemoveNearestRightExtracted(ref AVLNode node, ref AVLNode result, TKey key, Comparison<TKey> comparison)
            {
                if (node == null)
                {
                    return null;
                }
                var compare = comparison.Invoke(key, node._key);
                AVLNode tmp;
                if (compare == 0)
                {
                    tmp = node;
                    RemoveExtracted(ref node);
                    return tmp;
                }
                if (compare < 0)
                {
                    tmp = RemoveNearestRightExtracted(ref node._left, ref node, key, comparison);
                    if (tmp == null)
                    {
                        tmp = node;
                        RemoveExtracted(ref node);
                    }
                }
                else
                {
                    tmp = RemoveNearestRightExtracted(ref node._right, ref result, key, comparison);
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
                node._depth = Math.Max(right == null ? 0 : right._depth + 1, left == null ? 0 : left._depth + 1);
                node._balance = (right == null ? 0 : right._depth + 1) - (left == null ? 0 : left._depth + 1);
            }
        }
    }
}