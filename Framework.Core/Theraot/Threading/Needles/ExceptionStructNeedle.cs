// Needed for NET40

using System;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public struct ExceptionStructNeedle<T> : INeedle<T>, IEquatable<ExceptionStructNeedle<T>>
    {
        private readonly Exception _exception;

        public ExceptionStructNeedle(Exception exception)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        T INeedle<T>.Value
        {
            get { throw _exception; }

            set { throw new NotSupportedException(); }
        }

        public bool IsAlive
        {
            get { return false; }
        }

        public T Value
        {
            get { throw _exception; }
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
            var leftException = left._exception;
            var rightException = right._exception;
            if (leftException == null)
            {
                return rightException != null;
            }
            return !leftException.Equals(rightException);
        }

        public static bool operator ==(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            var leftException = left._exception;
            var rightException = right._exception;
            if (leftException == null)
            {
                return rightException == null;
            }
            return leftException.Equals(rightException);
        }

        public override bool Equals(object obj)
        {
            if (obj is ExceptionStructNeedle<T>)
            {
                return this == (ExceptionStructNeedle<T>)obj;
            }
            return obj is Exception && obj.Equals(_exception);
        }

        public bool Equals(ExceptionStructNeedle<T> other)
        {
            return this == other;
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
            return "<Dead Needle>";
        }
    }
}