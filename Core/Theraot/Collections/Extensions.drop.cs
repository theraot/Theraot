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
                throw new ArgumentNullException("dropPoint");
            }
            T item;
            if (dropPoint.TryTake(out item))
            {
                return item;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static bool TryTakeAndIgnore<T>(this IDropPoint<T> dropPoint)
        {
            if (dropPoint == null)
            {
                throw new ArgumentNullException("dropPoint");
            }
            T item;
            return dropPoint.TryTake(out item);
        }

        public static bool TryTakeUntil<T>(this IDropPoint<T> dropPoint, Predicate<T> check, out T item)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            if (dropPoint == null)
            {
                throw new ArgumentNullException("dropPoint");
            }
            back:
            if (dropPoint.TryTake(out item))
            {
                if (check(item))
                {
                    return true;
                }
                else
                {
                    goto back;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool TryTakeUntil<T>(this IDropPoint<T> dropPoint, Predicate<T> check, ICollection<T> trail)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            if (dropPoint == null)
            {
                throw new ArgumentNullException("dropPoint");
            }
            if (trail == null)
            {
                throw new ArgumentNullException("trail");
            }
            T item;
            back:
            if (dropPoint.TryTake(out item))
            {
                if (check(item))
                {
                    return true;
                }
                else
                {
                    trail.Add(item);
                    goto back;
                }
            }
            else
            {
                return false;
            }
        }
    }
}

#endif