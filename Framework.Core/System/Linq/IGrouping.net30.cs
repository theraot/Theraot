#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public interface IGrouping<out TKey, TElement> : IEnumerable<TElement>
    {
        TKey Key { get; }
    }
}

#endif