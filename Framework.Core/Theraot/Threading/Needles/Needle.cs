#if FAT

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class Needle<T> : IEquatable<Needle<T>>, IRecyclableNeedle<T>, IPromise<T>
    {
        private readonly int _hashCode;
        private readonly StrongDelegateCollection _onCompleted;
        private INeedle<T> _target; // Can be null - set in SetTargetValue and SetTargetError

        public Needle()
        {
            _target = null;
            _hashCode = base.GetHashCode();
            _onCompleted = new StrongDelegateCollection(true);
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

            _onCompleted = new StrongDelegateCollection(true);
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

        public bool IsFaulted => _target is ExceptionStructNeedle<T>;

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

            return rightTarget != null && leftTarget.Equals(rightTarget);
        }

        bool IPromise.IsCompleted => IsAlive;

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return target?.IsAlive == true;
            }
        }

        public virtual T Value
        {
            get => _target.Value;

            set => SetTargetValue(value);
        }

        public virtual void Free()
        {
            _target = null;
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

            return rightTarget != null && leftTarget.Equals(rightTarget);
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

            return obj != null && target.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            return IsAlive ? target.ToString() : "<Dead Needle>";
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

            _onCompleted.InvokeWithException(null);
            _target = new StructNeedle<T>(value);
        }

        public virtual void OnCompleted(Action continuation)
        {
            _onCompleted.Add(continuation);
        }
    }
}

#endif