// Needed for NET40

using System;

namespace Theraot.Collections
{
    public static class ObservableExtensions
    {
        public static IDisposable SubscribeAction<T>(this IObservable<T> observable, Action<T> listener)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            return observable.Subscribe(listener.ToObserver());
        }

        public static IObserver<T> ToObserver<T>(this Action<T> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            return new CustomObserver<T>(listener);
        }
    }
}