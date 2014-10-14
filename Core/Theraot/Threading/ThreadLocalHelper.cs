using System;

namespace Theraot.Threading
{
    internal static class ThreadLocalHelper
    {
        private static readonly Exception _recursionGuardException;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member", Justification = "False Positive")]
        static ThreadLocalHelper()
        {
            _recursionGuardException = new InvalidOperationException("Recursion");
        }

        public static Exception RecursionGuardException
        {
            get
            {
                return _recursionGuardException;
            }
        }
    }
}