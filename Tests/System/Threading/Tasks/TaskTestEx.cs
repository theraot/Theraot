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
        public void StartCancelled()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    var cancellation = new CancellationToken(true);
                    var task = new Task(() => { }, cancellation);
                    task.Start();
                }
            );
        }
    }
}