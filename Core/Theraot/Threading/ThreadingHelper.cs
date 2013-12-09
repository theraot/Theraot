using System;
using System.Threading;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class ThreadingHelper
    {
        private static readonly bool _isSingleCPU = Environment.ProcessorCount == 1;
        private static readonly int _sleepCountHint = 10;
        private static readonly int _spinWaitHint = 20;
        private static readonly RuntimeUniqueIdProdiver _threadIdProvider = new RuntimeUniqueIdProdiver();
        private static NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId> _threadRuntimeUniqueId = new NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId>(_threadIdProvider.GetNextId);

        public static RuntimeUniqueIdProdiver.UniqueId ThreadUniqueId
        {
            get
            {
                return _threadRuntimeUniqueId.Value;
            }
        }

        public static T VolatileRead<T>(ref T address)
            where T : class
        {
            T copy = address;
            Thread.MemoryBarrier();
            return copy;
        }

        public static void VolatileWrite<T>(ref T address, T value)
            where T : class
        {
            Thread.MemoryBarrier();
            address = value;
        }

        private static void SpinOnce(ref int count)
        {
            if (count == _sleepCountHint || _isSingleCPU)
            {
                count = 0;
                Thread.Sleep(0);
            }
            else
            {
                count++;
                Thread.SpinWait(_spinWaitHint);
            }
        }

        private static long TicksNow()
        {
            return DateTime.Now.Ticks;
        }

        private static long Milliseconds(long ticks)
        {
            return ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}