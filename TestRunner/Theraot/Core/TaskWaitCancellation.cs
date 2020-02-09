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