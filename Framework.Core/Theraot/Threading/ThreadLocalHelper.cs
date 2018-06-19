// Needed for NET35 (ThreadLocal)

using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal static class ThreadLocalHelper
    {
        private static readonly Exception _recursionGuardException;

        static ThreadLocalHelper()
        {
            _recursionGuardException = GetInvalidOperationException();
        }

        public static Exception RecursionGuardException
        {
            get { return _recursionGuardException; }
        }

        private static InvalidOperationException GetInvalidOperationException()
        {
            return new InvalidOperationException("Recursion");
        }
    }

    internal static class ThreadLocalHelper<T>
    {
        private static readonly INeedle<T> _recursionGuardNeedle = new ExceptionStructNeedle<T>(ThreadLocalHelper.RecursionGuardException);

        public static INeedle<T> RecursionGuardNeedle
        {
            get { return _recursionGuardNeedle; }
        }
    }
}