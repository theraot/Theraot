#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Dynamic.Utils
{
    // Miscellaneous helpers that don't belong anywhere else
    internal static class Helpers
    {
        internal static void IncrementCount<T>(T key, Dictionary<T, int> dict)
        {
            dict.TryGetValue(key, out var count);
            dict[key] = count + 1;
        }
    }
}

#endif