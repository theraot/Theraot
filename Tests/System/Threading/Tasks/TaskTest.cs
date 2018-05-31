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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Core;

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public partial class TaskTests
    {
        private const int _max = 6;

        private readonly object _cleanupMutex = new object();

        private int _completionPortThreads;

        private Task _parentWfc;

        private Task[] _tasks;

        private int _workerThreads;

        public static void WaitAny_ManyExceptions() // TODO: Review
        {
            using (var cde = new CountdownEvent(3))
            {
                var tasks = new[] {
                    Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } }),
                    Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } }),
                    Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } })
                };

                Assert.IsTrue(cde.Wait(1000), "#1");

                try
                {
                    Assert.IsTrue(Task.WaitAll(tasks, 1000), "#2");
                }
                catch (AggregateException e)
                {
                    Assert.AreEqual(3, e.InnerExceptions.Count, "#3");
                }
            }
        }

        [Test]
        public void AlreadyCompletedChildTaskShouldRunContinuationImmediately()
        {
            var result = "Failed";
            using
            (
                var testTask = new Task
                (
                    () =>
                    {
                        var child = new Task<string>
                        (
                            () => "Success",
                            TaskCreationOptions.AttachedToParent
                        );
                        child.RunSynchronously();
                        child.ContinueWith
                        (
                            x =>
                            {
                                Thread.Sleep(50);
                                result = x.Result;
                            },
                            TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted
                        );
                    }
                )
            )
            {
                testTask.RunSynchronously();

                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void AsyncWaitHandleSet()
        {
            var task = new TaskFactory().StartNew(() =>
            {
            });
            var ar = (IAsyncResult)task;
            Assert.IsFalse(ar.CompletedSynchronously, "#1");
            Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(5000), "#2");
        }

        [Test]
        public void CancelBeforeStart()
        {
            using (var src = new CancellationTokenSource())
            {
                using (var t = new Task(ActionHelper.GetNoopAction(), src.Token))
                {
                    src.Cancel();
                    Assert.AreEqual(TaskStatus.Canceled, t.Status, "#1");

                    try
                    {
                        t.Start();
                        Assert.Fail("#2");
                    }
                    catch (InvalidOperationException ex)
                    {
                        GC.KeepAlive(ex);
                    }
                }
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this fails if serialized
        [Category("ThreadPool")]
        public void CanceledContinuationExecuteSynchronouslyTest() // TODO: Review
        {
            using (var source = new CancellationTokenSource())
            {
                using (var evt = new ManualResetEventSlim())
                {
                    var token = source.Token;
                    var result = false;
                    var thrown = false;

                    var task = Task.Factory.StartNew(() => evt.Wait(100));
                    var cont = task.ContinueWith(t => result = true, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                    source.Cancel();
                    evt.Set();
                    task.Wait(100);
                    try
                    {
                        cont.Wait(100);
                    }
                    catch (Exception ex)
                    {
                        GC.KeepAlive(ex);
                        thrown = true;
                    }

                    Assert.IsTrue(task.IsCompleted);
                    Assert.IsTrue(cont.IsCanceled);
                    Assert.IsFalse(result);
                    Assert.IsTrue(thrown);
                }
            }
        }

        [Test]
        public void ContinueWith_CustomScheduleRejected()
        {
            var scheduler = new NonInlineableScheduler();
            var t = Task.Factory.StartNew(ActionHelper.GetNoopAction()).
                ContinueWith(r =>
                {
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, scheduler);

            Assert.IsTrue(t.Wait(5000));
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        public void ContinueWithChildren() // TODO: Review
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var result = false;

                    var t = Task.Factory.StartNew(() => Task.Factory.StartNew(() =>
                    {
                    }, TaskCreationOptions.AttachedToParent));

                    using (var mre = new ManualResetEvent(false))
                    {
                        t.ContinueWith(l =>
                        {
                            result = true;
                            mre.Set();
                        });
                        Assert.IsTrue(mre.WaitOne(1000), "#1");
                        Assert.IsTrue(result, "#2");
                    }
                },
                2
            );
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        [Ignore]
        public void ContinueWithDifferentOptionsAreCanceledTest() // TODO: Review
        {
            using (var mre = new ManualResetEventSlim())
            {
                var task = Task.Factory.StartNew(() => mre.Wait(200));
                var contFailed = task.ContinueWith(t =>
                {
                }, TaskContinuationOptions.OnlyOnFaulted);
                var contCanceled = task.ContinueWith(t =>
                {
                }, TaskContinuationOptions.OnlyOnCanceled);
                var contSuccess = task.ContinueWith(t =>
                {
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                mre.Set();
                contSuccess.Wait(100);

                Assert.IsTrue(contSuccess.IsCompleted);
                Assert.IsTrue(contFailed.IsCompleted);
                Assert.IsTrue(contCanceled.IsCompleted);
                Assert.IsFalse(contSuccess.IsCanceled);
                Assert.IsTrue(contFailed.IsCanceled);
                Assert.IsTrue(contCanceled.IsCanceled);
            }
        }

        [Test]
        public void ContinueWithInvalidArguments()
        {
            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            try
            {
                task.ContinueWith(null);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException e)
            {
                GC.KeepAlive(e);
            }

            try
            {
                task.ContinueWith(ActionHelper.GetNoopAction<Task>(), null);
                Assert.Fail("#2");
            }
            catch (ArgumentNullException e)
            {
                GC.KeepAlive(e);
            }

            try
            {
                task.ContinueWith(ActionHelper.GetNoopAction<Task>(), TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.NotOnCanceled);
                Assert.Fail("#3");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                task.ContinueWith(ActionHelper.GetNoopAction<Task>(), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion);
                Assert.Fail("#4");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void ContinueWithOnAbortedTestCase()
        {
            var result = false;
            var taskResult = false;

            using (var src = new CancellationTokenSource())
            {
                using (var t = new Task(() => taskResult = true, src.Token))
                {
                    var cont = t.ContinueWith(obj => result = true,
                        TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.ExecuteSynchronously);

                    src.Cancel();

                    Assert.AreEqual(TaskStatus.Canceled, t.Status, "#1a");
                    Assert.IsTrue(cont.IsCompleted, "#1b");
                    Assert.IsTrue(result, "#1c");

                    try
                    {
                        t.Start();
                        Assert.Fail("#2");
                    }
                    catch (InvalidOperationException ex)
                    {
                        GC.KeepAlive(ex);
                    }

                    Assert.IsTrue(cont.Wait(1000), "#3");

                    Assert.IsFalse(taskResult, "#4");

                    Assert.IsNull(cont.Exception, "#5");
                    Assert.AreEqual(TaskStatus.RanToCompletion, cont.Status, "#6");
                }
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void ContinueWithOnAnyTestCase()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var result = false;

                    var t = Task.Factory.StartNew(ActionHelper.GetNoopAction());
                    var cont = t.ContinueWith(obj => result = true, TaskContinuationOptions.None);
                    Assert.IsTrue(t.Wait(2000), "First wait, (status, {0})", t.Status);
                    Assert.IsTrue(cont.Wait(2000), "Cont wait, (result, {0}) (parent status, {2}) (status, {1})", result, cont.Status, t.Status);
                    Assert.IsNull(cont.Exception, "#1");
                    Assert.IsNotNull(cont, "#2");
                    Assert.IsTrue(result, "#3");
                }
            );
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void ContinueWithOnCompletedSuccessfullyTestCase()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var result = false;

                    var t = Task.Factory.StartNew(ActionHelper.GetNoopAction());
                    var cont = t.ContinueWith(obj => result = true, TaskContinuationOptions.OnlyOnRanToCompletion);
                    Assert.IsTrue(t.Wait(1000), "#4");
                    Assert.IsTrue(cont.Wait(1000), "#5");

                    Assert.IsNull(cont.Exception, "#1");
                    Assert.IsNotNull(cont, "#2");
                    Assert.IsTrue(result, "#3");
                }
            );
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        [Ignore]
        public void ContinueWithOnFailedTestCase()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var result = false;
                    var t = Task.Factory.StartNew(() =>
                    {
                        throw new Exception("foo");
                    });
                    var cont = t.ContinueWith(obj => result = true, TaskContinuationOptions.OnlyOnFaulted);
                    Assert.IsTrue(cont.Wait(1000), "#0");
                    Assert.IsNotNull(t.Exception, "#1");
                    Assert.IsNotNull(cont, "#2");
                    Assert.IsTrue(result, "#3");
                }
            );
        }

        [Test]
        public void ContinueWithWithStart()
        {
            // Do not dispose Task
            Task t = new Task<int>(() => 1);
            var u = t.ContinueWith(ActionHelper.GetNoopAction<Task>());
            try
            {
                u.Start();
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void CreationWhileInitiallyCanceled()
        {
            var token = new CancellationToken(true);
            using (var task = new Task(ActionHelper.GetNoopAction(), token))
            {
                try
                {
                    task.Start();
                    Assert.Fail("#1");
                }
                catch (InvalidOperationException ex)
                {
                    GC.KeepAlive(ex);
                }

                try
                {
                    task.Wait();
                    Assert.Fail("#2");
                }
                catch (AggregateException e)
                {
                    Assert.That(e.InnerException, Is.TypeOf(typeof(TaskCanceledException)), "#3");
                }

                Assert.IsTrue(task.IsCanceled, "#4");
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DisposeUnstartedTest()
        {
            var t = new Task(() =>
            {
            });
            t.Dispose();
        }

        [Test]
        public void DoubleTimeoutedWaitTest() // TODO: Review
        {
            // Do not dispose Task
            var t = new Task(ActionHelper.GetNoopAction());
            using (var cntd = new CountdownEvent(2))
            {
                var r1 = false;
                var r2 = false;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    r1 = !t.Wait(100);
                    cntd.Signal();
                });
                ThreadPool.QueueUserWorkItem(state =>
                {
                    r2 = !t.Wait(100);
                    cntd.Signal();
                });

                cntd.Wait(2000);
                Assert.IsTrue(r1);
                Assert.IsTrue(r2);
            }
        }

        [Test]
        public void InlineNotTrashingParentRelationship()
        {
            var r1 = false;
            var r2 = false;
            using
            (
                var t = new Task
                (
                    () =>
                    {
                        new Task(() => r1 = true,
                        TaskCreationOptions.AttachedToParent).RunSynchronously();
                        Task.Factory.StartNew
                        (
                            () =>
                            {
                                Thread.Sleep(100);
                                r2 = true;
                            },
                            TaskCreationOptions.AttachedToParent
                        );
                    }
                )
            )
            {
                t.RunSynchronously();

                Assert.IsTrue(r1);
                Assert.IsTrue(r2);
            }
        }

        [Test]
        public void LongRunning()
        {
            bool? isTp = null;
            bool? isBg = null;
            using (var t = new Task(() =>
             {
                 isTp = Thread.CurrentThread.IsThreadPoolThread;
                 isBg = Thread.CurrentThread.IsBackground;
             }))
            {
                t.Start();
                Assert.IsTrue(t.Wait(5000), "#0");
                Assert.IsTrue((bool)isTp, "#1");
                Assert.IsTrue((bool)isBg, "#2");
                isTp = null;
                isBg = null;
            }
            using (var t = new Task(() =>
                {
                    isTp = Thread.CurrentThread.IsThreadPoolThread;
                    isBg = Thread.CurrentThread.IsBackground;
                }, TaskCreationOptions.LongRunning))
            {
                t.Start();
                Assert.IsTrue(t.Wait(5000), "#10");
                Assert.IsFalse((bool)isTp, "#11");
                Assert.IsTrue((bool)isBg, "#12");
            }
        }

        [Test]
        public void MultipleTasks()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var r1 = false;
                    var r2 = false;
                    var r3 = false;

                    var t1 = Task.Factory.StartNew(() => r1 = true);
                    var t2 = Task.Factory.StartNew(() => r2 = true);
                    var t3 = Task.Factory.StartNew(() => r3 = true);

                    t1.Wait(2000);
                    t2.Wait(2000);
                    t3.Wait(2000);

                    Assert.IsTrue(r1, "#1");
                    Assert.IsTrue(r2, "#2");
                    Assert.IsTrue(r3, "#3");
                },
                100
            );
        }

        [Test]
        public void RunSynchronouslyArgumentChecks()
        {
            // Do not dispose Task
            var t = new Task(ActionHelper.GetNoopAction());
            try
            {
                t.RunSynchronously(null);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void RunSynchronouslyOnContinuation()
        {
            // Do not dispose Task
            Task t = new Task<int>(() => 1);
            var u = t.ContinueWith(ActionHelper.GetNoopAction<Task>());
            try
            {
                u.RunSynchronously();
                Assert.Fail("#1");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void RunSynchronouslyWithAttachedChildren()
        {
            var result = false;
            using
            (
                var t = new Task
                (
                    () =>
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(500);
                            result = true;
                        },
                        TaskCreationOptions.AttachedToParent);
                    }
                )
            )
            {
                t.RunSynchronously();
                Assert.IsTrue(result);
            }
        }

        [SetUp]
        public void Setup()
        {
            ThreadPool.GetMinThreads(out _workerThreads, out _completionPortThreads);
            ThreadPool.SetMinThreads(1, 1);

            _tasks = new Task[_max];
        }

        [Test]
        public void Start_NullArgument()
        {
            try
            {
                var t = new Task(ActionHelper.GetNoopAction());
                t.Start(null);
                // If we do not start the task, we should not dispose it... so, do not use using
                // We should have a NullArgumentException anyway, we are only calling Dispose to avoid a warning
                t.Dispose();
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void StartFinishedTaskTest()
        {
            var t = Task.Factory.StartNew(() =>
            {
            });
            t.Wait();

            t.Start();
        }

        [Test]
        public void StartOnBrokenScheduler()
        {
            using (var t = new Task(ActionHelper.GetNoopAction()))
            {
                try
                {
                    t.Start(new ExceptionScheduler());
                    Assert.Fail("#1");
                }
                catch (TaskSchedulerException e)
                {
                    Assert.AreEqual(TaskStatus.Faulted, t.Status, "#2");
                    Assert.AreSame(e, t.Exception.InnerException, "#3");
                    Assert.IsTrue(e.InnerException is ApplicationException, "#4");
                }
            }
        }

        [TearDown]
        public void Teardown()
        {
            ThreadPool.SetMinThreads(_workerThreads, _completionPortThreads);
            Cleanup(_tasks);
        }

        [Test]
        public void ThrowingUnrelatedCanceledExceptionTest()
        {
            using
            (
                var t = new Task
                (
                    () =>
                    {
                        throw new TaskCanceledException();
                    }
                )
            )
            {
                t.RunSynchronously();
                Assert.IsTrue(t.IsFaulted);
                Assert.IsFalse(t.IsCanceled);
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        public void UnobservedExceptionOnFinalizerThreadTest()
        {
            var wasCalled = false;
            TaskScheduler.UnobservedTaskException += (o, args) =>
            {
                wasCalled = true;
                args.SetObserved();
            };
            var inner = new ApplicationException();
            var t = new Thread(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    throw inner;
                });
            });
            t.Start();
            t.Join();
            Thread.Sleep(1000);
            GC.Collect();
            Thread.Sleep(1000);
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Wait_CancelledTask()
        {
            using (var src = new CancellationTokenSource())
            {
                var t = new Task(ActionHelper.GetNoopAction(), src.Token);
                src.Cancel();

                try
                {
                    t.Wait(1000);
                    Assert.Fail("#1");
                }
                catch (AggregateException e)
                {
                    var details = (TaskCanceledException)e.InnerException;
                    Assert.AreEqual(t, details.Task, "#1e");
                }

                try
                {
                    t.Wait();
                    Assert.Fail("#2");
                }
                catch (AggregateException e)
                {
                    var details = (TaskCanceledException)e.InnerException;
                    Assert.AreEqual(t, details.Task, "#2e");
                    Assert.IsNull(details.Task.Exception, "#2e2");
                }
            }
        }

        [Test]
        public void Wait_Inlined()
        {
            bool? previouslyQueued = null;

            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (task, b) => previouslyQueued = b;

            var tf = new TaskFactory(scheduler);
            var t = tf.StartNew(ActionHelper.GetNoopAction());
            t.Wait();

            Assert.AreEqual(true, previouslyQueued);
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        [Ignore]
        public void WaitAll_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                using (var taskA = new Task(cancelation.Cancel))
                {
                    using (var taskB = new Task(ActionHelper.GetNoopAction(), cancelation.Token))
                    {
                        var tasks = new[] { taskA, taskB };

                        tasks[0].Start();

                        try
                        {
                            Task.WaitAll(tasks);
                            Assert.Fail("#1");
                        }
                        catch (AggregateException e)
                        {
                            var inner = (TaskCanceledException)e.InnerException;
                            Assert.AreEqual(tasks[1], inner.Task, "#2");
                        }

                        Assert.IsTrue(tasks[0].IsCompleted, "#3");
                        Assert.IsTrue(tasks[1].IsCanceled, "#4");
                    }
                }
            }
        }

        [Test]
        public void WaitAll_ManyTasks()
        {
            for (var r = 0; r < 2000; ++r)
            {
                var tasks = new Task[60];

                for (var i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => Thread.Sleep(0));
                }
                Assert.IsTrue(Task.WaitAll(tasks, 5000));

                Cleanup(tasks);
            }
        }

        [Test]
        public void WaitAll_StartedUnderWait() // TODO: Review
        {
            using (var task1 = new Task(ActionHelper.GetNoopAction()))
            {
                ThreadPool.QueueUserWorkItem
                (
                    state =>
                    {
                        // Sleep little to let task to start and hit internal wait
                        Thread.Sleep(20);
                        task1.Start();
                    }
                );

                Assert.IsTrue(Task.WaitAll(new[] { task1 }, 1000), "#1");
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition, that when resolved sequentially will fail
        public void WaitAll_TimeoutWithExceptionsAfter() // TODO: Review
        {
            using (var cde = new CountdownEvent(2))
            {
                using (var mre = new ManualResetEvent(false))
                {
                    var tasks = new[] {
                        Task.Factory.StartNew (()=>Assert.IsTrue (mre.WaitOne (1500), "#0")),
                        Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } }),
                        Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } })
                    };

                    Assert.IsTrue(cde.Wait(1000), "#1");
                    Assert.IsFalse(Task.WaitAll(tasks, 1000), "#2");

                    mre.Set();

                    try
                    {
                        Task.WaitAll(tasks, 1000);
                        Assert.Fail("#4");
                    }
                    catch (AggregateException e)
                    {
                        Assert.AreEqual(2, e.InnerExceptions.Count, "#5");
                    }
                }
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition, that when resolved sequentially will be stuck
        public void WaitAll_TimeoutWithExceptionsBefore() // TODO: Review
        {
            using (var cde = new CountdownEvent(2))
            {
                using (var mre = new ManualResetEvent(false))
                {
                    var tasks = new[] {
                        Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } }),
                        Task.Factory.StartNew (()=>{ try { throw new ApplicationException (); } finally { cde.Signal (); } }),
                        Task.Factory.StartNew (()=>mre.WaitOne ()) // Keep lambda
                    };

                    Assert.IsTrue(cde.Wait(1000), "#1");
                    Assert.IsFalse(Task.WaitAll(tasks, 1000), "#2");

                    mre.Set();

                    try
                    {
                        Assert.IsTrue(Task.WaitAll(tasks, 1000), "#3");
                        Assert.Fail("#4");
                    }
                    catch (AggregateException e)
                    {
                        Assert.AreEqual(2, e.InnerExceptions.Count, "#5");
                    }
                }
            }
        }

        [Test]
        public void WaitAll_WithExceptions()
        {
            InitWithDelegate
            (
                () =>
                {
                    throw new ApplicationException();
                }
            );

            try
            {
                Task.WaitAll(_tasks);
                Assert.Fail("#1");
            }
            catch (AggregateException e)
            {
                Assert.AreEqual(6, e.InnerExceptions.Count, "#2");
            }

            Assert.IsNotNull(_tasks[0].Exception, "#3");
        }

        [Test]
        public void WaitAll_Zero()
        {
            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            Assert.IsFalse(Task.WaitAll(new[] { task }, 0), "#0");
            // Do not dispose Task
            task = new Task(ActionHelper.GetNoopAction());
            Assert.IsFalse(Task.WaitAll(new[] { task }, 10), "#1");
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        [Ignore]
        public void WaitAllExceptionThenCancelled() // TODO: Review
        {
            using (var cancelation = new CancellationTokenSource())
            {
                using
                (
                    var taskA = new Task
                    (
                        () =>
                        {
                            cancelation.Cancel();
                            throw new ApplicationException();
                        }
                    )
                )
                {
                    using
                    (
                        var taskB = new Task
                        (
                            ActionHelper.GetNoopAction(),
                            cancelation.Token
                        )
                    )
                    {
                        var tasks = new[] { taskA, taskB };

                        tasks[0].Start();

                        try
                        {
                            Task.WaitAll(tasks);
                            Assert.Fail("#1");
                        }
                        catch (AggregateException e)
                        {
                            Assert.That(e.InnerException, Is.TypeOf(typeof(ApplicationException)), "#2");
                            var inner = (TaskCanceledException)e.InnerExceptions[1];
                            Assert.AreEqual(tasks[1], inner.Task, "#3");
                        }

                        Assert.IsTrue(tasks[0].IsCompleted, "#4");
                        Assert.IsTrue(tasks[1].IsCanceled, "#5");
                    }
                }
            }
        }

        [Test]
        public void WaitAllTest()
        {
            Action action = () =>
            {
                var achieved = 0;
                InitWithDelegate(() => Interlocked.Increment(ref achieved));
                Task.WaitAll(_tasks);
                Assert.AreEqual(_max, achieved, "#1");
            };
            ParallelTestHelper.Repeat(action, 10000000);
        }

        [Test]
        public void WaitAny_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction(), cancelation.Token);
                var tasks = new[] { taskA, taskB };

                cancelation.Cancel();

                Assert.AreEqual(1, Task.WaitAny(tasks, 1000), "#1");
                Assert.IsTrue(tasks[1].IsCompleted, "#2");
                Assert.IsTrue(tasks[1].IsCanceled, "#3");
            }
        }

        [Test]
        public void WaitAny_CancelledWithoutExecution() // TODO: Review
        {
            using (var cancelation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction());
                var tasks = new[] { taskA, taskB };

                var res = 0;
                using (var mre = new ManualResetEventSlim(false))
                {
                    ThreadPool.QueueUserWorkItem
                    (
                        state =>
                        {
                            res = Task.WaitAny(tasks, 20);
                            mre.Set();
                        }
                    );
                    cancelation.Cancel();
                    Assert.IsTrue(mre.Wait(1000), "#1");
                    Assert.AreEqual(-1, res);
                }
            }
        }

        [Test]
        public void WaitAny_Empty()
        {
            Assert.AreEqual(-1, Task.WaitAny(new Task[0]));
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void WaitAny_ManyCanceled()
        {
            var cancellation = new CancellationToken(true);
            var tasks = new[] {
                Task.Factory.StartNew (ActionHelper.GetNoopAction(), cancellation),
                Task.Factory.StartNew (ActionHelper.GetNoopAction(), cancellation),
                Task.Factory.StartNew (ActionHelper.GetNoopAction(), cancellation)
            };

            try
            {
                Assert.IsTrue(Task.WaitAll(tasks, 1000), "#1");
            }
            catch (AggregateException e)
            {
                Assert.AreEqual(3, e.InnerExceptions.Count, "#2");
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition, that when resolved sequentially will fail
        public void WaitAny_OneException() // TODO: Review
        {
            using (var mre = new ManualResetEventSlim(false))
            {
                var tasks = new[] {
                    Task.Factory.StartNew (()=>mre.Wait (5000)),
                    Task.Factory.StartNew (()=>{ throw new ApplicationException (); })
                };

                Assert.AreEqual(1, Task.WaitAny(tasks, 3000), "#1");
                Assert.IsFalse(tasks[0].IsCompleted, "#2");
                Assert.IsTrue(tasks[1].IsFaulted, "#3");

                mre.Set();
            }
        }

        [Test]
        public void WaitAny_SingleCanceled() // TODO: Review
        {
            using (var src = new CancellationTokenSource())
            {
                var t = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(200);
                    src.Cancel();
                    src.Token.ThrowIfCancellationRequested();
                }, src.Token);
                Assert.AreEqual(0, Task.WaitAny(new[] { t }));
            }
        }

        [Test]
        public void WaitAny_Zero()
        {
            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            Assert.AreEqual(-1, Task.WaitAny(new[] { task }, 0), "#1");
            // Do not dispose Task
            task = new Task(ActionHelper.GetNoopAction());
            Assert.AreEqual(-1, Task.WaitAny(new[] { task }, 20), "#1");
        }

        [Test]
        public void WaitAnyTest()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var flag = 0;
                    var finished = 0;

                    InitWithDelegate
                    (
                        () =>
                        {
                            var times = Interlocked.Exchange(ref flag, 1);
                            if (times == 1)
                            {
                                var sw = new SpinWait();
                                while (finished == 0)
                                {
                                    sw.SpinOnce();
                                }
                            }
                            else
                            {
                                Interlocked.Increment(ref finished);
                            }
                        }
                    );

                    var index = Task.WaitAny(_tasks, 1000);

                    Assert.AreNotEqual(-1, index, "#3");
                    Assert.AreEqual(1, flag, "#1");
                    Assert.AreEqual(1, finished, "#2");
                }
            );
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        public void WaitChildTestCase() // TODO: Review
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var r1 = false;
                    var r2 = false;
                    var r3 = false;
                    using (var mre = new ManualResetEventSlim(false))
                    {
                        using (var mreStart = new ManualResetEventSlim(false))
                        {
                            var t = Task.Factory.StartNew
                            (
                                () =>
                                {
                                    Task.Factory.StartNew
                                    (
                                        () =>
                                        {
                                            mre.Wait(300);
                                            r1 = true;
                                        },
                                        TaskCreationOptions.AttachedToParent
                                    );
                                    Task.Factory.StartNew(() => r2 = true, TaskCreationOptions.AttachedToParent);
                                    Task.Factory.StartNew(() => r3 = true, TaskCreationOptions.AttachedToParent);
                                    mreStart.Set();
                                }
                            );

                            mreStart.Wait(300);
                            Assert.IsFalse(t.Wait(10), "#0a");
                            mre.Set();
                            Assert.IsTrue(t.Wait(500), "#0b");
                            Assert.IsTrue(r2, "#1");
                            Assert.IsTrue(r3, "#2");
                            Assert.IsTrue(r1, "#3");
                            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#4");
                        }
                    }
                },
                10
            );
        }

        [Test]
        public void WaitChildWithContinuationAttachedTest()
        {
            var result = false;
            using
            (
                var task = new Task
                (
                    () =>
                    {
                        Task.Factory.StartNew(() => Thread.Sleep(200), TaskCreationOptions.AttachedToParent).ContinueWith(t =>
                        {
                            Thread.Sleep(200);
                            result = true;
                        }, TaskContinuationOptions.AttachedToParent);
                    }
                )
            )
            {
                task.Start();
                task.Wait();
                Assert.IsTrue(result);
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void WaitChildWithContinuationNotAttachedTest()
        {
            using
            (
                var task = new Task
                (
                    () =>
                    {
                        Task.Factory.StartNew
                        (
                            () => Thread.Sleep(200),
                            TaskCreationOptions.AttachedToParent
                        ).ContinueWith
                        (
                            t => Thread.Sleep(3000)
                        );
                    }
                )
            )
            {
                task.Start();
                Assert.IsTrue(task.Wait(400));
            }
        }

        [Test]
        public void WaitChildWithNesting()
        {
            var result = false;
            var t = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                        result = true;
                    }, TaskCreationOptions.AttachedToParent);
                }, TaskCreationOptions.AttachedToParent);
            });
            Assert.IsTrue(t.Wait(4000), "#1");
            Assert.IsTrue(result, "#2");
        }

        [Test]
        public void WaitingForChildrenToComplete() // TODO: Review
        {
            Task nested = null;
            using (var mre = new ManualResetEvent(false))
            {
                _parentWfc = Task.Factory.StartNew(() =>
                {
                    nested = Task.Factory.StartNew(() =>
                    {
                        Assert.IsTrue(mre.WaitOne(4000), "parent_wfc needs to be set first");
                        Assert.IsFalse(_parentWfc.Wait(10), "#1a");
                        Assert.AreEqual(TaskStatus.WaitingForChildrenToComplete, _parentWfc.Status, "#1b");
                    }, TaskCreationOptions.AttachedToParent).ContinueWith(l =>
                    {
                        Assert.IsTrue(_parentWfc.Wait(2000), "#2a");
                        Assert.AreEqual(TaskStatus.RanToCompletion, _parentWfc.Status, "#2b");
                    }, TaskContinuationOptions.ExecuteSynchronously);
                });

                mre.Set();
                Assert.IsTrue(_parentWfc.Wait(2000), "#3");
                Assert.IsTrue(nested.Wait(2000), "#4");
            }
        }

        [Test]
        public void WhenChildTaskErrorIsThrownNotOnFaultedContinuationShouldNotBeExecuted()
        {
            var continuationRan = false;
            using
            (
                var testTask = new Task
                (
                    () =>
                    {
                        var task = new Task
                        (
                            () =>
                            {
                                throw new InvalidOperationException();
                            },
                            TaskCreationOptions.AttachedToParent
                        );
                        task.RunSynchronously();
                    }
                )
            )
            {
                var onErrorTask = testTask.ContinueWith(x => continuationRan = true, TaskContinuationOptions.NotOnFaulted);
                testTask.RunSynchronously();
                Assert.IsTrue(onErrorTask.IsCompleted);
                Assert.IsFalse(onErrorTask.IsFaulted);
                Assert.IsFalse(continuationRan);
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        public void WhenChildTaskErrorIsThrownOnlyOnFaultedContinuationShouldExecute()
        {
            var continuationRan = false;
            using
            (
                var testTask = new Task
                (
                    () =>
                    {
                        var task = new Task
                        (
                            () =>
                            {
                                throw new InvalidOperationException();
                            },
                            TaskCreationOptions.AttachedToParent
                        );
                        task.RunSynchronously();
                    }
                )
            )
            {
                var onErrorTask = testTask.ContinueWith(x => continuationRan = true, TaskContinuationOptions.OnlyOnFaulted);
                testTask.RunSynchronously();
                onErrorTask.Wait(100);
                Assert.IsTrue(continuationRan);
            }
        }

        [Test]
        public void WhenChildTaskErrorIsThrownParentTaskShouldBeFaulted()
        {
            Task innerTask = null;
            using
            (
                var testTask = new Task
                (
                    () =>
                    {
                        innerTask = new Task
                        (
                            () =>
                            {
                                throw new InvalidOperationException();
                            },
                            TaskCreationOptions.AttachedToParent
                        );
                        innerTask.RunSynchronously();
                    }
                )
            )
            {
                testTask.RunSynchronously();

                Assert.AreNotEqual(TaskStatus.Running, testTask.Status);
                Assert.IsNotNull(innerTask);
                Assert.IsTrue(innerTask.IsFaulted);
                Assert.IsNotNull(testTask.Exception);
                Assert.IsTrue(testTask.IsFaulted);
                Assert.IsNotNull(innerTask.Exception);
            }
        }

        [Test]
        public void WhenChildTaskSeveralLevelsDeepHandlesAggregateExceptionErrorStillBubblesToParent()
        {
            var continuationRan = false;
            AggregateException e = null;
            using
            (
                var testTask = new Task
                (
                    () =>
                    {
                        var child1 = new Task
                        (
                            () =>
                            {
                                var child2 = new Task
                                (
                                    () =>
                                    {
                                        throw new InvalidOperationException();
                                    },
                                    TaskCreationOptions.AttachedToParent
                                );
                                child2.RunSynchronously();
                            },
                            TaskCreationOptions.AttachedToParent
                        );
                        child1.RunSynchronously();
                        e = child1.Exception;
                        child1.Exception.Handle(ex => true);
                    }
                )
            )
            {
                var onErrorTask = testTask.ContinueWith(x => continuationRan = true, TaskContinuationOptions.OnlyOnFaulted);
                testTask.RunSynchronously();
                onErrorTask.Wait(1000);
                Assert.IsNotNull(e);
                Assert.IsTrue(continuationRan);
            }
        }

        private void Cleanup(Task[] tasks)
        {
            try
            {
                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }
            foreach (var t in tasks)
            {
                try
                {
                    t.Dispose();
                }
                catch (Exception ex)
                {
                    GC.KeepAlive(ex);
                }
            }
        }

        private void InitWithDelegate(Action action)
        {
            for (var i = 0; i < _max; i++)
            {
                _tasks[i] = Task.Factory.StartNew(action);
            }
        }

        private class ExceptionScheduler : TaskScheduler
        {
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new ApplicationException("1");
            }

            protected override void QueueTask(Task task)
            {
                throw new ApplicationException("2");
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                throw new ApplicationException("3");
            }
        }

        private class MockScheduler : TaskScheduler
        {
            public event Action<Task, bool> TryExecuteTaskInlineHandler;

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new NotImplementedException();
            }

            protected override void QueueTask(Task task)
            {
                GC.KeepAlive(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                var handler = TryExecuteTaskInlineHandler;
                if (handler != null)
                {
                    handler(task, taskWasPreviouslyQueued);
                }

                return TryExecuteTask(task);
            }
        }

        private class NonInlineableScheduler : TaskScheduler
        {
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new NotImplementedException();
            }

            protected override void QueueTask(Task task)
            {
                if (!TryExecuteTask(task))
                {
                    throw new ApplicationException();
                }
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return false;
            }
        }

        private class SynchronousScheduler : TaskScheduler
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
                return TryExecuteTask(task);
            }
        }
    }

    public partial class TaskTests
    {
#if NET20 || NET30 || NET35 || NET45

        [Test]
        public void ChildTaskWithUnscheduledContinuationAttachedToParent()
        {
            Task inner = null;
            var child = Task.Factory.StartNew(() =>
            {
                inner = Task.Run(() =>
                {
                    throw new ApplicationException();
                }).ContinueWith(task =>
                {
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            });

            var counter = 0;
            var t = child.ContinueWith(t2 => ++counter, TaskContinuationOptions.ExecuteSynchronously);
            Assert.IsTrue(t.Wait(5000), "#1");
            Assert.AreEqual(1, counter, "#2");
            Assert.AreEqual(TaskStatus.RanToCompletion, child.Status, "#3");
            Assert.AreEqual(TaskStatus.Canceled, inner.Status, "#4");
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        [Ignore]
        public void ContinuationOnBrokenScheduler()
        {
            var s = new ExceptionScheduler();
            using (var t = new Task(ActionHelper.GetNoopAction()))
            {
                var t2 = t.ContinueWith(ActionHelper.GetNoopAction<Task, object>(), TaskContinuationOptions.ExecuteSynchronously, s);

                var t3 = t.ContinueWith(ActionHelper.GetNoopAction<Task, object>(), TaskContinuationOptions.ExecuteSynchronously, s);

                t.Start();

                try
                {
                    Assert.IsTrue(t3.Wait(2000), "#0");
                    Assert.Fail("#1");
                }
                catch (AggregateException e)
                {
                    GC.KeepAlive(e);
                }

                Assert.AreEqual(TaskStatus.Faulted, t2.Status, "#2");
                Assert.AreEqual(TaskStatus.Faulted, t3.Status, "#3");
            }
        }

        [Test]
        public void ContinueWith_StateValue()
        {
            var t = Task.Factory.StartNew(l => Assert.AreEqual(1, l, "a-1"), 1);

            var c = t.ContinueWith((a, b) =>
            {
                Assert.AreEqual(t, a, "c-1");
                Assert.AreEqual(2, b, "c-2");
            }, 2);

            var d = t.ContinueWith((a, b) =>
            {
                Assert.AreEqual(t, a, "d-1");
                Assert.AreEqual(3, b, "d-2");
                return 77;
            }, 3);

            Assert.IsTrue(d.Wait(1000), "#1");

            Assert.AreEqual(1, t.AsyncState, "#2");
            Assert.AreEqual(2, c.AsyncState, "#3");
            Assert.AreEqual(3, d.AsyncState, "#4");
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        public void ContinueWith_StateValueGeneric()
        {
            var t = Task.Factory.StartNew(l =>
            {
                Assert.AreEqual(1, l, "a-1");
                return 80;
            }, 1);

            var c = t.ContinueWith((a, b) =>
            {
                Assert.AreEqual(t, a, "c-1");
                Assert.AreEqual(2, b, "c-2");
                return "c";
            }, 2);

            var d = t.ContinueWith((a, b) =>
            {
                Assert.AreEqual(t, a, "d-1");
                Assert.AreEqual(3, b, "d-2");
                return 'd';
            }, 3);

            Assert.IsTrue(d.Wait(1000), "#1");

            Assert.AreEqual(1, t.AsyncState, "#2");
            Assert.AreEqual(80, t.Result, "#2r");
            Assert.AreEqual(2, c.AsyncState, "#3");
            Assert.AreEqual("c", c.Result, "#3r");
            Assert.AreEqual(3, d.AsyncState, "#4");
            Assert.AreEqual('d', d.Result, "#3r");
        }

        [Test]
        public void Delay_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                var t = Task.Delay(5000, cancelation.Token);
                Assert.IsTrue(TaskStatus.WaitingForActivation == t.Status || TaskStatus.Running == t.Status, "#1");
                cancelation.Cancel();
                try
                {
                    t.Wait(1000);
                    Assert.Fail("#2");
                }
                catch (AggregateException)
                {
                    Assert.AreEqual(TaskStatus.Canceled, t.Status, "#3");
                }
            }
            using (var cancelation = new CancellationTokenSource())
            {
                var t = Task.Delay(Timeout.Infinite, cancelation.Token);
                Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");
                cancelation.Cancel();
                try
                {
                    t.Wait(1000);
                    Assert.Fail("#12");
                }
                catch (AggregateException)
                {
                    Assert.AreEqual(TaskStatus.Canceled, t.Status, "#13");
                }
            }
        }

        [Test]
        public void Delay_Invalid()
        {
            try
            {
                Task.Delay(-100);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void Delay_Simple()
        {
            var t = Task.Delay(300);
            Assert.IsTrue(TaskStatus.WaitingForActivation == t.Status || TaskStatus.Running == t.Status, "#1");
            Assert.IsTrue(t.Wait(400), "#2");
        }

        [Test]
        public void Delay_Start()
        {
            var t = Task.Delay(5000);
            try
            {
                t.Start();
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void Delay_TimeManagement()
        {
            var delay1 = Task.Delay(50);
            var delay2 = Task.Delay(25);
            Assert.IsTrue(Task.WhenAny(new[] { delay1, delay2 }).Wait(1000));
            Assert.AreEqual(TaskStatus.RanToCompletion, delay2.Status);
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void DenyChildAttachTest() // TODO: Review
        {
            using (var mre = new ManualResetEventSlim())
            {
                Task nested = null;
                var parent = Task.Factory.StartNew(() => nested = Task.Factory.StartNew(() => mre.Wait(2000), TaskCreationOptions.AttachedToParent), TaskCreationOptions.DenyChildAttach);
                Assert.IsTrue(parent.Wait(1000), "#1");
                mre.Set();
                Assert.IsTrue(nested.Wait(2000), "#2");
            }
        }

        [Test]
        public void FromResult()
        {
            var t = Task.FromResult<object>(null);
            Assert.IsTrue(t.IsCompleted, "#1");
            Assert.AreEqual(null, t.Result, "#2");
            t.Dispose();
            t.Dispose(); // Dispose should be indempotent
                         // I lament you static analysis, but avoiding double call to Dispose to avoid ObjectDisposedException is stupid
                         // my philosophy is that Dispose should hold itself to higher standards, one able to be called safely by multiple threads
                         // If Dispose can be called concurrently by multiple threads without risk, it should be possible to call it serially too
                         // This is particularly true when we talk about a class intended for threading or asynchronous operations, such as Task
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void HideSchedulerTest() // TODO: Review
        {
            using (var mre = new ManualResetEventSlim())
            {
                var ranOnDefault = false;
                var scheduler = new SynchronousScheduler();

                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        ranOnDefault = Thread.CurrentThread.IsThreadPoolThread;
                        mre.Set();
                    });
                }, CancellationToken.None, TaskCreationOptions.HideScheduler, scheduler);

                Assert.IsTrue(mre.Wait(10000), "#1");
                Assert.IsTrue(ranOnDefault, "#2");
            }
        }

        [Test]
        public void LazyCancelationTest()
        {
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();
                // Do not dispose Task
                var parent = new Task(ActionHelper.GetNoopAction());
                var cont = parent.ContinueWith(ActionHelper.GetNoopAction<Task>(), source.Token,
                    TaskContinuationOptions.LazyCancellation, TaskScheduler.Default);

                Assert.AreNotEqual(TaskStatus.Canceled, cont.Status, "#1");
                parent.Start();
                try
                {
                    Assert.IsTrue(cont.Wait(1000), "#2");
                    Assert.Fail();
                }
                catch (AggregateException ex)
                {
                    Assert.That(ex.InnerException, Is.TypeOf(typeof(TaskCanceledException)), "#3");
                }
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this fails if serialized
        [Category("ThreadPool")]
        [Ignore]
        public void Run()
        {
            var ranOnDefaultScheduler = false;
            var t = Task.Run(() => ranOnDefaultScheduler = Thread.CurrentThread.IsThreadPoolThread);
            Assert.AreEqual(TaskCreationOptions.DenyChildAttach, t.CreationOptions, "#1");
            t.Wait();
            Assert.IsTrue(ranOnDefaultScheduler, "#2");
        }

        [Test]
        public void Run_ArgumentCheck()
        {
            try
            {
                Task.Run(null as Action);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void Run_Cancel()
        {
            var t = Task.Run(() => 1, new CancellationToken(true));
            try
            {
                GC.KeepAlive(t.Result);
                Assert.Fail("#1");
            }
            catch (AggregateException ex)
            {
                GC.KeepAlive(ex);
            }

            Assert.IsTrue(t.IsCanceled, "#2");
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void Run_ExistingTask() // TODO: Review
        {
            using
            (
                var t = new Task
                (
                    () =>
                    {
                        throw new Exception("Foo");
                    }
                )
            )
            {
                var t2 = Task.Run
                (
                    () =>
                    {
                        t.Start();
                        return t;
                    }
                );

                try
                {
                    t2.Wait(1000);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    GC.KeepAlive(ex);
                }
                Assert.AreEqual(TaskStatus.Faulted, t.Status, "#2");
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void Run_ExistingTaskT() // TODO: Review
        {
            using (var t = new Task<int>(() => 5))
            {
                var t2 = Task.Run
                (
                    () =>
                    {
                        t.Start();
                        return t;
                    }
                );

                Assert.IsTrue(t2.Wait(1000), "#1");
                Assert.AreEqual(5, t2.Result, "#2");
            }
        }

        [Test]
        public void RunSynchronously()
        {
            var val = 0;
            using
            (
                var t = new Task
                (
                    () =>
                    {
                        Thread.Sleep(100);
                        val = 1;
                    }
                )
            )
            {
                t.RunSynchronously();

                Assert.AreEqual(1, val, "#1");
            }
            using
            (
                var t = new Task
                (
                    () =>
                    {
                        Thread.Sleep(0);
                        val = 2;
                    }
                )
            )
            {
                bool? previouslyQueued = null;

                var scheduler = new MockScheduler();
                scheduler.TryExecuteTaskInlineHandler += (task, b) => previouslyQueued = b;

                t.RunSynchronously(scheduler);

                Assert.AreEqual(2, val, "#2");
                Assert.AreEqual(false, previouslyQueued, "#2a");
            }
        }

        [Test]
        public void RunSynchronously_SchedulerException()
        {
            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (task, b) =>
            {
                throw new ApplicationException();
            };
            using (var t = new Task(ActionHelper.GetNoopAction()))
            {
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
        }

        [Test]
        public void TaskContinuationChainLeak()
        {
            // Start cranking out tasks, starting each new task upon completion of and from inside the prior task.
            //
            var tester = new TaskContinuationChainLeakTester();
            tester.Run();
            tester.TasksPilledUp.WaitOne();

            // Head task should be out of scope by now.  Manually run the GC and expect that it gets collected.
            //
            GC.Collect();
            GC.WaitForPendingFinalizers();

            try
            {
                // It's important that we do the asserting while the task recursion is still going, since that is the
                // crux of the problem scenario.
                //
                tester.Verify();
            }
            finally
            {
                tester.Stop();
            }
        }

        [Test]
        public void WaitAll_CancelledAndTimeout()
        {
            var ct = new CancellationToken(true);
            using (var t1 = new Task(ActionHelper.GetNoopAction(), ct))
            {
                var t2 = Task.Delay(3000);
                Assert.IsFalse(Task.WaitAll(new[] { t1, t2 }, 10));
            }
        }

        [Test]
        public void WaitAny_WithNull()
        {
            var tasks = new Task[] {
                Task.FromResult (2),
                null
            };

            try
            {
                Task.WaitAny(tasks);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void WhenAll()
        {
            using (var t1 = new Task(ActionHelper.GetNoopAction()))
            {
                using (var t2 = new Task(t1.Start))
                {
                    var tasks = new[] { t1, t2 };

                    var t = Task.WhenAll(tasks);
                    Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                    t2.Start();

                    Assert.IsTrue(t.Wait(1000), "#2");
                }
            }
        }

        [Test]
        public void WhenAll_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                using (var taskA = new Task(ActionHelper.GetNoopAction()))
                {
                    using (var taskB = new Task(ActionHelper.GetNoopAction(), cancelation.Token))
                    {
                        var tasks = new[] { taskA, taskB };

                        cancelation.Cancel();

                        var t = Task.WhenAll(tasks);
                        Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                        tasks[0].Start();

                        try
                        {
                            var result = t.Wait(10000);
                            Assert.Fail("#2 " + result);
                        }
                        catch (AggregateException e)
                        {
                            Assert.That(e.InnerException, Is.TypeOf(typeof(TaskCanceledException)), "#3");
                        }
                    }
                }
            }
        }

        [Test]
        public void WhenAll_Empty()
        {
            var tasks = new Task[0];

            var t = Task.WhenAll(tasks);

            Assert.IsTrue(t.Wait(1000), "#1");
        }

        [Test]
        public void WhenAll_Faulted()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new ApplicationException());

            var tcs2 = new TaskCompletionSource<object>();
            tcs2.SetException(new MinimalTaskTestsEx.CustomException());

            using (var cancelation = new CancellationTokenSource())
            {
                using (var taskA = new Task(ActionHelper.GetNoopAction()))
                {
                    using (var taskB = new Task(ActionHelper.GetNoopAction(), cancelation.Token))
                    {
                        var tasks = new[] { taskA, taskB, tcs.Task, tcs2.Task };

                        cancelation.Cancel();

                        var t = Task.WhenAll(tasks);
                        Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                        tasks[0].Start();

                        try
                        {
                            Assert.IsTrue(t.Wait(1000), "#2");
                            Assert.Fail("#2a");
                        }
                        catch (AggregateException e)
                        {
                            Assert.That(e.InnerException, Is.TypeOf(typeof(ApplicationException)), "#3");
                            Assert.That(e.InnerExceptions[1], Is.TypeOf(typeof(MinimalTaskTestsEx.CustomException)), "#4");
                        }
                    }
                }
            }
        }

        [Test]
        public void WhenAll_Start()
        {
            Task[] tasks = {
                Task.FromResult (2),
            };

            var t = Task.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            tasks = new[] { task };

            t = Task.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");

            try
            {
                t.Start();
                Assert.Fail("#12");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void WhenAll_WithNull()
        {
            var tasks = new Task[] {
                Task.FromResult (2),
                null
            };

            try
            {
                Task.WhenAll(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                Task.WhenAll(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void WhenAllResult() // TODO: Review
        {
            using
            (
                var t1 = new Task<string>
                (
                    () => "a"
                )
            )
            {
                using
                (
                    var t2 = new Task<string>
                    (
                        () =>
                        {
                            t1.Start();
                            return "b";
                        }
                    )
                )
                {
                    var tasks = new[] { t1, t2 };

                    var t = Task.WhenAll<string>(tasks);
                    Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                    t2.Start();

                    Assert.IsTrue(t.Wait(1000), "#2");
                    Assert.AreEqual(2, t.Result.Length, "#3");
                    Assert.AreEqual("a", t.Result[0], "#3a");
                    Assert.AreEqual("b", t.Result[1], "#3b");
                }
            }
        }

        [Test]
        public void WhenAllResult_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                using
                (
                    var taskA = new Task<int>
                    (
                        () => 9
                    )
                )
                {
                    using
                    (
                        var taskB = new Task<int>
                        (
                            () => 1,
                            cancelation.Token
                        )
                    )
                    {
                        var tasks = new[] { taskA, taskB };

                        cancelation.Cancel();

                        var t = Task.WhenAll(tasks);
                        Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                        tasks[0].Start();

                        try
                        {
                            Assert.IsTrue(t.Wait(1000), "#2");
                            Assert.Fail("#2a");
                        }
                        catch (AggregateException e)
                        {
                            Assert.That(e.InnerException, Is.TypeOf(typeof(TaskCanceledException)), "#3");
                        }

                        try
                        {
                            GC.KeepAlive(t.Result);
                            Assert.Fail("#4");
                        }
                        catch (AggregateException ex)
                        {
                            GC.KeepAlive(ex);
                        }
                    }
                }
            }
        }

        [Test]
        public void WhenAllResult_Completed()
        {
            var tasks = new[] {
                Task.FromResult (1),
                Task.FromResult (2)
            };

            var t = Task.WhenAll<int>(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
            Assert.AreEqual(2, t.Result.Length, "#2");
            Assert.AreEqual(1, t.Result[0], "#2a");
            Assert.AreEqual(2, t.Result[1], "#2b");
        }

        [Test]
        public void WhenAllResult_Empty()
        {
            var tasks = new Task<int>[0];

            var t = Task.WhenAll(tasks);

            Assert.IsTrue(t.Wait(1000), "#1");
            Assert.IsNotNull(t.Result, "#2");
            Assert.AreEqual(t.Result.Length, 0, "#3");
        }

        [Test]
        public void WhenAllResult_WithNull()
        {
            var tasks = new[] {
                Task.FromResult (2),
                null
            };

            try
            {
                Task.WhenAll<int>(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                Task.WhenAll<int>(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void WhenAny()
        {
            using (var t1 = new Task(ActionHelper.GetNoopAction()))
            {
                using (var t2 = new Task(t1.Start))
                {
                    var tasks = new[] { t1, t2 };

                    var t = Task.WhenAny(tasks);
                    Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1a");
                    t2.Start();
                    Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1b");
                    Assert.IsTrue(t.Wait(1000), "#2a");
                    Assert.IsTrue(t2.Wait(1000), "#2b");
                    Assert.IsNotNull(t.Result, "#3");
                }
            }
        }

        [Test]
        public void WhenAny_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction(), cancelation.Token);
                var tasks = new[] { taskA, taskB };

                cancelation.Cancel();

                var t = Task.WhenAny(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.AreEqual(TaskStatus.Canceled, t.Result.Status, "#3");
            }
        }

        [Test]
        public void WhenAny_Faulted()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new ApplicationException());

            var tcs2 = new TaskCompletionSource<object>();
            tcs2.SetException(new MinimalTaskTestsEx.CustomException());

            using (var cancelation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction(), cancelation.Token);
                var tasks = new[] { taskA, tcs.Task, taskB, tcs2.Task };

                cancelation.Cancel();

                var t = Task.WhenAny(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.IsNull(t.Exception, "#3");

                Assert.That(t.Result.Exception.InnerException, Is.TypeOf(typeof(ApplicationException)), "#4");
            }
        }

        [Test]
        public void WhenAny_Start()
        {
            Task[] tasks = {
                Task.FromResult (2),
            };

            var t = Task.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }

            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            tasks = new[] { task };

            t = Task.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");

            try
            {
                t.Start();
                Assert.Fail("#12");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void WhenAny_WithNull()
        {
            var tasks = new Task[] {
                Task.FromResult (2),
                null
            };

            try
            {
                Task.WhenAny(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                Task.WhenAny(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                Task.WhenAny(new Task[0]);
                Assert.Fail("#3");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeds if serialized
        [Category("ThreadPool")]
        public void WhenAnyResult() // TODO: Review
        {
            using
            (
                var t1 = new Task<byte>
                (
                    () => 3
                )
            )
            {
                using
                (
                    var t2 = new Task<byte>
                    (
                        () =>
                        {
                            t1.Start();
                            return 2;
                        }
                    )
                )
                {
                    var tasks = new[]
                    {
                        t1,
                        t2,
                    };

                    var t = Task.WhenAny<byte>(tasks);
                    Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                    t2.Start();

                    Assert.IsTrue(t.Wait(1000), "#2");
                    Assert.IsTrue(t.Result.Result > 1, "#3");
                }
            }
        }

        [Test]
        public void WhenAnyResult_Cancelled()
        {
            using (var cancelation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task<double>
                (
                    () => 1.1
                );
                // Do not dispose Task
                var taskB = new Task<double>
                (
                    () => -4.4,
                    cancelation.Token
                );
                var tasks = new[] { taskA, taskB };

                cancelation.Cancel();

                var t = Task.WhenAny<double>(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.AreEqual(TaskStatus.Canceled, t.Result.Status, "#3");
            }
        }

        [Test]
        public void WhenAnyResult_Faulted()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new ApplicationException());

            var tcs2 = new TaskCompletionSource<object>();
            tcs2.SetException(new MinimalTaskTestsEx.CustomException());

            using (var cancelation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task<object>
                (
                    () => null
                );
                // Do not dispose Task
                var taskB = new Task<object>
                (
                    () => "",
                    cancelation.Token
                );
                var tasks = new[]
                {
                    taskA,
                    tcs.Task,
                    taskB,
                    tcs2.Task
                };

                cancelation.Cancel();

                var t = Task.WhenAny<object>(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.IsNull(t.Exception, "#3");

                Assert.That(t.Result.Exception.InnerException, Is.TypeOf(typeof(ApplicationException)), "#4");
            }
        }

        [Test]
        public void WhenAnyResult_Start()
        {
            var tasks = new[]
            {
                Task.FromResult(2),
            };

            var t = Task.WhenAny<int>(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }

            // Do not dispose Task
            var task = new Task<int>
            (
                () => 55
            );
            tasks = new[] { task };

            t = Task.WhenAny<int>(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");

            try
            {
                t.Start();
                Assert.Fail("#12");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void WhenAnyResult_WithNull()
        {
            var tasks = new[] {
                Task.FromResult (2),
                null
            };

            try
            {
                Task.WhenAny<int>(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                Task.WhenAny<int>(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                Task.WhenAny<short>(new Task<short>[0]);
                Assert.Fail("#3");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }
        }

#endif
    }
}