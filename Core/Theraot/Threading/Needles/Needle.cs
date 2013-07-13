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
        private T _target;

        public Needle()
        {
            _target = default(T);
            _hashCode = base.GetHashCode();
        }

        public Needle(T target)
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
                return _target;
            }
            set
            {
                SetTarget(value);
            }
        }

        public static implicit operator T(Needle<T> needle)
        {
            return ToT(needle);
        }

        public static bool operator !=(Needle<T> left, Needle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(Needle<T> left, Needle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "By Design")]
        public static T ToT(Needle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            else
            {
                return needle.Value;
            }
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as Needle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtracted(_obj, _obj);
            }
            else
            {
                return false;
            }
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

        protected virtual void OnRelease()
        {
            //Emtpy
        }

        protected void SetTarget(T value)
        {
            _target = value;
            Thread.MemoryBarrier();
        }

        private static bool EqualsExtracted(Needle<T> left, Needle<T> right)
        {
            var _left = left.Value;
            if (left.IsAlive)
            {
                var _right = right.Value;
                if (right.IsAlive)
                {
                    return EqualityComparer<T>.Default.Equals(_left, _right);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return !right.IsAlive;
            }
        }

        private static bool NotEqualsExtracted(Needle<T> left, Needle<T> right)
        {
            var _left = left.Value;
            if (left.IsAlive)
            {
                var _right = right.Value;
                if (right.IsAlive)
                {
                    return !EqualityComparer<T>.Default.Equals(_left, _right);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return right.IsAlive;
            }
        }
    }
}