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

        public static IDisposable SubscribeAction<TInput, TOutput>(this IObservable<TInput> observable, Action<TOutput> listener, Func<TInput, TOutput> converter)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            return observable.Subscribe(listener.ToObserver(converter));
        }

        public static IDisposable SubscribeConverted<TInput, TOutput>(this IObservable<TInput> observable, IObserver<TOutput> observer, Func<TInput, TOutput> converter)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            return observable.Subscribe(new ConvertedObserver<TInput, TOutput>(observer, converter));
        }

        public static IDisposable SubscribeFiltered<T>(this IObservable<T> observable, IObserver<T> observer, Predicate<T> filter)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            return observable.Subscribe(new FilteredObserver<T>(observer, filter));
        }

        public static IDisposable SubscribeFilteredConverted<TInput, TOutput>(this IObservable<TInput> observable, IObserver<TOutput> observer, Predicate<TInput> filter, Func<TInput, TOutput> converter)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            return observable.Subscribe(new FilteredConvertedObserver<TInput, TOutput>(observer, filter, converter));
        }

        public static IObserver<T> ToObserver<T>(this Action<T> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            return new CustomObserver<T>(listener);
        }

        public static IObserver<TInput> ToObserver<TInput, TOutput>(this Action<TOutput> listener, Func<TInput, TOutput> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            return new CustomObserver<TInput>(input => listener(converter(input)));
        }
    }
}