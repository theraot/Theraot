using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Threading;
using ThreadState = System.Threading.ThreadState;

namespace TestRunner.System.Threading
{
    [TestFixture]
    [Category("Performance")]
    public static class ThreadTest
    {
        [Test]
        public static void CurrentThreadInTaskIsBackgroundOrRunning()
        {
            var found = default(ThreadState);
            TaskEx.Run(() => found = Thread.CurrentThread.ThreadState).Wait();
            Assert.IsTrue(found == ThreadState.Background || found == ThreadState.Running);
        }

        [Test]
        public static void CurrentThreadIsAlive()
        {
            Assert.IsTrue(Thread.CurrentThread.IsAlive);
        }

        [Test]
        public static void JoinNotStartedThrows()
        {
            var thread = new Thread(() => { });
            Assert.Throws<ThreadStateException>(thread.Join);
        }

        [Test]
        public static void NewThreadWithNullParametrizedThreadStartThrows()
        {
            const ParameterizedThreadStart ParameterizedStart = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException, Thread>(() => new Thread(ParameterizedStart));
        }

        [Test]
        public static void NewThreadWithNullThreadStartThrows()
        {
            const ThreadStart Start = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException, Thread>(() => new Thread(Start));
        }

        [Test]
        public static void NewThreadWithParametrizedThreadStartRespectsParameter()
        {
            object found = null;
            var thread = new Thread(param => found = param);
            var sent = new object();
            thread.Start(sent);
            thread.Join();
            Assert.AreEqual(sent, found);
        }

        [Test]
        public static void NewThreadWithParametrizedThreadStartRuns()
        {
            var value = 0;
            var thread = new Thread(_ => value = 1);
            thread.Start(new object());
            thread.Join();
            Assert.AreEqual(1, value);
        }

        [Test]
        public static void NewThreadWithThreadStartRuns()
        {
            var value = 0;
            var thread = new Thread(() => value = 1);
            thread.Start();
            thread.Join();
            Assert.AreEqual(1, value);
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
        public static void SleepDurationIsAtLeastMillisecondsTimeout
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))]
            int millisecondsTimeout
        )
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Thread.Sleep(millisecondsTimeout);
            Assert.IsTrue(stopWatch.Elapsed.TotalMilliseconds > millisecondsTimeout);
        }

        [Test]
        public static void StartingAnStartedThreadThrows()
        {
            var thread = new Thread(() => { });
            thread.Start();
            Assert.Throws<ThreadStateException>(thread.Start);
        }

        [Test]
        public static void StartingATaskThreadThrows()
        {
            Thread thread = null;
            TaskEx.Run(() => thread = Thread.CurrentThread).Wait();
            Assert.Throws<ThreadStateException>(thread.Start);
        }

        [Test]
        public static void StartingCurrentThreadThrows()
        {
            var thread = Thread.CurrentThread;
            Assert.Throws<ThreadStateException>(thread.Start);
        }

        [Test]
        public static void TaskThreadIsBackgroundOrRunningAfterWait()
        {
            Thread thread = null;
            TaskEx.Run(() => thread = Thread.CurrentThread).Wait();
            Assert.IsTrue(thread.ThreadState == ThreadState.Background || thread.ThreadState == ThreadState.Running);
        }

        [Test]
        public static void TaskThreadIsBackgroundWhileLooping()
        {
            var signal = new[] { 0 };
            Thread thread = null;
            TaskEx.Run
            (
                () =>
                {
                    thread = Thread.CurrentThread;
                    var spinWait = new SpinWait();
                    while (Volatile.Read(ref signal[0]) == 0)
                    {
                        spinWait.SpinOnce();
                    }
                }

            );
            ThreadingHelper.SpinWaitWhileNull(ref thread);
            Assert.IsTrue((thread.ThreadState & ThreadState.Background) != 0);
            ThreadingHelper.MemoryBarrier();
            Volatile.Write(ref signal[0], 1);
        }

        [Test]
        public static void TaskThreadIsBackgroundWhileWaiting()
        {
            var manualResetEvent = new ManualResetEvent[1];
            using (manualResetEvent[0] = new ManualResetEvent(false))
            {
                Thread thread = null;
                var task = TaskEx.Run
                (
                    () =>
                    {
                        thread = Thread.CurrentThread;
                        manualResetEvent[0].WaitOne();
                    }

                );
                ThreadingHelper.SpinWaitWhileNull(ref thread);
                Assert.IsTrue((thread.ThreadState & ThreadState.Background) != 0);
                manualResetEvent[0].Set();
                task.Wait();
            }
        }

        [Test]
        public static void ThreadIsRunningWhileLooping()
        {
            var signal = new[] { 0 };
            var thread = new Thread
            (
                () =>
                {
                    var spinWait = new SpinWait();
                    while (Volatile.Read(ref signal[0]) == 0)
                    {
                        spinWait.SpinOnce();
                    }
                }

            );
            thread.Start();
            Assert.AreEqual(ThreadState.Running, thread.ThreadState);
            Volatile.Write(ref signal[0], 1);
        }

        [Test]
        public static void ThreadIsRunningWhileWaiting()
        {
            var manualResetEvent = new ManualResetEvent[1];
            using (manualResetEvent[0] = new ManualResetEvent(false))
            {
                var thread = new Thread(() => manualResetEvent[0].WaitOne());
                thread.Start();
                Assert.AreEqual(ThreadState.Running, thread.ThreadState);
                manualResetEvent[0].Set();
                thread.Join();
            }
        }

        [Test]
        public static void ThreadIsStoppedAfterJoin()
        {
            var thread = new Thread(() => { });
            thread.Start();
            thread.Join();
            Assert.AreEqual(ThreadState.Stopped, thread.ThreadState);
        }

        [Test]
        public static void ThreadIsUnstartedBeforeStart()
        {
            var thread = new Thread(() => { });
            Assert.AreEqual(ThreadState.Unstarted, thread.ThreadState);
        }

        [Test(IsolatedThread = true)]
        public static void ThreadNameCanBeNull()
        {
            var thread = Thread.CurrentThread;
            thread.Name = null;
            Assert.IsNull(thread.Name);
        }

        [Test(IsolatedThread = true)]
        public static void ThreadNameCanBeSet(string name)
        {
            var thread = Thread.CurrentThread;
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
        }

        [Test(IsolatedThread = true)]
        public static void ThreadNameCanBeSetAfterBeingNull(string name)
        {
            var thread = Thread.CurrentThread;
            thread.Name = null;
            Assert.IsNull(thread.Name);
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
        }

        [Test(IsolatedThread = true)]
        public static void ThreadNameCannotBeSetAfterNotNull(string name, string secondName)
        {
            var thread = Thread.CurrentThread;
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
            Assert.Throws<InvalidOperationException>(() => thread.Name = secondName);
        }

        [Test(IsolatedThread = true)]
        public static void ThreadNameCannotBeSetToNullAfterNotNull(string name)
        {
            var thread = Thread.CurrentThread;
            thread.Name = name;
            Assert.AreEqual(name, thread.Name);
            Assert.Throws<InvalidOperationException>(() => thread.Name = null);
        }
    }
}