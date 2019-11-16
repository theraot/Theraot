using System;
using System.Threading;
using Theraot;

namespace TestRunner
{
    public static class ThreadPoolAvailabilityTests
    {
        public static void MethodAvailability()
        {
            No.Op<Func<WaitCallback, bool>>(ThreadPool.QueueUserWorkItem);
            No.Op<Func<WaitCallback, object, bool>>(ThreadPoolEx.QueueUserWorkItem);
            No.Op<Func<Action<object>, object, bool, bool>>(ThreadPoolEx.QueueUserWorkItem);
            No.Op<Func<WaitCallback, object, bool>>(ThreadPoolEx.UnsafeQueueUserWorkItem);
        }

        public static void TypeAvailability()
        {
            No.Op(typeof(ThreadPool));
            No.Op(typeof(WaitCallback));
        }
    }
}