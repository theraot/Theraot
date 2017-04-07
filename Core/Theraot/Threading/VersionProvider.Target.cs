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

            internal Target(out Advancer tryAdvance)
            {
                tryAdvance = TryAdvance;
            }

            public long Number
            {
                get { return _number; }
            }

            public static bool operator !=(Target left, Target right)
            {
                if (ReferenceEquals(left, null))
                {
                    return !ReferenceEquals(right, null);
                }
                return left.Equals(right);
            }

            public static bool operator <(Target left, Target right)
            {
                if (ReferenceEquals(left, null))
                {
                    return true;
                }
                return left.CompareTo(right) < 0;
            }

            public static bool operator ==(Target left, Target right)
            {
                if (ReferenceEquals(left, null))
                {
                    return ReferenceEquals(right, null);
                }
                return left.Equals(right);
            }

            public static bool operator >(Target left, Target right)
            {
                if (ReferenceEquals(left, null))
                {
                    return false;
                }
                return left.CompareTo(right) > 0;
            }

            public int CompareTo(Target other)
            {
                if (ReferenceEquals(other, null))
                {
                    return 1;
                }
                return CompareToExtracted(other);
            }

            public bool Equals(Target other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                return EqualsExtracted(other);
            }

            public override bool Equals(object obj)
            {
                return obj is VersionProvider && Equals((VersionToken)obj);
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