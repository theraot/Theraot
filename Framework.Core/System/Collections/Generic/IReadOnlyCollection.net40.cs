#if LESSTHAN_NET45

namespace System.Collections.Generic
{
#if NET40

    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count
        {
            get;
        }
    }

#else

    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        int Count { get; }
    }

#endif
}

#endif