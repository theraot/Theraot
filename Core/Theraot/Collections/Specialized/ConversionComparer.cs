#if FAT
using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ConversionComparer<TInput, TOutput> : IComparer<TInput>
    {
        private IComparer<TOutput> _comparer;
        private Converter<TInput, TOutput> _converter;

        public ConversionComparer(IComparer<TOutput> comparer, Converter<TInput, TOutput> converter)
        {
            _comparer = Check.NotNullArgument(comparer, "comparer");
            _converter = Check.NotNullArgument(converter, "converter");
        }

        public int Compare(TInput x, TInput y)
        {
            return _comparer.Compare(_converter(x), _converter(y));
        }
    }
}
#endif