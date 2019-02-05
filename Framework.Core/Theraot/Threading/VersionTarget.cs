#if FAT
using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed class VersionTarget : IComparable<VersionTarget>, IEquatable<VersionTarget>
    {
        private readonly long _time = ThreadingHelper.TicksNow();
        private long _number = long.MinValue;

        internal VersionTarget(out TryAdvance tryAdvance)
        {
            tryAdvance = TryAdvance;
        }

        public long Number => _number;

        public static bool operator !=(VersionTarget left, VersionTarget right)
        {
            if (left is null)
            {
                return !(right is null);
            }
            return left.Equals(right);
        }

        public static bool operator <(VersionTarget left, VersionTarget right)
        {
            if (left is null)
            {
                return true;
            }
            return left.CompareTo(right) < 0;
        }

        public static bool operator ==(VersionTarget left, VersionTarget right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static bool operator >(VersionTarget left, VersionTarget right)
        {
            if (left is null)
            {
                return false;
            }
            return left.CompareTo(right) > 0;
        }

        public int CompareTo(VersionTarget other)
        {
            return other is null ? 1 : CompareToExtracted(other);
        }

        public bool Equals(VersionTarget other)
        {
            return !(other is null) && EqualsExtracted(other);
        }

        public override bool Equals(object obj)
        {
            return obj is VersionTarget target && Equals(target);
        }

        public override int GetHashCode()
        {
            return _time.GetHashCode();
        }

        internal int CompareToExtracted(VersionTarget other)
        {
            return _time.CompareTo(other._time);
        }

        internal bool EqualsExtracted(VersionTarget other)
        {
            return _time == other._time;
        }

        private bool TryAdvance(out long number)
        {
            if (_number == long.MaxValue)
            {
                number = 0;
                return false;
            }
            number = Interlocked.Increment(ref _number);
            return true;
        }
    }
}

#endif