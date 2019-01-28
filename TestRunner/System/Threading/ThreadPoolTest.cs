using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class ThreadPoolTest
    {
        [Test]
        public static void ThreadPoolUserWorkItemRuns()
        {
            var waitHandle = new ManualResetEventSlim[1];
            using (waitHandle[0] = new ManualResetEventSlim(false))
            {
                ThreadPool.QueueUserWorkItem(_ => waitHandle[0].Set());
                waitHandle[0].Wait();
            }
        }
    }
}