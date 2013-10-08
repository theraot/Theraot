#if NET20 || NET30 || NET35

namespace System
{
    public interface IObservable<out T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}

#endif
