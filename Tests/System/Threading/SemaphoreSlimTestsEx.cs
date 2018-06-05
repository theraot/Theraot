using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;

namespace Tests.System.Threading
{
    [TestFixtureAttribute]
    public static class SemaphoreSlimTestsEx
    {
        [Test]
        public static void WaitAsyncWaitCorrectly()
        {
            var maxCount = 3;
            var maxTasks = 4;
            var log = new CircularBucket<string>(maxTasks * 3 + 2);
            var logCount = new CircularBucket<int>(maxTasks);
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(100)))
            {
                using (var semaphore = new SemaphoreSlim(0, maxCount))
                {
                    // No task should be able to enter semaphore at this point.
                    // Thus semaphore.CurrentCount should be 0
                    // We can directly check
                    Assert.AreEqual(0, semaphore.CurrentCount);
                    var padding = 0;
                    var tasks = Enumerable.Range(0, maxTasks)
                        .Select
                        (
                            _ =>
                            {
                                return Task.Factory.StartNew
                                (
                                    async () =>
                                    {
                                        log.Add("a");
                                        await semaphore.WaitAsync(source.Token);
                                        Interlocked.Add(ref padding, 100);
                                        logCount.Add(-1);
                                        log.Add("b");
                                        Thread.Sleep(1000 + padding);
                                        // Calling release should give increasing results per chunk
                                        var count = semaphore.Release();
                                        logCount.Add(count);
                                        log.Add("c");
                                    }
                                ).Unwrap();
                            }
                        ).ToArray();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    log.Add("x");
                    semaphore.Release(maxCount);
                    Task.WaitAll(tasks, source.Token);
                    log.Add("z");
                }
            }
            // We should see:
            // maxTask a
            // 1 x
            // chunks of at most maxCount b, separated by chunks of c
            foreach (var entry in log)
            {
                Console.WriteLine(entry);
            }
            // The results of release increase *per chunk of c*.
            var last = -1;
            var first = true;
            foreach (var entry in logCount)
            {
                if (entry == -1)
                {
                    first = true;
                    continue;
                }
                if (first)
                {
                    first = false;
                }
                else if (entry < last)
                {
                    Assert.Fail();
                }
                last = entry;
            }
        }
    }
}