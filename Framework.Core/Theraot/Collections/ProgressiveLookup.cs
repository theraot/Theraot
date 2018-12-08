using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveLookup<TKey, T> : ILookup<TKey, T>
    {
        private readonly IDictionary<TKey, IGrouping<TKey, T>> _cache;
        private readonly ProgressiveSet<TKey> _keysReadonly;

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
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Progressor = new Progressor<IGrouping<TKey, T>>(wrapped);
            Progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, T>>(obj.Key, obj)));
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            ItemComparer = itemComparer ?? EqualityComparer<T>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor.ConvertProgressive(input => input.Key), keyComparer);
        }

        protected ProgressiveLookup(IObservable<IGrouping<TKey, T>> wrapped, IDictionary<TKey, IGrouping<TKey, T>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Progressor = new Progressor<IGrouping<TKey, T>>(wrapped);
            Progressor.SubscribeAction(obj => _cache.Add(new KeyValuePair<TKey, IGrouping<TKey, T>>(obj.Key, obj)));
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            ItemComparer = itemComparer ?? EqualityComparer<T>.Default;
            _keysReadonly = new ProgressiveSet<TKey>(Progressor.ConvertProgressive(input => input.Key), keyComparer);
        }

        public int Count
        {
            get
            {
                Progressor.AsEnumerable().Consume();
                return _cache.Count;
            }
        }

        public bool EndOfEnumeration => Progressor.IsClosed;

        public IReadOnlyCollection<TKey> Keys => _keysReadonly;

        protected IEqualityComparer<T> ItemComparer { get; }

        protected IEqualityComparer<TKey> KeyComparer { get; }

        protected Progressor<IGrouping<TKey, T>> Progressor { get; }

        public IEnumerable<T> this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var grouping))
                {
                    return grouping;
                }
                return ArrayReservoir<T>.EmptyArray;
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

        public static ProgressiveLookup<TKey, T> Create<TGroupingDictionary>(IEnumerable<IGrouping<TKey, T>> wrapped, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
            where TGroupingDictionary : IDictionary<TKey, IGrouping<TKey, T>>, new()
        {
            return new ProgressiveLookup<TKey, T>(wrapped, new TGroupingDictionary(), keyComparer, itemComparer);
        }

        public static ProgressiveLookup<TKey, T> Create<TGroupingDictionary>(IObservable<IGrouping<TKey, T>> wrapped, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
            where TGroupingDictionary : IDictionary<TKey, IGrouping<TKey, T>>, new()
        {
            return new ProgressiveLookup<TKey, T>(wrapped, new TGroupingDictionary(), keyComparer, itemComparer);
        }

        public bool Contains(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                return true;
            }
            while (Progressor.TryTake(out var item))
            {
                if (KeyComparer.Equals(key, item.Key))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, T>>[] array)
        {
            Progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, 0);
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, T>>[] array, int arrayIndex)
        {
            Progressor.AsEnumerable().Consume();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, IGrouping<TKey, T>>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Progressor.While(() => _cache.Count < countLimit).Consume();
            Extensions.CopyTo(_cache, array, arrayIndex, countLimit);
        }

        public void CopyTo(IGrouping<TKey, T>[] array, int arrayIndex)
        {
            Progressor.AsEnumerable().Consume();
            _cache.Values.CopyTo(array, arrayIndex);
        }

        public void CopyTo(IGrouping<TKey, T>[] array)
        {
            Progressor.AsEnumerable().Consume();
            _cache.Values.CopyTo(array, 0);
        }

        public void CopyTo(IGrouping<TKey, T>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Progressor.While(() => _cache.Count < countLimit).Consume();
            Extensions.CopyTo(_cache.Values, array, arrayIndex, countLimit);
        }

        public IEnumerator<IGrouping<TKey, T>> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item.Value;
            }
            {
                while (Progressor.TryTake(out var item))
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
            while (Progressor.TryTake(out var item))
            {
                if (KeyComparer.Equals(key, item.Key))
                {
                    value = item;
                    return true;
                }
            }
            return false;
        }
    }
}