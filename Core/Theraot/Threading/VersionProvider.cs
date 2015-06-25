using System;
using System.Threading;

namespace Theraot.Threading
{
    public sealed class VersionProvider
    {
        private Target _target;
        private Advancer _tryAdvance;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By Design")]
        public VersionProvider(out Action nextVersion)
        {
            _target = new Target(out _tryAdvance);
            nextVersion = Advance;
        }

        public VersionProvider()
        {
            _target = new Target(out _tryAdvance);
        }

        internal delegate bool Advancer(out long number);

        public void Advance()
        {
            long number;
            if (!_tryAdvance.Invoke(out number))
            {
                _target = new Target(out _tryAdvance);
            }
        }

        public VersionToken AdvanceNewToken()
        {
            long number;
            if (!_tryAdvance.Invoke(out number))
            {
                _target = new Target(out _tryAdvance);
            }
            return new VersionToken(this, _target, number);
        }

        public VersionToken NewToken()
        {
            return new VersionToken(this);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By Design")]
        public struct VersionToken : IComparable<VersionToken>
        {
            private long _number;
            private VersionProvider _provider;
            private Target _target;

            internal VersionToken(VersionProvider provider)
            {
                _number = 0;
                _target = null;
                _provider = provider;
            }

            internal VersionToken(VersionProvider provider, Target target, long number)
            {
                _provider = provider;
                _target = target;
                _number = number;
            }

            public int CompareTo(VersionToken other)
            {
                if (ReferenceEquals(_target, other._target))
                {
                    return _number.CompareTo(other._number);
                }
                if (ReferenceEquals(_target, null))
                {
                    return -1;
                }
                if (ReferenceEquals(other._target, null))
                {
                    return 1;
                }
                var check = _target.CompareToExtracted(other._target);
                return check == 0 ? _number.CompareTo(other._number) : check;
            }

            public void Reset()
            {
                _target = null;
                _number = 0;
            }

            public bool SetTo(VersionToken other)
            {
                return
                    !ReferenceEquals(Interlocked.Exchange(ref _provider, other._provider), other._provider)
                    | !ReferenceEquals(Interlocked.Exchange(ref _target, other._target), other._target)
                    | Interlocked.Exchange(ref _number, other._number) != other._number;
            }

            public bool Update()
            {
                if (_provider == null)
                {
                    throw new InvalidOperationException("This VersionToken doesn't have a VersionProvider associated.");
                }
                var newTarget = _provider._target;
                var newNumber = newTarget.Number;
                return !ReferenceEquals(Interlocked.Exchange(ref _target, newTarget), newTarget) | Interlocked.Exchange(ref _number, newNumber) != newNumber;
            }

            public bool UpdateTo(VersionToken other)
            {
                return CompareTo(other) < 0 &&
                       (
                           !ReferenceEquals(Interlocked.Exchange(ref _provider, other._provider), other._provider)
                           | !ReferenceEquals(Interlocked.Exchange(ref _target, other._target), other._target)
                           | Interlocked.Exchange(ref _number, other._number) != other._number
                        );
            }
        }

        internal sealed class Target : IComparable<Target>
        {
            private readonly long _time = ThreadingHelper.TicksNow();
            private long _number = long.MinValue;

            internal Target(out Advancer tryAdvance)
            {
                tryAdvance = TryAdvance;
            }

            public long Number
            {
                get
                {
                    return _number;
                }
            }

            public int CompareTo(Target other)
            {
                if (ReferenceEquals(other, null))
                {
                    return 1;
                }
                return CompareToExtracted(other);
            }

            internal int CompareToExtracted(Target other)
            {
                return _time.CompareTo(other._time);
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