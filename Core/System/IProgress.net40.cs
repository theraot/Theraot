#if NET35 ||NET40

namespace System
{
    public interface IProgress<in T>
    {
        void Report(T value);
    }
}

#endif