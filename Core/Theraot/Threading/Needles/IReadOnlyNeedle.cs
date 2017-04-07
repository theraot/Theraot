// Needed for NET40

namespace Theraot.Threading.Needles
{
#if NETCF

    public interface IReadOnlyNeedle<T>
#else

    public interface IReadOnlyNeedle<out T>
#endif
    {
        bool IsAlive { get; }

        T Value { get; }
    }
}