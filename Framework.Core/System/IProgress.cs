#if LESSTHAN_NET45

namespace System
{
    public interface IProgress<in T>
    {
        void Report(T value);
    }
}

#endif