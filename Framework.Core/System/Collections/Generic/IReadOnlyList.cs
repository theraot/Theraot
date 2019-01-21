#if LESSTHAN_NET45

namespace System.Collections.Generic
{
#if NET40
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        T this[int index]
        {
            get;
        }
    }

#else

    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        T this[int index] { get; }
    }

#endif
}

#endif