using System;
using System.Collections.Generic;

using Theraot.Collections;

namespace Theraot.Core
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> Create<T>(Func<bool> condition, Func<T> iterate, Action after)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            try
            {
                while (_step.Invoke())
                {
                    yield return _iterate.Invoke();
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<T> Create<T>(Action before, Func<bool> condition, Func<T> iterate, Action after)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            try
            {
                before.SafeInvoke();
                while (_step.Invoke())
                {
                    yield return _iterate.Invoke();
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<T> Create<T>(Func<bool> condition, Func<T> iterate)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            while (_step.Invoke())
            {
                yield return _iterate.Invoke();
            }
        }

        public static IEnumerable<T> Create<T>(Func<T> iterate)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            while (true)
            {
                yield return _iterate.Invoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Converter<TState, TResult> resultSelector, Action after)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                while (_step.Invoke())
                {
                    yield return _converter(_iterate.Invoke());
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Func<TState, TResult> resultSelector, Action after)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                while (_step.Invoke())
                {
                    yield return _converter(_iterate.Invoke());
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Action before, Func<bool> condition, Func<TState> iterate, Converter<TState, TResult> resultSelector, Action after)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                before.SafeInvoke();
                while (_step.Invoke())
                {
                    yield return _converter(_iterate.Invoke());
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Action before, Func<bool> condition, Func<TState> iterate, Func<TState, TResult> resultSelector, Action after)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                before.SafeInvoke();
                while (_step.Invoke())
                {
                    yield return _converter(_iterate.Invoke());
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Converter<TState, TResult> resultSelector)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            while (_step.Invoke())
            {
                yield return _converter(_iterate.Invoke());
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<bool> condition, Func<TState> iterate, Func<TState, TResult> resultSelector)
        {
            var _step = Check.NotNullArgument(condition, "condition");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            while (_step.Invoke())
            {
                yield return _converter(_iterate.Invoke());
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<TState> iterate, Converter<TState, TResult> resultSelector)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            while (true)
            {
                yield return _converter(_iterate.Invoke());
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Func<TState> iterate, Func<TState, TResult> resultSelector)
        {
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            while (true)
            {
                yield return _converter(_iterate.Invoke());
            }
        }

        public static IEnumerable<T> Create<T>(TryTake<T> tryTake, Action after)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            try
            {
                T item;
                while (_tryTake.Invoke(out item))
                {
                    yield return item;
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<T> Create<T>(Action before, TryTake<T> tryTake, Action after)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            try
            {
                before.SafeInvoke();
                T item;
                while (_tryTake.Invoke(out item))
                {
                    yield return item;
                }
            }
            finally
            {
                after.SafeInvoke();
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

        public static IEnumerable<TResult> Create<TState, TResult>(TryTake<TState> tryTake, Converter<TState, TResult> resultSelector, Action after)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                TState item;
                while (_tryTake.Invoke(out item))
                {
                    yield return _converter.Invoke(item);
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(TryTake<TState> tryTake, Func<TState, TResult> resultSelector, Action after)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                TState item;
                while (_tryTake.Invoke(out item))
                {
                    yield return _converter.Invoke(item);
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Action before, TryTake<TState> tryTake, Converter<TState, TResult> resultSelector, Action after)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                before.SafeInvoke();
                TState item;
                while (_tryTake.Invoke(out item))
                {
                    yield return _converter.Invoke(item);
                }
            }
            finally
            {
                after.SafeInvoke();
            }
        }

        public static IEnumerable<TResult> Create<TState, TResult>(Action before, TryTake<TState> tryTake, Func<TState, TResult> resultSelector, Action after)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(resultSelector, "resultSelector");
            try
            {
                before.SafeInvoke();
                TState item;
                while (_tryTake.Invoke(out item))
                {
                    yield return _converter.Invoke(item);
                }
            }
            finally
            {
                after.SafeInvoke();
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
    }
}