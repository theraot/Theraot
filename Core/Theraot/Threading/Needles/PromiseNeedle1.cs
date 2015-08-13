// Needed for Workaround

using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class PromiseNeedle<T> : PromiseNeedle, ICacheNeedle<T>, IWaitablePromise<T>, IRecyclableNeedle<T>
    {
        private readonly int _hashCode;
        private T _target;

        public PromiseNeedle(bool done)
            : base(done)
        {
            _target = default(T);
            _hashCode = base.GetHashCode();
        }

        public PromiseNeedle(Exception exception)
            : base(exception)
        {
            _target = default(T);
            _hashCode = exception.GetHashCode();
        }

        protected PromiseNeedle(T target)
            : base(true)
        {
            _target = target;
            _hashCode = ReferenceEquals(target, null) ? base.GetHashCode() : target.GetHashCode();
        }

        public bool IsAlive
        {
            get
            {
                return _target != null;
            }
        }

        public virtual T Value
        {
            get
            {
                var exception = Exception;
                if (ReferenceEquals(exception, null))
                {
                    return _target;
                }
                throw exception;
            }
            set
            {
                _target = value;
                SetCompleted();
            }
        }

        public static PromiseNeedle<T> CreateFromValue(T target)
        {
            return new PromiseNeedle<T>(target);
        }

        public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public static explicit operator T(PromiseNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            return needle.Value;
        }

        public override bool Equals(object obj)
        {
            var needle = obj as PromiseNeedle<T>;
            if (!ReferenceEquals(null, needle))
            {
                return EqualsExtracted(this, needle);
            }
            return IsCompleted && Value.Equals(obj);
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override void Free()
        {
            base.Free();
            _target = default(T);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return IsCompleted
                ? (ReferenceEquals(Exception, null)
                    ? _target.ToString()
                    : Exception.ToString())
                : "[Not Created]";
        }

        public bool TryGetValue(out T target)
        {
            var result = IsCompleted;
            target = _target;
            return result;
        }

        private static bool EqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        private static bool NotEqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            return !left.Equals(right);
        }
    }
}