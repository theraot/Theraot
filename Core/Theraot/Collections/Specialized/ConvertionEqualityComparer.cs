#if FAT
﻿using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ConversionEqualityComparer<TInput, TOutput> : IEqualityComparer<TInput>
    {
        private IEqualityComparer<TOutput> _comparer;
        private Converter<TInput, TOutput> _converter;

        public ConversionEqualityComparer(IEqualityComparer<TOutput> comparer, Converter<TInput, TOutput> converter)
        {
            _comparer = comparer ?? EqualityComparer<TOutput>.Default;
            _converter = Check.NotNullArgument(converter, "converter");
        }

        public bool Equals(TInput x, TInput y)
        {
            return _comparer.Equals(_converter(x), _converter(y));
        }

        public int GetHashCode(TInput obj)
        {
            return _comparer.GetHashCode(_converter(obj));
        }
    }
}
#endif