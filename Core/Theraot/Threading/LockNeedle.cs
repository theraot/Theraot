using System;
using System.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal class LockNeedle<T> : INeedle<T>
    {
        private int _capture;
        private LockNeedleContext<T> _context;
        private int _hashCode;
        private int _lock;
        private T _target;

        internal LockNeedle(LockNeedleContext<T> context)
        {
            _context = context;
            _hashCode = base.GetHashCode();
        }

        internal LockNeedle(LockNeedleContext<T> context, T target)
        {
            _context = context;
            _target = target;
            if (ReferenceEquals(target, null))
            {
                _hashCode = base.GetHashCode();
            }
            else
            {
                _hashCode = target.GetHashCode();
            }
        }

        public bool IsAlive
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
                var @lock = Interlocked.Exchange(ref _lock, 0);
                var capture = Interlocked.Exchange(ref _capture, 0);
                if
                (
                    (@lock != 0 && _context.Read(Thread.VolatileRead(ref _lock), out value))
                    ||
                    (capture != 0 && _context.Read(Thread.VolatileRead(ref _capture), out value))
                )
                {
                    _target = value;
                }
                Thread.MemoryBarrier();
                return _target;
            }
            set
            {
                _target = value;
                Thread.MemoryBarrier();
            }
        }

        public static explicit operator T(LockNeedle<T> needle)
        {
            if (ReferenceEquals(needle, null))
            {
                throw new ArgumentNullException("needle");
            }
            else
            {
                return needle.Value;
            }
        }

        public static bool operator !=(LockNeedle<T> left, LockNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(LockNeedle<T> left, LockNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as LockNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtracted(this, _obj);
            }
            else
            {
                return _target.Equals(obj);
            }
        }

        public bool Equals(LockNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void Release()
        {
            if (Thread.VolatileRead(ref _capture) == 0)
            {
                _target = default(T);
            }
        }

        public override string ToString()
        {
            var target = Value;
            if (IsAlive)
            {
                return target.ToString();
            }
            else
            {
                return "<Dead Needle>";
            }
        }

        internal void Capture(int id)
        {
        again:
            int readed = Thread.VolatileRead(ref _capture);
            if ((readed & id) == 0)
            {
                if (Interlocked.CompareExchange(ref _capture, readed | id, readed) != readed)
                {
                    goto again;
                }
            }
        }

        internal bool Lock(int id)
        {
            if (Interlocked.CompareExchange(ref _lock, id, 0) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void Release(int id)
        {
        again:
            int readed = Thread.VolatileRead(ref _capture);
            if ((readed & id) != 0)
            {
                if (Interlocked.CompareExchange(ref _capture, readed & ~id, readed) != readed)
                {
                    goto again;
                }
            }
        }

        internal bool Unlock(int id)
        {
            if (Interlocked.CompareExchange(ref _lock, 0, id) == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool EqualsExtracted(LockNeedle<T> left, LockNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return left._target.Equals(left._target);
            }
        }

        private static bool NotEqualsExtracted(LockNeedle<T> left, LockNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return !left._target.Equals(left._target);
            }
        }
    }
}