using System;
using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class ThreadTest
    {
        [Test]
        public static void NameCurrentThread(string name, string secondName)
        {
            var thread = Thread.CurrentThread;
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
            Assert.Throws<InvalidOperationException>(() => { thread.Name = secondName;});
            Assert.IsTrue(thread.IsAlive);
        }
    }
}
