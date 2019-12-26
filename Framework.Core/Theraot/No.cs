using System.Diagnostics;

namespace Theraot
{
    public static class No
    {
        /// <summary>
        /// This is an empty method, it that takes one value and discards it. Calls to it will not appear in release builds.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="arg">The value.</param>
        [Conditional("THERAOT_NEVER")]
        public static void Op<T>(T arg)
        {
            _ = arg;
        }
    }
}