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
            var waitHandle = new ManualResetEventSlim[1];
            using (waitHandle[0] = new ManualResetEventSlim(false))
            {
                var blockingCollection = new BlockingCollection<Action>[1];
                using (blockingCollection[0] = new BlockingCollection<Action>())
                {
                    var thread = new Thread
                    (
                        () =>
                        {
                            try
                            {
                                waitHandle[0].Set();
                                var enumerable = cancellationToken.HasValue
                                    ? blockingCollection[0].GetConsumingEnumerable(cancellationToken.Value)
                                    : blockingCollection[0].GetConsumingEnumerable();
                                foreach (var action in enumerable)
                                {
                                    action();
                                }
                            }
                            catch (OperationCanceledException exception)
                            {
                                No.Op(exception);
                            }
                        }

                    );
                    thread.Start();

                    // Make sure thread is running
                    waitHandle[0].Wait();
                    Thread.Sleep(200);

                    blockingCollection[0].Add(() => called++);
                    blockingCollection[0].Add(() => called++);

                    blockingCollection[0].CompleteAdding();
                    thread.Join();
                    Assert.AreEqual(2, called);
                }
            }
        }
    }
}