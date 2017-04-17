#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class ConversionComparer<TInput, TOutput> : IComparer<TInput>
    {
        private readonly IComparer<TOutput> _comparer;
        private readonly Func<TInput, TOutput> _converter;

        public ConversionComparer(IComparer<TOutput> comparer, Func<TInput, TOutput> converter)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            _comparer = comparer;
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            _converter = converter;
        }

        public int Compare(TInput x, TInput y)
        {
            return _comparer.Compare(_converter(x), _converter(y));
        }
    }
}

#endif