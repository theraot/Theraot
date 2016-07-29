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
            private INeedle<AVLNode> _left;
            private INeedle<AVLNode> _right;

            private AVLNode(TKey key, TValue value)
            {
                _key = key;
                _value = value;
                _left = new StructNeedle<AVLNode>();
                _right = new StructNeedle<AVLNode>();
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
                    return _left.Value;
                }
            }

            public AVLNode Right
            {
                get
                {
                    return _right.Value;
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

            internal static void Add(INeedle<AVLNode> node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                while (true)
                {
                    int compare;
                    if (!node.IsAlive || (compare = comparison(key, node.Value._key)) == 0)
                    {
                        var result = AddExtracted(node, key, value);
                        if (result != -1)
                        {
                            while (stack.Count > 0)
                            {
                                MakeBalanced(stack.Pop());
                            }
                            return;
                        }
                        compare = -node.Value._balance;
                    }
                    stack.Push(node);
                    node = compare < 0 ? node.Value._left : node.Value._right;
                }
            }

            internal static bool AddNonDuplicate(INeedle<AVLNode> node, TKey key, TValue value, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                while (true)
                {
                    int compare;
                    if (!node.IsAlive || (compare = comparison(key, node.Value._key)) == 0)
                    {
                        var result = AddNonDuplicateExtracted(node, key, value);
                        if (result != -1)
                        {
                            while (stack.Count > 0)
                            {
                                MakeBalanced(stack.Pop());
                            }
                            return result != 0;
                        }
                        compare = -node.Value._balance;
                    }
                    stack.Push(node);
                    node = compare < 0 ? node.Value._left : node.Value._right;
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
                    node = compare > 0 ? node._right.Value : node._left.Value;
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
                        node = node._right.Value;
                    }
                    else
                    {
                        stack.Push(node);
                        node = node._left.Value;
                    }
                }
                while (true)
                {
                    if (node != null)
                    {
                        yield return node;
                        foreach (var item in EnumerateRoot(node._right.Value))
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

            internal static bool Remove(INeedle<AVLNode> node, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                while (true)
                {
                    int compare;
                    if (!node.IsAlive || (compare = comparison(key, node.Value._key)) == 0)
                    {
                        var result = RemoveExtracted(node);
                        if (result != -1)
                        {
                            while (stack.Count > 0)
                            {
                                MakeBalanced(stack.Pop());
                            }
                            return result != 0;
                        }
                        compare = -node.Value._balance;
                    }
                    stack.Push(node);
                    node = compare < 0 ? node.Value._left : node.Value._right;
                }
            }

            internal static AVLNode RemoveNearestLeft(INeedle<AVLNode> node, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                INeedle<AVLNode> result = null;
                while (node.IsAlive)
                {
                    stack.Push(node);
                    var compare = comparison.Invoke(key, node.Value._key);
                    if (compare >= 0)
                    {
                        result = node;
                    }
                    if (compare == 0)
                    {
                        break;
                    }
                    node = compare > 0 ? node.Value._right : node.Value._left;
                }
                if (result == null)
                {
                    return null;
                }
                var found = result.Value;
                RemoveExtracted(result);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (ReferenceEquals(current, result))
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

            internal static AVLNode RemoveNearestRight(INeedle<AVLNode> node, TKey key, Comparison<TKey> comparison)
            {
                var stack = new Stack<INeedle<AVLNode>>();
                INeedle<AVLNode> result = null;
                while (node.IsAlive)
                {
                    var compare = comparison.Invoke(key, node.Value._key);
                    if (compare <= 0)
                    {
                        result = node;
                    }
                    if (compare == 0)
                    {
                        break;
                    }
                    node = compare > 0 ? node.Value._right : node.Value._left;
                }
                if (result == null)
                {
                    return null;
                }
                var found = result.Value;
                RemoveExtracted(result);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (ReferenceEquals(current, result))
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
                    node = compare > 0 ? node._right.Value : node._left.Value;
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
                    node = compare < 0 ? node._left.Value : node._right.Value;
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
                    node = compare > 0 ? node._right.Value : node._left.Value;
                }
                return result;
            }

            private static int AddExtracted(INeedle<AVLNode> node, TKey key, TValue value)
            {
                if (node.IsAlive)
                {
                    return -1;
                }
                node.Value = new AVLNode(key, value);
                return 1;
            }

            private static int AddNonDuplicateExtracted(INeedle<AVLNode> node, TKey key, TValue value)
            {
                if (node.IsAlive)
                {
                    return 0;
                }
                node.Value = new AVLNode(key, value);
                return 1;
            }

            private static void DoubleLeft(INeedle<AVLNode> node)
            {
                if (node.Value._right.IsAlive)
                {
                    RotateRight(node.Value._right);
                    RotateLeft(node);
                }
            }

            private static void DoubleRight(INeedle<AVLNode> node)
            {
                if (node.Value._left.IsAlive)
                {
                    RotateLeft(node.Value._left);
                    RotateRight(node);
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

            private static void MakeBalanced(INeedle<AVLNode> node)
            {
                Update(node.Value);
                if (IsRightHeavy(node.Value))
                {
                    if (IsLeftHeavy(node.Value._right.Value))
                    {
                        DoubleLeft(node);
                    }
                    else
                    {
                        RotateLeft(node);
                    }
                }
                else if (IsLeftHeavy(node.Value))
                {
                    if (IsRightHeavy(node.Value._left.Value))
                    {
                        DoubleRight(node);
                    }
                    else
                    {
                        RotateRight(node);
                    }
                }
            }

            private static void Update(AVLNode node)
            {
                node._depth =
                    Math.Max
                        (
                            node._right.IsAlive ? node._right.Value._depth + 1 : 0,
                            node._left.IsAlive ? node._left.Value._depth + 1 : 0
                        );
                node._balance =
                    (node._right.IsAlive ? node._right.Value._depth + 1 : 0)
                    - (node._left.IsAlive ? node._left.Value._depth + 1 : 0);
            }

            private static int RemoveExtracted(INeedle<AVLNode> node)
            {
                if (!node.IsAlive)
                {
                    return 0;
                }
                if (!node.Value._right.IsAlive)
                {
                    node.Value = node.Value._left.Value;
                }
                else
                {
                    if (!node.Value._left.IsAlive)
                    {
                        node.Value = node.Value._right.Value;
                    }
                    else
                    {
                        var trunk = node.Value._right;
                        var successor = trunk;
                        while (successor.Value._left.IsAlive)
                        {
                            trunk = successor;
                            successor = trunk.Value._left;
                        }
                        if (ReferenceEquals(trunk, successor))
                        {
                            node.Value._right = successor.Value._right;
                        }
                        else
                        {
                            trunk.Value._left = successor.Value._right;
                        }
                        var tmpLeft = node.Value._left;
                        var tmpRight = node.Value._right;
                        var tmpBalance = node.Value._balance;
                        node.Value = new AVLNode(successor.Value._key, successor.Value._value)
                        {
                            _left = tmpLeft,
                            _right = tmpRight,
                            _balance = tmpBalance
                        };
                    }
                }
                if (node.IsAlive)
                {
                    MakeBalanced(node);
                }
                return 1;
            }

            private static void RotateLeft(INeedle<AVLNode> node)
            {
                if (node.Value._right.IsAlive)
                {
                    var root = node.Value;
                    var right = node.Value._right.Value;
                    var rightLeft = node.Value._right.Value._left.Value;
                    node.Value._right.Value = rightLeft;
                    right._left.Value = root;
                    node.Value = right;
                    Update(root);
                    Update(right);
                }
            }

            private static void RotateRight(INeedle<AVLNode> node)
            {
                if (node.Value._left.IsAlive)
                {
                    var root = node.Value;
                    var left = node.Value._left.Value;
                    var leftRight = node.Value._left.Value._right.Value;
                    node.Value._left.Value = leftRight;
                    left._right.Value = root;
                    node.Value = left;
                    Update(root);
                    Update(left);
                }
            }
        }
    }
}