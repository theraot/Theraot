using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Tests.Theraot.Threading.Needles
{
    [TestFixture]
    class LockableNeedleTest
    {
        [Test]
        public void SimpleTest()
        {
            var context = new LockableContext(16);
            var needle = new LockableNeedle<int>(5, context);

            Assert.AreEqual(5, needle.Value);
            Assert.Throws<InvalidOperationException>(() => needle.Value = 7);

            using (context.Enter())
            {
                needle.Capture();
                Assert.IsTrue(needle.Check());
                Assert.DoesNotThrow(() => needle.Value = 7);
                Assert.AreEqual(7, needle.Value);
            }

            Assert.Throws<InvalidOperationException>(() => needle.Value = 9);
        }

        [Test]
        public void TwoThreadsSet()
        {
            var context = new LockableContext(16);
            var needle = new LockableNeedle<int>(5, context);
            int[] count = {0};

            var info = new CircularBucket<string>(64);

            Assert.AreEqual(5, needle.Value);
            Assert.Throws<InvalidOperationException>(() => needle.Value = 7);

            var threads = new []
            {
                new Thread(() =>
                {
                    using (context.Enter())
                    {
                        try
                        {
                            info.Add("First thread did enter.");
                            needle.Capture();
                            info.Add("First thread did capture.");
                            var found = needle.Value;
                            info.Add("First thread found: " + found + " will set: " + (found + 2));
                            needle.Value = found + 2;
                            info.Add("First thread set: " + needle.Value);
                            info.Add("First thread set count to: " + Interlocked.Increment(ref count[0]));
                            info.Add("First thread done.");
                        }
                        catch (Exception exc)
                        {
                            info.Add("First thread exception: " + exc.Message);
                            throw;
                        }
                    }
                    info.Add("First thread left.");
                }),
                new Thread(() =>
                {
                    using (context.Enter())
                    {
                        try
                        {
                            info.Add("Second thread did enter.");
                            info.Add("Second thread did capture.");
                            needle.Capture();
                            var found = needle.Value;
                            info.Add("Second thread found: " + found + " will set: " + (found + 3));
                            needle.Value = found + 3;
                            info.Add("Second thread set: " + needle.Value);
                            info.Add("Second thread set count to: " + Interlocked.Increment(ref count[0]));
                            info.Add("Second thread done.");
                        }
                        catch (Exception exc)
                        {
                            info.Add("Second thread exception: " + exc.Message);
                            throw;
                        }
                    }
                    info.Add("Second thread left.");
                })
            };

            threads[0].Start();
            threads[1].Start();
            threads[0].Join();
            threads[1].Join();

            foreach (var item in info)
            {
                Trace.WriteLine(item);
            }

            Trace.WriteLine("Count = " + Thread.VolatileRead(ref count[0]));
            Trace.WriteLine("Found = " + needle.Value);

            Assert.IsTrue(needle.Value == 7 || needle.Value == 8 || needle.Value == 10);
        }

        [Test]
        public void TwoThreadsUpdate()
        {
            var context = new LockableContext(16);
            var needle = new LockableNeedle<int>(5, context);
            int[] count = { 0 };

            var info = new CircularBucket<string>(64);

            Assert.AreEqual(5, needle.Value);
            Assert.Throws<InvalidOperationException>(() => needle.Value = 7);

            var threads = new[]
            {
                new Thread(() =>
                {
                    using (context.Enter())
                    {
                        try
                        {
                            info.Add("First thread did enter.");
                            needle.Capture();
                            info.Add("First thread did capture.");
                            var found = needle.Value;
                            info.Add("First thread found: " + found + " will increment by 2");
                            needle.Update(value => value + 2);
                            info.Add("First thread set: " + needle.Value);
                            info.Add("First thread set count to: " + Interlocked.Increment(ref count[0]));
                            info.Add("First thread done.");
                        }
                        catch (Exception exc)
                        {
                            info.Add("First thread exception: " + exc.Message);
                            throw;
                        }
                    }
                    info.Add("First thread left.");
                }),
                new Thread(() =>
                {
                    using (context.Enter())
                    {
                        try
                        {
                            info.Add("Second thread did enter.");
                            info.Add("Second thread did capture.");
                            needle.Capture();
                            var found = needle.Value;
                            info.Add("Second thread found: " + found + " will increment by 3");
                            needle.Update(value => value + 3);
                            info.Add("Second thread set: " + needle.Value);
                            info.Add("Second thread set count to: " + Interlocked.Increment(ref count[0]));
                            info.Add("Second thread done.");
                        }
                        catch (Exception exc)
                        {
                            info.Add("Second thread exception: " + exc.Message);
                            throw;
                        }
                    }
                    info.Add("Second thread left.");
                })
            };

            threads[0].Start();
            threads[1].Start();
            threads[0].Join();
            threads[1].Join();

            foreach (var item in info)
            {
                Trace.WriteLine(item);
            }

            Trace.WriteLine("Count = " + Thread.VolatileRead(ref count[0]));
            Trace.WriteLine("Found = " + needle.Value);

            Assert.IsTrue(needle.Value == 10);
        }
    }
}
