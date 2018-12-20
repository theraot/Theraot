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
            var compoundComparer = new CustomComparer<(TKey, TNewKey)>(Compare);
            return new OrderedEnumerable<TElement, (TKey, TNewKey)>(_source, CompoundKeySelector, compoundComparer);
            (TKey, TNewKey) CompoundKeySelector(TElement item)
            {
                return (_keySelector(item), keySelector(item));
            }
            int Compare((TKey, TNewKey) x, (TKey, TNewKey) y)
            {
                var check = _comparer.Compare(x.Item1, y.Item1);
                if (check == 0)
                {
                    return comparer.Compare(x.Item2, y.Item2);
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
            var array = Extensions.AsArray(source);
            var keys = new (TKey, int)[array.Length];
            for (int index = 0; index < array.Length; index++)
            {
                keys[index] = (_keySelector.Invoke(array[index]), index);
            }
            Array.Sort(keys, Compare);
            return Enumerable();
            int Compare((TKey, int) x, (TKey, int) y)
            {
                var check = _comparer.Compare(x.Item1, y.Item1);
                if (check == 0)
                {
                    return x.Item2 - y.Item2;
                }
                return check;
            }
            IEnumerable<TElement> Enumerable()
            {
                foreach (var (_, index) in keys)
                {
                    yield return array[index];
                }
            }
        }
    }
}