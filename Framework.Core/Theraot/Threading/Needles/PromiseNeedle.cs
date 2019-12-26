// Needed for Workaround

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class PromiseNeedle<T> : Promise, ICacheNeedle<T>
    {
        private readonly int _hashCode;

        private T _target;

        public PromiseNeedle(bool done)
            : base(done)
        {
            _target = default!;
            _hashCode = base.GetHashCode();
        }

        public PromiseNeedle(Exception exception)
            : base(exception)
        {
            _target = default!;
            _hashCode = exception.GetHashCode();
        }

        protected PromiseNeedle(T target)
            : base(true)
        {
            _target = target;
            _hashCode = target == null ? base.GetHashCode() : target.GetHashCode();
        }

        public bool IsAlive => _target != null;

        public virtual T Value
        {
            get
            {
                var exception = Exception;
                if (exception == null)
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
            if (left is null)
            {
                return !(right is null);
            }

            if (right is null)
            {
                return true;
            }

            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return left is null
                ? right is null
                : !(right is null) && EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public override bool Equals(object? obj)
        {
            switch (obj)
            {
                case PromiseNeedle<T> other:
                    return Equals(other);

                case T otherValue:
                    return Equals(otherValue);

                default:
                    return false;
            }
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.TryGetValue(out var value))
            {
                return Equals(value);
            }

            return !IsAlive;
        }

        public override void Free()
        {
            base.Free();
            _target = default!;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = _target;
            if (!IsCompleted)
            {
                return "[Not Created]";
            }

            if (Exception == null)
            {
                return target?.ToString() ?? "[?]";
            }

            // ReSharper disable once ConstantNullCoalescingCondition
            return Exception.ToString() ?? "[?]";
        }

        public bool TryGetValue(out T value)
        {
            var result = IsCompleted;
            value = _target;
            return result;
        }

        private bool Equals(T otherValue)
        {
            var value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, otherValue);
        }
    }
}