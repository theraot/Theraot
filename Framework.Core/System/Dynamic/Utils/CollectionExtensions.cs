#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections.Specialized;

namespace System.Dynamic.Utils
{
    internal static class CollectionExtensions
    {
        // We could probably improve the hashing here
        public static int ListHashCode<T>(this ReadOnlyCollection<T> list)
        {
            var cmp = EqualityComparer<T>.Default;
            var h = 6551;
            foreach (var t in list)
            {
                h ^= (h << 5) ^ cmp.GetHashCode(t);
            }
            return h;
        }
    }
}

#endif