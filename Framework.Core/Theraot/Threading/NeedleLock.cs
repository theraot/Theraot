#if FAT
using System;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal sealed class NeedleLock<T> : IReadOnlyNeedle<T>
    {
        private readonly LockContext<T> _context;
        private readonly int _hashCode;
        private FlagArray _capture;
        private int _owner;
        private T _target;

        internal NeedleLock(LockContext<T> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hashCode = base.GetHashCode();
            _capture = new FlagArray(_context.Capacity);
            _owner = -1;
        }

        internal NeedleLock(LockContext<T> context, T target)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            if (target == null)
            {
                _hashCode = base.GetHashCode();
            }
            else
            {
                _target = target;
                _hashCode = target.GetHashCode();
            }
            _capture = new FlagArray(_context.Capacity);
            _owner = -1;
        }

        bool IReadOnlyNeedle<T>.IsAlive => _target != null;

        public T Value
        {
            get
            {
                if (_context.Read(_capture, ref _owner, out var slot))
                {
                    _target = slot.Value;
                }
                return _target;
            }
        }

        public static explicit operator T(NeedleLock<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static bool operator !=(NeedleLock<T> left, NeedleLock<T> right)
        {
            if (left is null)
            {
                return !(right is null);
            }
            if (right is null)
            {
                return true;
            }
            return !left._target.Equals(right._target);
        }

        public static bool operator ==(NeedleLock<T> left, NeedleLock<T> right)
        {
            if (left is null)
            {
                return right is null;
            }
            return !(right is null) && left._target.Equals(right._target);
        }

        public override bool Equals(object obj)
        {
            var needle = obj as NeedleLock<T>;
            if (needle == null)
            {
                return _target.Equals(obj);
            }
            return this == needle;
        }

        public bool Equals(NeedleLock<T> other)
        {
            return !(other is null) && _target.Equals(other._target);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            return (this as IReadOnlyNeedle<T>).IsAlive ? target.ToString() : "<Dead Needle>";
        }

        internal void Capture(LockSlot<T> slot)
        {
            _capture[slot.Id] = true;
        }

        internal void Release()
        {
            if (Volatile.Read(ref _capture).Flags.IsEmpty())
            {
                _target = default;
            }
        }

        internal void Release(LockSlot<T> slot)
        {
            Interlocked.CompareExchange(ref _owner, -1, slot.Id);
            _capture[slot.Id] = false;
        }
    }
}

#endif