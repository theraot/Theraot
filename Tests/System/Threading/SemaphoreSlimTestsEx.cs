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
            var log = new CircularBucket<string>(128);
            var max = 3;
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(100)))
            {
                using (var semaphore = new SemaphoreSlim(0, max))
                {
                    // No task should be able to enter semaphore at this point.
                    // Thus semaphore.CurrentCount should be 0
                    // We can directly check
                    Assert.AreEqual(0, semaphore.CurrentCount);
                    log.Add(string.Format("{0} task can enter the semaphore.", semaphore.CurrentCount));
                    var padding = 0;
                    var tasks = Enumerable.Range(0, 4)
                        .Select(_ =>
                        {
                            return Task.Factory.StartNew(async () =>
                            {
                                var CurrentId = Task.CurrentId;
                                log.Add(string.Format("Task {0} begins and waits for the semaphore.", CurrentId));

                                await semaphore.WaitAsync(source.Token);

                                Interlocked.Add(ref padding, 100);

                                log.Add(string.Format("Task {0} enters the semaphore.", CurrentId));

                                Thread.Sleep(1000 + padding);

                                log.Add(string.Format("Task {0} release the semaphore; previous count: {1} ", CurrentId, semaphore.Release()));
                            }).Unwrap();
                        }).ToArray();

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                    log.Add(string.Format("Main thread call Release({0}) -->", max));

                    semaphore.Release(max);

                    // We have a race condition here.
                    // The ammount we see here depends on how many tasks has managed to enter before we read.
                    // Ideally we should see 0 too.
                    log.Add(string.Format("{0} tasks can enter the semaphore.", semaphore.CurrentCount));

                    Task.WaitAll(tasks, source.Token);

                    log.Add("Main thread exits");
                }
            }
            foreach (var entry in log)
            {
                Console.WriteLine(entry);
            }
        }
    }
}