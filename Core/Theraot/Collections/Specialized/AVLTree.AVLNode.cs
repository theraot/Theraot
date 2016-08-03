// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections.Generic;
using Theraot.Threading.Needles;

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
            private INeedle<AVLNode> _leftNeedle;
            private INeedle<AVLNode> _rightNeedle;

            private AVLNode(TKey key, TValue value)
            {
                _key = key;
                _value = value;
                _leftNeedle = new StructNeedle<AVLNode>();
                _rightNeedle = new StructNeedle<AVLNode>();
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
                    return _leftNeedle.Value;
                }
            }

            public AVLNode Right
            {
                get
                {
                    return _rightNeedle.Value;
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

            internal static void Add(INeedle<AVLNode> nodeNeedle, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                while (true)
                {
                    int compare;
                    if (!nodeNeedle.IsAlive || (compare = comparison(key, nodeNeedle.Value._key)) == 0)
                    {
                        var result = AddExtracted(nodeNeedle, key, value);
                        if (result != -1)
                        {
                            while (stack.Count > 0)
                            {
                                MakeBalanced(stack.Pop());
                            }
                            return;
                        }
                        compare = -nodeNeedle.Value._balance;
                    }
                    stack.Push(nodeNeedle);
                    nodeNeedle = compare < 0 ? nodeNeedle.Value._leftNeedle : nodeNeedle.Value._rightNeedle;
                }
            }

            internal static bool AddNonDuplicate(INeedle<AVLNode> nodeNeedle, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                while (true)
                {
                    int compare;
                    if (!nodeNeedle.IsAlive || (compare = comparison(key, nodeNeedle.Value._key)) == 0)
                    {
                        var result = AddNonDuplicateExtracted(nodeNeedle, key, value);
                        if (result != -1)
                        {
                            while (stack.Count > 0)
                            {
                                MakeBalanced(stack.Pop());
                            }
                            return result != 0;
                        }
                        compare = -nodeNeedle.Value._balance;
                    }
                    stack.Push(nodeNeedle);
                    nodeNeedle = compare < 0 ? nodeNeedle.Value._leftNeedle : nodeNeedle.Value._rightNeedle;
                }
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
                    node = compare > 0 ? node._rightNeedle.Value : node._leftNeedle.Value;
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
                        node = node._rightNeedle.Value;
                    }
                    else
                    {
                        stack.Push(node);
                        node = node._leftNeedle.Value;
                    }
                }
                while (true)
                {
                    if (node != null)
                    {
                        yield return node;
                        foreach (var item in EnumerateRoot(node._rightNeedle.Value))
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

            internal static bool Remove(INeedle<AVLNode> nodeNeedle, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                while (true)
                {
                    int compare;
                    if (!nodeNeedle.IsAlive || (compare = comparison(key, nodeNeedle.Value._key)) == 0)
                    {
                        var result = RemoveExtracted(nodeNeedle);
                        if (result != -1)
                        {
                            while (stack.Count > 0)
                            {
                                MakeBalanced(stack.Pop());
                            }
                            return result != 0;
                        }
                        compare = -nodeNeedle.Value._balance;
                    }
                    stack.Push(nodeNeedle);
                    nodeNeedle = compare < 0 ? nodeNeedle.Value._leftNeedle : nodeNeedle.Value._rightNeedle;
                }
            }

            internal static AVLNode RemoveNearestLeft(INeedle<AVLNode> nodeNeedle, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                INeedle<AVLNode> resultNeedle = null;
                while (nodeNeedle.IsAlive)
                {
                    stack.Push(nodeNeedle);
                    var compare = comparison.Invoke(key, nodeNeedle.Value._key);
                    if (compare >= 0)
                    {
                        resultNeedle = nodeNeedle;
                    }
                    if (compare == 0)
                    {
                        break;
                    }
                    nodeNeedle = compare > 0 ? nodeNeedle.Value._rightNeedle : nodeNeedle.Value._leftNeedle;
                }
                if (resultNeedle == null)
                {
                    return null;
                }
                var found = resultNeedle.Value;
                RemoveExtracted(resultNeedle);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (ReferenceEquals(current, resultNeedle))
                    {
                        break;
                    }
                }
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    MakeBalanced(current);
                }
                return found;
            }

            internal static AVLNode RemoveNearestRight(INeedle<AVLNode> nodeNeedle, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                INeedle<AVLNode> resultNeedle = null;
                while (nodeNeedle.IsAlive)
                {
                    var compare = comparison.Invoke(key, nodeNeedle.Value._key);
                    if (compare <= 0)
                    {
                        resultNeedle = nodeNeedle;
                    }
                    if (compare == 0)
                    {
                        break;
                    }
                    nodeNeedle = compare > 0 ? nodeNeedle.Value._rightNeedle : nodeNeedle.Value._leftNeedle;
                }
                if (resultNeedle == null)
                {
                    return null;
                }
                var found = resultNeedle.Value;
                RemoveExtracted(resultNeedle);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (ReferenceEquals(current, resultNeedle))
                    {
                        break;
                    }
                }
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    MakeBalanced(current);
                }
                return found;
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
                    node = compare > 0 ? node._rightNeedle.Value : node._leftNeedle.Value;
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
                    node = compare < 0 ? node._leftNeedle.Value : node._rightNeedle.Value;
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
                    node = compare > 0 ? node._rightNeedle.Value : node._leftNeedle.Value;
                }
                return result;
            }

            private static int AddExtracted(INeedle<AVLNode> nodeNeedle, TKey key, TValue value)
            {
                if (nodeNeedle.IsAlive)
                {
                    return -1;
                }
                nodeNeedle.Value = new AVLNode(key, value);
                return 1;
            }

            private static int AddNonDuplicateExtracted(INeedle<AVLNode> nodeNeedle, TKey key, TValue value)
            {
                if (nodeNeedle.IsAlive)
                {
                    return 0;
                }
                nodeNeedle.Value = new AVLNode(key, value);
                return 1;
            }

            private static void DoubleLeft(INeedle<AVLNode> nodeNeedle)
            {
                if (nodeNeedle.Value._rightNeedle.IsAlive)
                {
                    RotateRight(nodeNeedle.Value._rightNeedle);
                    RotateLeft(nodeNeedle);
                }
            }

            private static void DoubleRight(INeedle<AVLNode> nodeNeedle)
            {
                if (nodeNeedle.Value._leftNeedle.IsAlive)
                {
                    RotateLeft(nodeNeedle.Value._leftNeedle);
                    RotateRight(nodeNeedle);
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

            private static void MakeBalanced(INeedle<AVLNode> nodeNeedle)
            {
                Update(nodeNeedle.Value);
                if (IsRightHeavy(nodeNeedle.Value))
                {
                    if (IsLeftHeavy(nodeNeedle.Value._rightNeedle.Value))
                    {
                        DoubleLeft(nodeNeedle);
                    }
                    else
                    {
                        RotateLeft(nodeNeedle);
                    }
                }
                else if (IsLeftHeavy(nodeNeedle.Value))
                {
                    if (IsRightHeavy(nodeNeedle.Value._leftNeedle.Value))
                    {
                        DoubleRight(nodeNeedle);
                    }
                    else
                    {
                        RotateRight(nodeNeedle);
                    }
                }
            }

            private static int RemoveExtracted(INeedle<AVLNode> nodeNeedle)
            {
                if (!nodeNeedle.IsAlive)
                {
                    return 0;
                }
                if (!nodeNeedle.Value._rightNeedle.IsAlive)
                {
                    nodeNeedle.Value = nodeNeedle.Value._leftNeedle.Value;
                }
                else
                {
                    if (!nodeNeedle.Value._leftNeedle.IsAlive)
                    {
                        nodeNeedle.Value = nodeNeedle.Value._rightNeedle.Value;
                    }
                    else
                    {
                        var trunk = nodeNeedle.Value._rightNeedle;
                        var successor = trunk;
                        while (successor.Value._leftNeedle.IsAlive)
                        {
                            trunk = successor;
                            successor = trunk.Value._leftNeedle;
                        }
                        if (ReferenceEquals(trunk, successor))
                        {
                            nodeNeedle.Value._rightNeedle = successor.Value._rightNeedle;
                        }
                        else
                        {
                            trunk.Value._leftNeedle = successor.Value._rightNeedle;
                        }
                        var tmpLeft = nodeNeedle.Value._leftNeedle;
                        var tmpRight = nodeNeedle.Value._rightNeedle;
                        var tmpBalance = nodeNeedle.Value._balance;
                        nodeNeedle.Value = new AVLNode(successor.Value._key, successor.Value._value)
                        {
                            _leftNeedle = tmpLeft,
                            _rightNeedle = tmpRight,
                            _balance = tmpBalance
                        };
                    }
                }
                if (nodeNeedle.IsAlive)
                {
                    MakeBalanced(nodeNeedle);
                }
                return 1;
            }

            private static void RotateLeft(INeedle<AVLNode> nodeNeedle)
            {
                if (nodeNeedle.Value._rightNeedle.IsAlive)
                {
                    var root = nodeNeedle.Value;
                    var right = nodeNeedle.Value._rightNeedle.Value;
                    var rightLeft = nodeNeedle.Value._rightNeedle.Value._leftNeedle.Value;
                    nodeNeedle.Value._rightNeedle.Value = rightLeft;
                    right._leftNeedle.Value = root;
                    nodeNeedle.Value = right;
                    Update(root);
                    Update(right);
                }
            }

            private static void RotateRight(INeedle<AVLNode> nodeNeedle)
            {
                if (nodeNeedle.Value._leftNeedle.IsAlive)
                {
                    var root = nodeNeedle.Value;
                    var left = nodeNeedle.Value._leftNeedle.Value;
                    var leftRight = nodeNeedle.Value._leftNeedle.Value._rightNeedle.Value;
                    nodeNeedle.Value._leftNeedle.Value = leftRight;
                    left._rightNeedle.Value = root;
                    nodeNeedle.Value = left;
                    Update(root);
                    Update(left);
                }
            }

            private static void Update(AVLNode node)
            {
                node._depth =
                    Math.Max
                        (
                            node._rightNeedle.IsAlive ? node._rightNeedle.Value._depth + 1 : 0,
                            node._leftNeedle.IsAlive ? node._leftNeedle.Value._depth + 1 : 0
                        );
                node._balance =
                    (node._rightNeedle.IsAlive ? node._rightNeedle.Value._depth + 1 : 0)
                    - (node._leftNeedle.IsAlive ? node._leftNeedle.Value._depth + 1 : 0);
            }
        }
    }
}