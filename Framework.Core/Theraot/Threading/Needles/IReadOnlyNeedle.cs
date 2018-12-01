// Needed for NET40

namespace Theraot.Threading.Needles
{
    public interface IReadOnlyNeedle<out T>
    {
        bool IsAlive { get; }

        T Value { get; }
    }
}