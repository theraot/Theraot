using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

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

        private delegate void TestAction();

        private static void GetConsumingEnumerableTestImpl(CancellationToken? cancellationToken)
        {
            var called = 0;
            var ev = new ManualResetEventSlim(false);

            var bc = new BlockingCollection<TestAction>();

            var t = new Thread(() => {
                try
                {
                    ev.Set();

                    if (cancellationToken.HasValue)
                    {
                        foreach (var e in bc.GetConsumingEnumerable(cancellationToken.Value))
                        {
                            e();
                        }
                    }
                    else
                    {
                        foreach (var e in bc.GetConsumingEnumerable())
                        {
                            e();
                        }
                    }
                }
                catch (OperationCanceledException)
                {

                }
            });
            t.Start();

            // Make sure thread is running
            ev.Wait();
            Thread.Sleep(200);

            bc.Add(() => called++);
            bc.Add(() => called++);

            bc.CompleteAdding();
            t.Join();

            Assert.AreEqual(2, called);
        }

        [Test]
        public static void GetConsumingEnumerableShouldBlockWithNoCancellationToken()
        {
            GetConsumingEnumerableTestImpl(null);
        }

        [Test]
        public static void GetConsumingEnumerableShouldBlockWithCancellationTokenNone()
        {
            GetConsumingEnumerableTestImpl(CancellationToken.None);
        }

        [Test]
        public static void GetConsumingEnumerableShouldBlockWithCancellationTokenNotNone()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            GetConsumingEnumerableTestImpl(cancellationTokenSource.Token);
        }
    }
}
