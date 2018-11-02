#if NET20 || NET30 || NET35

namespace System
{
    /// <summary>Defines a provider for push-based notification.</summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
#if NETCF

    public interface IObservable<T>
#else

    public interface IObservable<out T>
#endif
    {
        /// <summary>Notifies the provider that an observer is to receive notifications.</summary>
        /// <returns>A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.</returns>
        /// <param name="observer">The object that is to receive notifications.</param>
        IDisposable Subscribe(IObserver<T> observer);
    }
}

#endif