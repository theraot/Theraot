using System;
using System.Collections.Generic;

using Theraot.Collections;

namespace Theraot.Core
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> Create<T>(T initialState, Func<T, bool> condition, Func<T, T> iterate)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var currentState = initialState;
            do
            {
                currentState = _iterate.Invoke(currentState);
                yield return currentState;
            } while (_condition.Invoke(currentState));
        }

        public static IEnumerable<TResult> Create<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Converter<TState, TResult> resultSelector)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            var currentState = initialState;
            do
            {
                currentState = _iterate.Invoke(currentState);
                yield return _resultSelector.Invoke(currentState);
            } while (_condition.Invoke(currentState));
        }

        public static IEnumerable<TResult> Create<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            var currentState = initialState;
            do
            {
                currentState = _iterate.Invoke(currentState);
                yield return _resultSelector.Invoke(currentState);
            } while (_condition.Invoke(currentState));
        }

        public static IEnumerable<T> Create<T>(Func<bool> condition, Func<T> iterate)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            while (_condition.Invoke())
            {
                yield return _iterate.Invoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Converter<TState, TResult> resultSelector)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            while (_condition.Invoke())
            {
                yield return _resultSelector(_iterate.Invoke());
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Func<TState, TResult> resultSelector)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            while (_condition.Invoke())
            {
                yield return _resultSelector(_iterate.Invoke());
            }
        }

        public static IEnumerable<T> Create<T>(TryTake<T> tryTake)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            T item;
            while (_tryTake.Invoke(out item))
            {
                yield return item;
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(TryTake<TState> tryTake, Converter<TState, TResult> converter)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "resultSelector");
            TState item;
            while (_tryTake.Invoke(out item))
            {
                yield return _converter.Invoke(item);
            }
        }

        public static IEnumerable<T> CreateInfinite<T>(T initialState, Func<T, T> iterate)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var currentState = initialState;
            while (true)
            {
                currentState = _iterate.Invoke(currentState);
                yield return currentState;
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<TResult> CreateInfinite<TState, TResult>(TState initialState, Func<TState, TState> iterate, Converter<TState, TResult> resultSelector)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            var currentState = initialState;
            while (true)
            {
                currentState = _iterate.Invoke(currentState);
                yield return _resultSelector.Invoke(currentState);
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<TResult> CreateInfinite<TState, TResult>(TState initialState, Func<TState, TState> iterate, Func<TState, TResult> resultSelector)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            var currentState = initialState;
            while (true)
            {
                currentState = _iterate.Invoke(currentState);
                yield return _resultSelector.Invoke(currentState);
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<T> CreateInfinite<T>(Func<T> iterate)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            while (true)
            {
                yield return _iterate.Invoke();
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<TResult> CreateInfinite<TState, TResult>(Func<TState> iterate, Converter<TState, TResult> resultSelector)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            while (true)
            {
                yield return _resultSelector(_iterate.Invoke());
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<TResult> CreateInfinite<TState, TResult>(Func<TState> iterate, Func<TState, TResult> resultSelector)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _resultSelector = Check.NotNullArgument(resultSelector, "resultSelector");
            while (true)
            {
                yield return _resultSelector(_iterate.Invoke());
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }
    }
}