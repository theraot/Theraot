using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class TaskExFromTest
    {
        [Test]
        public static void TaskExFromCanceledIsCancelled()
        {
            var source = new CancellationTokenSource();
            source.Cancel();
            var task = TaskEx.FromCanceled(source.Token);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public static void TaskExFromCanceledThrowsOnNonCancelledToken()
        {
            Assert.Throws<ArgumentOutOfRangeException, Task>(() => TaskEx.FromCanceled(CancellationToken.None));
        }

        [Test]
        public static void TaskExFromExceptionIsFaulted()
        {
            var exception = new Exception();
            var task = TaskEx.FromException(exception);
            Assert.IsTrue(task.IsFaulted);
        }

        [Test]
        public static void TaskExFromExceptionRespectsException()
        {
            var exception = new Exception();
            var task = TaskEx.FromException(exception);
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
        public static void TaskExGenericFromCanceledIsCancelled()
        {
            var source = new CancellationTokenSource();
            source.Cancel();
            var task = TaskEx.FromCanceled<bool>(source.Token);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public static void TaskExGenericFromCanceledThrowsOnNonCancelledToken()
        {
            Assert.Throws<ArgumentOutOfRangeException, Task<bool>>(() => TaskEx.FromCanceled<bool>(CancellationToken.None));
        }

        [Test]
        public static void TaskExGenericFromExceptionIsFaulted()
        {
            var exception = new Exception();
            var task = TaskEx.FromException<bool>(exception);
            Assert.IsTrue(task.IsFaulted);
        }

        [Test]
        public static void TaskExGenericFromExceptionRespectsException()
        {
            var exception = new Exception();
            var task = TaskEx.FromException<bool>(exception);
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(exception, task.Exception.InnerException);
        }
    }
}