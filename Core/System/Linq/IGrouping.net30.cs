#if NET20 || NET30

using System.Collections.Generic;

namespace System.Linq
{
#if NETCF

    public interface IGrouping<TKey, TElement> : IEnumerable<TElement>
#else

    public interface IGrouping<out TKey, TElement> : IEnumerable<TElement>
#endif
    {
        TKey Key
        {
            get;
        }
    }
}

#endif