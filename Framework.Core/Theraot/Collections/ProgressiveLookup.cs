#pragma warning disable CS8714 // Nullability of type argument doesn't match 'notnull' constraint

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public class ProgressiveLookup<TKey, T> : ILookup<TKey, T>
    {
        private readonly NullAwareDictionary<TKey, IGrouping<TKey, T>> _cache;
        private readonly IDisposable _subscription;

        public ProgressiveLookup(IEnumerable<IGrouping<TKey, T>> enumerable)
            : this(Progressor<IGrouping<TKey, T>>.CreateFromIEnumerable(enumerable), new NullAwareDictionary<TKey, IGrouping<TKey, T>>(), null, null)
        {
            // Empty
        }

        public ProgressiveLookup(IEnumerable<IGrouping<TKey, T>> enumerable, IEqualityComparer<TKey> keyComparer)
            : this(Progressor<IGrouping<TKey, T>>.CreateFromIEnumerable(enumerable), new NullAwareDictionary<TKey, IGrouping<TKey, T>>(keyComparer), keyComparer, null)
        {
            // Empty
        }

        public ProgressiveLookup(IObservable<IGrouping<TKey, T>> observable)
            : this(Progressor<IGrouping<TKey, T>>.CreateFromIObservable(observable), new NullAwareDictionary<TKey, IGrouping<TKey, T>>(), null, null)
        {
            // Empty
        }

        public ProgressiveLookup(IObservable<IGrouping<TKey, T>> observable, IEqualityComparer<TKey> keyComparer)
            : this(Progressor<IGrouping<TKey, T>>.CreateFromIObservable(observable), new NullAwareDictionary<TKey, IGrouping<TKey, T>>(keyComparer), keyComparer, null)
        {
            // Empty
        }

        protected ProgressiveLookup(Progressor<IGrouping<TKey, T>> progressor, NullAwareDictionary<TKey, IGrouping<TKey, T>> cache, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<T>? itemComparer)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Progressor = progressor ?? throw new ArgumentNullException(nameof(progressor));
            _subscription = Progressor.SubscribeAction(obj => _cache.Add(obj.Key, obj));
            Comparer = keyComparer ?? EqualityComparer<TKey>.Default;
            ItemComparer = itemComparer ?? EqualityComparer<T>.Default;
            Keys = EnumerationList<TKey>.Create(this.ConvertProgressive(input => input.Key));
        }

        ~ProgressiveLookup()
        {
            Close();
        }

        public int Count
        {
            get
            {
                ConsumeAll();
                return _cache.Count;
            }
        }

        public IReadOnlyCollection<TKey> Keys { get; }

        protected IEqualityComparer<TKey> Comparer { get; }
        protected IEqualityComparer<T> ItemComparer { get; }
        private Progressor<IGrouping<TKey, T>> Progressor { get; }

        public IEnumerable<T> this[TKey key]
        {
            get
            {
                return TryGetValue(key, out var grouping) ? grouping : Enumerable.Empty<T>();
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

#if FAT
        internal static ProgressiveLookup<TKey, T> Create<TGroupingDictionary>(Progressor<IGrouping<TKey, T>> progressor, IEqualityComparer<TKey> keyComparer, IEqualityComparer<T> itemComparer)
            where TGroupingDictionary : IDictionary<TKey, IGrouping<TKey, T>>, new()
        {
            return new ProgressiveLookup<TKey, T>(progressor, new TGroupingDictionary(), keyComparer, itemComparer);
        }
#endif

        public bool Contains(TKey key)
        {
            return _cache.ContainsKey(key) || ProgressorWhere(Check).Any();

            bool Check(IGrouping<TKey, T> item)
            {
                return Comparer.Equals(key, item.Key);
            }
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
            _cache.CopyTo(array, arrayIndex, countLimit);
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
            _cache.Values.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<IGrouping<TKey, T>> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item.Value;
            }

            var knownCount = _cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (_cache.Count > knownCount)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out IGrouping<TKey, T> value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                return true;
            }

            foreach (var found in ProgressorWhere(Check))
            {
                value = found;
                return true;
            }

            return false;

            bool Check(IGrouping<TKey, T> item)
            {
                return Comparer.Equals(key, item.Key);
            }
        }

        protected void ConsumeAll()
        {
            Progressor.Consume();
        }

        protected IEnumerable<IGrouping<TKey, T>> ProgressorWhere(Predicate<IGrouping<TKey, T>> check)
        {
            var knownCount = _cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (_cache.Count <= knownCount)
                {
                    continue;
                }

                if (check(item))
                {
                    yield return item;
                }

                knownCount = _cache.Count;
            }
        }

        protected IEnumerable<IGrouping<TKey, T>> ProgressorWhile(Predicate<IGrouping<TKey, T>> check)
        {
            var knownCount = _cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (_cache.Count <= knownCount)
                {
                    continue;
                }

                if (check(item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }

                knownCount = _cache.Count;
            }
        }

        private void Close()
        {
            _subscription?.Dispose();
            Progressor?.Close();
        }
    }
}