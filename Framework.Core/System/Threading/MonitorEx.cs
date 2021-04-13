#if !(LESSTHAN_NET40 || NETSTANDARD1_0)

using System.Runtime.CompilerServices;

#endif

namespace System.Threading
{
    public static class MonitorEx
    {
#if !(LESSTHAN_NET40 || NETSTANDARD1_0)
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif

        public static void Enter(object obj, ref bool lockTaken)
        {
#if LESSTHAN_NET40 || NETSTANDARD1_0

            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (lockTaken)
            {
                throw new ArgumentException("Lock taken", nameof(lockTaken));
            }

            Monitor.Enter(obj);
            Volatile.Write(ref lockTaken, true);
#else
            Monitor.Enter(obj, ref lockTaken);
#endif
        }
    }
}