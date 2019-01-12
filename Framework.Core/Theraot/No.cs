using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Theraot
{
    public static class No
    {
        [Conditional("THERAOT_NEVER")]
#pragma warning disable RCS1163 // Unused parameter.
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
        // ReSharper disable once UnusedParameter.Global
        public static void Op<T>(T arg)
#pragma warning restore RCS1163 // Unused parameter.
        {
            // Empty
        }
    }
}
