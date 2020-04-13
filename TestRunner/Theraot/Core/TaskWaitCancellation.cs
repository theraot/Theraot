#pragma warning disable AsyncFixer04 // A disposable object used in a fire & forget async call

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestRunner.Theraot.Core
{
    [TestFixture]
    public static class TaskWaitCancellation
    {
        [Test]
        public static void TestWaitForCancellation()
        {
            using (var cts = new CancellationTokenSource())
            {
                var task = CreateTask(cts);
                cts.CancelAfter(10);
                // ReSharper disable once MethodSupportsCancellation
                task.Wait(); // never completes
            }
        }

        private static Task CreateTask(CancellationTokenSource cts)
        {
            return TaskEx.Run
            (
                async () =>
                {
                    try
                    {
                        await TaskEx.Delay(100, cts.Token).ConfigureAwait(true);
                    }
                    catch (OperationCanceledException e)
                    {
                        _ = e;
                    }
                },
                cts.Token
            );
        }
    }
}