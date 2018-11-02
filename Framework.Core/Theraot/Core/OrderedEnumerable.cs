#if FAT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Theraot.Collections;

namespace Theraot.Core
{
    public class OrderedEnumerable<TElement, TKey> : IOrderedEnumerable<TElement>
    {
        private readonly IComparer<TKey> _comparer;
        private readonly Func<TElement, TKey> _selector;
        private readonly IEnumerable<TElement> _source;

        public OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            _source = source;
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            _selector = keySelector;
        }

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TNewKey>(Func<TElement, TNewKey> keySelector, IComparer<TNewKey> comparer, bool descending)
        {
            comparer = comparer ?? Comparer<TNewKey>.Default;
            if (descending)
            {
                comparer = comparer.Reverse();
            }
            return new OrderedEnumerable<TElement, TNewKey>(_source, keySelector, comparer);
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
            var list = new SortedList<TKey, TElement>(_comparer);
            foreach (var item in source)
            {
                list.Add(_selector.Invoke(item), item);
            }
            return list.ConvertProgressive(input => input.Value);
        }
    }
}

#endif