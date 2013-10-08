#if NET20 || NET30 || NET35

namespace System
{
    public interface IObserver<in T>
    {
        void OnCompleted();

        void OnError(Exception error);

        void OnNext(T value);
    }
}

#endif
