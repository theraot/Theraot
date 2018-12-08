using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    internal sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        internal Grouping(TKey key, SafeCollection<TElement> items)
        {
            Items = items;
            Key = key;
        }

        public SafeCollection<TElement> Items { get; }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}