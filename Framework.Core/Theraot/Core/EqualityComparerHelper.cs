// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;

namespace Theraot.Core
{
    public static class EqualityComparerHelper
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IEqualityComparer<T> ToComparer<T>(this Func<T, T, bool> equalityComparison, Func<T, int> getHashCode)
        {
            return new CustomEqualityComparer<T>(equalityComparison, getHashCode);
        }
    }
}