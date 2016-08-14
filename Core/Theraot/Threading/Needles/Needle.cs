// Needed for NET40

using System;
using System.Threading;
using Theraot.Core;

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
            get
            {
                return false;
            }
        }

        bool IPromise.IsCompleted
        {
            get
            {
                return IsAlive;
            }
        }

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return !ReferenceEquals(target, null) && target.IsAlive;
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
            var needle = obj as Needle<T>;
            if (needle != null)
            {
                return EqualsExtracted(this, needle);
            }
            var target = _target;
            if (ReferenceEquals(_target, null))
            {
                return ReferenceEquals(obj, null);
            }
            if (ReferenceEquals(obj, null))
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
            Thread.MemoryBarrier();
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
            Thread.MemoryBarrier();
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
            if (ReferenceEquals(leftTarget, null))
            {
                return ReferenceEquals(rightTarget, null);
            }
            if (ReferenceEquals(rightTarget, null))
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
            if (ReferenceEquals(leftTarget, null))
            {
                return !ReferenceEquals(rightTarget, null);
            }
            if (ReferenceEquals(rightTarget, null))
            {
                return true;
            }
            return !leftTarget.Equals(rightTarget);
        }
    }
}