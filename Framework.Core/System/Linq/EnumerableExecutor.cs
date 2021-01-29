#if LESSTHAN_NET40

#pragma warning disable S2326 // 'T' is not used in the class

namespace System.Linq
{
    public abstract class EnumerableExecutor
    {
        // Empty
    }

    // ReSharper disable once UnusedTypeParameter
    public abstract class EnumerableExecutor<T> : EnumerableExecutor
    {
        // Empty
    }
}

#endif