// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections.Generic;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed partial class AVLTree<TKey, TValue>
    {
        private readonly Comparison<TKey> _comparison;

        private int _count;
        private INeedle<AVLNode> _root;

        public AVLTree()
        {
            _root = new StructNeedle<AVLNode>();
            _comparison = Comparer<TKey>.Default.Compare;
        }

        public AVLTree(IComparer<TKey> comparer)
        {
            _root = new StructNeedle<AVLNode>();
            _comparison = (comparer ?? Comparer<TKey>.Default).Compare;
        }

        public AVLTree(Comparison<TKey> comparison)
        {
            _root = new StructNeedle<AVLNode>();
            _comparison = comparison ?? Comparer<TKey>.Default.Compare;
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public AVLNode Root
        {
            get
            {
                return _root.Value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            AVLNode.Add(_root, key, value, _comparison);
            _count++;
        }

        public bool AddNonDuplicate(TKey key, TValue value)
        {
            if (AVLNode.AddNonDuplicate(_root, key, value, _comparison))
            {
                _count++;
                return true;
            }
            return false;
        }

        public void Bound(TKey key, out AVLNode lower, out AVLNode upper)
        {
            AVLNode.Bound(_root.Value, key, _comparison, out lower, out upper);
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        public IEnumerator<AVLNode> GetEnumerator()
        {
            return AVLNode.EnumerateRoot(_root.Value).GetEnumerator();
        }

        public IEnumerable<AVLNode> Range(TKey lower, TKey upper)
        {
            foreach (var item in AVLNode.EnumerateFrom(_root.Value, lower, _comparison))
            {
                var comparison = _comparison;
                if (comparison(item.Key, upper) > 0)
                {
                    break;
                }
                yield return item;
            }
        }

        public bool Remove(TKey key)
        {
            if (AVLNode.Remove(_root, key, _comparison))
            {
                _count--;
                return true;
            }
            return false;
        }

        public AVLNode RemoveNearestLeft(TKey key)
        {
            return AVLNode.RemoveNearestLeft(_root, key, _comparison);
        }

        public AVLNode RemoveNearestRight(TKey key)
        {
            return AVLNode.RemoveNearestRight(_root, key, _comparison);
        }

        public AVLNode Search(TKey key)
        {
            return AVLNode.Search(_root.Value, key, _comparison);
        }

        public AVLNode SearchNearestLeft(TKey key)
        {
            return AVLNode.SearchNearestLeft(_root.Value, key, _comparison);
        }

        public AVLNode SearchNearestRight(TKey key)
        {
            return AVLNode.SearchNearestRight(_root.Value, key, _comparison);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}