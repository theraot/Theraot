﻿#pragma warning disable 618
#pragma warning disable RCS1079 // Throwing of new NotImplementedException

//
// YieldAwaitableTest.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2011 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Runtime.CompilerServices
{
    [TestFixture]
    public class YieldAwaitableTest
    {
        private YieldAwaitable.YieldAwaiter _a;

        private SynchronizationContext _sc;

        [Test]
        public void IsCompleted()
        {
            Assert.IsFalse(_a.IsCompleted, "#1");
            _a.GetResult();
            Assert.IsFalse(_a.IsCompleted, "#1");
        }

        [Test]
        public void OnCompleted_1()
        {
            try
            {
                _a.OnCompleted(null);
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnCompleted_2()
        {
            TaskScheduler scheduler = null;
            SynchronizationContext.SetSynchronizationContext(null);
            var manualResetEvents = new ManualResetEvent[1];

            using (manualResetEvents[0] = new ManualResetEvent(false))
            {
                _a.OnCompleted
                (
                    () =>
                    {
                        scheduler = TaskScheduler.Current;
                        manualResetEvents[0].Set();
                    }
                );

                Assert.IsTrue(manualResetEvents[0].WaitOne(1000), "#1");
                Assert.AreEqual(TaskScheduler.Current, scheduler, "#2");
            }
        }

        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnCompleted_3()
        {
            var scheduler = new MyScheduler();
            TaskScheduler ranScheduler = null;
            SynchronizationContext.SetSynchronizationContext(null);

            var t = Task.Factory.StartNew(() =>
            {
                var manualResetEvents = new ManualResetEvent[1];
                using (manualResetEvents[0] = new ManualResetEvent(false))
                {
                    _a.OnCompleted
                    (
                        () =>
                        {
                            ranScheduler = TaskScheduler.Current;
                            manualResetEvents[0].Set();
                        }
                    );

                    manualResetEvents[0].WaitOne(1000);
                }
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);

            Assert.IsTrue(t.Wait(1000), "#1");
            Assert.AreEqual(scheduler, ranScheduler, "#2");
        }

        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnCompleted_4()
        {
            SynchronizationContext contextRan = null;
            var manualResetEvents = new ManualResetEvent[1];
            using (manualResetEvents[0] = new ManualResetEvent(false))
            {
                var context = new MyContext();
                SynchronizationContext.SetSynchronizationContext(context);
                _a.OnCompleted
                (
                    () =>
                    {
                        contextRan = SynchronizationContext.Current;
                        manualResetEvents[0].Set();
                    }
                );

                Assert.IsTrue(manualResetEvents[0].WaitOne(1000), "#1");
                Assert.IsNull(contextRan, "#2");
            }
        }

        [SetUp]
        public void Setup()
        {
            _sc = SynchronizationContext.Current;
            _a = new YieldAwaitable().GetAwaiter();
        }

        [TearDown]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void TearDown()
        {
            SynchronizationContext.SetSynchronizationContext(_sc);
        }

        private sealed class MyContext : SynchronizationContext
        {
            // For debug purposes
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once NotAccessedField.Local
            public int Completed;

            // For debug purposes
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once NotAccessedField.Local
            public int PostCounter;

            // For debug purposes
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once NotAccessedField.Local
            public int SendCounter;

            // For debug purposes
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once NotAccessedField.Local
            public int Started;

            public override void OperationCompleted()
            {
                ++Completed;
                base.OperationCompleted();
            }

            public override void OperationStarted()
            {
                ++Started;
                base.OperationStarted();
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                ++PostCounter;
                base.Post(d, state);
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                ++SendCounter;
                base.Send(d, state);
            }
        }

        private sealed class MyScheduler : TaskScheduler
        {
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new NotImplementedException();
            }

            protected override void QueueTask(Task task)
            {
                TryExecuteTask(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                throw new NotImplementedException();
            }
        }
    }
}