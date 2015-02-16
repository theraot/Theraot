using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class Needle<T> : INeedle<T>, IEquatable<Needle<T>>, IRecyclableNeedle<T>
    {
        private readonly int _hashCode;
        private INeedle<T> _target; // Can be null

        public Needle()
        {
            _target = null;
            _hashCode = NeedleHelper.GetNextHashCode();
        }

        public Needle(T target)
        {
            _hashCode = NeedleHelper.GetNextHashCode();
            if (ReferenceEquals(target, null))
            {
                _target = null;
            }
            else
            {
                _target = new StructNeedle<T>(target);
            }
        }

        public Exception Error
        {
            get
            {
                if (_target is ExceptionStructNeedle<T>)
                {
                    return ((ExceptionStructNeedle<T>)_target).Error;
                }
                return null;
            }
        }

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return !ReferenceEquals(_target, null) && target.IsAlive;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return _target is ExceptionStructNeedle<T>;
            }
        }

        public virtual T Value
        {
            get
            {
                // Let it throw NullReferenceException
                return _target.Value;
            }
            set
            {
                SetTargetValue(value);
            }
        }

        public static explicit operator T(Needle<T> needle)
        {
            return Check.NotNullArgument(needle, "needle").Value;
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
            var _obj = obj as Needle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtracted(this, _obj);
            }
            return _target.Equals(obj);
        }

        public bool Equals(Needle<T> other)
        {
            var target = _target;
            if (ReferenceEquals(target, null))
            {
                return ReferenceEquals(other._target, null);
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
            Thread.MemoryBarrier();
        }

        protected void SetTargetValue(T value)
        {
            if (_target is StructNeedle<T>)
            {
                _target.Value = value;
            }
            else
            {
                _target = new StructNeedle<T>(value);
                Thread.MemoryBarrier();
            }
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
            if (ReferenceEquals(left._target, null))
            {
                return ReferenceEquals(right._target, null);
            }
            if (ReferenceEquals(right._target, null))
            {
                return false;
            }
            return left._target.Equals(right._target);
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
            if (ReferenceEquals(left._target, null))
            {
                return !ReferenceEquals(right._target, null);
            }
            if (ReferenceEquals(right._target, null))
            {
                return true;
            }
            return !left._target.Equals(right._target);
        }
    }
}