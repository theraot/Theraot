#if FAT

using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Collections
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveLookup<TKey, TItem> : ILookup<TKey, TItem>
    {
        private readonly IDictionary<TKey, IGrouping<TKey, TItem>> _cache;
        private readonly IEqualityComparer<TItem> _itemComparer;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ProgressiveSet<TKey> _keysReadonly;
        private readonly Progressor<IGrouping<TKey, TItem>> _progressor;

        public ProgressiveLookup(IEnumerable<IGrouping<TKey, TItem>> wrapped)
            : this(wrapped, new NullAwareDictionary<TKey, IGrouping<TKey, TItem>>(), null, null)
        {
            // Empty
        }

        public ProgressiveLookup(IEnumerable<IGrouping<TKey, TItem>> wrapped, IEqualityComparer<TKey> keyComparer)
            : this(wrapped, new NullAwareDictionary<TKey, IGrouping<TKey, TItem>>(keyComparer), keyComparer, null)
        {
            // Empty
        }

        protected ProgressiveLookup(IEnumerable<IGrouping<TKey, TItem>> wrapped, IDictionary<TKey, IGrouping<TKey, TItem>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TItem> itemComparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<IGrouping<TKey, TItem>>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, TItem>>(obj.Key, obj)));
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor<TKey>.CreateConverted(Progressor, input => input.Key), keyComparer);
        }

        protected ProgressiveLookup(Progressor<IGrouping<TKey, TItem>> wrapped, IDictionary<TKey, IGrouping<TKey, TItem>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TItem> itemComparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<IGrouping<TKey, TItem>>(wrapped);
            _progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, TItem>>(obj.Key, obj)));
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor<TKey>.CreateConverted(Progressor, input => input.Key), keyComparer);
        }

        protected ProgressiveLookup(TryTake<IGrouping<TKey, TItem>> tryTake, IDictionary<TKey, IGrouping<TKey, TItem>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TItem> itemComparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _progressor = new Progressor<IGrouping<TKey, TItem>>(tryTake);
            _progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, TItem>>(obj.Key, obj)));
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
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
            get
            {
                return _progressor.IsClosed;
            }
        }

        public IReadOnlyCollection<TKey> Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        protected IEqualityComparer<TItem> ItemComparer
        {
            get
            {
                return _itemComparer;
            }
        }

        protected IEqualityComparer<TKey> KeyComparer
        {
            get
            {
                return _keyComparer;
            }
        }

        protected Progressor<IGrouping<TKey, TItem>> Progressor
        {
            get
            {
                return _progressor;
            }
        }

        public IEnumerable<TItem> this[TKey key]
        {
            get
            {
                IGrouping<TKey, TItem> grouping;
                if (TryGetValue(key, out grouping))
                {
                    return grouping;
                }
                else
                {
                    return ArrayReservoir<TItem>.EmptyArray;
                }
            }
        }

        public static ProgressiveLookup<TKey, TItem> Create(IEnumerable<TItem> source, Func<TItem, TKey> keySelector)
        {
            return new ProgressiveLookup<TKey, TItem>(source.GroupBy(keySelector));
        }

        public static ProgressiveLookup<TKey, TItem> Create(IEnumerable<TItem> source, Func<TItem, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        {
            return new ProgressiveLookup<TKey, TItem>(source.GroupBy(keySelector, keyComparer), keyComparer);
        }

        public static ProgressiveLookup<TKey, TItem> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TItem> elementSelector, IEqualityComparer<TKey> keyComparer)
        {
            return new ProgressiveLookup<TKey, TItem>(source.GroupBy(keySelector, elementSelector, keyComparer), keyComparer);
        }

        public static ProgressiveLookup<TKey, TItem> Create(IEnumerable<KeyValuePair<TKey, TItem>> source)
        {
            return new ProgressiveLookup<TKey, TItem>(source.GroupBy(item => item.Key, item => item.Value));
        }

        public static ProgressiveLookup<TKey, TItem> Create(IEnumerable<KeyValuePair<TKey, TItem>> source, IEqualityComparer<TKey> keyComparer)
        {
            return new ProgressiveLookup<TKey, TItem>(source.GroupBy(item => item.Key, item => item.Value, keyComparer), keyComparer);
        }

        public bool Contains(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                return true;
            }
            else
            {
                IGrouping<TKey, TItem> _item;
                while (_progressor.TryTake(out _item))
                {
                    if (_keyComparer.Equals(key, _item.Key))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, TItem>>[] array)
        {
            _progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, 0);
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, TItem>>[] array, int arrayIndex)
        {
            _progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, TItem>>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.While(() => _cache.Count < countLimit).Consume();
            _cache.CopyTo(array, arrayIndex, countLimit);
        }

        public void CopyTo(IGrouping<TKey, TItem>[] array, int arrayIndex)
        {
            _progressor.AsEnumerable().Consume();
            _cache.Values.CopyTo(array, arrayIndex);
        }

        public void CopyTo(IGrouping<TKey, TItem>[] array)
        {
            _progressor.AsEnumerable().Consume();
            _cache.Values.CopyTo(array, 0);
        }

        public void CopyTo(IGrouping<TKey, TItem>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.While(() => _cache.Count < countLimit).Consume();
            _cache.Values.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<IGrouping<TKey, TItem>> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item.Value;
            }
            {
                IGrouping<TKey, TItem> item;
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

        public bool TryGetValue(TKey key, out IGrouping<TKey, TItem> value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                return true;
            }
            IGrouping<TKey, TItem> _item;
            while (_progressor.TryTake(out _item))
            {
                if (_keyComparer.Equals(key, _item.Key))
                {
                    value = _item;
                    return true;
                }
            }
            return false;
        }
    }
}

#endif