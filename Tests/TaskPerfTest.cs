using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal static class TaskPerfTest
    {
#if !NET40

        [Test]
        [Category("Performance")]
        public static async Task RunAsync()
        {
            ThreadPool.SetMaxThreads(10, 10);
            var tasks = Enumerable.Range(0, 10).Select(x =>
              Task.Run(async () =>
              {
                  var guid = Guid.NewGuid();
                  var id1 = Thread.CurrentThread.ManagedThreadId;
                  Console.WriteLine("enter {0}", id1);
                  await Task.Delay(5 * 1000);
                  var id2 = Thread.CurrentThread.ManagedThreadId;
                  await Task.Delay(5 * 1000);
                  var id3 = Thread.CurrentThread.ManagedThreadId;
                  var result = new { guid, id1, id2, id3 };
                  Console.WriteLine(result);
                  return result;
              })).ToArray();
            var allTask = await Task.WhenAll(tasks);
        }

#endif
    }
}