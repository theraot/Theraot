using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public static class RangeExtensions
    {
        public static bool Contains<T>(this IEnumerable<Range<T>> ranges, T item)
            where T : IComparable<T>
        {
            T _item = Check.NotNullArgument(item, "item");
            foreach (Range<T> range in Check.NotNullArgument(ranges, "ranges"))
            {
                if (range.Contains(_item))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Range<T>> Overlapped<T>(this IEnumerable<Range<T>> ranges, Range<T> range)
            where T : IComparable<T>
        {
            Range<T> _range = Check.NotNullArgument(range, "item");
            foreach (Range<T> item in Check.NotNullArgument(ranges, "ranges"))
            {
                if (item.Overlaps(_range))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<Range<T>> Sort<T>(this IEnumerable<Range<T>> ranges)
            where T : IComparable<T>
        {
            var list = new List<Range<T>>(Check.NotNullArgument(ranges, "ranges"));
            list.Sort();
            return list;
        }

        public static IEnumerable<Range<T>> Sort<T>(this IEnumerable<Range<T>> ranges, Comparison<Range<T>> comparison)
            where T : IComparable<T>
        {
            var list = new List<Range<T>>(Check.NotNullArgument(ranges, "ranges"));
            list.Sort(Check.NotNullArgument(comparison, "comparison"));
            return list;
        }
    }
}