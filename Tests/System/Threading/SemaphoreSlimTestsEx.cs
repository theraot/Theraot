using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.System.Threading
{
    [TestFixtureAttribute]
    public class SemaphoreSlimTestsEx
    {
        [Test]
        public static void WaitAsyncWaitCorrectly()
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(100));
            var semaphore = new SemaphoreSlim(0, 3);
            Debug.WriteLine(string.Format("{0} task can enter the semaphore.", semaphore.CurrentCount));
            int padding = 0;
            Task[] tasks = Enumerable.Range(0, 4)
                .Select(index =>
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