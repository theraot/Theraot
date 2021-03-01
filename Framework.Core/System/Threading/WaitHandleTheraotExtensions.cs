#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Runtime.CompilerServices;

namespace System.Threading
{
    public static class WaitHandleTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool WaitOne(this WaitHandle waitHandle, int millisecondsTimeout, bool exitContext)
        {
            if (waitHandle == null)
            {
                throw new NullReferenceException();
            }

            _ = exitContext;
            return waitHandle.WaitOne(millisecondsTimeout);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool WaitOne(this WaitHandle waitHandle, TimeSpan timeout, bool exitContext)
        {
            if (waitHandle == null)
            {
                throw new NullReferenceException();
            }

            _ = exitContext;
            return waitHandle.WaitOne(timeout);
        }
    }
}

#endif