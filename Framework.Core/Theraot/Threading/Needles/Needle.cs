// Needed for NET40

using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class Needle<T> : IEquatable<Needle<T>>, IRecyclableNeedle<T>, IPromise<T>
    {
        private readonly int _hashCode;
        private INeedle<T> _target; // Can be null - set in SetTargetValue and SetTargetError

        public Needle()
        {
            _target = null;
            _hashCode = base.GetHashCode();
        }

        public Needle(T target)
        {
            if (target == null)
            {
                _target = null;
                _hashCode = base.GetHashCode();
            }
            else
            {
                _target = new StructNeedle<T>(target);
                _hashCode = target.GetHashCode();
            }
        }

        public Exception Exception
        {
            get
            {
                var target = _target;
                if (target is ExceptionStructNeedle<T> needle)
                {
                    return needle.Exception;
                }
                return null;
            }
        }

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return target != null && target.IsAlive;
            }
        }

        bool IPromise.IsCanceled => false;

        bool IPromise.IsCompleted => IsAlive;
        public bool IsFaulted => _target is ExceptionStructNeedle<T>;

        public virtual T Value
        {
            get => _target.Value;

            set => SetTargetValue(value);
        }

        public static explicit operator T(Needle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static implicit operator Needle<T>(T field)
        {
            return new Needle<T>(field);
        }

        public static bool operator !=(Needle<T> left, Needle<T> right)
        {
            if (left is null)
            {
                return !(right is null);
            }
            if (right is null)
            {
                return true;
            }
            var leftTarget = left._target;
            var rightTarget = right._target;
            if (leftTarget == null)
            {
                return rightTarget != null;
            }
            if (rightTarget == null)
            {
                return true;
            }
            return !leftTarget.Equals(rightTarget);
        }

        public static bool operator ==(Needle<T> left, Needle<T> right)
        {
            if (left is null)
            {
                return right is null;
            }
            if (right is null)
            {
                return false;
            }
            var leftTarget = left._target;
            var rightTarget = right._target;
            if (leftTarget == null)
            {
                return rightTarget == null;
            }
            if (rightTarget == null)
            {
                return false;
            }
            return leftTarget.Equals(rightTarget);
        }

        public override bool Equals(object obj)
        {
            if (obj is Needle<T> needle)
            {
                return this == needle;
            }
            var target = _target;
            if (_target == null)
            {
                return obj == null;
            }
            if (obj == null)
            {
                return false;
            }
            return target.Equals(obj);
        }

        public bool Equals(Needle<T> other)
        {
            if (other is null)
            {
                return false;
            }
            var leftTarget = _target;
            var rightTarget = other._target;
            if (leftTarget == null)
            {
                return rightTarget == null;
            }
            if (rightTarget == null)
            {
                return false;
            }
            return leftTarget.Equals(rightTarget);
        }

        public virtual void Free()
        {
            _target = null;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            if (IsAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }

        protected void SetTargetError(Exception error)
        {
            _target = new ExceptionStructNeedle<T>(error);
        }

        protected void SetTargetValue(T value)
        {
            if (_target is StructNeedle<T>)
            {
                // This may throw NotSupportedException if SetTargetError has just executed
                try
                {
                    _target.Value = value;
                    return;
                }
                catch (NotSupportedException)
                {
                    // preventing return
                }
            }
            _target = new StructNeedle<T>(value);
        }
    }
}