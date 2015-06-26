// Needed for NET35

using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal static class ThreadLocalHelper<T>
    {
        private static readonly INeedle<T> _recursionGuardNeedle = new ExceptionStructNeedle<T>(ThreadLocalHelper.RecursionGuardException);

        public static INeedle<T> RecursionGuardNeedle
        {
            get
            {
                return _recursionGuardNeedle;
            }
        }
    }
}