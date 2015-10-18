#if FAT

using System;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Core;
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
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
            _hashCode = NeedleHelper.GetNextHashCode();
            _capture = new FlagArray(_context.Capacity);
            _owner = -1;
        }

        internal NeedleLock(LockContext<T> context, T target)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
            _target = target;
            _hashCode = NeedleHelper.GetNextHashCode();
            _capture = new FlagArray(_context.Capacity);
            _owner = -1;
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return _target != null;
            }
        }

        public T Value
        {
            get
            {
                LockSlot<T> slot;
                if (_context.Read(_capture, ref _owner, out slot))
                {
                    _target = slot.Value;
                }
                Thread.MemoryBarrier();
                return _target;
            }
        }

        public static explicit operator T(NeedleLock<T> needle)
        {
            return Check.NotNullArgument(needle, "needle").Value;
        }

        public static bool operator !=(NeedleLock<T> left, NeedleLock<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(NeedleLock<T> left, NeedleLock<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            var needle = obj as NeedleLock<T>;
            if (needle == null)
            {
                Thread.MemoryBarrier();
                return _target.Equals(obj);
            }
            return EqualsExtracted(this, needle);
        }

        public bool Equals(NeedleLock<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            if ((this as IReadOnlyNeedle<T>).IsAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }

        internal void Release()
        {
            if (ThreadingHelper.VolatileRead(ref _capture).Flags.IsEmpty())
            {
                _target = default(T);
                Thread.MemoryBarrier();
            }
        }

        internal void Capture(LockSlot<T> slot)
        {
            _capture[slot.Id] = true;
        }

        internal void Uncapture(LockSlot<T> slot)
        {
            Interlocked.CompareExchange(ref _owner, -1, slot.Id);
            _capture[slot.Id] = false;
        }

        private static bool EqualsExtracted(NeedleLock<T> left, NeedleLock<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            Thread.MemoryBarrier();
            return left._target.Equals(right._target);
        }

        private static bool NotEqualsExtracted(NeedleLock<T> left, NeedleLock<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return false;
                }
                return true;
            }
            Thread.MemoryBarrier();
            return !left._target.Equals(right._target);
        }
    }
}

#endif