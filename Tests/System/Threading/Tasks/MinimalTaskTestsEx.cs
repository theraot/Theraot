using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

#if NET20 || NET30 || NET35 || NET45

using System.Threading;

#endif

namespace MonoTests.System.Threading.Tasks
{
    [TestFixture]
    public class MinimalTaskTestsEx
    {
#if NET20 || NET30 || NET35 || NET45

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

#endif

        [Test]
        public void WrapAggregateExceptionCorrectly()
        {
            using (var x = new Task(() =>
            {
                throw new AggregateException(new CustomException());
            }))
            {
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
        }

        [Test]
        public void WrapChildExceptionsCorrectly()
        {
            using
            (
                var x = new Task
                (
                    () =>
                    {
                        Task.Factory.StartNew(() =>
                        {
                            throw new CustomException();
                        }, TaskCreationOptions.AttachedToParent);
                        throw new OtherException();
                    }
                )
            )
            {
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
        }

        [Test]
        public void WrapCustomExceptionCorrectly()
        {
            using (var x = new Task(() =>
            {
                throw new CustomException();
            }))
            {
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
        }

        [Test]
        public void WrapObjectDisposedExceptionCorrectly()
        {
            const string ObjectName = "AAAAAAAAAAAAAAAA";
            using (var x = new Task(() =>
            {
                throw new ObjectDisposedException(ObjectName);
            }))
            {
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
        }

        [Serializable]
        public class CustomException : Exception
        {
            public CustomException()
            {
            }

            public CustomException(string message)
                : base(message)
            {
            }

            public CustomException(string message, Exception inner)
                : base(message, inner)
            {
            }

            protected CustomException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }

        [Serializable]
        public class OtherException : Exception
        {
            public OtherException()
            {
            }

            public OtherException(string message)
                : base(message)
            {
            }

            public OtherException(string message, Exception inner)
                : base(message, inner)
            {
            }

            protected OtherException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}