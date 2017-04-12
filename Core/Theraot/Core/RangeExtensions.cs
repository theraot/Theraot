#if FAT

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
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item");
            }
            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }
            foreach (Range<T> range in ranges)
            {
                if (range.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Range<T>> Overlapped<T>(this IEnumerable<Range<T>> ranges, Range<T> range)
            where T : IComparable<T>
        {
            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }
            foreach (Range<T> item in ranges)
            {
                if (item.Overlaps(range))
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
            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            var list = new List<Range<T>>(ranges);
            list.Sort(comparison);
            return list;
        }
    }
}

#endif