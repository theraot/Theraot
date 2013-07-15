using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class Needle<T> : INeedle<T>
    {
        private int _hashCode;
        private INeedle<T> _target;

        public Needle()
        {
            _target = null;
            _hashCode = base.GetHashCode();
        }

        public Needle(T target)
        {
            _target = new StructNeedle<T>(target);
            if (IsAlive)
            {
                _hashCode = target.GetHashCode();
            }
            else
            {
                _hashCode = base.GetHashCode();
            }
        }

        public Needle(INeedle<T> target)
        {
            _target = target;
            if (IsAlive)
            {
                _hashCode = target.GetHashCode();
            }
            else
            {
                _hashCode = base.GetHashCode();
            }
        }

        public bool IsAlive
        {
            get
            {
                return !ReferenceEquals(_target, null);
            }
        }

        public virtual T Value
        {
            get
            {
                Thread.MemoryBarrier();
                return (T)_target;
            }
            set
            {
                SetTarget(value);
            }
        }

        public static explicit operator T(Needle<T> needle)
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
            else
            {
                return _target.Equals(obj);
            }
        }

        public bool Equals(Needle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Empty")]
        void INeedle<T>.Release()
        {
            OnRelease();
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

        public void Unify(ref INeedle<T> value)
        {
            if (ReferenceEquals(value, null))
            {
                if (ReferenceEquals(_target, null))
                {
                    _target = new Needle<T>();
                    Thread.MemoryBarrier();
                    value = _target;
                }
                else
                {
                    value = _target;
                }
            }
            else
            {
                _target = value;
                Thread.MemoryBarrier();
            }
        }

        protected virtual void OnRelease()
        {
            _target = null;
        }

        protected void SetTarget(T value)
        {
            if (ReferenceEquals(_target, null))
            {
                _target = new StructNeedle<T>(value);
                Thread.MemoryBarrier();
            }
            else
            {
                _target.Value = value;
            }
        }

        private static bool EqualsExtracted(Needle<T> left, Needle<T> right)
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

        private static bool NotEqualsExtracted(Needle<T> left, Needle<T> right)
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