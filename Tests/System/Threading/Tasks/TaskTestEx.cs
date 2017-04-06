using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public class TaskTestsEx
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StartCancelled()
        {
            var cancellation = new CancellationToken(true);
            using (var task = new Task(() =>
            {
            }, cancellation))
            {
                task.Start();
            }
        }
    }
}