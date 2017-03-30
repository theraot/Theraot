// Needed for NET40

namespace Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>, IPromise
    {
        bool TryGetValue(out T value);
    }
}