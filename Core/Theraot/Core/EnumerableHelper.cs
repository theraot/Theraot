using System;
using System.Collections.Generic;

using Theraot.Collections;

namespace Theraot.Core
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> Create<T>(Func<bool> step, Func<T> iterate, Action final)
        {
            var _step = Check.NotNullArgument(step, "step");
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
                final.SafeInvoke();
            }
        }

        public static IEnumerable<T> Create<T>(Action initial, Func<bool> step, Func<T> iterate, Action final)
        {
            var _step = Check.NotNullArgument(step, "step");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            try
            {
                initial.SafeInvoke();
                while (_step.Invoke())
                {
                    yield return _iterate.Invoke();
                }
            }
            finally
            {
                final.SafeInvoke();
            }
        }

        public static IEnumerable<T> Create<T>(Func<bool> step, Func<T> iterate)
        {
            var _step = Check.NotNullArgument(step, "step");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            while (_step.Invoke())
            {
                yield return _iterate.Invoke();
            }
        }

        public static IEnumerable<TOutput> Create<TInput, TOutput>(Func<bool> step, Func<TInput> iterate, Converter<TInput, TOutput> converter, Action final)
        {
            var _step = Check.NotNullArgument(step, "step");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(converter, "converter");
            try
            {
                while (_step.Invoke())
                {
                    yield return _converter(_iterate.Invoke());
                }
            }
            finally
            {
                final.SafeInvoke();
            }
        }

        public static IEnumerable<TOutput> Create<TInput, TOutput>(Action initial, Func<bool> step, Func<TInput> iterate, Converter<TInput, TOutput> converter, Action final)
        {
            var _step = Check.NotNullArgument(step, "step");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(converter, "converter");
            try
            {
                initial.SafeInvoke();
                while (_step.Invoke())
                {
                    yield return _converter(_iterate.Invoke());
                }
            }
            finally
            {
                final.SafeInvoke();
            }
        }

        public static IEnumerable<TOutput> Create<TInput, TOutput>(Func<bool> step, Func<TInput> iterate, Converter<TInput, TOutput> converter)
        {
            var _step = Check.NotNullArgument(step, "step");
            var _iterate = Check.NotNullArgument(iterate, "iterate");
            var _converter = Check.NotNullArgument(converter, "converter");
            while (_step.Invoke())
            {
                yield return _converter(_iterate.Invoke());
            }
        }

        public static IEnumerable<T> Create<T>(TryTake<T> tryTake, Action final)
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
                final.SafeInvoke();
            }
        }

        public static IEnumerable<T> Create<T>(Action initial, TryTake<T> tryTake, Action final)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            try
            {
                initial.SafeInvoke();
                T item;
                while (_tryTake.Invoke(out item))
                {
                    yield return item;
                }
            }
            finally
            {
                final.SafeInvoke();
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

        public static IEnumerable<TOutput> Create<TInput, TOutput>(TryTake<TInput> tryTake, Converter<TInput, TOutput> converter, Action final)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            try
            {
                TInput item;
                while (_tryTake.Invoke(out item))
                {
                    yield return _converter.Invoke(item);
                }
            }
            finally
            {
                final.SafeInvoke();
            }
        }

        public static IEnumerable<TOutput> Create<TInput, TOutput>(Action initial, TryTake<TInput> tryTake, Converter<TInput, TOutput> converter, Action final)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            try
            {
                initial.SafeInvoke();
                TInput item;
                while (_tryTake.Invoke(out item))
                {
                    yield return _converter.Invoke(item);
                }
            }
            finally
            {
                final.SafeInvoke();
            }
        }

        public static IEnumerable<TOutput> Create<TInput, TOutput>(TryTake<TInput> tryTake, Converter<TInput, TOutput> converter)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            TInput item;
            while (_tryTake.Invoke(out item))
            {
                yield return _converter.Invoke(item);
            }
        }
    }
}