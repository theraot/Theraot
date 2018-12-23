using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class ThreadTest
    {
        [Test]
        public static void NameCurrentThread(string name)
        {
            var thread = Thread.CurrentThread;
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
            Assert.IsTrue(thread.IsAlive);
        }
    }
}
