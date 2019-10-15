// Needed for Workaround

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class PromiseNeedle<T> : Promise, IRecyclable, ICacheNeedle<T>
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

        public bool TryGetValue(out T value)
        {
            var result = IsCompleted;
            value = _target;
            return result;
        }

        public override void Free()
        {
            base.Free();
            _target = default!;
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
            if (left is null)
            {
                return right is null;
            }

            if (right is null)
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public override bool Equals(object obj)
        {
            if (obj is PromiseNeedle<T> other)
            {
                return Equals(other);
            }

            if (obj is T otherValue)
            {
                return Equals(otherValue);
            }

            return false;
        }

        private bool Equals(T otherValue)
        {
            var value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, otherValue);
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

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = _target;
            return IsCompleted
                ? Exception == null
                    ? target!.ToString()
                    : Exception.ToString()
                : "[Not Created]";
        }
    }
}