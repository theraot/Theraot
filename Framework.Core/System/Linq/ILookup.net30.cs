#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        int Count { get; }

        IEnumerable<TElement> this[TKey key] { get; }

        bool Contains(TKey key);
    }
}

#endif