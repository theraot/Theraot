#if NET20 || NET30

namespace System
{
    public delegate TResult Func<in T, out TResult>(T arg);
}

#endif
