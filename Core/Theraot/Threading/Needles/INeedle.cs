// Needed for NET40

namespace Theraot.Threading.Needles
{
    public interface INeedle<T> : IReadOnlyNeedle<T>
    {
        new T Value { get; set; }
    }
}