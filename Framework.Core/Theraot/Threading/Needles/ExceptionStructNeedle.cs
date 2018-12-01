// Needed for NET40

using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public struct ExceptionStructNeedle<T> : INeedle<T>, IEquatable<ExceptionStructNeedle<T>>
    {
        public ExceptionStructNeedle(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public bool IsAlive => false;

        T INeedle<T>.Value
        {
            get => throw Exception;

            set => throw new NotSupportedException();
        }

        public T Value => throw Exception;

        public static explicit operator Exception(ExceptionStructNeedle<T> needle)
        {
            return needle.Exception;
        }

        public static implicit operator ExceptionStructNeedle<T>(Exception exception)
        {
            return new ExceptionStructNeedle<T>(exception);
        }

        public static bool operator !=(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            var leftException = left.Exception;
            var rightException = right.Exception;
            if (leftException == null)
            {
                return rightException != null;
            }
            return !leftException.Equals(rightException);
        }

        public static bool operator ==(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
        {
            var leftException = left.Exception;
            var rightException = right.Exception;
            if (leftException == null)
            {
                return rightException == null;
            }
            return leftException.Equals(rightException);
        }

        public override bool Equals(object obj)
        {
            if (obj is ExceptionStructNeedle<T> needle)
            {
                return this == needle;
            }
            return obj is Exception && obj.Equals(Exception);
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
                return $"<Exception: {Exception}>";
            }
            return "<Dead Needle>";
        }
    }
}