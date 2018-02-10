// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using System.Threading.Tasks;

namespace System.Threading.Tests
{
    /// <summary>
    /// SemaphoreSlim unit tests
    /// </summary>
    [TestFixture]
    public static class SemaphoreSlimTests
    {
        /// <summary>
        /// SemaphoreSlim public methods and properties to be tested
        /// </summary>
        private enum SemaphoreSlimActions
        {
            Constructor,
            Wait,
            WaitAsync,
            Release,
            Dispose,
            CurrentCount,
            AvailableWaitHandle
        }

        [Test]
        public static void RunSemaphoreSlimTest0_Ctor()
        {
            RunSemaphoreSlimTest0_Ctor(0, 10, null);
            RunSemaphoreSlimTest0_Ctor(5, 10, null);
            RunSemaphoreSlimTest0_Ctor(10, 10, null);
        }

        [Test]
        public static void RunSemaphoreSlimTest0_Ctor_Negative()
        {
            RunSemaphoreSlimTest0_Ctor(10, 0, typeof(ArgumentOutOfRangeException));
            RunSemaphoreSlimTest0_Ctor(10, -1, typeof(ArgumentOutOfRangeException));
            RunSemaphoreSlimTest0_Ctor(-1, 10, typeof(ArgumentOutOfRangeException));
        }

        [Test]
        public static void RunSemaphoreSlimTest1_Wait()
        {
            // Infinite timeout
            RunSemaphoreSlimTest1_Wait(10, 10, -1, true, null);
            RunSemaphoreSlimTest1_Wait(1, 10, -1, true, null);

            // Zero timeout
            RunSemaphoreSlimTest1_Wait(10, 10, 0, true, null);
            RunSemaphoreSlimTest1_Wait(1, 10, 0, true, null);
            RunSemaphoreSlimTest1_Wait(0, 10, 0, false, null);

            // Positive timeout
            RunSemaphoreSlimTest1_Wait(10, 10, 10, true, null);
            RunSemaphoreSlimTest1_Wait(1, 10, 10, true, null);
            RunSemaphoreSlimTest1_Wait(0, 10, 10, false, null);
        }

        [Test]
        public static void RunSemaphoreSlimTest1_Wait_NegativeCases()
        {
            // Invalid timeout
            RunSemaphoreSlimTest1_Wait(10, 10, -10, true, typeof(ArgumentOutOfRangeException));
            RunSemaphoreSlimTest1_Wait
               (10, 10, new TimeSpan(0, 0, int.MaxValue), true, typeof(ArgumentOutOfRangeException));
        }

#if !NET40

        [Test]
        public static void RunSemaphoreSlimTest1_WaitAsync()
        {
            // Infinite timeout
            RunSemaphoreSlimTest1_WaitAsync(10, 10, -1, true, null);
            RunSemaphoreSlimTest1_WaitAsync(1, 10, -1, true, null);

            // Zero timeout
            RunSemaphoreSlimTest1_WaitAsync(10, 10, 0, true, null);
            RunSemaphoreSlimTest1_WaitAsync(1, 10, 0, true, null);
            RunSemaphoreSlimTest1_WaitAsync(0, 10, 0, false, null);

            // Positive timeout
            RunSemaphoreSlimTest1_WaitAsync(10, 10, 10, true, null);
            RunSemaphoreSlimTest1_WaitAsync(1, 10, 10, true, null);
            RunSemaphoreSlimTest1_WaitAsync(0, 10, 10, false, null);
        }

        [Test]
        public static void RunSemaphoreSlimTest1_WaitAsync_NegativeCases()
        {
            // Invalid timeout
            RunSemaphoreSlimTest1_WaitAsync(10, 10, -10, true, typeof(ArgumentOutOfRangeException));
            RunSemaphoreSlimTest1_WaitAsync
               (10, 10, new TimeSpan(0, 0, int.MaxValue), true, typeof(ArgumentOutOfRangeException));
        }

#endif

        [Test]
        public static void RunSemaphoreSlimTest2_Release()
        {
            // Valid release count
            RunSemaphoreSlimTest2_Release(5, 10, 1, null);
            RunSemaphoreSlimTest2_Release(0, 10, 1, null);
            RunSemaphoreSlimTest2_Release(5, 10, 5, null);
        }

