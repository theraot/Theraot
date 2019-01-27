#if FAT
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public struct Range<T>
        where T : IComparable<T>
    {
        public Range(T minimum, T maximum)
        {
            ClosedMaximum = true;
            ClosedMinimum = true;
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(minimum, maximum) > 0)
            {
                Minimum = maximum;
                Maximum = minimum;
            }
            else
            {
                Minimum = minimum;
                Maximum = maximum;
            }
        }

        public Range(T minimum, bool closedMinimum, T maximum, bool closedMaximum)
        {
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(minimum, maximum) > 0)
            {
                Minimum = maximum;
                Maximum = minimum;
                ClosedMinimum = closedMaximum;
                ClosedMaximum = closedMinimum;
            }
            else
            {
                Minimum = minimum;
                Maximum = maximum;
                ClosedMinimum = closedMinimum;
                ClosedMaximum = closedMaximum;
            }
        }

        public bool ClosedMaximum { get; }

        public bool ClosedMinimum { get; }

        public T Maximum { get; }

        public T Minimum { get; }

        public override int GetHashCode()
        {
            return Minimum.GetHashCode() * 7
                   + Maximum.GetHashCode();
        }

        public IEnumerable<T> Iterate(Func<T, T> increment)
        {
            return IterateIterator(increment, Comparer<T>.Default, Minimum, ClosedMinimum, Maximum, ClosedMaximum);
        }

        public IEnumerable<T> Iterate(Func<T, T> increment, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return IterateIterator(increment, comparer, Minimum, ClosedMinimum, Maximum, ClosedMaximum);
        }

        public IEnumerable<T> ReverseIterate(Func<T, T> decrement)
        {
            return ReverseIterateIterator(decrement, Comparer<T>.Default, Minimum, ClosedMinimum, Maximum, ClosedMaximum);
        }

        public IEnumerable<T> ReverseIterate(Func<T, T> decrement, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return ReverseIterateIterator(decrement, comparer, Minimum, ClosedMinimum, Maximum, ClosedMaximum);
        }

        public override string ToString()
        {
            return (ClosedMinimum ? "[" : "(") + Minimum + ", " + Maximum + (ClosedMaximum ? "]" : ")");
        }

        private static IEnumerable<T> IterateIterator(Func<T, T> increment, IComparer<T> comparer, T minimum, bool closedMinimum, T maximum, bool closedMaximum)
        {
            if (closedMinimum)
            {
                yield return minimum;
            }
            var item = increment.Invoke(minimum);
            while (comparer.Compare(maximum, item) > 0)
            {
                yield return item;
                item = increment.Invoke(item);
            }
            if (closedMaximum && comparer.Compare(maximum, item) == 0)
            {
                yield return item;
            }
        }

        private static IEnumerable<T> ReverseIterateIterator(Func<T, T> decrement, IComparer<T> comparer, T minimum, bool closedMinimum, T maximum, bool closedMaximum)
        {
            if (closedMaximum)
            {
                yield return maximum;
            }
            var item = decrement.Invoke(maximum);
            while (comparer.Compare(minimum, item) < 0)
            {
                yield return item;
                item = decrement.Invoke(item);
            }
            if (closedMinimum && comparer.Compare(minimum, item) == 0)
            {
                yield return item;
            }
        }
    }
}

#endif