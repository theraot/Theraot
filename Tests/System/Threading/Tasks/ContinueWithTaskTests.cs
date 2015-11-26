using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public class ContinueTaskTests
    {
        [Test]
        public void ContinueWithInvalidArguments()
        {
            var task = new Task(() => { });
            try
            {
                task.ContinueWith(null);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                task.ContinueWith(delegate { }, null);
                Assert.Fail("#2");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                task.ContinueWith(delegate { }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.NotOnCanceled);
                Assert.Fail("#3");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                task.ContinueWith(delegate { }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion);
                Assert.Fail("#4");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        [Test]
        public void ContinueWithOnAnyTestCase()
        {
            ParallelTestHelper.Repeat(delegate {
                bool result = false;

                Task t = Task.Factory.StartNew(delegate { });
                Task cont = t.ContinueWith(delegate { result = true; }, TaskContinuationOptions.None);
                Assert.IsTrue(t.Wait(2000), "First wait, (status, {0})", t.Status);
                Assert.IsTrue(cont.Wait(2000), "Cont wait, (result, {0}) (parent status, {2}) (status, {1})", result, cont.Status, t.Status);
                Assert.IsNull(cont.Exception, "#1");
                Assert.IsNotNull(cont, "#2");
                Assert.IsTrue(result, "#3");
            });
        }

        [Test]
        public void ContinueWithOnCompletedSuccessfullyTestCase()
        {
            ParallelTestHelper.Repeat(delegate {
                bool result = false;

                Task t = Task.Factory.StartNew(delegate { });
                Task cont = t.ContinueWith(delegate { result = true; }, TaskContinuationOptions.OnlyOnRanToCompletion);
                Assert.IsTrue(t.Wait(1000), "#4");
                Assert.IsTrue(cont.Wait(1000), "#5");

                Assert.IsNull(cont.Exception, "#1");
                Assert.IsNotNull(cont, "#2");
                Assert.IsTrue(result, "#3");
            });
        }

        [Test]
        public void ContinueWithOnAbortedTestCase()
        {
            bool result = false;
            bool taskResult = false;

            CancellationTokenSource src = new CancellationTokenSource();
            Task t = new Task(delegate { taskResult = true; }, src.Token);

            Task cont = t.ContinueWith(delegate { result = true; },
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
            catch (InvalidOperationException)
            {
            }

            Assert.IsTrue(cont.Wait(1000), "#3");

            Assert.IsFalse(taskResult, "#4");

            Assert.IsNull(cont.Exception, "#5");
            Assert.AreEqual(TaskStatus.RanToCompletion, cont.Status, "#6");
        }

        [Test]
        public void ContinueWithOnFailedTestCase()
        {
            ParallelTestHelper.Repeat(delegate {
                bool result = false;

                Task t = Task.Factory.StartNew(delegate { throw new Exception("foo"); });
                Task cont = t.ContinueWith(delegate { result = true; }, TaskContinuationOptions.OnlyOnFaulted);

                Assert.IsTrue(cont.Wait(1000), "#0");
                Assert.IsNotNull(t.Exception, "#1");
                Assert.IsNotNull(cont, "#2");
                Assert.IsTrue(result, "#3");
            });
        }

        [Test]
        public void ContinueWithWithStart()
        {
            Task t = new Task<int>(() => 1);
            t = t.ContinueWith(l => { });
            try
            {
                t.Start();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public void ContinueWithChildren()
        {
            ParallelTestHelper.Repeat(delegate {
                bool result = false;

                var t = Task.Factory.StartNew(() => Task.Factory.StartNew(() => { }, TaskCreationOptions.AttachedToParent));

                var mre = new ManualResetEvent(false);
                t.ContinueWith(l => {
                    result = true;
                    mre.Set();
                });

                Assert.IsTrue(mre.WaitOne(1000), "#1");
                Assert.IsTrue(result, "#2");
            }, 2);
        }

        [Test]
        public void ContinueWithDifferentOptionsAreCanceledTest()
        {
            var mre = new ManualResetEventSlim();
            var task = Task.Factory.StartNew(() => mre.Wait(200));
            var contFailed = task.ContinueWith(t => { }, TaskContinuationOptions.OnlyOnFaulted);
            var contCanceled = task.ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled);
            var contSuccess = task.ContinueWith(t => { }, TaskContinuationOptions.OnlyOnRanToCompletion);

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
}