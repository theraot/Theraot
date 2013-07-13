using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ReadOnlyNeedle<T> : IReadOnlyNeedle<T>, IEquatable<ReadOnlyNeedle<T>>
    {
        private readonly T _target;

        public ReadOnlyNeedle(T target)
        {
            _target = target;
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns True")]
            get
            {
                return true;
            }
        }

        public T Value
        {
            get
            {
                return _target;
            }
        }

        public static explicit operator T(ReadOnlyNeedle<T> needle)
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

        public static implicit operator ReadOnlyNeedle<T>(T field)
        {
            return new ReadOnlyNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public static bool operator ==(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            return EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyNeedle<T>)
            {
                return EqualityComparer<T>.Default.Equals(_target, ((ReadOnlyNeedle<T>)obj)._target);
            }
            else
            {
                if (obj is T)
                {
                    return EqualityComparer<T>.Default.Equals(_target, (T)obj);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Equals(ReadOnlyNeedle<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            else
            {
                return EqualityComparer<T>.Default.Equals(_target, other.Value);
            }
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(_target);
        }
    }
}