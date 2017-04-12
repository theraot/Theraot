// Needed for NET40

using System;

namespace Theraot.Core
{
    public static partial class FuncHelper
    {
        public static Func<TOutput> ChainConversion<TInput, TOutput>(this Func<TInput> source, Converter<TInput, TOutput> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            var _converter = converter;
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var _source = source;
            return () => _converter.Invoke(_source.Invoke());
        }

        public static Func<TReturn, TReturn> GetIdentityFunc<TReturn>()
        {
            return IdentityHelper<TReturn>.Instance;
        }

        public static Predicate<T> GetNotNullPredicate<T>()
            where T : class
        {
            return NotNullHelper<T>.Instance;
        }

        public static Predicate<T> GetNullPredicate<T>()
            where T : class
        {
            return NullHelper<T>.Instance;
        }

        private static class IdentityHelper<TReturn>
        {
            private static readonly Func<TReturn, TReturn> _instance;

            static IdentityHelper()
            {
                _instance = IdentityFunc;
            }

            public static Func<TReturn, TReturn> Instance
            {
                get { return _instance; }
            }

            private static TReturn IdentityFunc(TReturn target)
            {
                return target;
            }
        }

        private static class NotNullHelper<T>
        {
            private static readonly Predicate<T> _instance;

            static NotNullHelper()
            {
                _instance = NotNullPredicate;
            }

            public static Predicate<T> Instance
            {
                get { return _instance; }
            }

            private static bool NotNullPredicate(T target)
            {
                return !ReferenceEquals(target, null);
            }
        }

        private static class NullHelper<T>
        {
            private static readonly Predicate<T> _instance;

            static NullHelper()
            {
                _instance = NullPredicate;
            }

            public static Predicate<T> Instance
            {
                get { return _instance; }
            }

            private static bool NullPredicate(T target)
            {
                return ReferenceEquals(target, null);
            }
        }
    }
}