using System;
using System.Diagnostics;
using System.Threading;
using Theraot.Threading;

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
        public static void NewThreadNullDelegate()
        {
            ThreadStart start = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(new Thread(start)));
            ParameterizedThreadStart parameterizedStart = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(new Thread(parameterizedStart)));
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
        public static void SimpleSync()
        {
            var control = new int[3];
            var thread = new Thread
            (
                () =>
                {
                    var spinWait = new SpinWait();
                    Volatile.Write(ref control[0], 1);
                    while (Volatile.Read(ref control[1]) == 0)
                    {
                        spinWait.SpinOnce();
                    }
                    Volatile.Write(ref control[2], 1);
                }
            );
            thread.Start();
            ThreadingHelper.SpinWaitUntil(ref control[0], 1);
            Volatile.Write(ref control[1], 1);
            thread.Join();
            Assert.AreEqual(1, Volatile.Read(ref control[2]));
        }

        [Test]
        public static void SleepSleeps()
        {
            Assert.IsTrue(Thread.CurrentThread.IsAlive);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Thread.Sleep(1000);
            Assert.IsTrue(stopWatch.Elapsed.TotalMilliseconds > 1000);
            Assert.IsTrue(Thread.CurrentThread.IsAlive);
        }
    }
}