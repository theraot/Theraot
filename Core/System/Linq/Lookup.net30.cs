#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace System.Linq
{
    public class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly IDictionary<TKey, Grouping> _groupings;

        internal Lookup(IEqualityComparer<TKey> comparer)
        {
            if (typeof(TKey).CanBeNull())
            {
                _groupings = new NullAwareDictionary<TKey, Grouping>(comparer);
            }
            else
            {
                _groupings = new Dictionary<TKey, Grouping>(comparer);
            }
        }

        public int Count
        {
            get { return _groupings.Count; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                Grouping grouping;
                if (_groupings.TryGetValue(key, out grouping))
                {
                    return grouping;
                }
                else
                {
                    return ArrayReservoir<TElement>.EmptyArray;
                }
            }
        }

        public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            foreach (var group in _groupings.Values)
            {
                yield return resultSelector(group.Key, group);
            }
        }

        public bool Contains(TKey key)
        {
            return _groupings.ContainsKey(key);
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (var grouping in _groupings.Values)
            {
                yield return grouping;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static Lookup<TKey, TElement> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            var result = new Lookup<TKey, TElement>(comparer);
            foreach (var item in source)
            {
                result.GetOrCreateGrouping(keySelector(item)).Add(elementSelector(item));
            }
            return result;
        }

        private ICollection<TElement> GetOrCreateGrouping(TKey key)
        {
            Grouping grouping;
            if (!_groupings.TryGetValue(key, out grouping))
            {
                grouping = new Grouping(key);
                _groupings.Add(key, grouping);
            }
            return grouping.Items;
        }

        internal sealed class Grouping : IGrouping<TKey, TElement>
        {
            private readonly Collection<TElement> _items;

            internal Grouping(TKey key)
            {
                _items = new Collection<TElement>();
                Key = key;
            }

            public Collection<TElement> Items
            {
                get { return _items; }
            }

            public TKey Key { get; set; }

            public IEnumerator<TElement> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _items.GetEnumerator();
            }
        }
    }
}

#endif