using System.Diagnostics;

namespace Theraot
{
    public static class No
    {
        [Conditional("THERAOT_NEVER")]
        public static void Op<T>(T arg)
        {
            // Empty
        }
    }
}
