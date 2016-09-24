#if NET20 || NET30 || NET35 ||NET40

namespace System
{
#if NETCF
    public interface IProgress<T>
#else
    public interface IProgress<in T>
#endif
    {
        void Report(T value);
    }
}

#endif