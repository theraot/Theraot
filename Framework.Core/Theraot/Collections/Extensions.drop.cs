#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static T TakeAndReturn<T>(this IDropPoint<T> dropPoint)
        {
            if (dropPoint == null)
            {
                throw new ArgumentNullException(nameof(dropPoint));
            }
            if (dropPoint.TryTake(out T item))
            {
                return item;
            }
            throw new InvalidOperationException();
        }

        public static bool TryTakeAndIgnore<T>(this IDropPoint<T> dropPoint)
        {
            if (dropPoint == null)
            {
                throw new ArgumentNullException(nameof(dropPoint));
            }
            return dropPoint.TryTake(out T item);
        }

        public static bool TryTakeUntil<T>(this IDropPoint<T> dropPoint, Predicate<T> check, out T item)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }
            if (dropPoint == null)
            {
                throw new ArgumentNullException(nameof(dropPoint));
            }
            back:
            if (dropPoint.TryTake(out item))
            {
                if (check(item))
                {
                    return true;
                }
                goto back;
            }
            return false;
        }

        public static bool TryTakeUntil<T>(this IDropPoint<T> dropPoint, Predicate<T> check, ICollection<T> trail)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }
            if (dropPoint == null)
            {
                throw new ArgumentNullException(nameof(dropPoint));
            }
            if (trail == null)
            {
                throw new ArgumentNullException(nameof(trail));
            }
            back:
            if (dropPoint.TryTake(out T item))
            {
                if (check(item))
                {
                    return true;
                }
                trail.Add(item);
                goto back;
            }
            return false;
        }
    }
}

#endif