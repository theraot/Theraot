#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static class RangeExtensions
    {
        public static RangeSituation CompareTo<T>(this Range<T> x, Range<T> y, IComparer<T> comparer)
            where T : IComparable<T>
        {
            var ii = comparer.Compare(x.Minimun, y.Minimun);
            if (ii == 0)
            {
                if (x.ClosedMinimun && !y.ClosedMinimun)
                {
                    ii = -1;
                }
                else if (!x.ClosedMinimun && y.ClosedMinimun)
                {
                    ii = 1;
                }
            }
            var aa = comparer.Compare(x.Maximun, y.Maximun);
            if (aa == 0)
            {
                if (x.ClosedMaximun && !y.ClosedMaximun)
                {
                    aa = 1;
                }
                else if (!x.ClosedMaximun && y.ClosedMaximun)
                {
                    aa = -1;
                }
            }
            if (ii == 0 && aa == 0)
            {
                return RangeSituation.Equals;
            }
            if (ii <= 0 && aa >= 0)
            {
                return RangeSituation.Contains;
            }
            if (ii >= 0 && aa <= 0)
            {
                return RangeSituation.Contained;
            }
            if (ii < 0)
            {
                var ai = comparer.Compare(x.Maximun, y.Minimun);
                if (ai == 0)
                {
                    if (x.ClosedMaximun && y.ClosedMinimun)
                    {
                        ai = 1;
                    }
                    else if (!x.ClosedMaximun && !y.ClosedMinimun)
                    {
                        ai = -1;
                    }
                }
                if (ai < 0)
                {
                    return RangeSituation.BeforeSeparated;
                }
                if (ai > 0)
                {
                    return RangeSituation.BeforeOverlapped;
                }
                return RangeSituation.BeforeTouching;
            }
            var ia = comparer.Compare(x.Minimun, y.Maximun);
            if (ia == 0)
            {
                if (x.ClosedMinimun && y.ClosedMaximun)
                {
                    ia = -1;
                }
                else if (!x.ClosedMinimun && !y.ClosedMaximun)
                {
                    ia = 1;
                }
            }
            if (ia < 0)
            {
                return RangeSituation.AfterOverlapped;
            }
            if (ia > 0)
            {
                return RangeSituation.AfterSeparated;
            }
            return RangeSituation.AfterTouching;
        }

        public static int CompareTo<T>(this Range<T> x, T y, IComparer<T> comparer)
            where T : IComparable<T>
        {
            var i = comparer.Compare(x.Minimun, y);
            if (i == 0)
            {
                i = x.ClosedMinimun ? -1 : 1;
            }
            if (i > 0)
            {
                return -1;
            }
            var a = comparer.Compare(x.Maximun, y);
            if (a == 0)
            {
                a = x.ClosedMaximun ? 1 : -1;
            }
            if (a < 0)
            {
                return 1;
            }
            return 0;
        }

        public static bool IsEmpty<T>(this Range<T> x, IComparer<T> comparer)
            where T : IComparable<T>
        {
            return comparer.Compare(x.Minimun, x.Maximun) == 0 && (!x.ClosedMinimun || !x.ClosedMaximun);
        }
    }
}

#endif