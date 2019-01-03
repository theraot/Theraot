using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class ThreadPoolTest
    {
        [Test]
        public static void ThreadPoolUserWorkItemRuns()
        {
            var waitHandle = new ManualResetEventSlim(false);
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    waitHandle.Set();
                }
            );
            waitHandle.Wait();
        }
    }
}