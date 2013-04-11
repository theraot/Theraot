#if FAT
﻿using System;
using System.Collections.Generic;

using Theraot.Collections.Specialized;

namespace Theraot.Core
{
    public static class EqualityComparerHelper
    {
        public static IEqualityComparer<T> ToComparer<T>(this Func<T, T, bool> equalityComparison, Func<T, int> getHashCode)
        {
            return new CustomEqualityComparer<T>(equalityComparison, getHashCode);
        }
    }
}
#endif