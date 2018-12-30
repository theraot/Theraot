using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class TaskExFromTest
    {
        [Test]
        public static void TaskExFromCanceledThrowsOnNonCancelledToken()
        {
            Assert.Throws<ArgumentOutOfRangeException, Task>(() => TaskEx.FromCanceled(CancellationToken.None));
        }
    }
}