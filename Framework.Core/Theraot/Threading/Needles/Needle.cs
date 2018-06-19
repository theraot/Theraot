// Needed for NET40

using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
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
            if (ReferenceEquals(target, null))
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
                if (target is ExceptionStructNeedle<T>)
                {
                    return ((ExceptionStructNeedle<T>)target).Exception;
                }
                return null;
            }
        }

        bool IPromise.IsCanceled
        {
            get { return false; }
        }

        bool IPromise.IsCompleted
        {
            get { return IsAlive; }
        }

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return target != null && target.IsAlive;
            }
        }

        public bool IsFaulted
        {
            get { return _target is ExceptionStructNeedle<T>; }
        }

        public virtual T Value
        {
            get { return _target.Value; }

            set { SetTargetValue(value); }
        }

        public static explicit operator T(Needle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            return needle.Value;
        }

        public static implicit operator Needle<T>(T field)
        {
            return new Needle<T>(field);
        }

        public static bool operator !=(Needle<T> left, Needle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(Needle<T> left, Needle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            var needle = obj as Needle<T>;
            if (needle != null)
            {
                return EqualsExtracted(this, needle);
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
            var target = _target;
            if (target == null)
            {
                return other._target == null;
            }
            return EqualsExtracted(this, other);
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

        private static bool EqualsExtracted(Needle<T> left, Needle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
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

        private static bool NotEqualsExtracted(Needle<T> left, Needle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
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
    }
}