        [Test]
        public static void RunSemaphoreSlimTest2_Release_NegativeCases()
        {
            // Invalid release count
            RunSemaphoreSlimTest2_Release(5, 10, 0, typeof(ArgumentOutOfRangeException));
            RunSemaphoreSlimTest2_Release(5, 10, -1, typeof(ArgumentOutOfRangeException));

            // Semaphore Full
            RunSemaphoreSlimTest2_Release(10, 10, 1, typeof(SemaphoreFullException));
            RunSemaphoreSlimTest2_Release(5, 10, 6, typeof(SemaphoreFullException));
            RunSemaphoreSlimTest2_Release(int.MaxValue - 1, int.MaxValue, 10, typeof(SemaphoreFullException));
        }

#if !NET40

        [Test]
        public static void RunSemaphoreSlimTest4_Dispose()
        {
            RunSemaphoreSlimTest4_Dispose(5, 10, null, null);
            RunSemaphoreSlimTest4_Dispose(5, 10, SemaphoreSlimActions.CurrentCount, null);
            RunSemaphoreSlimTest4_Dispose
               (5, 10, SemaphoreSlimActions.Wait, typeof(ObjectDisposedException));
            RunSemaphoreSlimTest4_Dispose
               (5, 10, SemaphoreSlimActions.WaitAsync, typeof(ObjectDisposedException));
            RunSemaphoreSlimTest4_Dispose
              (5, 10, SemaphoreSlimActions.Release, typeof(ObjectDisposedException));
            RunSemaphoreSlimTest4_Dispose
              (5, 10, SemaphoreSlimActions.AvailableWaitHandle, typeof(ObjectDisposedException));
        }

        [Test]
        public static void RunSemaphoreSlimTest5_CurrentCount()
        {
            RunSemaphoreSlimTest5_CurrentCount(5, 10, null);
            RunSemaphoreSlimTest5_CurrentCount(5, 10, SemaphoreSlimActions.Wait);
            RunSemaphoreSlimTest5_CurrentCount(5, 10, SemaphoreSlimActions.WaitAsync);
            RunSemaphoreSlimTest5_CurrentCount(5, 10, SemaphoreSlimActions.Release);
        }

        [Test]
        public static void RunSemaphoreSlimTest7_AvailableWaitHandle()
        {
            RunSemaphoreSlimTest7_AvailableWaitHandle(5, 10, null, true);
            RunSemaphoreSlimTest7_AvailableWaitHandle(0, 10, null, false);

            RunSemaphoreSlimTest7_AvailableWaitHandle(5, 10, SemaphoreSlimActions.Wait, true);
            RunSemaphoreSlimTest7_AvailableWaitHandle(1, 10, SemaphoreSlimActions.Wait, false);
            RunSemaphoreSlimTest7_AvailableWaitHandle(5, 10, SemaphoreSlimActions.Wait, true);

            RunSemaphoreSlimTest7_AvailableWaitHandle(5, 10, SemaphoreSlimActions.WaitAsync, true);
            RunSemaphoreSlimTest7_AvailableWaitHandle(1, 10, SemaphoreSlimActions.WaitAsync, false);
            RunSemaphoreSlimTest7_AvailableWaitHandle(5, 10, SemaphoreSlimActions.WaitAsync, true);
            RunSemaphoreSlimTest7_AvailableWaitHandle(0, 10, SemaphoreSlimActions.Release, true);
        }

#endif

        /// <summary>
        /// Test SemaphoreSlim constructor
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="exceptionType">The type of the thrown exception in case of invalid cases,
        /// null for valid cases</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest0_Ctor(int initial, int maximum, Type exceptionType)
        {
            var methodFailed = "RunSemaphoreSlimTest0_Ctor(" + initial + "," + maximum + "):  FAILED.  ";
            Exception exception = null;
            try
            {
                using (var semaphore = new SemaphoreSlim(initial, maximum))
                {
                    Assert.AreEqual(initial, semaphore.CurrentCount);
                }
            }
            catch (Exception ex)
            {
                Assert.NotNull(exceptionType);
                Assert.IsTrue(exceptionType.IsInstanceOfType(ex));
                exception = ex;
            }
        }

