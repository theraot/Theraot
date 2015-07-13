// Needed for NET40

namespace Theraot.Collections
{
    public delegate bool TryTake<T>(out T item);

    internal delegate bool TryConvert<in TInput, TOutput>(TInput input, out TOutput output);
}