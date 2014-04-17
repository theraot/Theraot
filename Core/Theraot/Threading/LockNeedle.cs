using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal sealed class LockNeedle<T> : INeedle<T>
    {
        private readonly int _hashCode;
        private FlagArray _capture;
        private int _lock;
        private T _target;

        internal LockNeedle()
        {
            _hashCode = GetHashCode();
            _capture = new FlagArray(LockNeedleContext<T>.Instance.Capacity);
        }

        internal LockNeedle(T target)
        {
            _target = target;
            if (ReferenceEquals(target, null))
            {
                _hashCode = GetHashCode();
            }
            else
            {
                _hashCode = target.GetHashCode();
            }
            _capture = new FlagArray(LockNeedleContext<T>.Instance.Capacity);
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
                if (LockNeedleContext<T>.Instance.Read(_lock, out value) || LockNeedleContext<T>.Instance.Read(_capture, out value))
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
            return Check.NotNullArgument(needle, "needle").Value;
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
            if (ReferenceEquals(null, _obj))
            {
                return _target.Equals(obj);
            }
            else
            {
                return EqualsExtracted(this, _obj);
            }
        }

        public bool Equals(LockNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public void Free()
        {
            if (System.Linq.Enumerable.Count(ThreadingHelper.VolatileRead(ref _capture).Flags) == 0)
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
            else
            {
                return "<Dead Needle>";
            }
        }

        internal void Capture(int id)
        {
            _capture[id] = true;
        }

        internal bool Lock(int id)
        {
            return Interlocked.CompareExchange(ref _lock, id, 0) == 0;
        }

        internal void Uncapture(int id)
        {
            _capture[id] = false;
        }

        internal bool Unlock(int id)
        {
            return Interlocked.CompareExchange(ref _lock, 0, id) == id;
        }

        private static bool EqualsExtracted(LockNeedle<T> left, LockNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            else
            {
                return left._target.Equals(right._target);
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
                return !left._target.Equals(right._target);
            }
        }
    }
}