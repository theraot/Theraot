#pragma warning disable CC0057 // Unused parameters
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
// ReSharper disable UnusedParameter.Global

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Theraot
{
    public static class No
    {
        [Conditional("THERAOT_NEVER")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
        public static void Op<T>(T arg)
        {
            // Empty
        }
    }
}
