// Needed for NET35 (ThreadLocal)

using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal static class ThreadLocalHelper
    {
        static ThreadLocalHelper()
        {
            RecursionGuardException = GetInvalidOperationException();
        }

        public static Exception RecursionGuardException { get; }

        private static InvalidOperationException GetInvalidOperationException()
        {
            return new InvalidOperationException("Recursion");
        }
    }

    internal static class ThreadLocalHelper<T>
    {
        public static INeedle<T> RecursionGuardNeedle { get; } = new ExceptionStructNeedle<T>(ThreadLocalHelper.RecursionGuardException);
    }
}