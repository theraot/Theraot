using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ConversionSet<TInput, TOutput> : ProgressiveSet<TOutput>
    {
        public ConversionSet(IEnumerable<TInput> wrapped, Converter<TInput, TOutput> converter)
            : base(BuildEnumerable(wrapped, converter))
        {
            //Empty
        }

        public ConversionSet(IEnumerable<TInput> wrapped, Converter<TInput, TOutput> converter, Predicate<TInput> filter)
            : base(BuildEnumerable(wrapped, converter, filter))
        {
            //Empty
        }

        private static IEnumerable<TOutput> BuildEnumerable(IEnumerable<TInput> wrapped, Converter<TInput, TOutput> converter)
        {
            var _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            var _converter = Check.NotNullArgument(converter, "converter");
            foreach (var item in _wrapped)
            {
                yield return _converter(item);
            }
        }

        private static IEnumerable<TOutput> BuildEnumerable(IEnumerable<TInput> wrapped, Converter<TInput, TOutput> converter, Predicate<TInput> filter)
        {
            var _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            foreach (var item in _wrapped)
            {
                if (_filter(item))
                {
                    yield return _converter(item);
                }
            }
        }
    }
}