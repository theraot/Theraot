using System;
using System.Threading;

namespace Tests.System.Threading
{
    internal class SemaphoreSlimTestsEx
    {
        public static void WaitAsyncWaitCorrectly()
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(100));
            var semaphore = new SemaphoreSlim(0, 3);
            System.Diagnostics.Debug.WriteLine($"{semaphore.CurrentCount} task can enter the semaphore.");
            int padding = 0;
            Task[] tasks = Enumerable.Range(0, 4)
                .Select(index =>
                {
                    return Task.Factory.StartNew(async () =>
                    {
                        var CurrentId = Task.CurrentId;
                        System.Diagnostics.Debug.WriteLine($"Task {CurrentId} begins and waits for the semaphore.");

                        await semaphore.WaitAsync(source.Token);

                        Interlocked.Add(ref padding, 100);

                        System.Diagnostics.Debug.WriteLine($"Task {CurrentId} enters the semaphore.");

                        Thread.Sleep(1000 + padding);

                        System.Diagnostics.Debug.WriteLine($"Task {CurrentId} release the semaphore; previous count: {semaphore.Release()} ");
                    }).Unwrap();
                }).ToArray();

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            System.Diagnostics.Debug.WriteLine($"Main thread call Release(3) -->");

            semaphore.Release(3);

            System.Diagnostics.Debug.WriteLine($"{semaphore.CurrentCount} tasks can enter the semaphore.");

            Task.WaitAll(tasks, source.Token);

            System.Diagnostics.Debug.WriteLine("Main thread exits");
        }
    }
}