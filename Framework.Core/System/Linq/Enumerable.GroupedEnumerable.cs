#if LESSTHAN_NET35

// ReSharper disable LoopCanBeConvertedToQuery

using System.Collections;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace System.Linq
{
    public static partial class Enumerable
    {
        internal class GroupedEnumerable<TSource, TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
        {
            private readonly IEqualityComparer<TKey> _comparer;
            private readonly Func<TSource, TElement> _elementSelector;
            private readonly Func<TSource, TKey> _keySelector;
            private readonly IEnumerable<TSource> _source;

            public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
                _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
                _elementSelector = elementSelector ?? throw new ArgumentNullException(nameof(elementSelector));
                _comparer = comparer ?? EqualityComparer<TKey>.Default;
            }

            public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
            {
                var groupings = new NullAwareDictionary<TKey, Tuple<Grouping<TKey, TElement>, List<TElement>>>(_comparer);
                foreach (var item in _source)
                {
                    var key = _keySelector(item);
                    if (!groupings.TryGetValue(key, out var tuple))
                    {
                        var collection = new List<TElement>();
                        tuple = new Tuple<Grouping<TKey, TElement>, List<TElement>>(new Grouping<TKey, TElement>(key, collection), collection);
                        groupings.Add(key, tuple);
                    }
                    tuple.Item2.Add(_elementSelector(item));
                }
                return Enumerator(groupings);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static IEnumerator<IGrouping<TKey, TElement>> Enumerator(IDictionary<TKey, Tuple<Grouping<TKey, TElement>, List<TElement>>> groupings)
            {
                foreach (var grouping in groupings.Values)
                {
                    yield return grouping.Item1;
                }
            }
        }
    }
}

#endif