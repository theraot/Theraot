#if LESSTHAN_NET35
using System.Collections.Generic;

namespace System.Linq
{
    public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
    {
        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
    }
}

#endif