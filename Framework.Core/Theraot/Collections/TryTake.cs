// Needed for NET40

using System.Diagnostics.CodeAnalysis;

namespace Theraot.Collections
{
    public delegate bool TryTake<T>([MaybeNull] out T item);
}