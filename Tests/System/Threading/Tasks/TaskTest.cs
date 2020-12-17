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

#pragma warning disable AsyncFixer04 // A disposable object used in a fire & forget async call
#pragma warning disable CC0022 // Should dispose object
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tests.Helpers;
using Theraot.Core;

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public partial class TaskTests
    {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
        private int _completionPortThreads;
        private int _workerThreads;
#endif
    }

    [TestFixture]
    public partial class TaskTests
    {
        private const int _max = 6;

        private Task _parentWfc;
        private Task[] _tasks;

        public static void WaitAnyManyExceptions()
        {
            var countdownEvents = new CountdownEvent[1];
            using (countdownEvents[0] = new CountdownEvent(3))
            {
                var tasks = new[]
                {
                    Task.Factory.StartNew
                    (
                        () =>
                        {
                            try
                            {
                                throw new ApplicationException();
                            }
                            finally
                            {
                                countdownEvents[0].Signal();
                            }
                        }
                    ),
                    Task.Factory.StartNew
                    (
                        () =>
                        {
                            try
                            {
                                throw new ApplicationException();
                            }
                            finally
                            {
                                countdownEvents[0].Signal();
                            }
                        }
                    ),
                    Task.Factory.StartNew
                    (
                        () =>
                        {
                            try
                            {
                                throw new ApplicationException();
                            }
                            finally
                            {
                                countdownEvents[0].Signal();
                            }
                        }
                    )
                };

                Assert.IsTrue(countdownEvents[0].Wait(1000), "#1");

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
            );
            testTask.RunSynchronously();

            Assert.AreEqual("Success", result);
        }

        [Test]
        public void AsyncWaitHandleSet()
        {
            var task = new TaskFactory().StartNew
            (
                () =>
                {
                    // Empty
                }
            );
            var ar = (IAsyncResult)task;
            Assert.IsFalse(ar.CompletedSynchronously, "#1");
            Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(5000), "#2");
        }

        [Test]
        public void CancelBeforeStart()
        {
            using (var src = new CancellationTokenSource())
            {
                var t = new Task(ActionHelper.GetNoopAction(), src.Token);
                src.Cancel();
                Assert.AreEqual(TaskStatus.Canceled, t.Status, "#1");

                try
                {
                    t.Start();
                    Assert.Fail("#2");
                }
                catch (InvalidOperationException ex)
                {
                    Theraot.No.Op(ex);
                }
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this fails if serialized
        [Category("ThreadPool")]
        public void CanceledContinuationExecuteSynchronouslyTest()
        {
            var cancellationTokenSources = new CancellationTokenSource[1];
            var manualResetEvents = new ManualResetEventSlim[1];
            using (cancellationTokenSources[0] = new CancellationTokenSource())
            {
                using (manualResetEvents[0] = new ManualResetEventSlim())
                {
                    var token = cancellationTokenSources[0].Token;
                    var result = false;
                    var thrown = false;

                    var task = Task.Factory.StartNew(() => manualResetEvents[0].Wait(100));
                    var cont = task.ContinueWith(_ => result = true, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                    cancellationTokenSources[0].Cancel();
                    manualResetEvents[0].Set();
                    task.Wait(100);
                    try
                    {
                        cont.Wait(100);
                    }
                    catch (Exception ex)
                    {
                        Theraot.No.Op(ex);
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
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        public void ContinueWithChildren()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var result = false;

                    var t = Task.Factory.StartNew(() => Task.Factory.StartNew(() =>
                    {
                        // Empty
                    }, TaskCreationOptions.AttachedToParent));
                    var manualResetEvents = new ManualResetEvent[1];
                    using (manualResetEvents[0] = new ManualResetEvent(false))
                    {
                        t.ContinueWith
                        (
                            _ =>
                            {
                                result = true;
                                manualResetEvents[0].Set();
                            }
                        );
                        Assert.IsTrue(manualResetEvents[0].WaitOne(1000), "#1");
                        Assert.IsTrue(result, "#2");
                    }
                },
                2
            );
        }

        [Test]
        public void ContinueWithCustomScheduleRejected()
        {
            var scheduler = new NonInlineableScheduler();
            var t = Task.Factory.StartNew
            (
                ActionHelper.GetNoopAction()
            ).ContinueWith
            (
                _ =>
                {
                    // Empty
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler
            );

            Assert.IsTrue(t.Wait(5000));
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        [Ignore("Not working")]
        public void ContinueWithDifferentOptionsAreCanceledTest()
        {
            var manualResetEvents = new ManualResetEventSlim[1];
            using (manualResetEvents[0] = new ManualResetEventSlim())
            {
                var task = Task.Factory.StartNew(() => manualResetEvents[0].Wait(200));
                var contFailed = task.ContinueWith(_ =>
                {
                    // Empty
                }, TaskContinuationOptions.OnlyOnFaulted);
                var contCanceled = task.ContinueWith(_ =>
                {
                    // Empty
                }, TaskContinuationOptions.OnlyOnCanceled);
                var contSuccess = task.ContinueWith(_ =>
                {
                    // Empty
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                manualResetEvents[0].Set();
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
                Theraot.No.Op(e);
            }

            try
            {
                task.ContinueWith(ActionHelper.GetNoopAction<Task>(), null);
                Assert.Fail("#2");
            }
            catch (ArgumentNullException e)
            {
                Theraot.No.Op(e);
            }

            try
            {
                task.ContinueWith(ActionHelper.GetNoopAction<Task>(), TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.NotOnCanceled);
                Assert.Fail("#3");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                task.ContinueWith(ActionHelper.GetNoopAction<Task>(), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion);
                Assert.Fail("#4");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void ContinueWithOnAbortedTestCase()
        {
            var result = false;
            var taskResult = false;

            using (var src = new CancellationTokenSource())
            {
                var t = new Task(() => taskResult = true, src.Token);
                var cont = t.ContinueWith(_ => result = true,
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
                    Theraot.No.Op(ex);
                }

                Assert.IsTrue(cont.Wait(1000), "#3");

                Assert.IsFalse(taskResult, "#4");

                Assert.IsNull(cont.Exception, "#5");
                Assert.AreEqual(TaskStatus.RanToCompletion, cont.Status, "#6");
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
                    var cont = t.ContinueWith(_ => result = true, TaskContinuationOptions.None);
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
                    var cont = t.ContinueWith(_ => result = true, TaskContinuationOptions.OnlyOnRanToCompletion);
                    Assert.IsTrue(t.Wait(1000), "#4");
                    Assert.IsTrue(cont.Wait(1000), "#5");

                    Assert.IsNull(cont.Exception, "#1");
                    Assert.IsNotNull(cont, "#2");
                    Assert.IsTrue(result, "#3");
                }
            );
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        [Ignore("Not working")]
        public void ContinueWithOnFailedTestCase()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var result = false;
                    var t = Task.Factory.StartNew(() => throw new Exception("foo"));
                    var cont = t.ContinueWith(_ => result = true, TaskContinuationOptions.OnlyOnFaulted);
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
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void CreationWhileInitiallyCanceled()
        {
            var token = new CancellationToken(true);
            var task = new Task(ActionHelper.GetNoopAction(), token);
            try
            {
                task.Start();
                Assert.Fail("#1");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
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

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

        [Test]
        public void DisposeUnstartedTest()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    var t = new Task
                    (
                        () =>
                        {
                            // Empty
                        }
                    );
                    t.Dispose();
                }
            );
        }

#endif

        [Test]
        [Category("RaceCondition")]
        public void DoubleTimedOutWaitTest()
        {
            // Do not dispose Task
            var t = new Task(ActionHelper.GetNoopAction());
            var countdownEvents = new CountdownEvent[1];
            using (countdownEvents[0] = new CountdownEvent(2))
            {
                var r1 = false;
                var r2 = false;
                ThreadPool.QueueUserWorkItem
                (
                    _ =>
                    {
                        r1 = !t.Wait(100);
                        countdownEvents[0].Signal();
                    }
                );
                ThreadPool.QueueUserWorkItem
                (
                    _ =>
                    {
                        r2 = !t.Wait(100);
                        countdownEvents[0].Signal();
                    }
                );

                countdownEvents[0].Wait(2000);
                Assert.IsTrue(r1);
                Assert.IsTrue(r2);
            }
        }

        [Test]
        public void InlineNotTrashingParentRelationship()
        {
            var r1 = false;
            var r2 = false;
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
            );
            t.RunSynchronously();

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
        }

        [Test]
        public void LongRunningTest()
        {
            bool? isTp = null;
            bool? isBg = null;
            var t = new Task
            (
                () =>
                {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
                    isTp = Thread.CurrentThread.IsThreadPoolThread;
#endif
                    isBg = Thread.CurrentThread.IsBackground;
                }
            );
            t.Start();
            Assert.IsTrue(t.Wait(5000), "#0");
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || TARGETS_NETSTANDARD
            Assert.IsTrue(isTp == true, "#1");
#endif
            Assert.IsTrue(isBg == true, "#2");
            isTp = null;
            isBg = null;
            var t2 = new Task
            (
                () =>
                {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
                    isTp = Thread.CurrentThread.IsThreadPoolThread;
#endif
                    isBg = Thread.CurrentThread.IsBackground;
                },
                TaskCreationOptions.LongRunning
            );
            t2.Start();
            Assert.IsTrue(t2.Wait(5000), "#10");
            Assert.IsFalse(isTp == true, "#11");
            Assert.IsTrue(isBg == true, "#12");
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
                Theraot.No.Op(ex);
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
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void RunSynchronouslyWithAttachedChildren()
        {
            var result = false;
            var t = new Task
            (
                () => Task.Factory.StartNew
                (
                    () =>
                    {
                        Thread.Sleep(500);
                        result = true;
                    },
                    TaskCreationOptions.AttachedToParent
                )
            );
            t.RunSynchronously();
            Assert.IsTrue(result);
        }

        [SetUp]
        public void Setup()
        {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
            ThreadPool.GetMinThreads(out _workerThreads, out _completionPortThreads);
            ThreadPool.SetMinThreads(1, 1);
#endif

            _tasks = new Task[_max];
        }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

        [Test]
        public void StartFinishedTaskTest()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    var t = Task.Factory.StartNew
                    (
                        () =>
                        {
                            // Empty
                        }
                    );
                    t.Wait();

                    t.Start();
                }
            );
        }

        [Test]
        public void StartNullArgument()
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
                Theraot.No.Op(ex);
            }
        }

#endif

        [Test]
        public void StartOnBrokenScheduler()
        {
            var t = new Task(ActionHelper.GetNoopAction());
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

        [TearDown]
        public void Teardown()
        {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
            ThreadPool.SetMinThreads(_workerThreads, _completionPortThreads);
#endif
            Cleanup(_tasks);
        }

        [Test]
        public void ThrowingUnrelatedCanceledExceptionTest()
        {
            var t = new Task
            (
                () => throw new TaskCanceledException()
            );
            t.RunSynchronously();
            Assert.IsTrue(t.IsFaulted);
            Assert.IsFalse(t.IsCanceled);
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
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
            var t = new Thread(() => Task.Factory.StartNew(() => throw inner));
            t.Start();
            t.Join();
            Thread.Sleep(1000);
            GC.Collect();
            Thread.Sleep(1000);
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        [Ignore("Not working")]
        public void WaitAllCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                var taskA = new Task(cancellation.Cancel);
                var taskB = new Task(ActionHelper.GetNoopAction(), cancellation.Token);
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

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        [Ignore("Not working")]
        public void WaitAllExceptionThenCancelled()
        {
            var cancellationTokenSources = new CancellationTokenSource[1];
            using (cancellationTokenSources[0] = new CancellationTokenSource())
            {
                var taskA = new Task
                (
                    () =>
                    {
                        cancellationTokenSources[0].Cancel();
                        throw new ApplicationException();
                    }
                );

                var taskB = new Task
                (
                    ActionHelper.GetNoopAction(),
                    cancellationTokenSources[0].Token
                );
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

        [Test]
        [Category("LongRunning")]
        public void WaitAllManyTasks()
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
        public void WaitAllStartedUnderWait()
        {
            var task1 = new Task(ActionHelper.GetNoopAction());
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    // Sleep little to let task to start and hit internal wait
                    Thread.Sleep(20);
                    task1.Start();
                }
            );

            Assert.IsTrue(Task.WaitAll(new[] { task1 }, 1000), "#1");
        }

        [Test]
        [Category("LongRunning")]
        public void WaitAllTest()
        {
            void Action()
            {
                var achieved = 0;
                InitWithDelegate(() => Interlocked.Increment(ref achieved));
                Task.WaitAll(_tasks);
                Assert.AreEqual(_max, achieved, "#1");
            }

            ParallelTestHelper.Repeat(Action, 1000);
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition, that when resolved sequentially will fail
        public void WaitAllTimeoutWithExceptionsAfter()
        {
            var countdownEvents = new CountdownEvent[1];
            var manualResetEvents = new ManualResetEvent[1];
            using (countdownEvents[0] = new CountdownEvent(2))
            {
                using (manualResetEvents[0] = new ManualResetEvent(false))
                {
                    var tasks = new[]
                    {
                        Task.Factory.StartNew
                        (
                            () => Assert.IsTrue(manualResetEvents[0].WaitOne(1500), "#0")
                        ),
                        Task.Factory.StartNew
                        (
                            () =>
                            {
                                try
                                {
                                    throw new ApplicationException();
                                }
                                finally
                                {
                                    countdownEvents[0].Signal();
                                }
                            }
                        ),
                        Task.Factory.StartNew
                        (
                            () =>
                            {
                                try
                                {
                                    throw new ApplicationException();
                                }
                                finally
                                {
                                    countdownEvents[0].Signal();
                                }
                            }
                        )
                    };

                    Assert.IsTrue(countdownEvents[0].Wait(1000), "#1");
                    Assert.IsFalse(Task.WaitAll(tasks, 1000), "#2");

                    manualResetEvents[0].Set();

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
        public void WaitAllTimeoutWithExceptionsBefore()
        {
            var countdownEvents = new CountdownEvent[1];
            var manualResetEvents = new ManualResetEvent[1];
            using (countdownEvents[0] = new CountdownEvent(2))
            {
                using (manualResetEvents[0] = new ManualResetEvent(false))
                {
                    var tasks = new[]
                    {
                        Task.Factory.StartNew
                        (
                            () =>
                            {
                                try
                                {
                                    throw new ApplicationException();
                                }
                                finally
                                {
                                    countdownEvents[0].Signal();
                                }
                            }
                        ),
                        Task.Factory.StartNew
                        (
                            () =>
                            {
                                try
                                {
                                    throw new ApplicationException();
                                }
                                finally
                                {
                                    countdownEvents[0].Signal();
                                }
                            }
                        ),
                        Task.Factory.StartNew
                        (
                            () => manualResetEvents[0].WaitOne()
                        ) // Keep lambda
                    };

                    Assert.IsTrue(countdownEvents[0].Wait(1000), "#1");
                    Assert.IsFalse(Task.WaitAll(tasks, 1000), "#2");

                    manualResetEvents[0].Set();

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
        public void WaitAllWithExceptions()
        {
            InitWithDelegate
            (
                () => throw new ApplicationException()
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
        public void WaitAllZero()
        {
            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            Assert.IsFalse(Task.WaitAll(new[] { task }, 0), "#0");
            // Do not dispose Task
            task = new Task(ActionHelper.GetNoopAction());
            Assert.IsFalse(Task.WaitAll(new[] { task }, 10), "#1");
        }

        [Test]
        public void WaitAnyCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction(), cancellation.Token);
                var tasks = new[] { taskA, taskB };

                cancellation.Cancel();

                Assert.AreEqual(1, Task.WaitAny(tasks, 1000), "#1");
                Assert.IsTrue(tasks[1].IsCompleted, "#2");
                Assert.IsTrue(tasks[1].IsCanceled, "#3");
            }
        }

        [Test]
        public void WaitAnyCancelledWithoutExecution()
        {
            var cancellationTokenSources = new CancellationTokenSource[1];
            using (cancellationTokenSources[0] = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction());
                var tasks = new[] { taskA, taskB };

                var res = 0;
                var manualResetEvents = new ManualResetEventSlim[1];
                using (manualResetEvents[0] = new ManualResetEventSlim(false))
                {
                    ThreadPool.QueueUserWorkItem
                    (
                        _ =>
                        {
                            res = Task.WaitAny(tasks, 20);
                            manualResetEvents[0].Set();
                        }
                    );
                    cancellationTokenSources[0].Cancel();
                    Assert.IsTrue(manualResetEvents[0].Wait(1000), "#1");
                    Assert.AreEqual(-1, res);
                }
            }
        }

        [Test]
        public void WaitAnyEmpty()
        {
            Assert.AreEqual(-1, Task.WaitAny());
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void WaitAnyManyCanceled()
        {
            var cancellation = new CancellationToken(true);
            var tasks = new[]
            {
                Task.Factory.StartNew(ActionHelper.GetNoopAction(), cancellation),
                Task.Factory.StartNew(ActionHelper.GetNoopAction(), cancellation),
                Task.Factory.StartNew(ActionHelper.GetNoopAction(), cancellation)
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
        public void WaitAnyOneException()
        {
            var manualResetEvents = new ManualResetEventSlim[1];
            using (manualResetEvents[0] = new ManualResetEventSlim(false))
            {
                var tasks = new[]
                {
                    Task.Factory.StartNew(() => manualResetEvents[0].Wait(5000)),
                    Task.Factory.StartNew(() => throw new ApplicationException())
                };

                Assert.AreEqual(1, Task.WaitAny(tasks, 3000), "#1");
                Assert.IsFalse(tasks[0].IsCompleted, "#2");
                Assert.IsTrue(tasks[1].IsFaulted, "#3");

                manualResetEvents[0].Set();
            }
        }

        [Test]
        public void WaitAnySingleCanceled()
        {
            var cancellationTokenSources = new CancellationTokenSource[1];
            using (cancellationTokenSources[0] = new CancellationTokenSource())
            {
                var t = Task.Factory.StartNew
                (
                    () =>
                    {
                        Thread.Sleep(200);
                        cancellationTokenSources[0].Cancel();
                        cancellationTokenSources[0].Token.ThrowIfCancellationRequested();
                    },
                    cancellationTokenSources[0].Token
                );
                Assert.AreEqual(0, Task.WaitAny(t));
            }
        }

        [Test]
        [Category("RaceCondition")]
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
        public void WaitAnyZero()
        {
            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            Assert.AreEqual(-1, Task.WaitAny(new[] { task }, 0), "#1");
            // Do not dispose Task
            task = new Task(ActionHelper.GetNoopAction());
            Assert.AreEqual(-1, Task.WaitAny(new[] { task }, 20), "#1");
        }

        [Test]
        public void WaitCancelledTask()
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
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        public void WaitChildTestCase()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var r1 = false;
                    var r2 = false;
                    var r3 = false;
                    var manualResetEvents = new ManualResetEventSlim[2];
                    using (manualResetEvents[0] = new ManualResetEventSlim(false))
                    {
                        using (manualResetEvents[1] = new ManualResetEventSlim(false))
                        {
                            var t = Task.Factory.StartNew
                            (
                                () =>
                                {
                                    Task.Factory.StartNew
                                    (
                                        () =>
                                        {
                                            manualResetEvents[0].Wait(300);
                                            r1 = true;
                                        },
                                        TaskCreationOptions.AttachedToParent
                                    );
                                    Task.Factory.StartNew(() => r2 = true, TaskCreationOptions.AttachedToParent);
                                    Task.Factory.StartNew(() => r3 = true, TaskCreationOptions.AttachedToParent);
                                    manualResetEvents[1].Set();
                                }
                            );

                            manualResetEvents[1].Wait(300);
                            Assert.IsFalse(t.Wait(10), "#0a");
                            manualResetEvents[0].Set();
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

            var task = new Task
            (
                () =>
                    Task.Factory.StartNew
                    (
                        () => Thread.Sleep(200),
                        TaskCreationOptions.AttachedToParent
                    ).ContinueWith
                    (
                        _ =>
                        {
                            Thread.Sleep(200);
                            result = true;
                        },
                        TaskContinuationOptions.AttachedToParent
                    )
            );

            task.Start();
            task.Wait();
            Assert.IsTrue(result);
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void WaitChildWithContinuationNotAttachedTest()
        {
            var task = new Task
            (
                () => Task.Factory.StartNew
                (
                    () => Thread.Sleep(200),
                    TaskCreationOptions.AttachedToParent
                ).ContinueWith
                (
                    _ => Thread.Sleep(3000)
                )
            );

            task.Start();
            Assert.IsTrue(task.Wait(400));
        }

        [Test]
        public void WaitChildWithNesting()
        {
            var result = false;
            var t = Task.Factory.StartNew
            (
                () =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(500);
                            result = true;
                        }, TaskCreationOptions.AttachedToParent);
                    }, TaskCreationOptions.AttachedToParent);
                }
            );
            Assert.IsTrue(t.Wait(4000), "#1");
            Assert.IsTrue(result, "#2");
        }

        [Test]
        public void WaitingForChildrenToComplete()
        {
            Task nested = null;
            var manualResetEvents = new ManualResetEvent[1];
            using (manualResetEvents[0] = new ManualResetEvent(false))
            {
                _parentWfc = Task.Factory.StartNew
                (
                    () =>
                    {
                        nested = Task.Factory.StartNew
                        (
                            () =>
                            {
                                Assert.IsTrue(manualResetEvents[0].WaitOne(4000), "parent_wfc needs to be set first");
                                Assert.IsFalse(_parentWfc.Wait(10), "#1a");
                                Assert.AreEqual(TaskStatus.WaitingForChildrenToComplete, _parentWfc.Status, "#1b");
                            },
                            TaskCreationOptions.AttachedToParent
                        ).ContinueWith
                        (
                            _ =>
                            {
                                Assert.IsTrue(_parentWfc.Wait(2000), "#2a");
                                Assert.AreEqual(TaskStatus.RanToCompletion, _parentWfc.Status, "#2b");
                            },
                            TaskContinuationOptions.ExecuteSynchronously
                        );
                    }
                );

                manualResetEvents[0].Set();
                Assert.IsTrue(_parentWfc.Wait(2000), "#3");
                Assert.IsTrue(nested.Wait(2000), "#4");
            }
        }

        [Test]
        public void WaitInlined()
        {
            bool? previouslyQueued = null;

            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (_, b) => previouslyQueued = b;

            var tf = new TaskFactory(scheduler);
            var t = tf.StartNew(ActionHelper.GetNoopAction());
            t.Wait();

            Assert.AreEqual(true, previouslyQueued);
        }

        [Test]
        public void WhenChildTaskErrorIsThrownNotOnFaultedContinuationShouldNotBeExecuted()
        {
            var continuationRan = false;

            var testTask = new Task
            (
                () =>
                {
                    var task = new Task
                    (
                        () => throw new InvalidOperationException(),
                        TaskCreationOptions.AttachedToParent
                    );
                    task.RunSynchronously();
                }
            );
            var onErrorTask =
                testTask.ContinueWith(_ => continuationRan = true, TaskContinuationOptions.NotOnFaulted);
            testTask.RunSynchronously();
            Assert.IsTrue(onErrorTask.IsCompleted);
            Assert.IsFalse(onErrorTask.IsFaulted);
            Assert.IsFalse(continuationRan);
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        public void WhenChildTaskErrorIsThrownOnlyOnFaultedContinuationShouldExecute()
        {
            var continuationRan = false;

            var testTask = new Task
            (
                () =>
                {
                    var task = new Task
                    (
                        () => throw new InvalidOperationException(),
                        TaskCreationOptions.AttachedToParent
                    );
                    task.RunSynchronously();
                }
            );

            var onErrorTask = testTask.ContinueWith(_ => continuationRan = true, TaskContinuationOptions.OnlyOnFaulted);
            testTask.RunSynchronously();
            onErrorTask.Wait(100);
            Assert.IsTrue(continuationRan);
        }

        [Test]
        public void WhenChildTaskErrorIsThrownParentTaskShouldBeFaulted()
        {
            Task innerTask = null;
            var testTask = new Task
            (
                () =>
                {
                    innerTask = new Task
                    (
                        () => throw new InvalidOperationException(),
                        TaskCreationOptions.AttachedToParent
                    );
                    innerTask.RunSynchronously();
                }
            );
            testTask.RunSynchronously();

            Assert.AreNotEqual(TaskStatus.Running, testTask.Status);
            Assert.IsNotNull(innerTask);
            Assert.IsTrue(innerTask.IsFaulted);
            Assert.IsNotNull(testTask.Exception);
            Assert.IsTrue(testTask.IsFaulted);
            Assert.IsNotNull(innerTask.Exception);
        }

        [Test]
        public void WhenChildTaskSeveralLevelsDeepHandlesAggregateExceptionErrorStillBubblesToParent()
        {
            var continuationRan = false;
            AggregateException e = null;

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
                                () => throw new InvalidOperationException(),
                                TaskCreationOptions.AttachedToParent
                            );
                            child2.RunSynchronously();
                        },
                        TaskCreationOptions.AttachedToParent
                    );
                    child1.RunSynchronously();
                    e = child1.Exception;
                    child1.Exception.Handle(_ => true);
                }
            );

            var onErrorTask = testTask.ContinueWith(_ => continuationRan = true, TaskContinuationOptions.OnlyOnFaulted);
            testTask.RunSynchronously();
            onErrorTask.Wait(1000);
            Assert.IsNotNull(e);
            Assert.IsTrue(continuationRan);
        }

        private static void Cleanup(Task[] tasks)
        {
            try
            {
                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                Theraot.No.Op(ex);
            }
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
            foreach (var t in tasks)
            {
                try
                {
                    t.Dispose();
                }
                catch (Exception ex)
                {
                    Theraot.No.Op(ex);
                }
            }
#endif
        }

        private void InitWithDelegate(Action action)
        {
            for (var i = 0; i < _max; i++)
            {
                _tasks[i] = Task.Factory.StartNew(action);
            }
        }

        private sealed class ExceptionScheduler : TaskScheduler
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

        private sealed class MockScheduler : TaskScheduler
        {
            public event Action<Task, bool> TryExecuteTaskInlineHandler;

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new NotImplementedException();
            }

            protected override void QueueTask(Task task)
            {
                Theraot.No.Op(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                var handler = TryExecuteTaskInlineHandler;
                handler?.Invoke(task, taskWasPreviouslyQueued);
                return TryExecuteTask(task);
            }
        }

        private sealed class NonInlineableScheduler : TaskScheduler
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
    }

    public partial class TaskTests
    {
        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void ChildTaskWithUnscheduledContinuationAttachedToParent()
        {
            Task inner = null;
            var child = Task.Factory.StartNew
            (
                () =>
                {
                    inner = TaskEx.Run(() => throw new ApplicationException()).ContinueWith
                    (
                        _ =>
                        {
                            // Empty
                        },
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously
                    );
                }
            );

            var counter = 0;
            var t = child.ContinueWith(_ => ++counter, TaskContinuationOptions.ExecuteSynchronously);
            Assert.IsTrue(t.Wait(5000), "#1");
            Assert.AreEqual(1, counter, "#2");
            Assert.AreEqual(TaskStatus.RanToCompletion, child.Status, "#3");
            Assert.AreEqual(TaskStatus.Canceled, inner.Status, "#4");
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        [Ignore("Not working")]
        public void ContinuationOnBrokenScheduler()
        {
            var s = new ExceptionScheduler();
            var t = new Task(ActionHelper.GetNoopAction());
            var t2 = t.ContinueWith(ActionHelper.GetNoopAction<Task, object>(),
                TaskContinuationOptions.ExecuteSynchronously, s);

            var t3 = t.ContinueWith(ActionHelper.GetNoopAction<Task, object>(),
                TaskContinuationOptions.ExecuteSynchronously, s);

            t.Start();

            try
            {
                Assert.IsTrue(t3.Wait(2000), "#0");
                Assert.Fail("#1");
            }
            catch (AggregateException e)
            {
                Theraot.No.Op(e);
            }

            Assert.AreEqual(TaskStatus.Faulted, t2.Status, "#2");
            Assert.AreEqual(TaskStatus.Faulted, t3.Status, "#3");
        }

        [Test]
        public void ContinueWithStateValue()
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
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        public void ContinueWithStateValueGeneric()
        {
            var taskA = Task.Factory.StartNew(l =>
            {
                Assert.AreEqual(1, l, "a-1");
                return 80;
            }, 1);

            var taskB = taskA.ContinueWith((a, b) =>
            {
                Assert.AreEqual(taskA, a, "c-1");
                Assert.AreEqual(2, b, "c-2");
                return "c";
            }, 2);

            var taskC = taskA.ContinueWith((a, b) =>
            {
                Assert.AreEqual(taskA, a, "d-1");
                Assert.AreEqual(3, b, "d-2");
                return 'd';
            }, 3);

            Assert.IsTrue(taskC.Wait(1000), "#1");

            Assert.AreEqual(1, taskA.AsyncState, "#2");
            Assert.AreEqual(80, taskA.Result, "#2r");
            Assert.AreEqual(2, taskB.AsyncState, "#3");
            Assert.AreEqual("c", taskB.Result, "#3r");
            Assert.AreEqual(3, taskC.AsyncState, "#4");
            Assert.AreEqual('d', taskC.Result, "#3r");
        }

        [Test]
        public void DelayCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                var t = TaskEx.Delay(5000, cancellation.Token);
                Assert.IsTrue(TaskStatus.WaitingForActivation == t.Status || TaskStatus.Running == t.Status, "#1");
                cancellation.Cancel();
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

            using (var cancellation = new CancellationTokenSource())
            {
                var t = TaskEx.Delay(Timeout.Infinite, cancellation.Token);
                Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");
                cancellation.Cancel();
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
        public void DelayInvalid()
        {
            try
            {
                TaskEx.Delay(-100);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void DelaySimple()
        {
            var t = TaskEx.Delay(300);
            Assert.IsTrue(TaskStatus.WaitingForActivation == t.Status || TaskStatus.Running == t.Status, "#1");
            Assert.IsTrue(t.Wait(400), "#2");
        }

        [Test]
        public void DelayStart()
        {
            var t = TaskEx.Delay(5000);
            try
            {
                t.Start();
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        [Category("RaceCondition")]
        public void DelayTimeManagement()
        {
            var delay1 = TaskEx.Delay(50);
            var delay2 = TaskEx.Delay(25);
            Assert.IsTrue(TaskEx.WhenAny(delay1, delay2).Wait(1000));
            Assert.AreEqual(TaskStatus.RanToCompletion, delay2.Status);
        }

        [Test]
        public void FromResult()
        {
            var t = TaskEx.FromResult<object>(null);
            Assert.IsTrue(t.IsCompleted, "#1");
            Assert.AreEqual(null, t.Result, "#2");
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
            t.Dispose();
            // Dispose should be idempotent
            // I lament you static analysis, but avoiding double call to Dispose to avoid ObjectDisposedException is stupid
            // my philosophy is that Dispose should hold itself to higher standards, one able to be called safely by multiple threads
            // If Dispose can be called concurrently by multiple threads without risk, it should be possible to call it serially too
            // This is particularly true when we talk about a class intended for threading or asynchronous operations, such as Task
            t.Dispose();
#endif
        }

        [Test]
        public void RunArgumentCheck()
        {
            try
            {
                TaskEx.Run(null as Action);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void RunCancel()
        {
            var t = TaskEx.Run(() => 1, new CancellationToken(true));
            try
            {
                GC.KeepAlive(t.Result);
                Assert.Fail("#1");
            }
            catch (AggregateException ex)
            {
                Theraot.No.Op(ex);
            }

            Assert.IsTrue(t.IsCanceled, "#2");
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void RunExistingTask()
        {
            var t = new Task
            (
                () => throw new Exception("Foo")
            );
            var t2 = TaskEx.Run
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
                Theraot.No.Op(ex);
            }

            Assert.AreEqual(TaskStatus.Faulted, t.Status, "#2");
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void RunExistingTaskT()
        {
            var t = new Task<int>(() => 5);
            var t2 = TaskEx.Run
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

        [Test]
        public void RunSynchronously()
        {
            var val = 0;
            var t = new Task
            (
                () =>
                {
                    Thread.Sleep(100);
                    val = 1;
                }
            );
            t.RunSynchronously();

            Assert.AreEqual(1, val, "#1");
            var t2 = new Task
            (
                () =>
                {
                    Thread.Sleep(0);
                    val = 2;
                }
            );
            bool? previouslyQueued = null;

            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (_, b) => previouslyQueued = b;

            t2.RunSynchronously(scheduler);

            Assert.AreEqual(2, val, "#2");
            Assert.AreEqual(false, previouslyQueued, "#2a");
        }

        [Test]
        public void RunSynchronouslySchedulerException()
        {
            var scheduler = new MockScheduler();
            scheduler.TryExecuteTaskInlineHandler += (_, __) => throw new ApplicationException();
            var t = new Task(ActionHelper.GetNoopAction());
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
        public void TaskContinuationChainLeak()
        {
            // Start cranking out tasks, starting each new task upon completion of and from inside the prior task.
            //
            var tester = new TaskContinuationChainLeakTester();
            tester.Run();
            tester.TasksPiledUp.WaitOne();

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
        public void WaitAllCancelledAndTimeout()
        {
            var ct = new CancellationToken(true);
            var t1 = new Task(ActionHelper.GetNoopAction(), ct);
            var t2 = TaskEx.Delay(3000);
            Assert.IsFalse(Task.WaitAll(new[] { t1, t2 }, 10));
        }

        [Test]
        public void WaitAnyWithNull()
        {
            var tasks = new Task[]
            {
                TaskEx.FromResult(2),
                null
            };

            try
            {
                Task.WaitAny(tasks);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void WhenAll()
        {
            var t1 = new Task(ActionHelper.GetNoopAction());
            var t2 = new Task(t1.Start);
            var tasks = new[] { t1, t2 };

            var t = TaskEx.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
            t2.Start();

            Assert.IsTrue(t.Wait(1000), "#2");
        }

        [Test]
        public void WhenAllCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                var taskA = new Task(ActionHelper.GetNoopAction());
                var taskB = new Task(ActionHelper.GetNoopAction(), cancellation.Token);
                var tasks = new[] { taskA, taskB };

                cancellation.Cancel();

                var t = TaskEx.WhenAll(tasks);
                Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
                tasks[0].Start();

                try
                {
                    var result = t.Wait(10000);
                    Assert.Fail($"#2 {result}");
                }
                catch (AggregateException e)
                {
                    Assert.That(e.InnerException, Is.TypeOf(typeof(TaskCanceledException)), "#3");
                }
            }
        }

        [Test]
        public void WhenAllEmpty()
        {
            var tasks = ArrayEx.Empty<Task>();

            var t = TaskEx.WhenAll(tasks);

            Assert.IsTrue(t.Wait(1000), "#1");
        }

        [Test]
        public void WhenAllFaulted()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new ApplicationException());

            var tcs2 = new TaskCompletionSource<object>();
            tcs2.SetException(new MinimalTaskTestsEx.CustomException());

            using (var cancellation = new CancellationTokenSource())
            {
                var taskA = new Task(ActionHelper.GetNoopAction());
                var taskB = new Task(ActionHelper.GetNoopAction(), cancellation.Token);
                var tasks = new[] { taskA, taskB, tcs.Task, tcs2.Task };

                cancellation.Cancel();

                var t = TaskEx.WhenAll(tasks);
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

        [Test]
        public void WhenAllResult()
        {
            var t1 = new Task<string>
            (
                () => "a"
            );
            var t2 = new Task<string>
            (
                () =>
                {
                    t1.Start();
                    return "b";
                }
            );

            var tasks = new[] { t1, t2 };

            var t = TaskEx.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
            t2.Start();

            Assert.IsTrue(t.Wait(1000), "#2");
            Assert.AreEqual(2, t.Result.Length, "#3");
            Assert.AreEqual("a", t.Result[0], "#3a");
            Assert.AreEqual("b", t.Result[1], "#3b");
        }

        [Test]
        public void WhenAllResultCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                var taskA = new Task<int>
                (
                    () => 9
                );

                var taskB = new Task<int>
                (
                    () => 1,
                    cancellation.Token
                );

                var tasks = new[] { taskA, taskB };

                cancellation.Cancel();

                var t = TaskEx.WhenAll(tasks);
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
                    Theraot.No.Op(ex);
                }
            }
        }

        [Test]
        public void WhenAllResultCompleted()
        {
            var tasks = new[]
            {
                TaskEx.FromResult(1),
                TaskEx.FromResult(2)
            };

            var t = TaskEx.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
            Assert.AreEqual(2, t.Result.Length, "#2");
            Assert.AreEqual(1, t.Result[0], "#2a");
            Assert.AreEqual(2, t.Result[1], "#2b");
        }

        [Test]
        public void WhenAllResultEmpty()
        {
            var tasks = ArrayEx.Empty<Task<int>>();

            var t = TaskEx.WhenAll(tasks);

            Assert.IsTrue(t.Wait(1000), "#1");
            Assert.IsNotNull(t.Result, "#2");
            Assert.AreEqual(t.Result.Length, 0, "#3");
        }

        [Test]
        public void WhenAllResultWithNull()
        {
            var tasks = new[]
            {
                TaskEx.FromResult(2),
                null
            };

            try
            {
                TaskEx.WhenAll(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                TaskEx.WhenAll<int>(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void WhenAllStart()
        {
            Task[] tasks =
            {
                TaskEx.FromResult(2)
            };

            var t = TaskEx.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }

            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            tasks = new[] { task };

            t = TaskEx.WhenAll(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");

            try
            {
                t.Start();
                Assert.Fail("#12");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void WhenAllWithNull()
        {
            var tasks = new Task[]
            {
                TaskEx.FromResult(2),
                null
            };

            try
            {
                TaskEx.WhenAll(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                TaskEx.WhenAll(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        [Category("RaceCondition")]
        public void WhenAny()
        {
            // On high load, this test will result in attempting to dispose a non completed task
            // How?
            // As you can see, if the task didn't complete, wait would have been false, and the task faulted, but that didn't happen
            // Here is is how: the fact that the task has completed was not visible to the thread that is disposing
            var t1 = new Task(ActionHelper.GetNoopAction());
            var t2 = new Task(t1.Start);
            var tasks = new[] { t1, t2 };

            var t = TaskEx.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1a");
            t2.Start();
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1b");
            Assert.IsTrue(t.Wait(2000), "#2a");
            Assert.IsTrue(t2.Wait(1000), "#2b");
            Assert.IsNotNull(t.Result, "#3");
        }

        [Test]
        public void WhenAnyCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction(), cancellation.Token);
                var tasks = new[] { taskA, taskB };

                cancellation.Cancel();

                var t = TaskEx.WhenAny(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.AreEqual(TaskStatus.Canceled, t.Result.Status, "#3");
            }
        }

        [Test]
        public void WhenAnyFaulted()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new ApplicationException());

            var tcs2 = new TaskCompletionSource<object>();
            tcs2.SetException(new MinimalTaskTestsEx.CustomException());

            using (var cancellation = new CancellationTokenSource())
            {
                // Do not dispose Task
                var taskA = new Task(ActionHelper.GetNoopAction());
                // Do not dispose Task
                var taskB = new Task(ActionHelper.GetNoopAction(), cancellation.Token);
                var tasks = new[] { taskA, tcs.Task, taskB, tcs2.Task };

                cancellation.Cancel();

                var t = TaskEx.WhenAny(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.IsNull(t.Exception, "#3");

                Assert.That(t.Result.Exception.InnerException, Is.TypeOf(typeof(ApplicationException)), "#4");
            }
        }

        [Test]
        [Category("NotWorking")] // This task relies on a race condition and the ThreadPool is too slow to schedule tasks prior to .NET 4.0 - this succeeds if serialized
        [Category("ThreadPool")]
        public void WhenAnyResult()
        {
            var t1 = new Task<byte>
            (
                () => 3
            );
            var t2 = new Task<byte>
            (
                () =>
                {
                    t1.Start();
                    return 2;
                }
            );
            var tasks = new[]
            {
                t1,
                t2
            };

            var t = TaskEx.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#1");
            t2.Start();

            Assert.IsTrue(t.Wait(1000), "#2");
            Assert.IsTrue(t.Result.Result > 1, "#3");
        }

        [Test]
        public void WhenAnyResultCancelled()
        {
            using (var cancellation = new CancellationTokenSource())
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
                    cancellation.Token
                );
                var tasks = new[] { taskA, taskB };

                cancellation.Cancel();

                var t = TaskEx.WhenAny(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.AreEqual(TaskStatus.Canceled, t.Result.Status, "#3");
            }
        }

        [Test]
        public void WhenAnyResultFaulted()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(new ApplicationException());

            var tcs2 = new TaskCompletionSource<object>();
            tcs2.SetException(new MinimalTaskTestsEx.CustomException());

            using (var cancellation = new CancellationTokenSource())
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
                    cancellation.Token
                );
                var tasks = new[]
                {
                    taskA,
                    tcs.Task,
                    taskB,
                    tcs2.Task
                };

                cancellation.Cancel();

                var t = TaskEx.WhenAny(tasks);
                Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");
                tasks[0].Start();

                Assert.IsTrue(t.Wait(1000), "#2");
                Assert.IsNull(t.Exception, "#3");

                Assert.That(t.Result.Exception.InnerException, Is.TypeOf(typeof(ApplicationException)), "#4");
            }
        }

        [Test]
        public void WhenAnyResultStart()
        {
            var tasks = new[]
            {
                TaskEx.FromResult(2)
            };

            var t = TaskEx.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }

            // Do not dispose Task
            var task = new Task<int>
            (
                () => 55
            );
            tasks = new[] { task };

            t = TaskEx.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");

            try
            {
                t.Start();
                Assert.Fail("#12");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void WhenAnyResultWithNull()
        {
            var tasks = new[]
            {
                TaskEx.FromResult(2),
                null
            };

            try
            {
                TaskEx.WhenAny(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                TaskEx.WhenAny<int>(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                TaskEx.WhenAny(ArrayEx.Empty<Task<short>>());
                Assert.Fail("#3");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void WhenAnyStart()
        {
            Task[] tasks =
            {
                TaskEx.FromResult(2)
            };

            var t = TaskEx.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status, "#1");

            try
            {
                t.Start();
                Assert.Fail("#2");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }

            // Do not dispose Task
            var task = new Task(ActionHelper.GetNoopAction());
            tasks = new[] { task };

            t = TaskEx.WhenAny(tasks);
            Assert.AreEqual(TaskStatus.WaitingForActivation, t.Status, "#11");

            try
            {
                t.Start();
                Assert.Fail("#12");
            }
            catch (InvalidOperationException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void WhenAnyWithNull()
        {
            var tasks = new Task[]
            {
                TaskEx.FromResult(2),
                null
            };

            try
            {
                TaskEx.WhenAny(tasks);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                TaskEx.WhenAny(null);
                Assert.Fail("#2");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                TaskEx.WhenAny();
                Assert.Fail("#3");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }
    }

    public partial class TaskTests
    {
#if LESSTHAN_NET40

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void DenyChildAttachTest()
        {
            var manualResetEvents = new ManualResetEventSlim[0];
            using (manualResetEvents[0] = new ManualResetEventSlim())
            {
                Task nested = null;
                var parent = Task.Factory.StartNew
                (
                    () => nested = Task.Factory.StartNew
                    (
                        () => manualResetEvents[0].Wait(2000),
                        TaskCreationOptions.AttachedToParent
                    ),
                    TaskCreationOptions.DenyChildAttach
                );
                Assert.IsTrue(parent.Wait(1000), "#1");
                manualResetEvents[0].Set();
                Assert.IsTrue(nested.Wait(2000), "#2");
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void HideSchedulerTest()
        {
            var manualResetEvents = new ManualResetEventSlim[1];
            using (manualResetEvents[0] = new ManualResetEventSlim())
            {
                var ranOnDefault = false;
                var scheduler = new SynchronousScheduler();
                Task.Factory.StartNew
                (
                    () =>
                    {
                        Task.Factory.StartNew
                        (
                            () =>
                            {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || TARGETS_NETSTANDARD
                                ranOnDefault = Thread.CurrentThread.IsThreadPoolThread;
#endif
                                manualResetEvents[0].Set();
                            }
                        );
                    },
                    CancellationToken.None,
                    TaskCreationOptions.HideScheduler,
                    scheduler
                );

                Assert.IsTrue(manualResetEvents[0].Wait(10000), "#1");
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || TARGETS_NETSTANDARD
                Assert.IsTrue(ranOnDefault, "#2");
#endif
            }
        }

        [Test]
        public void LazyCancellationTest()
        {
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();
                // Do not dispose Task
                var parent = new Task(ActionHelper.GetNoopAction());
                var cont = parent.ContinueWith
                (
                    ActionHelper.GetNoopAction<Task>(),
                    source.Token,
                    TaskContinuationOptions.LazyCancellation,
                    TaskScheduler.Default
                );

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
        [Ignore("Not working")]
        public void Run()
        {
            var ranOnDefaultScheduler = false;
            var t = TaskEx.Run(() => ranOnDefaultScheduler = Thread.CurrentThread.IsThreadPoolThread);
            Assert.AreEqual(TaskCreationOptions.DenyChildAttach, t.CreationOptions, "#1");
            t.Wait();
            Assert.IsTrue(ranOnDefaultScheduler, "#2");
        }

        private sealed class SynchronousScheduler : TaskScheduler
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

#endif
    }
}