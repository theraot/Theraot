// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Dynamic.Utils
{
    // Miscellaneous helpers that don't belong anywhere else
    internal static class Helpers
    {
        internal static T CommonNode<T>(T first, T second, Func<T, T> parent) where T : class
        {
#if FAT
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
#endif
            var cmp = EqualityComparer<T>.Default;
            if (cmp.Equals(first, second))
            {
                return first;
            }
            var set = new HashSet<T>(cmp);
            for (var t = first; t != null; t = parent(t))
            {
                set.Add(t);
            }
            for (var t = second; t != null; t = parent(t))
            {
                if (set.Contains(t))
                {
                    return t;
                }
            }
            return null;
        }

        internal static void IncrementCount<T>(T key, Dictionary<T, int> dict)
        {
            int count;
            dict.TryGetValue(key, out count);
            dict[key] = count + 1;
        }
    }
}