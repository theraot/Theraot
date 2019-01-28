using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestRunner.System.Threading
{
    [TestFixture]
    internal static class TaskPerfTest
    {
        [Test]
        [Category("Performance")]
        public static async Task RunAsync()
        {
            var tasks = Enumerable.Range(0, 10).Select
            (
                _ =>
                TaskEx.Run
                (
                    async () =>
                    {
                        var guid = Guid.NewGuid();
                        var id1 = Thread.CurrentThread.ManagedThreadId;
                        Console.WriteLine($"enter {id1}");
                        await TaskEx.Delay(5 * 1000).ConfigureAwait(false);
                        var id2 = Thread.CurrentThread.ManagedThreadId;
                        await TaskEx.Delay(5 * 1000).ConfigureAwait(false);
                        var id3 = Thread.CurrentThread.ManagedThreadId;
                        var result = new { guid, id1, id2, id3 };
                        Console.WriteLine(result);
                        return result;
                    }
                )
            ).ToArray();
            GC.KeepAlive(await TaskEx.WhenAll(tasks).ConfigureAwait(false));
        }
    }
}