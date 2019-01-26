using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Theraot;

namespace TestRunner.System.Collections.Concurrent
{
    [TestFixture]
    public static class BlockingCollectionTest
    {
        [Test]
        public static void GetConsumingEnumerableShouldBlockWithCancellationTokenNone()
        {
            GetConsumingEnumerableTestImpl(CancellationToken.None);
        }

        [Test]
        public static void GetConsumingEnumerableShouldBlockWithCancellationTokenNotNone()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                GetConsumingEnumerableTestImpl(cancellationTokenSource.Token);
            }
        }

        [Test]
        public static void GetConsumingEnumerableShouldBlockWithNoCancellationToken()
        {
            GetConsumingEnumerableTestImpl(null);
        }

        [Test]
        public static void TryTakeFromEmptyBlockingCollectionShouldNotThrow()
        {
            using (var a = new BlockingCollection<int>())
            {
                a.TryTake(out _);
            }
        }

        [Test]
        public static void TryTakeFromEmptyBlockingCollectionWithTimeoutShouldWait()
        {
            using (var a = new BlockingCollection<int>())
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                a.TryTake(out _, 200);
                stopWatch.Stop();
                Assert.IsTrue(stopWatch.ElapsedMilliseconds >= 200);
            }
        }

        private static void GetConsumingEnumerableTestImpl(CancellationToken? cancellationToken)
        {
            var called = 0;
            var ev = new ManualResetEventSlim[1];
            using (ev[0] = new ManualResetEventSlim(false))
            {
                var bc = new BlockingCollection<Action>[1];
                using (bc[0] = new BlockingCollection<Action>())
                {
                    var t = new Thread
                    (
                        () =>
                        {
                            try
                            {
                                ev[0].Set();

                                if (cancellationToken.HasValue)
                                {
                                    foreach (var e in bc[0].GetConsumingEnumerable(cancellationToken.Value))
                                    {
                                        e();
                                    }
                                }
                                else
                                {
                                    foreach (var e in bc[0].GetConsumingEnumerable())
                                    {
                                        e();
                                    }
                                }
                            }
                            catch (OperationCanceledException exception)
                            {
                                No.Op(exception);
                            }
                        }
                    );
                    t.Start();

                    // Make sure thread is running
                    ev[0].Wait();
                    Thread.Sleep(200);

                    bc[0].Add(() => called++);
                    bc[0].Add(() => called++);

                    bc[0].CompleteAdding();
                    t.Join();
                    Assert.AreEqual(2, called);
                }
            }
        }
    }
}