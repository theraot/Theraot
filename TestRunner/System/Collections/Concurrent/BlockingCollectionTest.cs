using System.Collections.Concurrent;
using System.Diagnostics;

namespace TestRunner.System.Collections.Concurrent
{
    [TestFixture]
    public static class BlockingCollectionTest
    {
        [Test]
        public static void TryTakeFromEmptyBlockingCollectionShouldNotThrow()
        {
            var a = new BlockingCollection<int>();
            a.TryTake(out var _);
        }

        [Test]
        public static void TryTakeFromEmptyBlockingCollectionWithTimeoutShouldWait()
        {
            var a = new BlockingCollection<int>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            a.TryTake(out var _, 200);
            stopWatch.Stop();
            Assert.IsTrue(stopWatch.ElapsedMilliseconds >= 200);
        }
    }
}
