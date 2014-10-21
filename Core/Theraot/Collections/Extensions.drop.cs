#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static TItem TakeAndReturn<TItem>(this IDropPoint<TItem> dropPoint)
        {
            TItem item;
            if (Check.NotNullArgument(dropPoint, "dropPoint").TryTake(out item))
            {
                return item;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static bool TryTakeAndIgnore<TItem>(this IDropPoint<TItem> dropPoint)
        {
            TItem item;
            return Check.NotNullArgument(dropPoint, "dropPoint").TryTake(out item);
        }

        public static bool TryTakeUntil<TItem>(this IDropPoint<TItem> dropPoint, Predicate<TItem> check, out TItem item)
        {
            var _check = Check.NotNullArgument(check, "check");
            var _dropPoint = Check.NotNullArgument(dropPoint, "dropPoint");
        back:
            if (_dropPoint.TryTake(out item))
            {
                if (_check(item))
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

        public static bool TryTakeUntil<TItem>(this IDropPoint<TItem> dropPoint, Predicate<TItem> check, ICollection<TItem> trail)
        {
            var _check = Check.NotNullArgument(check, "check");
            var _dropPoint = Check.NotNullArgument(dropPoint, "dropPoint");
            var _trail = Check.NotNullArgument(trail, "trail");
            TItem item;
        back:
            if (_dropPoint.TryTake(out item))
            {
                if (_check(item))
                {
                    return true;
                }
                else
                {
                    _trail.Add(item);
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