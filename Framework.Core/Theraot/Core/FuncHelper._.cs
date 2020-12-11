// Needed for NET40

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
    }
}