// Needed for NET40

using System.Diagnostics.CodeAnalysis;

namespace Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>, IPromise<T>
    {
        bool TryGetValue([MaybeNullWhen(false)] out T value);
    }
}