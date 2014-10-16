#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public interface IExtendedReadOnlyList<T> : IReadOnlyList<T>
    {
        int IndexOf(T item);
    }
}

#endif