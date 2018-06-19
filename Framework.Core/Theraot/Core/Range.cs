#if FAT

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public struct Range<T> : IComparable<Range<T>>, IComparable<T>, IComparable
        where T : IComparable<T>
    {
        private readonly T _maximun;
        private readonly T _minimun;

        public Range(T minimun, T maximun)
        {
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

        public T Maximun
        {
            get { return _maximun; }
        }

        public T Minimun
        {
            get { return _minimun; }
        }

        public static bool operator !=(Range<T> left, Range<T> right)
        {
            return !(left == right);
        }

        public static bool operator <(Range<T> left, Range<T> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <(Range<T> left, T right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Range<T> left, Range<T> right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator <=(Range<T> left, T right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator ==(Range<T> left, Range<T> right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator >(Range<T> left, Range<T> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >(Range<T> left, T right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Range<T> left, Range<T> right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator >=(Range<T> left, T right)
        {
            return left.CompareTo(right) >= 0;
        }

        public int CompareTo(Range<T> other)
        {
            var comparer = Comparer<T>.Default;
            return comparer.Compare(_minimun, other._minimun);
        }

        public int CompareTo(Range<T> other, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return comparer.Compare(_minimun, other._minimun);
        }

        public int CompareTo(T other)
        {
            var comparer = Comparer<T>.Default;
            return comparer.Compare(_minimun, other);
        }

        public int CompareTo(T other, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return comparer.Compare(_minimun, other);
        }

        public int CompareTo(object obj)
        {
            if (obj is Range<T>)
            {
                return CompareTo((Range<T>)obj);
            }
            // Keep the "is" operator
            if (obj is T)
            {
                return CompareTo((T)obj);
            }
            throw new ArgumentException("obj is not of a compatible type.");
        }

        public int CompareTo(object obj, IComparer<T> comparer)
        {
            if (obj is Range<T>)
            {
                return CompareTo((Range<T>)obj, comparer);
            }
            // Keep the "is" operator
            if (obj is T)
            {
                return CompareTo((T)obj, comparer);
            }
            throw new ArgumentException("obj is not of a compatible type.");
        }

        public bool Contains(T value)
        {
            var comparer = Comparer<T>.Default;
            return comparer.Compare(_minimun, value) <= 0 && comparer.Compare(_maximun, value) >= 0;
        }

        public bool Contains(T value, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return comparer.Compare(_minimun, value) <= 0 && comparer.Compare(_maximun, value) >= 0;
        }

        public bool Contains(Range<T> range)
        {
            var comparer = Comparer<T>.Default;
            return comparer.Compare(_minimun, range._minimun) <= 0 && comparer.Compare(_maximun, range._maximun) >= 0;
        }

        public bool Contains(Range<T> range, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return comparer.Compare(_minimun, range._minimun) <= 0 && comparer.Compare(_maximun, range._maximun) >= 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Range<T>)
            {
                return CompareTo((Range<T>)obj) == 0 && _maximun.CompareTo(((Range<T>)obj)._maximun) == 0;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (_minimun.GetHashCode() * 7) + _maximun.GetHashCode();
        }

        public bool IsContainedBy(Range<T> range)
        {
            return range.Contains(this);
        }

        public bool IsContainedBy(Range<T> range, IComparer<T> comparer)
        {
            return range.Contains(this, comparer);
        }

        public bool IsContiguousWith(Range<T> range)
        {
            return Overlaps(range) || range.Overlaps(this) || range.Contains(this) || Contains(range) || Touches(range);
        }

        public bool IsContiguousWith(Range<T> range, IComparer<T> comparer)
        {
            return Overlaps(range, comparer) || range.Overlaps(this, comparer) || range.Contains(this, comparer) || Contains(range, comparer) || Touches(range, comparer);
        }

        public IEnumerable<T> Iterate(Func<T, T> increment)
        {
            yield return _minimun;
            T item = increment.Invoke(_minimun);
            var comparer = Comparer<T>.Default;
            while (comparer.Compare(_maximun, item) >= 0)
            {
                yield return item;
                item = increment.Invoke(item);
            }
        }

        public IEnumerable<T> Iterate(Func<T, T> increment, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            yield return _minimun;
            T item = increment.Invoke(_minimun);
            while (comparer.Compare(_maximun, item) >= 0)
            {
                yield return item;
                item = increment.Invoke(item);
            }
        }

        public bool Overlaps(Range<T> range)
        {
            return Contains(range._minimun) || Contains(range._maximun) || range.Contains(_minimun) || range.Contains(_maximun);
        }

        public bool Overlaps(Range<T> range, IComparer<T> comparer)
        {
            return Contains(range._minimun, comparer) || Contains(range._maximun, comparer) || range.Contains(_minimun, comparer) || range.Contains(_maximun, comparer);
        }

        public IEnumerable<T> ReverseIterate(Func<T, T> decrement)
        {
            yield return _maximun;
            T item = decrement.Invoke(_maximun);
            var comparer = Comparer<T>.Default;
            while (comparer.Compare(_minimun, item) <= 0)
            {
                yield return item;
                item = decrement.Invoke(item);
            }
        }

        public IEnumerable<T> ReverseIterate(Func<T, T> decrement, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            yield return _maximun;
            T item = decrement.Invoke(_maximun);
            while (comparer.Compare(_minimun, item) <= 0)
            {
                yield return item;
                item = decrement.Invoke(item);
            }
        }

        public override string ToString()
        {
            return "{" + _minimun + "->" + _maximun + "}";
        }

        public bool Touches(Range<T> range)
        {
            var comparer = Comparer<T>.Default;
            return comparer.Compare(_maximun, range._minimun) == 0 || comparer.Compare(_minimun, range._maximun) == 0;
        }

        public bool Touches(Range<T> range, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return comparer.Compare(_maximun, range._minimun) == 0 || comparer.Compare(_minimun, range._maximun) == 0;
        }
    }
}

#endif