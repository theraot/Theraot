﻿#if (LESSTHAN_NET472 && GREATERTHAN_NET35) || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

#pragma warning disable CA2201 // Do not raise reserved exception types

namespace System.Collections.Generic
{
    public static class SortedSetTheraotExtensions
    {
        public static bool TryGetValue<T>(this SortedSet<T> sortedSet, T equalValue, out T actualValue)
        {
            if (sortedSet == null)
            {
                throw new NullReferenceException(nameof(sortedSet));
            }

            var view = sortedSet.GetViewBetween(equalValue, equalValue);
            actualValue = default!;
            foreach (var result in view)
            {
                actualValue = result;
                return true;
            }

            return false;
        }
    }
}

#endif