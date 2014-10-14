using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public struct ReadOnlyStructNeedle<T> : INeedle<T>, IEquatable<ReadOnlyStructNeedle<T>>
    {
        private readonly T _target;

        public ReadOnlyStructNeedle(T target)
        {
            _target = target;
        }

        T INeedle<T>.Value
        {
            get
            {
                return _target;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsAlive
        {
            get
            {
                return !ReferenceEquals(_target, null);
            }
        }

        public T Value
        {
            get
            {
                return _target;
            }
        }

        public static explicit operator T(ReadOnlyStructNeedle<T> needle)
        {
            return needle._target;
        }

        public static implicit operator ReadOnlyStructNeedle<T>(T field)
        {
            return new ReadOnlyStructNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyStructNeedle<T>)
            {
                return EqualsExtracted(this, (ReadOnlyStructNeedle<T>)obj);
            }
            else
            {
                if (obj is T)
                {
                    if (IsAlive)
                    {
                        return EqualityComparer<T>.Default.Equals(_target, (T)obj);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Equals(ReadOnlyStructNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        void INeedle<T>.Free()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            if (IsAlive)
            {
                return _target.ToString();
            }
            else
            {
                return "<Dead Needle>";
            }
        }

        private static bool EqualsExtracted(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            var _left = left._target;
            if (left.IsAlive)
            {
                var _right = right._target;
                return right.IsAlive && EqualityComparer<T>.Default.Equals(_left, _right);
            }
            else
            {
                return !right.IsAlive;
            }
        }

        private static bool NotEqualsExtracted(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            var _left = left._target;
            if (left.IsAlive)
            {
                var _right = right._target;
                return !right.IsAlive || !EqualityComparer<T>.Default.Equals(_left, _right);
            }
            else
            {
                return right.IsAlive;
            }
        }
    }
}