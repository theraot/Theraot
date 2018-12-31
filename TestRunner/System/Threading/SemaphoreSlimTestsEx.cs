using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class SemaphoreSlimTestsEx
    {
        [Test]
        [Category("Performance")]
        public static void LongWait()
        {
            var semaphore1 = new SemaphoreSlim(0);
            var semaphore2 = new SemaphoreSlim(0);
            var semaphore3 = new SemaphoreSlim(0);
            var thread = new Thread
            (
                () =>
                {
                    Thread.Sleep(5000);
                    semaphore1.Release();
                    Thread.Sleep(5000);
                    semaphore2.Release();
                }
            );
            thread.Start();
            semaphore1.Wait();
            var source1 = new CancellationTokenSource();
            source1.CancelAfter(10000);
            semaphore2.Wait(source1.Token);
            var source2 = new CancellationTokenSource();
            source2.CancelAfter(10);
            try
            {
                semaphore3.Wait(source2.Token);
            }
            catch (OperationCanceledException exception)
            {
                GC.KeepAlive(exception);
                return;
            }

            Assert.Fail();
        }

        [Test]
        [Category("Performance")]
        public static void WaitAsyncWaitCorrectly()
        {
            for (var count = 0; count < 10; count++)
            {
                int maxTasks = 3 + count;
                var spinWait = new SpinWait();
                while (!WaitAsyncWaitCorrectlyExtractedExtracted(3, maxTasks))
                {
                    spinWait.SpinOnce();
                }
            }
        }

        private static bool WaitAsyncWaitCorrectlyExtractedExtracted(int maxCount, int maxTasks)
        {
            // Note: if WaitAsync takes to long, "x" can happen before the chunk of "a" has completed.
            var log = new CircularBucket<string>(maxTasks * 4 + 2);
            var logCount = new CircularBucket<int>(maxTasks * 2 + 2);
            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromSeconds(100));
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
                                // ReSharper disable once MethodSupportsCancellation
                                return Task.Factory.StartNew
                                (
                                    async () =>
                                    {
                                        log.Add("a");
                                        // ReSharper disable once AccessToDisposedClosure
                                        await semaphore.WaitAsync
                                        (
                                            // ReSharper disable once AccessToDisposedClosure
                                            source.Token
                                        );
                                        Interlocked.Add(ref padding, 100);
                                        logCount.Add(-1);
                                        log.Add("b");
                                        Thread.Sleep(1000 + padding);
                                        // Calling release should give increasing results per chunk
                                        log.Add("c");
                                        // ReSharper disable once AccessToDisposedClosure
                                        var count = semaphore.Release();
                                        logCount.Add(count);
                                        log.Add("d");
                                    }
                                ).Unwrap();
                            }
                        ).ToArray();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    log.Add("x");
                    var tmp = semaphore.Release(maxCount);
                    logCount.Add(-1);
                    logCount.Add(tmp);
                    Task.WaitAll(tasks, source.Token);
                    log.Add("z");
                }
            }

            // We should see:
            // maxTask a
            // 1 x
            // chunks of at most maxCount b, separated by chunks of c
            var sb = new StringBuilder(log.Capacity);
            foreach (var entry in log)
            {
                sb.Append(entry);
            }

            var str = sb.ToString();
            Console.WriteLine(str);
            // Make sure that threads have not sneaked in the ordering
            // If this has happen, it would have been a false failure
            // So, we will retry until it does not happen
            if (new Regex("c[bc]+d").IsMatch(str))
            {
                Console.WriteLine("...");
                return false;
            }

            var regexSuccess = $"a{{{maxTasks}}}x(b{{0,{maxCount}}}(cd)+)+z";
            Assert.IsTrue(new Regex(regexSuccess).IsMatch(str));
            // The results of release increase *per chunk of c*.
            var last = -1;
            var first = true;
            foreach (var entry in logCount)
            {
                Console.WriteLine(entry.ToString());
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

            return true;
        }
    }
}