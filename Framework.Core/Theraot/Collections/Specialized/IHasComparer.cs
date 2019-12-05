// Needed for NET40

using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
#if LESSTHAN_NET40

    public interface IHasComparer<TKey>
#else
    public interface IHasComparer<in TKey>
#endif
    {
        IEqualityComparer<TKey> Comparer { get; }
    }
}