#if LESSTHAN_NET40 || NETSTANDARD1_0

namespace System.Threading
{
    public class MonitorEx
    {
        public static void Enter(object obj, ref bool lockTaken)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (lockTaken)
            {
                throw new ArgumentException("Lock taken", nameof(lockTaken));
            }

            var taken = Monitor.TryEnter(obj);

            if (!taken)
            {
                Monitor.Enter(obj);
            }

            lockTaken = taken;
        }
    }
}

#endif