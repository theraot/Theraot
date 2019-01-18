#if LESSTHAN_NET40

namespace System.Linq
{
    public abstract class EnumerableExecutor
    {
        //Empty
    }

    // ReSharper disable once UnusedTypeParameter
    public abstract class EnumerableExecutor<T> : EnumerableExecutor
    {
        //Empty
    }
}

#endif