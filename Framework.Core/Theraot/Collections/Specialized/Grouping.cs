using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    internal sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly SafeCollection<TElement> _items;

        internal Grouping(TKey key)
        {
            _items = new SafeCollection<TElement>();
            Key = key;
        }

        public void Add(TElement element)
        {
            _items.Add(element);
        }

        public TKey Key { get; }

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