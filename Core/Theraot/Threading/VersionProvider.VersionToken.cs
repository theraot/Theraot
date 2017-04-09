#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    public sealed partial class VersionProvider
    {
        public struct VersionToken : IComparable<VersionToken>, IEquatable<VersionToken>
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

            public static bool operator !=(VersionToken left, VersionToken right)
            {
                return left.Equals(right);
            }

            public static bool operator <(VersionToken left, VersionToken right)
            {
                return left.CompareTo(right) < 0;
            }

            public static bool operator ==(VersionToken left, VersionToken right)
            {
                return left.Equals(right);
            }

            public static bool operator >(VersionToken left, VersionToken right)
            {
                return left.CompareTo(right) > 0;
            }

            /// <summary>
            /// Compares this instance with a specified VersionToken and indicates whether this instances precedes, follows, or represents the same version.
            /// </summary>
            /// <param name="other">A VersionToken to compare with.</param>
            /// <returns>A 32-bit signed integer indicating whether this instances precedes, follows, or represents the same version.</returns>
            public int CompareTo(VersionToken other)
            {
                if (_target == null)
                {
                    return -1;
                }
                if (other._target == null)
                {
                    return 1;
                }
                if (_target == other._target)
                {
                    return _number.CompareTo(other._number);
                }
                var check = _target.CompareToExtracted(other._target);
                return check == 0 ? _number.CompareTo(other._number) : check;
            }

            public bool Equals(VersionToken other)
            {
                if (_target == null)
                {
                    return false;
                }
                if (other._target == null)
                {
                    return false;
                }
                if (_target == other._target)
                {
                    return _number == other._number;
                }
                var check = _target.EqualsExtracted(other._target);
                return check && _number == other._number;
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
                    Interlocked.Exchange(ref _provider, other._provider) != other._provider
                    || Interlocked.Exchange(ref _target, other._target) != other._target
                    || Interlocked.Exchange(ref _number, other._number) != other._number;
            }

            /// <summary>
            /// Updates the version of this instance to current up to date version of the VersionProvider from which it was created
            /// </summary>
            /// <returns>Returns true if the version was changed; otherwise false.</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public bool Update()
            {
                if (_provider == null)
                {
                    throw new InvalidOperationException("This VersionToken doesn't have a VersionProvider associated.");
                }
                var newTarget = _provider._target;
                var newNumber = newTarget.Number;
                return Interlocked.Exchange(ref _target, newTarget) != newTarget
                || Interlocked.Exchange(ref _number, newNumber) != newNumber;
            }

            /// <summary>
            /// Updates the version of this instance to current up to that of an specefied VersionToken if it is newer
            /// </summary>
            /// <param name="other">The VersionToken to copy the version from</param>
            /// <returns>Returns true if the version was changed; otherwise false.</returns>
            public bool UpdateTo(VersionToken other)
            {
                return CompareTo(other) < 0
                       && (
                            Interlocked.Exchange(ref _provider, other._provider) != other._provider
                            || Interlocked.Exchange(ref _target, other._target) != other._target
                            || Interlocked.Exchange(ref _number, other._number) != other._number
                        );
            }

            public override bool Equals(object obj)
            {
                return obj is VersionProvider && Equals((VersionToken)obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}

#endif