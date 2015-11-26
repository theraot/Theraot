//
// TaskTest.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
// Copyright (C) 2011 Xamarin Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public class MinimalTaskTests
    {
        [Test]
        public void CancelBeforeStart()
        {
            var src = new CancellationTokenSource();

            Task t = new Task(delegate { }, src.Token);
            src.Cancel();
            Assert.AreEqual(TaskStatus.Canceled, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public void Wait_Inlined()
        {
            bool? previouslyQueued = null;

            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (task, b) => {
                previouslyQueued = b;
            };

            var tf = new TaskFactory(scheduler);
            var t = tf.StartNew(() => { });
            t.Wait();

            Assert.AreEqual(true, previouslyQueued);
        }

        [Test]
        public void MultipleTasks()
        {
            ParallelTestHelper.Repeat(delegate {
                bool r1 = false, r2 = false, r3 = false;

                Task t1 = Task.Factory.StartNew(delegate {
                    r1 = true;
                });
                Task t2 = Task.Factory.StartNew(delegate {
                    r2 = true;
                });
                Task t3 = Task.Factory.StartNew(delegate {
                    r3 = true;
                });

                t1.Wait(2000);
                t2.Wait(2000);
                t3.Wait(2000);

                Assert.IsTrue(r1, "#1");
                Assert.IsTrue(r2, "#2");
                Assert.IsTrue(r3, "#3");
            }, 100);
        }

        [Test]
        public void WaitChildTestCase()
        {
            ParallelTestHelper.Repeat(delegate {
                bool r1 = false, r2 = false, r3 = false;
                var mre = new ManualResetEventSlim(false);
                var mreStart = new ManualResetEventSlim(false);

                Task t = Task.Factory.StartNew(delegate {
                    Task.Factory.StartNew(delegate {
                        mre.Wait(300);
                        r1 = true;
                    }, TaskCreationOptions.AttachedToParent);
                    Task.Factory.StartNew(delegate {
                        r2 = true;
                    }, TaskCreationOptions.AttachedToParent);
                    Task.Factory.StartNew(delegate {
                        r3 = true;
                    }, TaskCreationOptions.AttachedToParent);
                    mreStart.Set();
                });

                mreStart.Wait(300);
                Assert.IsFalse(t.Wait(10), "#0a");
                mre.Set();
                Assert.IsTrue(t.Wait(500), "#0b");
                Assert.IsTrue(r2, "#1");
                Assert.IsTrue(r3, "#2");
                Assert.IsTrue(r1, "#3");
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#4");
            }, 10);
        }

        [Test]
        public void WaitChildWithNesting()
        {
            var result = false;
            var t = Task.Factory.StartNew(() => {
                Task.Factory.StartNew(() => {
                    Task.Factory.StartNew(() => {
                        Thread.Sleep(500);
                        result = true;
                    }, TaskCreationOptions.AttachedToParent);
                }, TaskCreationOptions.AttachedToParent);
            });
            Assert.IsTrue(t.Wait(4000), "#1");
            Assert.IsTrue(result, "#2");
        }

        [Test]
        public void DoubleWaitTest()
        {
            ParallelTestHelper.Repeat(delegate {
                var evt = new ManualResetEventSlim();
                var monitor = new object();
                int finished = 0;
                var t = Task.Factory.StartNew(delegate {
                    var r = evt.Wait(5000);
                    lock (monitor)
                    {
                        finished++;
                        Monitor.Pulse(monitor);
                    }
                    return r ? 1 : 10; //1 -> ok, 10 -> evt wait failed
                });
                var cntd = new CountdownEvent(2);
                var cntd2 = new CountdownEvent(2);

                int r1 = 0, r2 = 0;
                ThreadPool.QueueUserWorkItem(delegate {
                    cntd.Signal();
                    if (!t.Wait(1000))
                        r1 = 20; // 20 -> task wait failed
                    else if (t.Result != 1)
                        r1 = 30 + t.Result; // 30 -> task result is bad
                    else
                        r1 = 2; //2 -> ok
                    cntd2.Signal();
                    lock (monitor)
                    {
                        finished++;
                        Monitor.Pulse(monitor);
                    }
                });
                ThreadPool.QueueUserWorkItem(delegate {
                    cntd.Signal();
                    if (!t.Wait(1000))
                        r2 = 40; // 40 -> task wait failed
                    else if (t.Result != 1)
                        r2 = 50 + t.Result; // 50 -> task result is bad
                    else
                        r2 = 3; //3 -> ok

                    cntd2.Signal();
                    lock (monitor)
                    {
                        finished++;
                        Monitor.Pulse(monitor);
                    }
                });
                Assert.IsTrue(cntd.Wait(2000), "#1");
                evt.Set();
                Assert.IsTrue(cntd2.Wait(2000), "#2");
                Assert.AreEqual(2, r1, "r1");
                Assert.AreEqual(3, r2, "r2");

                // Wait for everything to finish to avoid overloading the tpool
                lock (monitor)
                {
                    while (true)
                    {
                        if (finished == 3)
                            break;
                        else
                            Monitor.Wait(monitor);
                    }
                }
            }, 10);
        }

        [Test]
        public void DoubleTimeoutedWaitTest()
        {
            var evt = new ManualResetEventSlim();
            var t = new Task(delegate { });
            var cntd = new CountdownEvent(2);

            bool r1 = false, r2 = false;
            ThreadPool.QueueUserWorkItem(delegate { r1 = !t.Wait(100); cntd.Signal(); });
            ThreadPool.QueueUserWorkItem(delegate { r2 = !t.Wait(100); cntd.Signal(); });

            cntd.Wait(2000);
            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
        }

        [Test]
        public void RunSynchronously()
        {
            var val = 0;
            Task t = new Task(() => { Thread.Sleep(100); val = 1; });
            t.RunSynchronously();

            Assert.AreEqual(1, val, "#1");

            t = new Task(() => { Thread.Sleep(0); val = 2; });

            bool? previouslyQueued = null;

            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (task, b) => {
                previouslyQueued = b;
            };

            t.RunSynchronously(scheduler);

            Assert.AreEqual(2, val, "#2");
            Assert.AreEqual(false, previouslyQueued, "#2a");
        }

        [Test]
        public void RunSynchronouslyArgumentChecks()
        {
            Task t = new Task(() => { });
            try
            {
                t.RunSynchronously(null);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [Test]
        public void RunSynchronously_SchedulerException()
        {
            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (task, b) => {
                throw new ApplicationException();
            };

            Task t = new Task(() => { });
            try
            {
                t.RunSynchronously(scheduler);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(t.Exception.InnerException, e);
            }
        }

        [Test]
        public void RunSynchronouslyWithAttachedChildren()
        {
            var result = false;
            var t = new Task(() => {
                Task.Factory.StartNew(() => { Thread.Sleep(500); result = true; }, TaskCreationOptions.AttachedToParent);
            });
            t.RunSynchronously();
            Assert.IsTrue(result);
        }

        [Test]
        public void UnobservedExceptionOnFinalizerThreadTest()
        {
            bool wasCalled = false;
            TaskScheduler.UnobservedTaskException += (o, args) => {
                wasCalled = true;
                args.SetObserved();
            };
            var inner = new ApplicationException();
            Thread t = new Thread(delegate () {
                Task.Factory.StartNew(() => { throw inner; });
            });
            t.Start();
            t.Join();
            Thread.Sleep(1000);
            GC.Collect();
            Thread.Sleep(1000);
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(wasCalled);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void StartFinishedTaskTest()
        {
            var t = Task.Factory.StartNew(delegate () { });
            t.Wait();

            t.Start();
        }

        [Test]
        public void Start_NullArgument()
        {
            var t = new Task(() => { });
            try
            {
                t.Start(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DisposeUnstartedTest()
        {
            var t = new Task(() => { });
            t.Dispose();
        }

        [Test]
        public void WhenChildTaskErrorIsThrownParentTaskShouldBeFaulted()
        {
            Task innerTask = null;
            var testTask = new Task(() =>
            {
                innerTask = new Task(() =>
                {
                    throw new InvalidOperationException();
                }, TaskCreationOptions.AttachedToParent);
                innerTask.RunSynchronously();
            });
            testTask.RunSynchronously();

            Assert.AreNotEqual(TaskStatus.Running, testTask.Status);
            Assert.IsNotNull(innerTask);
            Assert.IsTrue(innerTask.IsFaulted);
            Assert.IsNotNull(testTask.Exception);
            Assert.IsTrue(testTask.IsFaulted);
            Assert.IsNotNull(innerTask.Exception);
        }

        [Test]
        public void LongRunning()
        {
            var is_tp = false;
            var is_bg = false;
            Action action =
                () =>
                {
                    is_tp = Thread.CurrentThread.IsThreadPoolThread;
                    is_bg = Thread.CurrentThread.IsBackground;
                };
            var t = new Task(action);
            t.Start();
            Thread.Sleep(1); // Preventing the task to be executed form Wait
            Assert.IsTrue(t.Wait(5000), "#0");
            Assert.IsTrue(is_tp, "#1");
            Assert.IsTrue(is_bg, "#2");

            is_tp = true;
            is_bg = false;
            t = new Task(action, TaskCreationOptions.LongRunning);
            t.Start();
            Assert.IsTrue(t.Wait(5000), "#10");
            Assert.IsFalse(is_tp, "#11");
            Assert.IsTrue(is_bg, "#12");
        }

#if !NET40
        [Test]
        public void Run_ArgumentCheck()
        {
            try
            {
                Task.Run(null as Action);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [Test]
        public void Run()
        {
            var expectedScheduler = TaskScheduler.Current;
            TaskScheduler foundScheduler = null;
            var t = Task.Run(() =>
            {
                foundScheduler = TaskScheduler.Current;
                Console.WriteLine("Task Scheduler: {0}", TaskScheduler.Current);
                Console.WriteLine("IsThreadPoolThread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            });
            Assert.AreEqual(TaskCreationOptions.DenyChildAttach, t.CreationOptions, "#1");
            t.Wait();
            Assert.AreEqual(expectedScheduler, foundScheduler);
        }

        [Test]
        public void Run_Cancel()
        {
            var t = Task.Run(() => 1, new CancellationToken(true));
            try
            {
                var r = t.Result;
                Assert.Fail("#1");
            }
            catch (AggregateException)
            {
            }

            Assert.IsTrue(t.IsCanceled, "#2");
        }

        [Test]
        public void Run_ExistingTaskT()
        {
            var t = new Task<int>(() => 5);
            var t2 = Task.Run(() => { t.Start(); return t; });

            Assert.IsTrue(t2.Wait(1000), "#1");
            Assert.AreEqual(5, t2.Result, "#2");
        }

        [Test]
        public void Run_ExistingTask()
        {
            var t = new Task(delegate { throw new Exception("Foo"); });
            var t2 = Task.Run(() => { t.Start(); return t; });

            try
            {
                t2.Wait(1000);
                Assert.Fail();
            }
            catch (Exception) { }

            Assert.AreEqual(TaskStatus.Faulted, t.Status, "#2");
        }

        [Test]
        public void DenyChildAttachTest()
        {
            var mre = new ManualResetEventSlim();
            Task nested = null;
            Action innerAction = () =>
            {
                mre.Wait(2000);
            };
            Action outerAction = () =>
            {
                nested = Task.Factory.StartNew
                (
                    innerAction,
                    TaskCreationOptions.AttachedToParent
                );
            };
            Task parent = Task.Factory.StartNew
                (
                    outerAction,
                    TaskCreationOptions.DenyChildAttach
                );
            Assert.IsTrue(parent.Wait(10000), "#1");
            mre.Set();
            Assert.IsTrue(nested.Wait(2000), "#2");
        }
#endif

        class SynchronousScheduler : TaskScheduler
        {
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new NotImplementedException();
            }

            protected override void QueueTask(Task task)
            {
                TryExecuteTaskInline(task, false);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return base.TryExecuteTask(task);
            }
        }

#if !NET40
        [Test]
        public void HideSchedulerTest()
        {
            var mre = new ManualResetEventSlim();
            var ranOnDefault = false;
            var scheduler = new SynchronousScheduler();

            Task parent = Task.Factory.StartNew(() => {
                Task.Factory.StartNew(() => {
                    ranOnDefault = Thread.CurrentThread.IsThreadPoolThread;
                    mre.Set();
                });
            }, CancellationToken.None, TaskCreationOptions.HideScheduler, scheduler);

            Assert.IsTrue(mre.Wait(1000), "#1");
            Assert.IsTrue(ranOnDefault, "#2");
        }
#endif
    }
}