#pragma warning disable AsyncFixer04 // A disposable object used in a fire & forget async call
#pragma warning disable CA2201 // Do not raise reserved exception types

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class TaskExFromTest
    {
        [Test]
        public static void TaskExFromCanceledIsCanceled()
        {
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();
                var task = TaskExEx.FromCanceled(source.Token);
                Assert.IsTrue(task.IsCanceled);
            }
        }

        [Test]
        public static void TaskExFromCanceledThrowsOnNonCanceledToken()
        {
            Assert.Throws<ArgumentOutOfRangeException, Task>(() => TaskExEx.FromCanceled(CancellationToken.None));
        }

        [Test]
        public static void TaskExFromExceptionIsFaulted()
        {
            var exception = new Exception();
            var task = TaskExEx.FromException(exception);
            Assert.IsTrue(task.IsFaulted);
        }

        [Test]
        public static void TaskExFromExceptionRespectsException()
        {
            var exception = new Exception();
            var task = TaskExEx.FromException(exception);
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(exception, task.Exception.InnerException);
        }

        [Test]
        public static void TaskExFromResultIsCompleted()
        {
            var result = new object();
            var task = TaskEx.FromResult(result);
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public static void TaskExFromResultRespectsResult()
        {
            var result = new object();
            var task = TaskEx.FromResult(result);
            Assert.AreEqual(result, task.Result);
        }

        [Test]
        public static void TaskExGenericFromCanceledIsCanceled()
        {
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();
                var task = TaskExEx.FromCanceled<bool>(source.Token);
                Assert.IsTrue(task.IsCanceled);
            }
        }

        [Test]
        public static void TaskExGenericFromCanceledThrowsOnNonCanceledToken()
        {
            Assert.Throws<ArgumentOutOfRangeException, Task<bool>>(() => TaskExEx.FromCanceled<bool>(CancellationToken.None));
        }

        [Test]
        public static void TaskExGenericFromExceptionIsFaulted()
        {
            var exception = new Exception();
            var task = TaskExEx.FromException<bool>(exception);
            Assert.IsTrue(task.IsFaulted);
        }

        [Test]
        public static void TaskExGenericFromExceptionRespectsException()
        {
            var exception = new Exception();
            var task = TaskExEx.FromException<bool>(exception);
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(exception, task.Exception.InnerException);
        }
    }
}