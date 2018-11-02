#if FAT

using System;
using System.Collections.Generic;

using Theraot.Collections;

namespace Theraot.Core
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> Create<T>(T initialState, Func<T, bool> condition, Func<T, T> iterate)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            var currentState = initialState;
            do
            {
                currentState = iterate.Invoke(currentState);
                yield return currentState;
            } while (condition.Invoke(currentState));
        }

        public static IEnumerable<TResult> Create<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var currentState = initialState;
            do
            {
                currentState = iterate.Invoke(currentState);
                yield return resultSelector.Invoke(currentState);
            } while (condition.Invoke(currentState));
        }

        public static IEnumerable<T> Create<T>(Func<bool> condition, Func<T> iterate)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            while (condition.Invoke())
            {
                yield return iterate.Invoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Func<TState, TResult> resultSelector)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            while (condition.Invoke())
            {
                yield return resultSelector(iterate.Invoke());
            }
        }

        public static IEnumerable<T> Create<T>(TryTake<T> tryTake)
        {
            if (tryTake == null)
            {
                throw new ArgumentNullException("tryTake");
            }
            T item;
            while (tryTake.Invoke(out item))
            {
                yield return item;
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(TryTake<TState> tryTake, Func<TState, TResult> converter)
        {
            if (tryTake == null)
            {
                throw new ArgumentNullException("tryTake");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            TState item;
            while (tryTake.Invoke(out item))
            {
                yield return converter.Invoke(item);
            }
        }

        public static IEnumerable<T> CreateInfinite<T>(T initialState, Func<T, T> iterate)
        {
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            var currentState = initialState;
            while (true)
            {
                currentState = iterate.Invoke(currentState);
                yield return currentState;
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<TResult> CreateInfinite<TState, TResult>(TState initialState, Func<TState, TState> iterate, Func<TState, TResult> resultSelector)
        {
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var currentState = initialState;
            while (true)
            {
                currentState = iterate.Invoke(currentState);
                yield return resultSelector.Invoke(currentState);
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<T> CreateInfinite<T>(Func<T> iterate)
        {
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            while (true)
            {
                yield return iterate.Invoke();
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }

        public static IEnumerable<TResult> CreateInfinite<TState, TResult>(Func<TState> iterate, Func<TState, TResult> resultSelector)
        {
            if (iterate == null)
            {
                throw new ArgumentNullException("iterate");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            while (true)
            {
                yield return resultSelector(iterate.Invoke());
            }
            // Infinite Loop - This method creates an endless IEnumerable<T>
        }
    }
}

#endif