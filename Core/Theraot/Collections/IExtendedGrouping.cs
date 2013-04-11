using System.Linq;

namespace Theraot.Collections
{
    public interface IExtendedGrouping<TKey, TElement> : IGrouping<TKey, TElement>, IExtendedReadOnlyCollection<TElement>
    {
        //Empty
    }
}