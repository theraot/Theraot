using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Tests.Theraot
{
    [TestFixture]
    public class TransactionalTests
    {
        [Test]
        public void NoRaceCondition()
        {
            var handle = new ManualResetEvent(false);
            int[] count = { 0, 0 };
            var needleA = new Transact.Needle<int>(5);
            var needleB = new Transact.Needle<int>(5);
            Assert.AreEqual(needleA.Value, 5);
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
                        needleB.Value += 5;
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
            // Both
            Assert.IsTrue(needleA.Value == 7 && needleB.Value == 10);
            handle.Close();
        }

        [Test]
        public void NotCommitedTransaction()
        {
            var transact = new Transact();
            var needle = new Transact.Needle<int>(5);
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
            var needle = new Transact.Needle<int>(5);
            Assert.AreEqual(needle.Value, 5);
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        do
                        {
                            Interlocked.Increment(ref count[0]);
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
                        do
                        {
                            Interlocked.Increment(ref count[0]);
                            handle.WaitOne();
                            needle.Value += 5;
                        } while (!transact.Commit());
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
            // Both
            Assert.IsTrue(needle.Value == 12);
            handle.Close();
        }

        [Test]
        public void RaceCondition()
        {
            var handle = new ManualResetEvent(false);
            int[] count = { 0, 0 };
            var needle = new Transact.Needle<int>(5);
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
            var needle = new Transact.Needle<int>(5);
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
            var transact = new Transact();
            var needleA = new Transact.Needle<int>(5);
            var needleB = new Transact.Needle<int>(5);
            try
            {
                using (transact)
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
            Assert.AreEqual(needleA.Value, 5);
            Assert.AreEqual(needleB.Value, 5);
        }

        [Test]
        public void SimpleTransaction()
        {
            var transact = new Transact();
            var needle = new Transact.Needle<int>(5);
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
            var bucket = new NeedleBucket<int, Transact.Needle<int>>(index => index, 5);
            var handle = new ManualResetEvent(false);
            var didA = false;
            var didB = false;
            int[] count = { 0, 0 };
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    using (var transact = new Transact())
                    {
                        Interlocked.Increment(ref count[0]);
                        handle.WaitOne();
                        // foreach will not trigger the creation of items
                        for (var index = 0; index < 5; index++)
                        {
                            bucket.GetNeedle(index).Value++;
                        }
                        didA = transact.Commit();
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
                        // foreach will not trigger the creation of items
                        for (var index = 0; index < 5; index++)
                        {
                            bucket.GetNeedle(index).Value *= 2;
                        }
                        didB = transact.Commit();
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
            handle.Close();
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
                Assert.IsTrue(didB);
                Assert.IsFalse(didA);
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
            Assert.Fail(":(");
        }

        private static void ThrowException()
        {
            throw new InvalidOperationException("Oh no!");
        }
    }
}