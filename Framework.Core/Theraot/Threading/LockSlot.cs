#if FAT

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Threading;

using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed class LockSlot<T> : IComparable<LockSlot<T>>, INeedle<T>, IEquatable<LockSlot<T>>
    {
        internal readonly int Id;

        private readonly LockContext<T> _context;

        private int _free;

        private VersionToken _versionToken;

        internal LockSlot(LockContext<T> context, int id, VersionToken versionToken)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _versionToken = versionToken;
            Id = id;
        }

        public T Value { get; set; }

        bool IReadOnlyNeedle<T>.IsAlive => Value != null;

        internal bool IsOpen => Volatile.Read(ref _free) == 0;

        public static bool operator ==(LockSlot<T> left, LockSlot<T> right)
        {
            if (left is null)
            {
                return right is null;
            }

            return !(right is null) && left.Equals(right);
        }

        public static bool operator >(LockSlot<T> left, LockSlot<T> right)
        {
            if (left is null)
            {
                return false;
            }

            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(LockSlot<T> left, LockSlot<T> right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }

        public static bool operator !=(LockSlot<T> left, LockSlot<T> right)
        {
            if (left is null)
            {
                return !(right is null);
            }

            if (right is null)
            {
                return true;
            }

            return !left.Equals(right);
        }

        public static bool operator <(LockSlot<T> left, LockSlot<T> right)
        {
            if (left is null)
            {
                return true;
            }

            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(LockSlot<T> left, LockSlot<T> right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public void Close()
        {
            if (Interlocked.CompareExchange(ref _free, 1, 0) != 0)
            {
                return;
            }

            Value = default;
            _context.Close(this);
        }

        public int CompareTo(LockSlot<T> other)
        {
            return other is null ? 1 : _versionToken.CompareTo(other._versionToken);
        }

        public override bool Equals(object obj)
        {
            return obj is LockSlot<T> slot && Equals(slot);
        }

        public bool Equals(LockSlot<T> other)
        {
            return !(other is null) && _versionToken.Equals(other._versionToken);
        }

        public override int GetHashCode()
        {
            return _context.GetHashCode();
        }

        internal void Open(VersionToken versionToken)
        {
            if (Interlocked.CompareExchange(ref _free, 0, 1) == 1)
            {
                _versionToken = versionToken;
            }
        }
    }
}

#endif