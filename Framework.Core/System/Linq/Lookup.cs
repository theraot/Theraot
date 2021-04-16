#if LESSTHAN_NET35

using System.Collections;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Reflection;

namespace System.Linq
{
    public class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly IDictionary<TKey, Grouping<TKey, TElement>> _groupings;

        internal Lookup(IEqualityComparer<TKey> comparer)
        {
            _groupings = typeof(TKey).CanBeNull()
                ? new NullAwareDictionary<TKey, Grouping<TKey, TElement>>(comparer)
                : new Dictionary<TKey, Grouping<TKey, TElement>>(comparer);
        }

        public int Count => _groupings.Count;

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                if (_groupings.TryGetValue(key, out var grouping))
                {
                    return grouping;
                }

                return ArrayEx.Empty<TElement>();
            }
        }

        public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            // MICROSOFT does not null check resultSelector
            return _groupings.Values.Select(group => resultSelector(group.Key, group));
        }

        public bool Contains(TKey key)
        {
            return _groupings.ContainsKey(key);
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return _groupings.Values.Cast<IGrouping<TKey, TElement>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static Lookup<TKey, TElement> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (elementSelector == null)
            {
                throw new ArgumentNullException(nameof(elementSelector));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var nonNullComparer = comparer ?? EqualityComparer<TKey>.Default;
            var result = new Lookup<TKey, TElement>(nonNullComparer);
            var collections = new NullAwareDictionary<TKey, List<TElement>>(nonNullComparer);
            foreach (var item in source)
            {
                var key = keySelector(item);
                if (!collections.TryGetValue(key, out var collection))
                {
                    collection = new List<TElement>();
                    collections.Add(key, collection);
                }

                if (!result._groupings.ContainsKey(key))
                {
                    result._groupings.Add(key, new Grouping<TKey, TElement>(key, collection));
                }

                collection.Add(elementSelector(item));
            }

            return result;
        }
    }
}

#endif