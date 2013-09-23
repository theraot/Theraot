#if FAT
using System;
using System.Threading;

namespace Theraot.Threading
{
    public sealed class VersionProvider
    {
        private Target _current;
        private Func<bool> _tryAdvance;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By Design")]
        public VersionProvider(out Action nextVersion)
        {
            _current = new Target(out _tryAdvance);
            nextVersion = Advance;
        }

        public void Advance()
        {
            if (!_tryAdvance.Invoke())
            {
                _current = new Target(out _tryAdvance);
            }
        }

        public VersionToken CreateToken()
        {
            return new VersionToken(this);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By Design")]
        public sealed class VersionToken : IComparable<VersionToken>
        {
            private long _number;
            private VersionProvider _provider;
            private Target _target;

            internal VersionToken(VersionProvider provider)
            {
                _provider = provider;
            }

            public int CompareTo(VersionToken other)
            {
                if (ReferenceEquals(other, null))
                {
                    return 1;
                }
                else
                {
                    var check = _target.CompareTo(other._target);
                    if (check == 0)
                    {
                        return _number.CompareTo(other._number);
                    }
                    else
                    {
                        return check;
                    }
                }
            }

            public void UpdateIfNeeded(Action update)
            {
                if (update == null)
                {
                    throw new ArgumentNullException("update");
                }
                else
                {
                    var nextTarget = _provider._current;
                    if
                    (
                        !ReferenceEquals(Interlocked.Exchange<Target>(ref _target, nextTarget), nextTarget) ||
                        Interlocked.Exchange(ref _number, nextTarget.Number) != nextTarget.Number
                    )
                    {
                        update();
                    }
                }
            }
        }

        private sealed class Target : IComparable<Target>
        {
            private long _number = long.MinValue;
            private long _time = DateTime.Now.Ticks;

            internal Target(out Func<bool> tryAdvance)
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
                else
                {
                    return _time.CompareTo(other._time);
                }
            }

            private bool TryAdvance()
            {
                if (_number == long.MaxValue)
                {
                    return false;
                }
                else
                {
                    _number++;
                    return true;
                }
            }
        }
    }
}
#endif