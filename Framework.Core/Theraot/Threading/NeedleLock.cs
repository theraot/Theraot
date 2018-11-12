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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
            _hashCode = base.GetHashCode();
            _capture = new FlagArray(_context.Capacity);
            _owner = -1;
        }

        internal NeedleLock(LockContext<T> context, T target)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
            if (ReferenceEquals(target, null))
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

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get { return !ReferenceEquals(_target, null); }
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
            if (left == null)
            {
                if (right == null)
                {
                    return false;
                }
                return true;
            }
            return !left._target.Equals(right._target);
        }

        public static bool operator ==(NeedleLock<T> left, NeedleLock<T> right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left._target.Equals(right._target);
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
            return this == other;
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

        internal void Capture(LockSlot<T> slot)
        {
            _capture[slot.Id] = true;
        }

        internal void Release()
        {
            if (Volatile.Read(ref _capture).Flags.IsEmpty())
            {
                _target = default(T);
            }
        }

        internal void Uncapture(LockSlot<T> slot)
        {
            Interlocked.CompareExchange(ref _owner, -1, slot.Id);
            _capture[slot.Id] = false;
        }
    }
}

#endif