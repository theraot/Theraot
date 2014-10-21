#if FAT

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, IEnumerable<T> append)
        {
            return new ExtendedEnumerable<T>(target, append);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, T append)
        {
            return new ExtendedEnumerable<T>(target, AsUnaryEnumerable(append));
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, IEnumerable<T> append, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(target, append, match);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, T append, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(target, AsUnaryEnumerable(append), match);
        }

        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            else
            {
                while (true)
                {
                    foreach (var item in source)
                    {
                        yield return item;
                    }
                }
                // Infinite Loop - This method creates an endless IEnumerable<T>
            }
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> target, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(target, null, match);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, IEnumerable<T> prepend)
        {
            return new ExtendedEnumerable<T>(prepend, target);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, T prepend)
        {
            return new ExtendedEnumerable<T>(AsUnaryEnumerable(prepend), target);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, IEnumerable<T> prepend, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(prepend, target, match);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, T prepend, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(AsUnaryEnumerable(prepend), target, match);
        }
    }
}

#endif