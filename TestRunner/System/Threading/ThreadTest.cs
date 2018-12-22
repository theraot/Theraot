using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class ThreadTest
    {
        [Test]
        public static void NameCurrentThread()
        {
            var thread = Thread.CurrentThread;
            thread.Name = "threadName";
            Assert.AreEqual("threadName", thread.Name);
            Assert.IsTrue(thread.IsAlive);
        }
    }
}
