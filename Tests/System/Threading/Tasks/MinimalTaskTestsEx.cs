using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Threading;

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public partial class MinimalTaskTestsEx
    {
        [Test]
        public void WrapAggregateExceptionCorrectly()
        {
            var x = new Task(() => throw new AggregateException(new CustomException()));
            try
            {
                x.Start();
                x.Wait();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is AggregateException);
                Assert.IsTrue(ex.InnerException is AggregateException);
                Assert.IsTrue(((AggregateException)ex.InnerException).InnerException is CustomException);
            }
        }

        [Test]
        public void WrapChildExceptionsCorrectly()
        {
            var x = new Task
            (
                () =>
                {
                    Task.Factory.StartNew(() => throw new CustomException(), TaskCreationOptions.AttachedToParent);
                    throw new OtherException();
                }
            );
            try
            {
                x.Start();
                x.Wait();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is AggregateException);
                Assert.IsTrue(ex.InnerException is OtherException);
                Assert.IsTrue(((AggregateException)ex).InnerExceptions.Count == 2);
                Assert.IsTrue(((AggregateException)ex).InnerExceptions[0] is OtherException);
                var aggregateException = ((AggregateException)ex).InnerExceptions[1] as AggregateException;
                Assert.IsTrue(aggregateException != null);
                Assert.IsTrue(aggregateException.InnerException is CustomException);
                Assert.IsTrue(aggregateException.InnerExceptions.Count == 1);
                Assert.IsTrue(aggregateException.InnerExceptions[0] is CustomException);
            }
        }

        [Test]
        public void WrapCustomExceptionCorrectly()
        {
            var x = new Task(() => throw new CustomException());
            try
            {
                x.Start();
                x.Wait();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is AggregateException);
                Assert.IsTrue(ex.InnerException is CustomException);
            }
        }

        [Test]
        public void WrapObjectDisposedExceptionCorrectly()
        {
            const string ObjectName = "AAAAAAAAAAAAAAAA";
            var x = new Task(() => throw new ObjectDisposedException(ObjectName));
            try
            {
                x.Start();
                x.Wait();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is AggregateException);
                Assert.IsTrue(ex.InnerException is ObjectDisposedException);
                Assert.IsTrue(((ObjectDisposedException)ex.InnerException).ObjectName == ObjectName);
            }
        }

        [Serializable]
        public class CustomException : Exception
        {
            public CustomException()
            {
                // Empty
            }

            public CustomException(string message)
                : base(message)
            {
                // Empty
            }

            public CustomException(string message, Exception inner)
                : base(message, inner)
            {
                // Empty
            }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

            protected CustomException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                // Empty
            }

#endif
        }

        [Serializable]
        public class OtherException : Exception
        {
            public OtherException()
            {
                // Empty
            }

            public OtherException(string message)
                : base(message)
            {
                // Empty
            }

            public OtherException(string message, Exception inner)
                : base(message, inner)
            {
                // Empty
            }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

            protected OtherException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                // Empty
            }

#endif
        }
    }

    public partial class MinimalTaskTestsEx
    {
#if LESSTHAN_NET40

        [Test]
        public void Run()
        {
            var expectedScheduler = TaskScheduler.Current;
            TaskScheduler foundScheduler = null;
            var t = TaskEx.Run(() =>
            {
                foundScheduler = TaskScheduler.Current;
                Console.WriteLine("Task Scheduler: {0}", TaskScheduler.Current);
                Console.WriteLine("IsThreadPoolThread: {0}", Thread.CurrentThread.IsThreadPoolThread);
            });
            Assert.AreEqual(TaskCreationOptions.DenyChildAttach, t.CreationOptions, "#1");
            t.Wait();
            Assert.AreEqual(expectedScheduler, foundScheduler);
        }

#endif
    }
}