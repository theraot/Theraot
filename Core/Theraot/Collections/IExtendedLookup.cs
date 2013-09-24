#if FAT

using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections
{
    public interface IExtendedLookup<TKey, TElement> : IEnumerable<IExtendedGrouping<TKey, TElement>>, ILookup<TKey, TElement>, IDictionary<TKey, IExtendedGrouping<TKey, TElement>>
    {
        //Empty
    }
}

#endif