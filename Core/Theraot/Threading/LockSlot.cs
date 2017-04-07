#if FAT

using System;
using System.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed class LockSlot<T> : IComparable<LockSlot<T>>, INeedle<T>, IEquatable<LockSlot<T>>
    {
        internal readonly int _id;
        private readonly LockContext<T> _context;
        private int _free;
        private VersionProvider.VersionToken _versionToken;

        internal LockSlot(LockContext<T> context, int id, VersionProvider.VersionToken versionToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
            _versionToken = versionToken;
            _id = id;
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get { return !ReferenceEquals(Value, null); }
        }

        public T Value { get; set; }

        internal bool IsOpen
        {
            get { return Volatile.Read(ref _free) == 0; }
        }

        public static bool operator !=(LockSlot<T> left, LockSlot<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            return !left.Equals(right);
        }

        public static bool operator <(LockSlot<T> left, LockSlot<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return true;
            }
            return left.CompareTo(right) < 0;
        }

        public static bool operator ==(LockSlot<T> left, LockSlot<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator >(LockSlot<T> left, LockSlot<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return false;
            }
            return left.CompareTo(right) > 0;
        }

        public void Close()
        {
            if (Interlocked.CompareExchange(ref _free, 1, 0) == 0)
            {
                Value = default(T);
                _context.Close(this);
            }
        }

        public int CompareTo(LockSlot<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            return _versionToken.CompareTo(other._versionToken);
        }

        public override bool Equals(object obj)
        {
            return obj is LockSlot<T> && Equals((LockSlot<T>)obj);
        }

        public bool Equals(LockSlot<T> other)
        {
            if (other == null)
            {
                return false;
            }
            return _versionToken.Equals(other._versionToken);
        }

        public override int GetHashCode()
        {
            return _context.GetHashCode();
        }

        internal void Open(VersionProvider.VersionToken versionToken)
        {
            if (Interlocked.CompareExchange(ref _free, 0, 1) == 1)
            {
                _versionToken = versionToken;
            }
        }
    }
}

#endif