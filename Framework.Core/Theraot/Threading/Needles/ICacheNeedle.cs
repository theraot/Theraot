// Needed for NET40

namespace Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>, IPromise<T>
    {
        bool TryGetValue(out T value);
    }
}