//Needed for NET20 (Linq)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections;
using Theraot.Collections.Specialized;

namespace Theraot.Core
{
    public class OrderedEnumerable<TElement, TKey> : IOrderedEnumerable<TElement>
    {
        private readonly IComparer<TKey> _comparer;
        private readonly Func<TElement, TKey> _keySelector;
        private readonly IEnumerable<TElement> _source;

        public OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            if (descending)
            {
                _comparer = _comparer.Reverse();
            }
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TNewKey>(Func<TElement, TNewKey> keySelector, IComparer<TNewKey> comparer, bool descending)
        {
            comparer = comparer ?? Comparer<TNewKey>.Default;
            if (descending)
            {
                comparer = comparer.Reverse();
            }
            var compoundComparer = new CustomComparer<KeyValuePair<TKey, TNewKey>>(Compare);
            return new OrderedEnumerable<TElement, KeyValuePair<TKey, TNewKey>>(_source, CompoundKeySelector, compoundComparer);
            KeyValuePair<TKey, TNewKey> CompoundKeySelector(TElement item)
            {
                return new KeyValuePair<TKey, TNewKey>(_keySelector(item), keySelector(item));
            }
            int Compare(KeyValuePair<TKey, TNewKey> x, KeyValuePair<TKey, TNewKey> y)
            {
                var check = _comparer.Compare(x.Key, y.Key);
                if (check == 0)
                {
                    return comparer.Compare(x.Value, y.Value);
                }
                return check;
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Sort(_source).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<TElement> Sort(IEnumerable<TElement> source)
        {
            var array = Extensions.AsArrayInternal(source);
            var keys = new KeyValuePair<TKey, int>[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                keys[index] = new KeyValuePair<TKey, int>(_keySelector.Invoke(array[index]), index);
            }
            Array.Sort(keys, Compare);
            return Enumerable();
            int Compare(KeyValuePair<TKey, int> x, KeyValuePair<TKey, int> y)
            {
                var check = _comparer.Compare(x.Key, y.Key);
                if (check == 0)
                {
                    return x.Value - y.Value;
                }
                return check;
            }
            IEnumerable<TElement> Enumerable()
            {
                foreach (var pair in keys)
                {
                    yield return array[pair.Value];
                }
            }
        }
    }
}