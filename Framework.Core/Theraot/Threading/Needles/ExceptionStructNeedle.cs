// Needed for NET40

#pragma warning disable 659, 660, 661

using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public struct ExceptionStructNeedle<T> : INeedle<T>, IEquatable<ExceptionStructNeedle<T>>
    {
        private readonly ExceptionDispatchInfo _exceptionDispatchInfo;

        public ExceptionStructNeedle(Exception exception)
        {
            _exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
        }

        public Exception Exception
        {
            get
            {
                return _exceptionDispatchInfo.SourceException;
            }
        }

        public bool IsAlive => false;

        public T Value
        {
            get
            {
                _exceptionDispatchInfo.Throw();
                return default!;
            }
        }

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

        public override bool Equals(object? obj)
        {
            switch (obj)
            {
                case ExceptionStructNeedle<T> needle:
                    return this == needle;

                case ExceptionDispatchInfo info:
                    return _exceptionDispatchInfo.Equals(info);

                default:
                    return obj is Exception exc && exc.Equals(Exception);
            }
        }

        public override string ToString()
        {
            return IsAlive ? $"<Exception: {Exception}>" : "<Dead Needle>";
        }
    }
}