using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public static class DebugEx
    {
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Fail(string message)
        {
#if LESSTHAN_NETSTANDARD13
            Debug.Assert(false, message);
#else
            Debug.Fail(message);
#endif
        }
    }
}