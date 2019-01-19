using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    internal sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly ICollection<TElement> _items;

        internal Grouping(TKey key, ICollection<TElement> items)
        {
            _items = items;
            Key = key;
        }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public override string ToString()
        {
            return $"<Key = {Key}>";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}