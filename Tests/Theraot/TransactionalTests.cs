using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Tests.Theraot
{
    [TestFixture]
    public class TransactionalTests
    {
        [Test]
        public void AbsentTransaction()
        {
            var needle = Transact.CreateNeedle(5);
            Assert.AreEqual(5, needle.Value);
        }

        [Test]
        public void NoRaceCondition()
        {
            var handle = new ManualResetEvent(false);
            int[] count = { 0, 0 };
            var needleA = Transact.CreateNeedle(5);
            var needleB = Transact.CreateNeedle(5);
            Assert.AreEqual(needleA.Value, 5);
            Assert.AreEqual(needleB.Value, 5);
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        needleA.Value += 2;
                        transact.Commit();
                    }
                    Interlocked.Increment(ref count[1]);
                }
            );
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        needleB.Value += 5;
                        transact.Commit();
                    }
                    Interlocked.Increment(ref count[1]);
                }
            );
            global::Theraot.Threading.ThreadingHelper.SpinWaitUntil(ref count[0], 2);
            handle.Set();
            global::Theraot.Threading.ThreadingHelper.SpinWaitUntil(ref count[1], 2);
            // Both
            Assert.AreEqual(7, needleA.Value);
            Assert.AreEqual(10, needleB.Value);
            handle.Close();
        }

        [Test]
        public void NotCommitedTransaction()
        {
            var transact = new Transact();
            var needle = Transact.CreateNeedle(5);
            using (transact)
            {
                needle.Value = 7;
            }
            Assert.AreEqual(needle.Value, 5);
        }

        [Test]
        public void RaceAndRetry()
        {
            var handle = new ManualResetEvent(false);
            int[] count = { 0, 0 };
            var needle = Transact.CreateNeedle(5);
            Assert.AreEqual(needle.Value, 5);
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        do
                        {
                            Thread.Sleep(0);
                            handle.WaitOne();
                            needle.Value += 2;
                        } while (!transact.Commit());
                    }
                    Interlocked.Increment(ref count[1]);
                }
            );
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        do
                        {
                            Thread.Sleep(0);
                            handle.WaitOne();
                            needle.Value += 5;
                        } while (!transact.Commit());
                    }
                    Interlocked.Increment(ref count[1]);
                }
            );
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            // Both
            // This is initial 5 with +2 and +5 - that's 12
            Assert.AreEqual(12, needle.Value);
            handle.Close();
        }

        [Test]
        public void RaceCondition()
        {
            var handle = new ManualResetEvent(false);
            int[] count = { 0, 0 };
            var needle = Transact.CreateNeedle(5);
            var winner = 0;
            Assert.AreEqual(needle.Value, 5);
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        needle.Value += 2;
                        if (transact.Commit())
                        {
                            winner = 1;
                        }
                        Interlocked.Increment(ref count[1]);
                    }
                }
            );
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        needle.Value += 5;
                        if (transact.Commit())
                        {
                            winner = 2;
                        }
                        Interlocked.Increment(ref count[1]);
                    }
                }
            );
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            // One, the other, or both
            Assert.IsTrue((winner == 1 && needle.Value == 7) || (winner == 2 && needle.Value == 10) || (needle.Value == 12));
            handle.Close();
        }

        [Test]
        public void ReadonlyTransaction()
        {
            var handle = new ManualResetEvent(false);
            int[] count = { 0, 0 };
            var needle = Transact.CreateNeedle(5);
            Assert.AreEqual(needle.Value, 5);
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        // This one only reads
                        GC.KeepAlive(needle.Value);
                        transact.Commit();
                        Interlocked.Increment(ref count[1]);
                    }
                }
            );
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        needle.Value += 5;
                        transact.Commit();
                        Interlocked.Increment(ref count[1]);
                    }
                }
            );
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            // There is no reason for failure
            Assert.IsTrue(needle.Value == 10);
            handle.Close();
        }

        [Test]
        public void Rollback()
        {
            var needleA = Transact.CreateNeedle(5);
            var needleB = Transact.CreateNeedle(5);
            try
            {
                using (var transact = new Transact())
                {
                    const int movement = 2;
                    needleA.Value += movement;
                    ThrowException();
                    // Really, it is evident this code will not run
                    needleB.Value -= movement;
                    transact.Commit();
                }
            }
            catch (Exception exception)
            {
                // Pokemon
                GC.KeepAlive(exception);
            }
            // We did not commit
            Assert.AreEqual(5, needleA.Value);
            Assert.AreEqual(5, needleB.Value);

            //---

            using (var transact = new Transact())
            {
                needleA.Value = 9;
                Assert.AreEqual(9, needleA.Value);
                Assert.AreEqual(5, needleB.Value);

                transact.Rollback();

                Assert.AreEqual(5, needleA.Value);
                Assert.AreEqual(5, needleB.Value);
            }
            // We did rollback
            Assert.AreEqual(5, needleA.Value);
            Assert.AreEqual(5, needleB.Value);

            using (var transact = new Transact())
            {
                needleA.Value = 9;
                Assert.AreEqual(9, needleA.Value);
                Assert.AreEqual(5, needleB.Value);

                transact.Rollback();

                Assert.AreEqual(5, needleA.Value);
                Assert.AreEqual(5, needleB.Value);
                needleA.Value = 11;
                Assert.AreEqual(11, needleA.Value);
                Assert.AreEqual(5, needleB.Value);

                transact.Commit();
            }

            // We did rollback and commit again
            Assert.AreEqual(11, needleA.Value);
            Assert.AreEqual(5, needleB.Value);
        }

        [Test]
        public void SimpleTest()
        {
            var needle = Transact.CreateNeedle(1);
            var autoResetEvent = new AutoResetEvent(false);
            new Thread(() =>
            {
                using (var transaction = new Transact())
                {
                    needle.Value = 2;
                    transaction.Commit();
                    autoResetEvent.Set();
                }
            }).Start();
            autoResetEvent.WaitOne();
            Assert.AreEqual(2, needle.Value);
        }

        [Test]
        public void SimpleTransaction()
        {
            var transact = new Transact();
            var needle = Transact.CreateNeedle(5);
            using (transact)
            {
                needle.Value = 7;
                transact.Commit();
            }
            Assert.AreEqual(needle.Value, 7);
        }

        [Test]
        public void TransactionalDataStructure()
        {
            var info = new CircularBucket<string>(32);
            var bucket = new NeedleBucket<int, Transact.Needle<int>>(index => index, 5);
            bool didA = false;
            bool didB;
            ManualResetEvent[] handles =
            {
                new ManualResetEvent(false) /*work a - entered*/,
                new ManualResetEvent(false) /*work b - entered*/,
                new ManualResetEvent(false) /*done*/
            };
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    info.Add("Work A - start");
                    using (var transact = new Transact())
                    {
                        info.Add("Work A - enter");
                        handles[0].Set();
                        info.Add("Work A - reported, waiting");
                        handles[1].WaitOne();
                        info.Add("Work A - going");
                        // foreach will not trigger the creation of items
                        var got = new int[5];
                        var set = new int[5];
                        for (var index = 0; index < 5; index++)
                        {
                            got[index] = bucket.GetNeedle(index).Value;
                            set[index] = got[index] + 1;
                            bucket.GetNeedle(index).Value = set[index];
                        }
                        info.Add(string.Format("Work A - Got: [{0}, {1}, {2}, {3}, {4}] - Set: [{5}, {6}, {7}, {8}, {9}]", got[0], got[1], got[2], got[3], got[4], set[0], set[1], set[2], set[3], set[4]));
                        if (!bucket.SequenceEqual(set))
                        {
                            info.Add("Work A - ??");
                        }
                        info.Add("Work A - before commit");
                        didA = transact.Commit();
                        info.Add("Work A - after commit: " + didA);
                        if (didA != bucket.SequenceEqual(set))
                        {
                            info.Add("Work A - ???");
                        }
                        info.Add("Work A - report");
                        handles[2].Set();
                        info.Add("Work A - done");
                    }
                }
            );
            {
                info.Add("Work B - start");
                using (var transact = new Transact())
                {
                    info.Add("Work B - waiting A to enter");
                    handles[0].WaitOne();
                    info.Add("Work B - telling Work A to go");
                    handles[1].Set();
                    info.Add("Work B - going");
                    // foreach will not trigger the creation of items
                    var got = new int[5];
                    var set = new int[5];
                    for (var index = 0; index < 5; index++)
                    {
                        got[index] = bucket.GetNeedle(index).Value;
                        set[index] = got[index] * 2;
                        bucket.GetNeedle(index).Value = set[index];
                    }
                    info.Add(string.Format("Work A - Got: [{0}, {1}, {2}, {3}, {4}] - Set: [{5}, {6}, {7}, {8}, {9}]", got[0], got[1], got[2], got[3], got[4], set[0], set[1], set[2], set[3], set[4]));
                    if (!bucket.SequenceEqual(set))
                    {
                        info.Add("Work B - ??");
                    }
                    info.Add("Work B - before commit");
                    didB = transact.Commit();
                    info.Add("Work B - after commit: " + didB);
                    if (didB != bucket.SequenceEqual(set))
                    {
                        info.Add("Work B - ???");
                    }
                    info.Add("Work B - waiting report");
                    handles[2].WaitOne();
                    info.Add("Work B - done");
                }
            }
            var result = bucket;
            // These are more likely in debug mode
            // (+1)
            if (result.SequenceEqual(new[] { 1, 2, 3, 4, 5 }))
            {
                Assert.IsTrue(didA);
                Assert.IsFalse(didB);
                return;
            }
            // (*2)
            if (result.SequenceEqual(new[] { 0, 2, 4, 6, 8 }))
            {
                Assert.IsFalse(didA);
                Assert.IsTrue(didB);
                return;
            }
            // This are more likely with optimization enabled
            // (+1) and then (*2)
            if (result.SequenceEqual(new[] { 2, 4, 6, 8, 10 }))
            {
                Assert.IsTrue(didA);
                Assert.IsTrue(didB);
                return;
            }
            // (*2) and then (+1)
            if (result.SequenceEqual(new[] { 1, 3, 5, 7, 9 }))
            {
                Assert.IsTrue(didA);
                Assert.IsTrue(didB);
                return;
            }
            //---
            if (result.SequenceEqual(new[] { 0, 1, 2, 3, 4 }))
            {
                Assert.IsFalse(didA);
                Assert.IsFalse(didB);
                return;
            }
            var found = result.ToArray();
            Trace.WriteLine(" --- REPORT --- ");
            foreach (var msg in info)
            {
                Trace.WriteLine(msg);
            }
            Assert.Fail("T_T - This is what was found: [{0}, {1}, {2}, {3}, {4}]", found[0], found[1], found[2], found[3], found[4]);
        }

        [Test]
        public void UsingClonable()
        {
            var needle = Transact.CreateNeedle(new ClonableClass(7));
            using (var transact = new Transact())
            {
                needle.Value.Value = 9;
                Assert.AreEqual(9, needle.Value.Value);
                transact.Commit();
            }
            Assert.AreEqual(9, needle.Value.Value);
        }

        private static void ThrowException()
        {
            throw new InvalidOperationException("Oh no!");
        }

        class ClonableClass : ICloneable<ClonableClass>
        {
            public ClonableClass(int value)
            {
                Value = value;
            }

            public int Value { get; set; }
            public ClonableClass Clone()
            {
                return new ClonableClass(Value);
            }

            object ICloneable.Clone()
            {
                return Clone();
            }
        }
    }
}