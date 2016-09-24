#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    /// <summary>
    /// Provides a mechanism to get an object that represents a version of a mutable resource
    /// </summary>
    public sealed class VersionProvider
    {
        private Target _target;
        private Advancer _tryAdvance;

        /// <summary>
        /// Creates a new VersionProvider
        /// </summary>
        public VersionProvider()
        {
            _target = new Target(out _tryAdvance);
        }

        internal delegate bool Advancer(out long number);

        /// <summary>
        /// Advances the current up to date version
        /// </summary>
        public void Advance()
        {
            long number;
            if (!_tryAdvance.Invoke(out number))
            {
                _target = new Target(out _tryAdvance);
            }
        }

        /// <summary>
        /// Advances the current up to date version and returns a VersionToken for the new version
        /// </summary>
        /// <returns>A VersionToken representing the advanced version</returns>
        public VersionToken AdvanceNewToken()
        {
            long number;
            if (!_tryAdvance.Invoke(out number))
            {
                _target = new Target(out _tryAdvance);
            }
            return new VersionToken(this, _target, number);
        }

        /// <summary>
        /// Creates a new VersionToken, it should be considered to be out of date
        /// </summary>
        /// <returns>A new VersionToken</returns>
        public VersionToken NewToken()
        {
            return new VersionToken(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By Design")]
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

            /// <summary>
            /// Compares this instance with a specified VersionToken and indicates whether this instances precedes, follows, or represents the same version.
            /// </summary>
            /// <param name="other">A VersionToken to compare with.</param>
            /// <returns>A 32-bit signed integer indicating whether this instances precedes, follows, or represents the same version.</returns>
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

            /// <summary>
            /// Marks the current VersionToken as outdated
            /// </summary>
            public void Reset()
            {
                _target = null;
                _number = 0;
            }

            /// <summary>
            /// Set the version of this instance to that of an specefied VersionToken
            /// </summary>
            /// <param name="other">The VersionToken to copy the version from</param>
            /// <returns>Returns true if the version was changed; otherwise false.</returns>
            public bool SetTo(VersionToken other)
            {
                return
                    !ReferenceEquals(Interlocked.Exchange(ref _provider, other._provider), other._provider)
                    | !ReferenceEquals(Interlocked.Exchange(ref _target, other._target), other._target)
                    | Interlocked.Exchange(ref _number, other._number) != other._number;
            }

            /// <summary>
            /// Updates the version of this instance to current up to date version of the VersionProvider from which it was created
            /// </summary>
            /// <returns>Returns true if the version was changed; otherwise false.</returns>
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

            /// <summary>
            /// Updates the version of this instance to current up to that of an specefied VersionToken if it is newer
            /// </summary>
            /// <param name="other">The VersionToken to copy the version from</param>
            /// <returns>Returns true if the version was changed; otherwise false.</returns>
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

#endif