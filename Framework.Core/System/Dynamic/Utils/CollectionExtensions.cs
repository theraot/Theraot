#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Dynamic.Utils
{
    internal static class CollectionExtensions
    {

#if NET20 || NET30
        public static TrueReadOnlyCollection<T> AddFirst<T>(this ReadOnlyCollection<T> list, T item)
        {
            T[] res = new T[list.Count + 1];
            res[0] = item;
            list.CopyTo(res, 1);
            return new TrueReadOnlyCollection<T>(res);
        }
#endif

        public static bool ListEquals<T>(this ReadOnlyCollection<T> first, ReadOnlyCollection<T> second)
        {
            if (first == second)
            {
                return true;
            }

            int count = first.Count;

            if (count != second.Count)
            {
                return false;
            }

            EqualityComparer<T> cmp = EqualityComparer<T>.Default;
            for (int i = 0; i != count; ++i)
            {
                if (!cmp.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // We could probably improve the hashing here
        public static int ListHashCode<T>(this ReadOnlyCollection<T> list)
        {
            EqualityComparer<T> cmp = EqualityComparer<T>.Default;
            int h = 6551;
            foreach (T t in list)
            {
                h ^= (h << 5) ^ cmp.GetHashCode(t);
            }
            return h;
        }

        /// <summary>
        /// Wraps the provided enumerable into a ReadOnlyCollection{T}
        ///
        /// Copies all of the data into a new array, so the data can't be
        /// changed after creation. The exception is if the enumerable is
        /// already a ReadOnlyCollection{T}, in which case we just return it.
        /// </summary>
        public static ReadOnlyCollection<T> ToTrueReadOnly<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return EmptyReadOnlyCollection<T>.Instance;
            }
            if (enumerable is TrueReadOnlyCollection<T> trueReadOnlyCollection)
            {
                return trueReadOnlyCollection;
            }
            var array = Theraot.Collections.Extensions.AsArray(enumerable);
            return array.Length == 0 ? EmptyReadOnlyCollection<T>.Instance : new TrueReadOnlyCollection<T>(array);
        }
    }
}

#endif