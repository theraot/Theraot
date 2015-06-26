// Needed for NET40

using System;

namespace Theraot.Core
{
    public static partial class FuncHelper
    {
        public static Func<TOutput> ChainConversion<TInput, TOutput>(this Func<TInput> source, Converter<TInput, TOutput> converter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _source = Check.NotNullArgument(source, "source");
            return () => _converter.Invoke(_source.Invoke());
        }

        public static Func<TReturn, TReturn> GetIdentityFunc<TReturn>()
        {
            return IdentityHelper<TReturn>.Instance;
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
                get
                {
                    return _instance;
                }
            }

            private static TReturn IdentityFunc(TReturn target)
            {
                return target;
            }
        }
    }
}