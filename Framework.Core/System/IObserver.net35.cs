#if NET20 || NET30 || NET35

namespace System
{
    /// <summary>Provides a mechanism for receiving push-based notifications.</summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    public interface IObserver<in T>
    {
        /// <summary>Notifies the observer that the provider has finished sending push-based notifications.</summary>
        void OnCompleted();

        /// <summary>Notifies the observer that the provider has experienced an error condition.</summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        void OnError(Exception error);

        /// <summary>Provides the observer with new data.</summary>
        /// <param name="value">The current notification information.</param>
        void OnNext(T value);
    }
}

#endif