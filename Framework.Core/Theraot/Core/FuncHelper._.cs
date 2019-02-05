// Needed for NET40

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;

namespace Theraot.Core
{
    public static partial class FuncHelper
    {
        public static Func<TOutput> ChainConversion<TInput, TOutput>(this Func<TInput> source, Func<TInput, TOutput> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return () => converter.Invoke(source.Invoke());
        }

        public static Func<TReturn, TReturn> GetIdentityFunc<TReturn>()
        {
            return IdentityHelper<TReturn>.Instance;
        }

        private static class IdentityHelper<TReturn>
        {
            static IdentityHelper()
            {
                Instance = IdentityFunc;
            }

            public static Func<TReturn, TReturn> Instance { get; }

            private static TReturn IdentityFunc(TReturn target)
            {
                return target;
            }
        }

#if FAT
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

        private static class NotNullHelper<T>
        {
            static NotNullHelper()
            {
                Instance = NotNullPredicate;
            }

            public static Predicate<T> Instance { get; }

            private static bool NotNullPredicate(T target)
            {
                return target != null;
            }
        }

        private static class NullHelper<T>
        {
            static NullHelper()
            {
                Instance = NullPredicate;
            }

            public static Predicate<T> Instance { get; }

            private static bool NullPredicate(T target)
            {
                return target == null;
            }
        }
#endif
    }
}