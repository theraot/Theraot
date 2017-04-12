#if FAT

using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveLookup<TKey, T> : ILookup<TKey, T>
    {
        private readonly IDictionary<TKey, IGrouping<TKey, T>> _cache;
        private readonly IEqualityComparer<T> _itemComparer;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ProgressiveSet<TKey> _keysReadonly;
        private readonly Progressor<IGrouping<TKey, T>> _progressor;

        public ProgressiveLookup(IEnumerable<IGrouping<TKey, T>> wrapped)
            : this(wrapped, new NullAwareDictionary<TKey, IGrouping<TKey, T>>(), null, null)
        {
            // Empty
        }

        public ProgressiveLookup(IEnumerable<IGrouping<TKey, T>> wrapped, IEqualityComparer<TKey> keyComparer)
            : this(wrapped, new NullAwareDictionary<TKey, IGrouping<TKey, T>>(keyComparer), keyComparer, null)
        {
            // Empty
        }

        protected ProgressiveLookup(IEnumerable<IGrouping<TKey, T>> wrapped, IDictionary<TKey, IGrouping<TKey, T>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
            _progressor = new Progressor<IGrouping<TKey, T>>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, T>>(obj.Key, obj)));
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<T>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor<TKey>.CreateConverted(Progressor, input => input.Key), keyComparer);
        }

        protected ProgressiveLookup(Progressor<IGrouping<TKey, T>> wrapped, IDictionary<TKey, IGrouping<TKey, T>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
            _progressor = new Progressor<IGrouping<TKey, T>>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, T>>(obj.Key, obj)));
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<T>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor<TKey>.CreateConverted(Progressor, input => input.Key), keyComparer);
        }

        protected ProgressiveLookup(TryTake<IGrouping<TKey, T>> tryTake, IDictionary<TKey, IGrouping<TKey, T>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
            _progressor = new Progressor<IGrouping<TKey, T>>(tryTake, false); // false because the underlaying structure may change
            _progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, T>>(obj.Key, obj)));
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<T>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor<TKey>.CreateConverted(Progressor, input => input.Key), keyComparer);
        }

        public int Count
        {
            get
            {
                _progressor.AsEnumerable().Consume();
                return _cache.Count;
            }
        }

        public bool EndOfEnumeration
        {
            get { return _progressor.IsClosed; }
        }

        public IReadOnlyCollection<TKey> Keys
        {
            get { return _keysReadonly; }
        }

        protected IEqualityComparer<T> ItemComparer
        {
            get { return _itemComparer; }
        }

        protected IEqualityComparer<TKey> KeyComparer
        {
            get { return _keyComparer; }
        }

        protected Progressor<IGrouping<TKey, T>> Progressor
        {
            get { return _progressor; }
        }

        public IEnumerable<T> this[TKey key]
        {
            get
            {
                IGrouping<TKey, T> grouping;
                if (TryGetValue(key, out grouping))
                {
                    return grouping;
                }
                else
                {
                    return ArrayReservoir<T>.EmptyArray;
                }
            }
        }

        public static ProgressiveLookup<TKey, T> Create(IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return new ProgressiveLookup<TKey, T>(source.GroupProgressiveBy(keySelector));
        }

        public static ProgressiveLookup<TKey, T> Create(IEnumerable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        {
            return new ProgressiveLookup<TKey, T>(source.GroupProgressiveBy(keySelector, keyComparer), keyComparer);
        }

        public static ProgressiveLookup<TKey, T> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, T> elementSelector, IEqualityComparer<TKey> keyComparer)
        {
            return new ProgressiveLookup<TKey, T>(source.GroupProgressiveBy(keySelector, elementSelector, keyComparer), keyComparer);
        }

        public static ProgressiveLookup<TKey, T> Create(IEnumerable<KeyValuePair<TKey, T>> source)
        {
            return new ProgressiveLookup<TKey, T>(source.GroupProgressiveBy(item => item.Key, item => item.Value));
        }

        public static ProgressiveLookup<TKey, T> Create(IEnumerable<KeyValuePair<TKey, T>> source, IEqualityComparer<TKey> keyComparer)
        {
            return new ProgressiveLookup<TKey, T>(source.GroupProgressiveBy(item => item.Key, item => item.Value, keyComparer), keyComparer);
        }

        public bool Contains(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                return true;
            }
            else
            {
                IGrouping<TKey, T> item;
                while (_progressor.TryTake(out item))
                {
                    if (_keyComparer.Equals(key, item.Key))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, T>>[] array)
        {
            _progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, 0);
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, T>>[] array, int arrayIndex)
        {
            _progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, T>>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.While(() => _cache.Count < countLimit).Consume();
            _cache.CopyTo(array, arrayIndex, countLimit);
        }

        public void CopyTo(IGrouping<TKey, T>[] array, int arrayIndex)
        {
            _progressor.AsEnumerable().Consume();
            _cache.Values.CopyTo(array, arrayIndex);
        }

        public void CopyTo(IGrouping<TKey, T>[] array)
        {
            _progressor.AsEnumerable().Consume();
            _cache.Values.CopyTo(array, 0);
        }

        public void CopyTo(IGrouping<TKey, T>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.While(() => _cache.Count < countLimit).Consume();
            _cache.Values.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<IGrouping<TKey, T>> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item.Value;
            }
            {
                IGrouping<TKey, T> item;
                while (_progressor.TryTake(out item))
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out IGrouping<TKey, T> value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                return true;
            }
            IGrouping<TKey, T> item;
            while (_progressor.TryTake(out item))
            {
                if (_keyComparer.Equals(key, item.Key))
                {
                    value = item;
                    return true;
                }
            }
            return false;
        }
    }
}

#endif