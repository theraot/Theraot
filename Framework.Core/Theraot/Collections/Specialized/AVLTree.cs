// Needed for NET35 (SortedSet, OrderedCollection)

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed class AVLTree<TKey, TValue> : IEnumerable<AVLNode<TKey, TValue>>
    {
        private readonly IComparer<TKey> _comparer;
        private AVLNode<TKey, TValue> _root;

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

        public AVLNode<TKey, TValue> Root => _root;

        public void Add(TKey key, TValue value)
        {
            AVLNode<TKey, TValue>.Add(ref _root, key, value, _comparer);
            Count++;
        }

        public bool AddNonDuplicate(TKey key, TValue value)
        {
            if (AVLNode<TKey, TValue>.AddNonDuplicate(ref _root, key, value, _comparer))
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

        public AVLNode<TKey, TValue> Get(TKey key)
        {
            return AVLNode<TKey, TValue>.Get(_root, key, _comparer);
        }

        public IEnumerator<AVLNode<TKey, TValue>> GetEnumerator()
        {
            return AVLNode<TKey, TValue>.EnumerateRoot(_root).GetEnumerator();
        }

        public AVLNode<TKey, TValue> GetNearestLeft(TKey key)
        {
            return AVLNode<TKey, TValue>.GetNearestLeft(_root, key, _comparer);
        }

        public AVLNode<TKey, TValue> GetNearestRight(TKey key)
        {
            return AVLNode<TKey, TValue>.GetNearestRight(_root, key, _comparer);
        }

#if FAT
        public AVLNode<TKey, TValue> GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            var result = AVLNode<TKey, TValue>.GetOrAdd(ref _root, key, factory, _comparer, out var isNew);
            if (isNew)
            {
                Count++;
            }
            return result;
        }
#endif

        public IEnumerable<AVLNode<TKey, TValue>> Range(TKey lower, TKey upper)
        {
            foreach (var item in AVLNode<TKey, TValue>.EnumerateFrom(_root, lower, _comparer))
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
            if (AVLNode<TKey, TValue>.Remove(ref _root, key, _comparer))
            {
                Count--;
                return true;
            }

            return false;
        }

        public AVLNode<TKey, TValue> RemoveNearestLeft(TKey key)
        {
            return AVLNode<TKey, TValue>.RemoveNearestLeft(ref _root, key, _comparer);
        }

        public AVLNode<TKey, TValue> RemoveNearestRight(TKey key)
        {
            return AVLNode<TKey, TValue>.RemoveNearestRight(ref _root, key, _comparer);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}