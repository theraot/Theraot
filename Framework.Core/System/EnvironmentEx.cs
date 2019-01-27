// Needed for Workaround

using System.Runtime.CompilerServices;

namespace System
{
    public static class EnvironmentEx
    {
        public static bool Is64BitProcess
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get => IntPtr.Size == 8;
        }
    }
}