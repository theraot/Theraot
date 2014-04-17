using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static class ProgressorExtensions
    {
        public static IDisposable SubscribeAction<T>(this IObservable<T> observable, Action<T> listener)
        {
            return Check.NotNullArgument(observable, "observable").Subscribe(listener.ToObserver());
        }

        public static IDisposable SubscribeAction<TInput, TOutput>(this IObservable<TInput> observable, Action<TOutput> listener, Converter<TInput, TOutput> converter)
        {
            return Check.NotNullArgument(observable, "observable").Subscribe(listener.ToObserver(converter));
        }

        public static IEnumerable<T> All<T>(this IProgressor<T> progressor)
        {
            var _progressor = Check.NotNullArgument(progressor, "progressor");
            T item;
            while (_progressor.TryTake(out item))
            {
                yield return item;
            }
        }

        public static IEnumerable<T> While<T>(this IProgressor<T> progressor, Predicate<T> predicate)
        {
            var _progressor = Check.NotNullArgument(progressor, "progressor");
            var _condition = Check.NotNullArgument(predicate, "condition");
            T item;
            while (_progressor.TryTake(out item))
            {
                if (_condition(item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        public static IEnumerable<T> While<T>(this IProgressor<T> progressor, Func<bool> condition)
        {
            var _progressor = Check.NotNullArgument(progressor, "progressor");
            var _condition = Check.NotNullArgument(condition, "condition");
            T item;
            while (_progressor.TryTake(out item))
            {
                if (_condition())
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        public static IObserver<T> ToObserver<T>(this Action<T> listener)
        {
            return new ActionObserver<T>(listener);
        }

        public static IObserver<TInput> ToObserver<TInput, TOutput>(this Action<TOutput> listener, Converter<TInput, TOutput> converter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            return new ActionObserver<TInput>(input => listener(_converter.Invoke(input)));
        }

        [Serializable]
        private sealed class ActionObserver<T> : IObserver<T>
        {
            private readonly Action<T> _action;

            public ActionObserver(Action<T> action)
            {
                _action = Check.NotNullArgument(action, "action");
            }

            public void OnCompleted()
            {
                //Empty
            }

            public void OnError(Exception error)
            {
                GC.KeepAlive(error);
            }

            public void OnNext(T value)
            {
                _action.Invoke(value);
            }
        }
    }
}