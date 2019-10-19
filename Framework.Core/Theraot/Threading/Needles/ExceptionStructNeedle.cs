// Needed for NET40

#pragma warning disable 659, 660, 661

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

        public T Value => throw Exception;

        T INeedle<T>.Value { get => throw Exception; set => throw new NotSupportedException(); }

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

        public bool Equals(ExceptionStructNeedle<T> other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is ExceptionStructNeedle<T> needle)
            {
                return this == needle;
            }

            return obj is Exception exc && exc.Equals(Exception);
        }

        public override string ToString()
        {
            return IsAlive ? $"<Exception: {Exception}>" : "<Dead Needle>";
        }
    }
}