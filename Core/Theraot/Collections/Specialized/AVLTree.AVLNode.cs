// Needed for NET35

using System;
using System.Collections.Generic;

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
            private AVLNode _left;
            private AVLNode _right;

            internal AVLNode(TKey key, TValue value)
            {
                _key = key;
                _value = value;
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
                if (node == null)
                {
                    node = new AVLNode(key, value);
                }
                else
                {
                    AddExtracted(ref node, key, value, comparison);
                }
            }

            internal static bool AddNonDuplicate(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                if (node == null)
                {
                    node = new AVLNode(key, value);
                    return true;
                }
                return AddNonDuplicateExtracted(ref node, key, value, comparison);
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

            internal static bool Remove(ref AVLNode node, TKey key, Comparison<TKey> comparison)
            {
                if (node == null)
                {
                    return false;
                }
                var compare = comparison(key, node._key);
                if (compare == 0)
                {
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
                                if (successor._right != null)
                                {
                                    node._balance--;
                                }
                            }
                            else
                            {
                                trunk._left = successor._right;
                                if (successor._right != null)
                                {
                                    trunk._balance++;
                                }
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
                if (compare < 0)
                {
                    if (Remove(ref node._left, key, comparison))
                    {
                        node._balance++;
                        MakeBalanced(ref node);
                        return true;
                    }
                    return false;
                }
                if (Remove(ref node._right, key, comparison))
                {
                    node._balance--;
                    MakeBalanced(ref node);
                    return true;
                }
                return false;
            }

            internal static AVLNode Search(AVLNode node, TKey key, Comparison<TKey> comparison)
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

            internal static AVLNode SearchNearestLeft(AVLNode node, TKey key, Comparison<TKey> comparison)
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
                    node = compare > 0 ? node._right : node._left;
                }
                return result;
            }

            internal static AVLNode SearchNearestRight(AVLNode node, TKey key, Comparison<TKey> comparison)
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

            private static void AddExtracted(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var comparisonResult = comparison(key, node._key);
                if (comparisonResult < 0)
                {
                    if (node._left == null)
                    {
                        node._left = new AVLNode(key, value);
                        node._balance--;
                    }
                    else
                    {
                        AddExtracted(ref node._left, key, value, comparison);
                        node._balance--;
                    }
                }
                else
                {
                    if (node._right == null)
                    {
                        node._right = new AVLNode(key, value);
                        node._balance++;
                    }
                    else
                    {
                        AddExtracted(ref node._right, key, value, comparison);
                        node._balance++;
                    }
                }
                MakeBalanced(ref node);
            }

            private static bool AddNonDuplicateExtracted(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var comparisonResult = comparison(key, node._key);
                if (comparisonResult < 0)
                {
                    if (node._left == null)
                    {
                        node._left = new AVLNode(key, value);
                        node._balance--;
                        MakeBalanced(ref node);
                        return true;
                    }
                    if (AddNonDuplicateExtracted(ref node._left, key, value, comparison))
                    {
                        node._balance--;
                        MakeBalanced(ref node);
                        return true;
                    }
                    return false;
                }
                if (comparisonResult > 0)
                {
                    if (node._right == null)
                    {
                        node._right = new AVLNode(key, value);
                        node._balance++;
                        MakeBalanced(ref node);
                        return true;
                    }
                    if (AddNonDuplicateExtracted(ref node._right, key, value, comparison))
                    {
                        node._balance++;
                        MakeBalanced(ref node);
                        return true;
                    }
                    return false;
                }
                return false;
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

            private static bool IsLeftHeavy(AVLNode node)
            {
                return node._balance <= -2;
            }

            private static bool IsRightHeavy(AVLNode node)
            {
                return node._balance >= 2;
            }

            private static void MakeBalanced(ref AVLNode node)
            {
                if (IsRightHeavy(node))
                {
                    if (IsLeftHeavy(node._right))
                    {
                        DoubleLeft(ref node);
                    }
                    else
                    {
                        RotateLeft(ref node);
                    }
                }
                else if (IsLeftHeavy(node))
                {
                    if (IsRightHeavy(node._left))
                    {
                        DoubleRight(ref node);
                    }
                    else
                    {
                        RotateRight(ref node);
                    }
                }
            }

            private static void RotateLeft(ref AVLNode node)
            {
                if (node._right != null)
                {
                    var root = node;
                    var right = node._right;
                    var rightLeft = node._right._left;
                    node._right = rightLeft;
                    right._left = root;
                    node = right;
                    var check = root._balance + right._balance;
                    root._balance = root._balance - (check / 2) - 1;
                    right._balance = right._balance + (check % 2) - 2;
                }
            }

            private static void RotateRight(ref AVLNode node)
            {
                if (node._left != null)
                {
                    var root = node;
                    var left = node._left;
                    var leftRight = node._left._right;
                    node._left = leftRight;
                    left._right = root;
                    node = left;
                    var check = root._balance + left._balance;
                    root._balance = root._balance - (check / 2) + 1;
                    left._balance = left._balance + (check % 2) + 2;
                }
            }
        }
    }
}