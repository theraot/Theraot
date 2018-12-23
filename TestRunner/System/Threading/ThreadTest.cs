using System;
using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class ThreadTest
    {
        [Test(IsolatedThread = true)]
        public static void NameCurrentThread(string name, string secondName)
        {
            var thread = Thread.CurrentThread;
            thread.Name = null;
            Assert.IsNull(thread.Name);
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
            Assert.Throws<InvalidOperationException>(() => { thread.Name = null; });
            Assert.Throws<InvalidOperationException>(() => { thread.Name = secondName; });
            Assert.IsTrue(thread.IsAlive);
        }

        [Test]
        public static void NewThreadRuns()
        {
            var value = 0;
            var thread = new Thread(() => value = 1);
            thread.Start();
            thread.Join();
            Assert.AreEqual(1, value);
        }

        [Test]
        public static void ParametrizedThreadStart()
        {
            object found = null;
            var thread = new Thread(param => found = param);
            var sent = new object();
            thread.Start(sent);
            thread.Join();
            Assert.AreEqual(sent, found);
        }

        [Test]
        public static void NewThreadNullDelegate()
        {
            ThreadStart start = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(new Thread(start)));
            ParameterizedThreadStart parameterizedStart = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(new Thread(parameterizedStart)));
        }
    }
}
