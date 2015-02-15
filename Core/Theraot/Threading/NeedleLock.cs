#if FAT

using System;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal sealed class NeedleLock<T> : INeedle<T>
    {
        private readonly LockContext<T> _context;
        private readonly int _hashCode;
        private FlagArray _capture;
        private int _lock;
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
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return !ReferenceEquals(_target, null);
            }
        }

        public T Value
        {
            get
            {
                T value;
                if (_context.Read(_lock, out value) || _context.Read(_capture, out value, out _lock))
                {
                    Thread.MemoryBarrier();
                    _target = value;
                    NotifyValueChanged();
                }
                return _target;
            }
            set
            {
                _target = value;
                Thread.MemoryBarrier();
                NotifyValueChanged();
            }
        }

        public void WaitValueChange()
        {
            lock (_capture)
            {
                Monitor.Wait(_capture);
            }
        }

        private void NotifyValueChanged()
        {
            lock (_capture)
            {
                Monitor.Pulse(_capture);
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
            var _obj = obj as NeedleLock<T>;
            if (ReferenceEquals(null, _obj))
            {
                return _target.Equals(obj);
            }
            return EqualsExtracted(this, _obj);
        }

        public bool Equals(NeedleLock<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public void Release()
        {
            if (ThreadingHelper.VolatileRead(ref _capture).Flags.IsEmpty())
            {
                _target = default(T);
            }
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

        internal void Capture(int id)
        {
            _capture[id] = true;
            NotifyValueChanged();
        }

        internal void Uncapture(int id)
        {
            if (_lock == id)
            {
                _lock = -1;
            }
            _capture[id] = false;
            NotifyValueChanged();
        }

        internal bool Lock(int id)
        {
            return Interlocked.CompareExchange(ref _lock, id, 0) == 0;
        }

        internal bool Unlock(int id)
        {
            return Interlocked.CompareExchange(ref _lock, 0, id) == id;
        }

        private static bool EqualsExtracted(NeedleLock<T> left, NeedleLock<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
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
            return !left._target.Equals(right._target);
        }
    }
}

#endif