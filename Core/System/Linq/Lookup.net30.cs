#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections;
using Theraot.Collections.Specialized;
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
            get
            {
                return _groupings.Count;
            }
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
                    return EmptySet<TElement>.Instance;
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
            var _source = Check.NotNullArgument(source, "source");
            var _elementSelector = Check.NotNullArgument(elementSelector, "elementSelector");
            var _keySelector = Check.NotNullArgument(keySelector, "keySelector");
            var result = new Lookup<TKey, TElement>(comparer);
            foreach (TSource item in _source)
            {
                result.GetOrCreateGrouping(_keySelector(item)).Add(_elementSelector(item));
            }
            return result;
        }

        private ICollection<TElement> GetOrCreateGrouping(TKey key)
        {
            Grouping grouping;
            if (_groupings.TryGetValue(key, out grouping))
            {
                return grouping.Items;
            }
            else
            {
                return EmptySet<TElement>.Instance;
            }
        }

        internal sealed class Grouping : IGrouping<TKey, TElement>
        {
            private readonly Collection<TElement> _items;
            private TKey _key;

            internal Grouping(TKey key)
            {
                _items = new Collection<TElement>();
                _key = key;
            }

            public Collection<TElement> Items
            {
                get
                {
                    return _items;
                }
            }

            public TKey Key
            {
                get
                {
                    return _key;
                }
                set
                {
                    _key = value;
                }
            }

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