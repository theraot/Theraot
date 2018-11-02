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
        private bool _closedMaximun;
        private bool _closedMinimun;
        private T _maximun;
        private T _minimun;

        public Range(T minimun, T maximun)
        {
            _closedMaximun = true;
            _closedMinimun = true;
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(minimun, maximun) > 0)
            {
                _minimun = maximun;
                _maximun = minimun;
            }
            else
            {
                _minimun = minimun;
                _maximun = maximun;
            }
        }

        public Range(T minimun, bool closedMinimun, T maximun, bool closedMaximun)
        {
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(minimun, maximun) > 0)
            {
                _minimun = maximun;
                _maximun = minimun;
                _closedMinimun = closedMaximun;
                _closedMaximun = closedMinimun;
            }
            else
            {
                _minimun = minimun;
                _maximun = maximun;
                _closedMinimun = closedMinimun;
                _closedMaximun = closedMaximun;
            }
        }

        public bool ClosedMaximun
        {
            get { return _closedMaximun; }
        }

        public bool ClosedMinimun
        {
            get { return _closedMinimun; }
        }

        public T Maximun
        {
            get { return _maximun; }
        }

        public T Minimun
        {
            get { return _minimun; }
        }

        public override int GetHashCode()
        {
            return (_minimun.GetHashCode() * 7) + _maximun.GetHashCode();
        }

        public IEnumerable<T> Iterate(Func<T, T> increment)
        {
            return IterateIterator(increment, Comparer<T>.Default, _minimun, _closedMinimun, _maximun, _closedMaximun);
        }

        public IEnumerable<T> Iterate(Func<T, T> increment, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return IterateIterator(increment, comparer, _minimun, _closedMinimun, _maximun, _closedMaximun);
        }

        public IEnumerable<T> ReverseIterate(Func<T, T> decrement)
        {
            return ReverseIterateIterator(decrement, Comparer<T>.Default, _minimun, _closedMinimun, _maximun, _closedMaximun);
        }

        public IEnumerable<T> ReverseIterate(Func<T, T> decrement, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return ReverseIterateIterator(decrement, comparer, _minimun, _closedMinimun, _maximun, _closedMaximun);
        }

        public override string ToString()
        {
            return (_closedMinimun ? "[" : "(") + _minimun + ", " + _maximun + (_closedMaximun ? "]" : ")");
        }

        private static IEnumerable<T> IterateIterator(Func<T, T> increment, IComparer<T> comparer, T minimun, bool closedMinimun, T maximun, bool closedMaximun)
        {
            if (closedMinimun)
            {
                yield return minimun;
            }
            var item = increment.Invoke(minimun);
            while (comparer.Compare(maximun, item) > 0)
            {
                yield return item;
                item = increment.Invoke(item);
            }
            if (closedMaximun && comparer.Compare(maximun, item) == 0)
            {
                yield return item;
            }
        }

        private static IEnumerable<T> ReverseIterateIterator(Func<T, T> decrement, IComparer<T> comparer, T minimun, bool closedMinimun, T maximun, bool closedMaximun)
        {
            if (closedMaximun)
            {
                yield return maximun;
            }
            var item = decrement.Invoke(maximun);
            while (comparer.Compare(minimun, item) < 0)
            {
                yield return item;
                item = decrement.Invoke(item);
            }
            if (closedMinimun && comparer.Compare(minimun, item) == 0)
            {
                yield return item;
            }
        }
    }
}

#endif