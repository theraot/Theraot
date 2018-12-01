#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class ConversionComparer<TInput, TOutput> : IComparer<TInput>
    {
        private readonly IComparer<TOutput> _comparer;
        private readonly Func<TInput, TOutput> _converter;

        public ConversionComparer(IComparer<TOutput> comparer, Func<TInput, TOutput> converter)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public int Compare(TInput x, TInput y)
        {
            return _comparer.Compare(_converter(x), _converter(y));
        }
    }
}

#endif