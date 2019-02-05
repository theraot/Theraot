#if FAT
using System;
using System.Threading;

namespace Theraot.Threading
{
    public sealed partial class VersionProvider
    {
        internal sealed class Target : IComparable<Target>, IEquatable<Target>
        {
            private readonly long _time = ThreadingHelper.TicksNow();
            private long _number = long.MinValue;

            internal Target(out TryAdvance tryAdvance)
            {
                tryAdvance = TryAdvance;
            }

            public long Number => _number;

            public static bool operator !=(Target left, Target right)
            {
                if (left is null)
                {
                    return !(right is null);
                }
                return left.Equals(right);
            }

            public static bool operator <(Target left, Target right)
            {
                if (left is null)
                {
                    return true;
                }
                return left.CompareTo(right) < 0;
            }

            public static bool operator ==(Target left, Target right)
            {
                if (left is null)
                {
                    return right is null;
                }
                return left.Equals(right);
            }

            public static bool operator >(Target left, Target right)
            {
                if (left is null)
                {
                    return false;
                }
                return left.CompareTo(right) > 0;
            }

            public int CompareTo(Target other)
            {
                return other is null ? 1 : CompareToExtracted(other);
            }

            public bool Equals(Target other)
            {
                return !(other is null) && EqualsExtracted(other);
            }

            public override bool Equals(object obj)
            {
                return obj is Target target && Equals(target);
            }

            public override int GetHashCode()
            {
                return _time.GetHashCode();
            }

            internal int CompareToExtracted(Target other)
            {
                return _time.CompareTo(other._time);
            }

            internal bool EqualsExtracted(Target other)
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
}

#endif