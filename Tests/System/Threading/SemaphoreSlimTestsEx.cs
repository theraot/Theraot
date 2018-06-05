using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.System.Threading
{
    [TestFixtureAttribute]
    public static class SemaphoreSlimTestsEx
    {
        [Test]
        public static void WaitAsyncWaitCorrectly()
        {
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(100)))
            {
                using (var semaphore = new SemaphoreSlim(0, 3))
                {
                    Debug.WriteLine(string.Format("{0} task can enter the semaphore.", semaphore.CurrentCount));
                    var padding = 0;
                    var tasks = Enumerable.Range(0, 4)
                        .Select(_ =>
                        {
                            return Task.Factory.StartNew(async () =>
                            {
                                var CurrentId = Task.CurrentId;
                                Debug.WriteLine(string.Format("Task {0} begins and waits for the semaphore.", CurrentId));

                                await semaphore.WaitAsync(source.Token);

                                Interlocked.Add(ref padding, 100);

                                Debug.WriteLine(string.Format("Task {0} enters the semaphore.", CurrentId));

                                Thread.Sleep(1000 + padding);

                                Debug.WriteLine(string.Format("Task {0} release the semaphore; previous count: {1} ", CurrentId, semaphore.Release()));
                            }).Unwrap();
                        }).ToArray();

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                    Debug.WriteLine("Main thread call Release(3) -->");

                    semaphore.Release(3);

                    Debug.WriteLine(string.Format("{0} tasks can enter the semaphore.", semaphore.CurrentCount));

                    Task.WaitAll(tasks, source.Token);

                    Debug.WriteLine("Main thread exits");
                }
            }
        }
    }
}