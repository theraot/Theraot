#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public interface IReadOnlyDropPoint<T> : IReadOnlyCollection<T>
    {
        T Item
        {
            get;
        }
    }
}

#endif