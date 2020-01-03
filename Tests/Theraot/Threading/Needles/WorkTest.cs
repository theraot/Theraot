using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tests.Helpers;
using Theraot.Collections;

namespace Tests.Theraot.Threading.Needles
{
    [TestFixture]
    public class
        WorkTest
    {
        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void CountdownEvent_Signal_Concurrent()
        {
            for (var r = 0; r < 100; ++r)
            {
                var countDownEvents = new CountdownEvent[1];
                using (countDownEvents[0] = new CountdownEvent(500))
                {
                    for (var i = 0; i < countDownEvents[0].InitialCount; ++i)
                    {
                        Task.Factory.StartNew
                        (
                            delegate
                            {
                                countDownEvents[0].Signal();
                            }
                        );
                    }

                    Assert.IsTrue(countDownEvents[0].Wait(1000), "#1");
                }
            }
        }

        [Test]
        [Category("RaceToDeadLock")] // This test creates a race condition, that when resolved sequentially will be stuck
        public void ManualResetEventSlim_SetAfterDisposeTest()
        {
            var mre = new ManualResetEventSlim();

            ParallelTestHelper.Repeat
            (
                delegate
                {
                    Exception disp = null, setting = null;

                    var countdownEvents = new CountdownEvent[2];
                    countdownEvents[0] = new CountdownEvent(2);
                    countdownEvents[1] = new CountdownEvent(2);

                    Task.Factory.StartNew
                    (
                        delegate
                        {
                            try
                            {
                                countdownEvents[0].Signal();
                                countdownEvents[0].Wait(1000);
                                mre.Dispose();
                            }
                            catch (Exception e)
                            {
                                disp = e;
                            }

                            countdownEvents[1].Signal();
                        }
                    );
                    Task.Factory.StartNew
                    (
                        delegate
                        {
                            try
                            {
                                countdownEvents[0].Signal();
                                countdownEvents[0].Wait(1000);
                                mre.Set();
                            }
                            catch (Exception e)
                            {
                                setting = e;
                            }

                            countdownEvents[1].Signal();
                        }
                    );

                    var bb = countdownEvents[1].Wait(1000);
                    if (!bb)
                    {
                        Assert.AreEqual(true, countdownEvents[1].IsSet);
                    }

                    Assert.IsTrue(bb, "#0");
                    Assert.IsNull(disp, "#1");
                    Assert.IsNull(setting, "#2");

                    countdownEvents[0].Dispose();
                    countdownEvents[1].Dispose();
                }
            );
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void ManualResetEventSlim_Wait_SetConcurrent() // TODO: VERY BAD TEST
        {
            for (var i = 0; i < 10000; ++i)
            {
                var manualResetEvents = new ManualResetEventSlim[1];
                using (manualResetEvents[0] = new ManualResetEventSlim())
                {
                    var b = true;

                    Task.Factory.StartNew(manualResetEvents[0].Set);

                    Task.Factory.StartNew
                    (
                        delegate
                        {
                            b &= manualResetEvents[0].Wait(1000);
                        }
                    );

                    Assert.IsTrue(manualResetEvents[0].Wait(1000), i.ToString());
                    Assert.IsTrue(b, i.ToString());
                }
            }
        }

        [Test]
        [Category("RaceToDeadLock")] // This test creates a race condition, that when resolved sequentially will be stuck
        public void Progressor_ThreadedUse()
        {
            var source = Progressor<int>.CreateFromIList
            (
                new List<int>
                {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9
                }
            );
            var manualResetEvents = new ManualResetEvent[1];
            using (manualResetEvents[0] = new ManualResetEvent(false))
            {
                int[] count = { 0, 0, 0 };

                void Work()
                {
                    Interlocked.Increment(ref count[0]);
                    manualResetEvents[0].WaitOne();
                    foreach (var item in source)
                    {
                        GC.KeepAlive(item);
                        Interlocked.Increment(ref count[2]);
                    }

                    Interlocked.Increment(ref count[1]);
                }

                Task.Factory.StartNew(Work);
                Task.Factory.StartNew(Work);
                while (Volatile.Read(ref count[0]) != 2)
                {
                    Thread.Sleep(0);
                }

                manualResetEvents[0].Set();
                while (Volatile.Read(ref count[1]) != 2)
                {
                    Thread.Sleep(0);
                }

                Assert.AreEqual(10, Volatile.Read(ref count[2]));
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
                manualResetEvents[0].Close();
#endif
            }
        }
    }
}