// Needed for NET40

using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    public interface IHasComparer<TKey>
    {
        IEqualityComparer<TKey> Comparer { get; }
    }
}