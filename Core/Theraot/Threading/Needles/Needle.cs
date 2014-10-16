#if FAT

using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class Needle<T> : INeedle<T>, IEquatable<Needle<T>>
    {
        private readonly int _hashCode;
        private INeedle<T> _target;

        public Needle()
        {
            _target = null;
            _hashCode = base.GetHashCode();
        }

        public Needle(T target)
        {
            if (ReferenceEquals(target, null))
            {
                _hashCode = base.GetHashCode();
                _target = null;
            }
            else
            {
                _target = new StructNeedle<T>(target);
                _hashCode = target.GetHashCode();
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
                _target = null;
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

        public virtual T Value
        {
            get
            {
                Thread.MemoryBarrier();
                return _target.Value;
            }
            set
            {
                SetTarget(value);
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
            else
            {
                return _target.Equals(obj);
            }
        }

        public bool Equals(Needle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public void Free()
        {
            _target = null;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public Needle<T> Simplify()
        {
            var result = this;
            while (result._target is Needle<T>)
            {
                result = result._target as Needle<T>;
            }
            return result;
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

        internal INeedle<T> CompareExchange(INeedle<T> value, INeedle<T> comparand)
        {
            return Interlocked.CompareExchange(ref _target, value, comparand);
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
                return ReferenceEquals(right, null);
            }
            else
            {
                if (ReferenceEquals(right, null))
                {
                    return false;
                }
                else
                {
                    return left._target.Equals(right._target);
                }
            }
        }

        private static bool NotEqualsExtracted(Needle<T> left, Needle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            else
            {
                if (ReferenceEquals(right, null))
                {
                    return true;
                }
                else
                {
                    return !left._target.Equals(right._target);
                }
            }
        }
    }
}

#endif