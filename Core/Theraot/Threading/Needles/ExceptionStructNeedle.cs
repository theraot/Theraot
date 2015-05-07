using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public struct ExceptionStructNeedle<T> : INeedle<T>, IEquatable<ExceptionStructNeedle<T>>
    {
        private readonly AggregateException _exception;

        public ExceptionStructNeedle(Exception exception)
        {
            _exception = new AggregateException (exception);
        }

        public AggregateException Exception
        {
            get
            {
                return _exception;
            }
        }

        T INeedle<T>.Value
        {
            get
            {
                throw _exception;
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
                return false;
            }
        }

        public T Value
        {
            get
            {
                throw _exception;
            }
        }

        public static explicit operator Exception(ExceptionStructNeedle<T> needle)
        {
            return needle._exception;
        }

        public static implicit operator ExceptionStructNeedle<T>(Exception exception)
        {
            return new ExceptionStructNeedle<T>(exception);
        }

        public static bool operator !=(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            if (obj is ExceptionStructNeedle<T>)
            {
                return EqualsExtracted(this, (ExceptionStructNeedle<T>)obj);
            }
            else
            {
                return obj is Exception && obj.Equals(_exception);
            }
        }

        public bool Equals(ExceptionStructNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (IsAlive)
            {
                return string.Format("<Exception: {0}>", _exception);
            }
            else
            {
                return "<Dead Needle>";
            }
        }

        private static bool EqualsExtracted(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            var _left = left._exception;
            var _right = right._exception;
            if (ReferenceEquals(_left, null))
            {
                return ReferenceEquals(_right, null);
            }
            else
            {
                return _left.Equals(_right);
            }
        }

        private static bool NotEqualsExtracted(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            var _left = left._exception;
            var _right = right._exception;
            if (ReferenceEquals(_left, null))
            {
                return !ReferenceEquals(_right, null);
            }
            else
            {
                return !_left.Equals(_right);
            }
        }
    }
}