using System.Linq;

namespace Theraot.Collections
{
#if NET35
    public interface IExtendedGrouping<TKey, TElement> : IGrouping<TKey, TElement>, IExtendedReadOnlyCollection<TElement>
#else
    public interface IExtendedGrouping<out TKey, TElement> : IGrouping<TKey, TElement>, IExtendedReadOnlyCollection<TElement>
#endif
    {
        //Empty
    }
}