        /// <summary>
        /// Test SemaphoreSlim Wait
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="timeout">The timeout parameter for the wait method, it must be either int or TimeSpan</param>
        /// <param name="returnValue">The expected wait return value</param>
        /// <param name="exceptionType">The type of the thrown exception in case of invalid cases,
        /// null for valid cases</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest1_Wait
            (int initial, int maximum, object timeout, bool returnValue, Type exceptionType)
        {
            using (var semaphore = new SemaphoreSlim(initial, maximum))
            {
                try
                {
                    var result = false;
                    result = timeout is TimeSpan ? semaphore.Wait((TimeSpan)timeout) : semaphore.Wait((int)timeout);
                    Assert.AreEqual(returnValue, result);
                    if (result)
                    {
                        Assert.AreEqual(initial - 1, semaphore.CurrentCount);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(exceptionType);
                    Assert.IsTrue(exceptionType.IsInstanceOfType(ex));
                }
            }
        }

#if !NET40

        /// <summary>
        /// Test SemaphoreSlim WaitAsync
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="timeout">The timeout parameter for the wait method, it must be either int or TimeSpan</param>
        /// <param name="returnValue">The expected wait return value</param>
        /// <param name="exceptionType">The type of the thrown exception in case of invalid cases,
        /// null for valid cases</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest1_WaitAsync
            (int initial, int maximum, object timeout, bool returnValue, Type exceptionType)
        {
            using (var semaphore = new SemaphoreSlim(initial, maximum))
            {
                try
                {
                    var result = false;
                    result = timeout is TimeSpan ? semaphore.WaitAsync((TimeSpan)timeout).Result : semaphore.WaitAsync((int)timeout).Result;
                    Assert.AreEqual(returnValue, result);
                    if (result)
                    {
                        Assert.AreEqual(initial - 1, semaphore.CurrentCount);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(exceptionType);
                    Assert.IsTrue(exceptionType.IsInstanceOfType(ex));
                }
            }
        }

        /// <summary>
        /// Test SemaphoreSlim WaitAsync
        /// The test verifies that SemaphoreSlim.Release() does not execute any user code synchronously.
        /// </summary>
        [Test]
        [Category("NotWorking")] // The current implementation allows WaitAsync to awake concurrently
        public static void RunSemaphoreSlimTest1_WaitAsync2() // TODO: review
        {
            using (var semaphore = new SemaphoreSlim(1))
            {
                using (var counter = new ThreadLocal<int>(() => 0))
                {
                    using (var mre = new ManualResetEvent(false))
                    {
                        var nonZeroObserved = false;

                        const int AsyncActions = 20;
                        var remAsyncActions = AsyncActions;
                        Func<int, Task> doWorkAsync = async (int i) =>
                        {
                            await semaphore.WaitAsync();
                            nonZeroObserved |= counter.Value > 0;

                            counter.Value = counter.Value + 1;
                            semaphore.Release();
                            counter.Value = counter.Value - 1;

                            if (Interlocked.Decrement(ref remAsyncActions) == 0)
                            {
                                mre.Set();
                            }
                        };

                        semaphore.Wait();
                        for (var i = 0; i < AsyncActions; i++)
                        {
                            doWorkAsync(i);
                        }
                        semaphore.Release();

                        mre.WaitOne();

                        Assert.False(nonZeroObserved,
                            "RunSemaphoreSlimTest1_WaitAsync2:  FAILED.  SemaphoreSlim.Release() seems to have synchronously invoked a continuation.");
                    }
                }
            }
        }

#endif

        /// <summary>
        /// Test SemaphoreSlim Release
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="releaseCount">The release count for the release method</param>
        /// <param name="exceptionType">The type of the thrown exception in case of invalid cases,
        /// null for valid cases</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest2_Release
           (int initial, int maximum, int releaseCount, Type exceptionType)
        {
            using (var semaphore = new SemaphoreSlim(initial, maximum))
            {
                try
                {
                    var oldCount = semaphore.Release(releaseCount);
                    Assert.AreEqual(initial, oldCount);
                    Assert.AreEqual(initial + releaseCount, semaphore.CurrentCount);
                }
                catch (Exception ex)
                {
                    Assert.NotNull(exceptionType);
                    Assert.IsTrue(exceptionType.IsInstanceOfType(ex));
                }
            }
        }

#if !NET40

        /// <summary>
        /// Call specific SemaphoreSlim method or property
        /// </summary>
        /// <param name="semaphore">The SemaphoreSlim instance</param>
        /// <param name="action">The action name</param>
        /// <param name="param">The action parameter, null if it takes no parameters</param>
        /// <returns>The action return value, null if the action returns void</returns>
        private static object CallSemaphoreAction
            (SemaphoreSlim semaphore, SemaphoreSlimActions? action, object param)
        {
            if (action == SemaphoreSlimActions.Wait)
            {
                if (param is TimeSpan)
                {
                    return semaphore.Wait((TimeSpan)param);
                }
                if (param is int)
                {
                    return semaphore.Wait((int)param);
                }
                semaphore.Wait();
                return null;
            }
            if (action == SemaphoreSlimActions.WaitAsync)
            {
                if (param is TimeSpan)
                {
                    return semaphore.WaitAsync((TimeSpan)param).Result;
                }
                if (param is int)
                {
                    return semaphore.WaitAsync((int)param).Result;
                }
                semaphore.WaitAsync().Wait();
                return null;
            }
            if (action == SemaphoreSlimActions.Release)
            {
                if (param != null)
                {
                    return semaphore.Release((int)param);
                }
                return semaphore.Release();
            }
            if (action == SemaphoreSlimActions.Dispose)
            {
                semaphore.Dispose();
                return null;
            }
            if (action == SemaphoreSlimActions.CurrentCount)
            {
                return semaphore.CurrentCount;
            }
            if (action == SemaphoreSlimActions.AvailableWaitHandle)
            {
                return semaphore.AvailableWaitHandle;
            }

            return null;
        }

        /// <summary>
        /// Test SemaphoreSlim Dispose
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="action">SemaphoreSlim action to be called after Dispose</param>
        /// <param name="exceptionType">The type of the thrown exception in case of invalid cases,
        /// null for valid cases</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest4_Dispose(int initial, int maximum, SemaphoreSlimActions? action, Type exceptionType)
        {
            var semaphore = new SemaphoreSlim(initial, maximum);
            try
            {
                semaphore.Dispose();
                CallSemaphoreAction(semaphore, action, null);
            }
            catch (Exception ex)
            {
                Assert.NotNull(exceptionType);
                Assert.IsTrue(exceptionType.IsInstanceOfType(ex));
            }
        }

        /// <summary>
        /// Test SemaphoreSlim CurrentCount property
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="action">SemaphoreSlim action to be called before CurrentCount</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest5_CurrentCount(int initial, int maximum, SemaphoreSlimActions? action)
        {
            var semaphore = new SemaphoreSlim(initial, maximum);

            CallSemaphoreAction(semaphore, action, null);
            if (action == null)
            {
                Assert.AreEqual(initial, semaphore.CurrentCount);
            }
            else
            {
                Assert.AreEqual(initial + (action == SemaphoreSlimActions.Release ? 1 : -1), semaphore.CurrentCount);
            }
        }

        /// <summary>
        /// Test SemaphoreSlim AvailableWaitHandle property
        /// </summary>
        /// <param name="initial">The initial semaphore count</param>
        /// <param name="maximum">The maximum semaphore count</param>
        /// <param name="action">SemaphoreSlim action to be called before WaitHandle</param>
        /// <param name="state">The expected wait handle state</param>
        /// <returns>True if the test succeeded, false otherwise</returns>
        private static void RunSemaphoreSlimTest7_AvailableWaitHandle(int initial, int maximum, SemaphoreSlimActions? action, bool state)
        {
            var semaphore = new SemaphoreSlim(initial, maximum);

            CallSemaphoreAction(semaphore, action, null);
            Assert.NotNull(semaphore.AvailableWaitHandle);
            Assert.AreEqual(state, semaphore.AvailableWaitHandle.WaitOne(0));
        }

#endif

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public static void RunSemaphoreSlimTest8_ConcWaitAndRelease()
        {
            RunSemaphoreSlimTest8_ConcWaitAndRelease_Private(5, 1000, 50, 50, 50, 0, 5, 1000);
            RunSemaphoreSlimTest8_ConcWaitAndRelease_Private(0, 1000, 50, 25, 25, 25, 0, 500);
            RunSemaphoreSlimTest8_ConcWaitAndRelease_Private(0, 1000, 50, 0, 0, 50, 0, 100);
        }

        private static void RunSemaphoreSlimTest8_ConcWaitAndRelease_Private(int initial, int maximum,
            int waitThreads, int releaseThreads, int succeededWait, int failedWait, int finalCount, int timeout)
        {
            using (var semaphore = new SemaphoreSlim(initial, maximum))
            {
                var threads = new Task[waitThreads + releaseThreads];
                var succeeded = 0;
                var failed = 0;
                using (var mre = new ManualResetEvent(false))
                {
                    // launch threads
                    for (var i = 0; i < threads.Length; i++)
                    {
                        if (i < waitThreads)
                        {
                            // We are creating the Task using TaskCreationOptions.LongRunning to
                            // force usage of another thread (which will be the case on the default scheduler
                            // with its current implementation).  Without this, the release tasks will likely get
                            // queued behind the wait tasks in the pool, making it very likely that the wait tasks
                            // will starve the very tasks that when run would unblock them.
                            threads[i] = new Task(delegate ()
                            {
                                mre.WaitOne();
                                if (semaphore.Wait(timeout))
                                {
                                    Interlocked.Increment(ref succeeded);
                                }
                                else
                                {
                                    Interlocked.Increment(ref failed);
                                }
                            }, TaskCreationOptions.LongRunning);
                        }
                        else
                        {
                            threads[i] = new Task(delegate ()
                            {
                                mre.WaitOne();
                                semaphore.Release();
                            });
                        }
                        threads[i].Start(TaskScheduler.Default);
                    }

                    mre.Set();
                    //wait work to be done;
                    Task.WaitAll(threads);
                    //check the number of succeeded and failed wait
                    Assert.AreEqual(succeededWait, succeeded);
                    Assert.AreEqual(failedWait, failed);
                    Assert.AreEqual(finalCount, semaphore.CurrentCount);
                }
            }
        }

#if !NET40

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public static void RunSemaphoreSlimTest8_ConcWaitAsyncAndRelease() // TODO: review
        {
            RunSemaphoreSlimTest8_ConcWaitAsyncAndRelease_Private(5, 1000, 50, 50, 50, 0, 5, 500);
            RunSemaphoreSlimTest8_ConcWaitAsyncAndRelease_Private(0, 1000, 50, 25, 25, 25, 0, 500);
            RunSemaphoreSlimTest8_ConcWaitAsyncAndRelease_Private(0, 1000, 50, 0, 0, 50, 0, 100);
        }

        private static void RunSemaphoreSlimTest8_ConcWaitAsyncAndRelease_Private(int initial, int maximum,
            int waitThreads, int releaseThreads, int succeededWait, int failedWait, int finalCount, int timeout)
        {
            using (var semaphore = new SemaphoreSlim(initial, maximum))
            {
                var tasks = new Task[waitThreads + releaseThreads];
                var succeeded = 0;
                var failed = 0;
                var semaphorea = new[] { semaphore };
                using (var mre = new ManualResetEvent(false))
                {
                    var mrea = new[] { mre };
                    // launch threads
                    for (var i = 0; i < tasks.Length; i++)
                    {
                        if (i < waitThreads)
                        {
                            tasks[i] = Task.Run(async delegate
                            {
                                mrea[0].WaitOne();
                                if (await semaphorea[0].WaitAsync(timeout))
                                {
                                    Interlocked.Increment(ref succeeded);
                                }
                                else
                                {
                                    Interlocked.Increment(ref failed);
                                }
                            });
                        }
                        else
                        {
                            tasks[i] = TaskEx.Run(delegate
                            {
                                mrea[0].WaitOne();
                                semaphorea[0].Release();
                            });
                        }
                    }

                    mre.Set();
                    //wait work to be done;
                    Task.WaitAll(tasks);

                    Assert.AreEqual(succeededWait, succeeded);
                    Assert.AreEqual(failedWait, failed);
                    Assert.AreEqual(finalCount, semaphore.CurrentCount);
                }
            }
        }

        [Test]
        public static void TestConcurrentWaitAndWaitAsync()
        {
            TestConcurrentWaitAndWaitAsync_Private(10, 10);
            TestConcurrentWaitAndWaitAsync_Private(1, 10);
            TestConcurrentWaitAndWaitAsync_Private(10, 1);
        }

        private static void TestConcurrentWaitAndWaitAsync_Private(int syncWaiters, int asyncWaiters)
        {
            var totalWaiters = syncWaiters + asyncWaiters;

            using (var semaphore = new SemaphoreSlim(0))
            {
                var tasks = new Task[totalWaiters];

                const int ITERS = 10;
                var randSeed = unchecked((int)DateTime.Now.Ticks);
                for (var i = 0; i < syncWaiters; i++)
                {
                    tasks[i] = TaskEx.Run(delegate
                    {
                        for (var iter = 0; iter < ITERS; iter++)
                        {
                            semaphore.Wait();
                            semaphore.Release();
                        }
                    });
                }
                for (var i = syncWaiters; i < totalWaiters; i++)
                {
                    tasks[i] = Task.Run(async delegate
                    {
                        for (var iter = 0; iter < ITERS; iter++)
                        {
                            await semaphore.WaitAsync();
                            semaphore.Release();
                        }
                    });
                }

                semaphore.Release(totalWaiters / 2);
                Task.WaitAll(tasks);
            }
        }

#endif
    }
}