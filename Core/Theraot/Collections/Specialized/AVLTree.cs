// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed partial class AVLTree<TKey, TValue>
    {
        private readonly Comparison<TKey> _comparison;

        private int _count;
        private AVLNode _root;

        public AVLTree()
        {
            _root = null;
            _comparison = Comparer<TKey>.Default.Compare;
        }

        public AVLTree(IComparer<TKey> comparer)
        {
            _root = null;
            _comparison = (comparer ?? Comparer<TKey>.Default).Compare;
        }

        public AVLTree(Comparison<TKey> comparison)
        {
            _root = null;
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
                return _root;
            }
        }

        public void Add(TKey key, TValue value)
        {
            AVLNode.Add(ref _root, key, value, _comparison);
            _count++;
        }

        public bool AddNonDuplicate(TKey key, TValue value)
        {
            if (AVLNode.AddNonDuplicate(ref _root, key, value, _comparison))
            {
                _count++;
                return true;
            }
            return false;
        }

        public void Bound(TKey key, out AVLNode lower, out AVLNode upper)
        {
            AVLNode.Bound(_root, key, _comparison, out lower, out upper);
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        public IEnumerator<AVLNode> GetEnumerator()
        {
            return AVLNode.EnumerateRoot(_root).GetEnumerator();
        }

        public IEnumerable<AVLNode> Range(TKey lower, TKey upper)
        {
            foreach (var item in AVLNode.EnumerateFrom(_root, lower, _comparison))
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
            if (AVLNode.Remove(ref _root, key, _comparison))
            {
                _count--;
                return true;
            }
            return false;
        }

        public AVLNode Search(TKey key)
        {
            return AVLNode.Search(_root, key, _comparison);
        }

        public AVLNode SearchNearestLeft(TKey key)
        {
            return AVLNode.SearchNearestLeft(_root, key, _comparison);
        }

        public AVLNode SearchNearestRight(TKey key)
        {
            return AVLNode.SearchNearestRight(_root, key, _comparison);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}