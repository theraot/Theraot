#if FAT

using NUnit.Framework;
using System;
using System.Threading;
using Theraot.Threading;

namespace Tests.Theraot.Threading
{
    [TestFixture]
    internal class ReadWriteLockTest
    {
        [Test]
        public void CanEnterRead()
        {
            using (var x = new ReadWriteLock())
            {
                using (x.EnterRead())
                {
                    Assert.IsFalse(x.IsCurrentThreadWriter);
                }
            }
        }

        [Test]
        public void CanEnterReadEx()
        {
            using (var x = new ReadWriteLock())
            {
                IDisposable engagement = null;
                try
                {
                    if (x.TryEnterRead(out engagement))
                    {
                        Assert.IsFalse(x.IsCurrentThreadWriter);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    if (engagement != null)
                    {
                        engagement.Dispose();
                    }
                }
            }
        }

        [Test]
        public void CanEnterWrite()
        {
            using (var x = new ReadWriteLock())
            {
                using (x.EnterWrite())
                {
                    Assert.IsTrue(x.IsCurrentThreadWriter);
                }
            }
        }

        [Test]
        public void CanEnterWriteEx()
        {
            using (var x = new ReadWriteLock())
            {
                IDisposable engagement = null;
                try
                {
                    if (x.TryEnterWrite(out engagement))
                    {
                        Assert.IsTrue(x.IsCurrentThreadWriter);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    if (engagement != null)
                    {
                        engagement.Dispose();
                    }
                }
            }
        }

        [Test]
        public void CanKnowIsReader()
        {
            // Reentrant ReadWriteLock is able to tell if a therad is a reader
            using (var x = new ReadWriteLock(true))
            {
                Assert.IsFalse(x.HasReader);
                using (x.EnterRead())
                {
                    Assert.IsTrue(x.IsCurrentThreadReader);
                    Assert.IsTrue(x.HasReader);
                }
                // Not Reentrant ReadWriteLock is not
                using (var y = new ReadWriteLock(false))
                {
                    Assert.IsFalse(y.HasReader);
                    using (y.EnterRead())
                    {
                        Assert.Throws(typeof(NotSupportedException), () => GC.KeepAlive(y.IsCurrentThreadReader));
                        Assert.IsTrue(y.HasReader);
                    }
                    // ReadWriteLock is not reentrant by default
                    using (var z = new ReadWriteLock())
                    {
                        Assert.IsFalse(z.HasReader);
                        using (z.EnterRead())
                        {
                            Assert.Throws(typeof(NotSupportedException), () => GC.KeepAlive(z.IsCurrentThreadReader));
                            Assert.IsTrue(z.HasReader);
                        }
                    }
                }
            }
        }

        [Test]
        public void CanKnowIsWriter()
        {
            // ReadWriteLock always able to tell if a therad is the writer
            using (var x = new ReadWriteLock(true))
            {
                Assert.IsFalse(x.HasWriter);
                using (x.EnterWrite())
                {
                    Assert.IsTrue(x.IsCurrentThreadWriter);
                    Assert.IsTrue(x.HasWriter);
                }
                // Not Reentrant ReadWriteLock is not
                using (var y = new ReadWriteLock(false))
                {
                    Assert.IsFalse(y.HasWriter);
                    using (y.EnterWrite())
                    {
                        Assert.IsTrue(y.IsCurrentThreadWriter);
                        Assert.IsTrue(y.HasWriter);
                    }
                    // ReadWriteLock is not reentrant by default
                    using (var z = new ReadWriteLock())
                    {
                        Assert.IsFalse(z.HasWriter);
                        using (z.EnterWrite())
                        {
                            Assert.IsTrue(z.IsCurrentThreadWriter);
                            Assert.IsTrue(z.HasWriter);
                        }
                    }
                }
            }
        }

        [Test]
        public void CannotReadWhileWriting()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                using (x.EnterWrite())
                {
                    Assert.IsTrue(x.IsCurrentThreadWriter);
                    var a = new Thread
                    (
                        () =>
                        {
                            IDisposable engagement = null;
                            try
                            {
                                ok &= !x.TryEnterRead(out engagement);
                            }
                            finally
                            {
                                if (engagement != null)
                                {
                                    engagement.Dispose();
                                }
                                doneThread = true;
                            }
                        }
                    );
                    a.Start();
                    a.Join();
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CannotReadWhileWritingEx()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterWrite(out engagementA))
                    {
                        Assert.IsTrue(x.IsCurrentThreadWriter);
                        var a = new Thread
                        (
                            () =>
                            {
                                IDisposable engagementB = null;
                                try
                                {
                                    ok &= !x.TryEnterRead(out engagementB);
                                }
                                finally
                                {
                                    if (engagementB != null)
                                    {
                                        engagementB.Dispose();
                                    }
                                    doneThread = true;
                                }
                            }
                        );
                        a.Start();
                        a.Join();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                    doneThread = true;
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CannotReentryReadToWriteWhenThereAreMoreReaders()
        {
            using (var w1 = new ManualResetEvent(false))
            {
                using (var w2 = new ManualResetEvent(false))
                {
                    using (var x = new ReadWriteLock(true))
                    {
                        var ok = true;
                        var a = new Thread
                        (
                            () =>
                            {
                                using (x.EnterRead())
                                {
                                    w2.Set();
                                    w1.WaitOne();
                                }
                            }
                        );
                        a.Start();
                        w2.WaitOne();
                        using (x.EnterRead())
                        {
                            Assert.IsFalse(x.IsCurrentThreadWriter);
                            IDisposable engagement = null;
                            try
                            {
                                ok &= !x.TryEnterWrite(out engagement);
                            }
                            finally
                            {
                                if (engagement != null)
                                {
                                    engagement.Dispose();
                                }
                            }
                        }
                        w1.Set();
                        a.Join();
                        using (x.EnterRead())
                        {
                            using (x.EnterWrite())
                            {
                                Assert.IsTrue(x.IsCurrentThreadWriter);
                                Assert.IsTrue(ok);
                            }
                        }
                    } // This code results in a dead lock in a not reentrant ReadWriteLock
                }
            }
        }

        [Test]
        public void CannotWriteWhileReading()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                using (x.EnterRead())
                {
                    Assert.IsFalse(x.IsCurrentThreadWriter);
                    var a = new Thread
                    (
                        () =>
                        {
                            IDisposable engagement = null;
                            try
                            {
                                ok &= !x.TryEnterWrite(out engagement);
                            }
                            finally
                            {
                                if (engagement != null)
                                {
                                    engagement.Dispose();
                                }
                                doneThread = true;
                            }
                        }
                    );
                    a.Start();
                    a.Join();
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CannotWriteWhileReadingEx()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterRead(out engagementA))
                    {
                        Assert.IsFalse(x.IsCurrentThreadWriter);
                        var a = new Thread
                        (
                            () =>
                            {
                                IDisposable engagementB = null;
                                try
                                {
                                    ok &= !x.TryEnterWrite(out engagementB);
                                }
                                finally
                                {
                                    if (engagementB != null)
                                    {
                                        engagementB.Dispose();
                                    }
                                    doneThread = true;
                                }
                            }
                        );
                        a.Start();
                        a.Join();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                    doneThread = true;
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CannotWriteWhileWriting()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                using (x.EnterWrite())
                {
                    Assert.IsTrue(x.IsCurrentThreadWriter);
                    var a = new Thread
                    (
                        () =>
                        {
                            IDisposable engagement = null;
                            try
                            {
                                ok &= !x.TryEnterWrite(out engagement);
                            }
                            finally
                            {
                                if (engagement != null)
                                {
                                    engagement.Dispose();
                                }
                                doneThread = true;
                            }
                        }
                    );
                    a.Start();
                    a.Join();
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CannotWriteWhileWritingEx()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterWrite(out engagementA))
                    {
                        Assert.IsTrue(x.IsCurrentThreadWriter);
                        var a = new Thread
                        (
                            () =>
                            {
                                IDisposable engagementB = null;
                                try
                                {
                                    ok &= !x.TryEnterWrite(out engagementB);
                                }
                                finally
                                {
                                    if (engagementB != null)
                                    {
                                        engagementB.Dispose();
                                    }
                                    doneThread = true;
                                }
                            }
                        );
                        a.Start();
                        a.Join();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                    doneThread = true;
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CanReadWhileReading()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                using (x.EnterRead())
                {
                    Assert.IsFalse(x.IsCurrentThreadWriter);
                    var a = new Thread
                    (
                        () =>
                        {
                            IDisposable engagement = null;
                            try
                            {
                                if (x.TryEnterRead(out engagement))
                                {
                                }
                                else
                                {
                                    ok = false;
                                }
                            }
                            finally
                            {
                                if (engagement != null)
                                {
                                    engagement.Dispose();
                                }
                                doneThread = true;
                            }
                        }
                    );
                    a.Start();
                    a.Join();
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CanReadWhileReadingEx()
        {
            using (var x = new ReadWriteLock())
            {
                var ok = true;
                var doneThread = false;
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterRead(out engagementA))
                    {
                        Assert.IsFalse(x.IsCurrentThreadWriter);
                        var a = new Thread
                        (
                            () =>
                            {
                                IDisposable engagementB = null;
                                try
                                {
                                    if (x.TryEnterRead(out engagementB))
                                    {
                                    }
                                    else
                                    {
                                        ok = false;
                                    }
                                }
                                finally
                                {
                                    if (engagementB != null)
                                    {
                                        engagementB.Dispose();
                                    }
                                    doneThread = true;
                                }
                            }
                        );
                        a.Start();
                        a.Join();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                    doneThread = true;
                }
                Assert.IsTrue(ok);
                Assert.IsTrue(doneThread);
            }
        }

        [Test]
        public void CanReentryReadToRead()
        {
            using (var x = new ReadWriteLock())
            {
                using (x.EnterRead())
                {
                    Assert.IsFalse(x.IsCurrentThreadWriter);
                    using (x.EnterRead())
                    {
                        Assert.IsFalse(x.IsCurrentThreadWriter);
                    }
                }
            }
        }

        [Test]
        public void CanReentryReadToReadEx()
        {
            using (var x = new ReadWriteLock())
            {
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterRead(out engagementA))
                    {
                        Assert.IsFalse(x.IsCurrentThreadWriter);
                        IDisposable engagementB = null;
                        try
                        {
                            if (x.TryEnterRead(out engagementB))
                            {
                                Assert.IsFalse(x.IsCurrentThreadWriter);
                            }
                            else
                            {
                                Assert.Fail();
                            }
                        }
                        finally
                        {
                            if (engagementB != null)
                            {
                                engagementB.Dispose();
                            }
                        }
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                }
            }
        }

        [Test]
        public void CanReentryReadToWrite()
        {
            using (var x = new ReadWriteLock(true))
            {
                using (x.EnterRead())
                {
                    Assert.IsFalse(x.IsCurrentThreadWriter);
                    // If a thread is a reader it can become a writer as long as there are no other readers
                    using (x.EnterWrite())
                    {
                        Assert.IsTrue(x.IsCurrentThreadWriter);
                    }
                }
            } // This code results in a dead lock in a not reentrant ReadWriteLock
        }

        [Test]
        public void CanReentryReadToWriteEx()
        {
            using (var x = new ReadWriteLock(false))
            {
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterRead(out engagementA))
                    {
                        Assert.IsFalse(x.IsCurrentThreadWriter);
                        IDisposable engagementB = null;
                        try
                        {
                            if (x.TryEnterWrite(out engagementB))
                            {
                                Assert.Fail();
                            }
                            else
                            {
                                // Not reentrant ReadWriteLock will not be able to upgrade the lock
                                Assert.IsFalse(x.IsCurrentThreadWriter);
                            }
                        }
                        finally
                        {
                            if (engagementB != null)
                            {
                                engagementB.Dispose();
                            }
                        }
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                }
                //
                using (var y = new ReadWriteLock(true))
                {
                    IDisposable engagementC = null;
                    try
                    {
                        if (y.TryEnterRead(out engagementC))
                        {
                            Assert.IsFalse(y.IsCurrentThreadWriter);
                            IDisposable engagementD = null;
                            try
                            {
                                if (y.TryEnterWrite(out engagementD))
                                {
                                    // Reentrant ReadWriteLock will be able to upgrade the lock
                                    Assert.IsTrue(y.IsCurrentThreadWriter);
                                }
                                else
                                {
                                    Assert.Fail();
                                }
                            }
                            finally
                            {
                                if (engagementD != null)
                                {
                                    engagementD.Dispose();
                                }
                            }
                        }
                        else
                        {
                            Assert.Fail();
                        }
                    }
                    finally
                    {
                        if (engagementC != null)
                        {
                            engagementC.Dispose();
                        }
                    }
                }
            }
        }

        [Test]
        public void CanReentryWriteToRead()
        {
            using (var x = new ReadWriteLock())
            {
                using (x.EnterWrite())
                {
                    // If a thread is a writer it can be also a reader
                    using (x.EnterRead())
                    {
                    }
                }
            }
        }

        [Test]
        public void CanReentryWriteToReadEx()
        {
            using (var x = new ReadWriteLock())
            {
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterWrite(out engagementA))
                    {
                        Assert.IsTrue(x.IsCurrentThreadWriter);
                        IDisposable engagementB = null;
                        try
                        {
                            if (x.TryEnterRead(out engagementB))
                            {
                                Assert.IsTrue(x.IsCurrentThreadWriter);
                            }
                            else
                            {
                                Assert.Fail();
                            }
                        }
                        finally
                        {
                            if (engagementB != null)
                            {
                                engagementB.Dispose();
                            }
                        }
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                }
            }
        }

        [Test]
        public void CanReentryWriteToWriteEx()
        {
            using (var x = new ReadWriteLock())
            {
                IDisposable engagementA = null;
                try
                {
                    if (x.TryEnterWrite(out engagementA))
                    {
                        Assert.IsTrue(x.IsCurrentThreadWriter);
                        IDisposable engagementB = null;
                        try
                        {
                            if (x.TryEnterWrite(out engagementB))
                            {
                                Assert.IsTrue(x.IsCurrentThreadWriter);
                            }
                            else
                            {
                                Assert.Fail();
                            }
                        }
                        finally
                        {
                            if (engagementB != null)
                            {
                                engagementB.Dispose();
                            }
                        }
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                finally
                {
                    if (engagementA != null)
                    {
                        engagementA.Dispose();
                    }
                }
            }
        }

        [Test]
        public void MultipleReadersAtTheTime()
        {
            using (var w = new ManualResetEvent(false))
            {
                using (var x = new ReadWriteLock())
                {
                    int[] z = { 0 };
                    var threads = new Thread[5];
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index] = new Thread
                        (
                            () =>
                            {
                                w.WaitOne();
                                using (x.EnterRead())
                                {
                                    Interlocked.Increment(ref z[0]);
                                    Thread.Sleep(10);
                                }
                            }
                        );
                    }
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Start();
                    }
                    w.Set();
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Join();
                    }
                    Assert.AreEqual(6, Interlocked.Increment(ref z[0]));
                }
            }
        }

        [Test]
        public void OnlyOneWriterAtTheTime()
        {
            using (var w = new ManualResetEvent(false))
            {
                using (var x = new ReadWriteLock())
                {
                    int[] z = { 0 };
                    var ok = true;
                    var threads = new Thread[5];
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index] = new Thread
                        (
                            () =>
                            {
                                w.WaitOne();
                                using (x.EnterWrite())
                                {
                                    var got = Interlocked.Increment(ref z[0]);
                                    Thread.Sleep(10);
                                    ok = ok && Interlocked.Increment(ref z[0]) == got + 1;
                                }
                            }
                        );
                    }
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Start();
                    }
                    w.Set();
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Join();
                    }
                    Assert.IsTrue(ok);
                    Assert.AreEqual(11, Interlocked.Increment(ref z[0]));
                }
            }
        }

        [Test]
        public void OnlyOneWriterAtTheTimeEx()
        {
            using (var w = new ManualResetEvent(false))
            {
                using (var x = new ReadWriteLock())
                {
                    var doneThread = 0;
                    var ok = true;
                    ThreadStart tmp = () =>
                    {
                        w.WaitOne();
                        IDisposable engagementA = null;
                        try
                        {
                            ok &= !x.TryEnterWrite(out engagementA);
                        }
                        finally
                        {
                            if (engagementA != null)
                            {
                                engagementA.Dispose();
                            }
                            Interlocked.Increment(ref doneThread);
                        }
                    };
                    var a = new Thread(tmp);
                    using (x.EnterWrite())
                    {
                        a.Start();
                        w.Set();
                        Thread.Sleep(10);
                    }
                    a.Join();
                    Assert.IsTrue(ok);
                    Assert.AreEqual(1, doneThread);
                    var b = new Thread(tmp);
                    IDisposable engagementB = null;
                    try
                    {
                        if (x.TryEnterWrite(out engagementB))
                        {
                            Assert.IsTrue(x.IsCurrentThreadWriter);
                            b.Start();
                            w.Set();
                            b.Join();
                        }
                        else
                        {
                            Assert.Fail();
                        }
                    }
                    finally
                    {
                        if (engagementB != null)
                        {
                            engagementB.Dispose();
                        }
                    }
                    Assert.IsTrue(ok);
                    Assert.AreEqual(2, doneThread);
                }
            }
        }

        [Test]
        public void ReentryReadToWriteCheck()
        {
            using (var w = new ManualResetEvent(false))
            {
                using (var x = new ReadWriteLock())
                {
                    var enterCount = 0;
                    var doneCount = 0;
                    var errorCount = 0;
                    var successCount = 0;
                    ThreadStart tmp = () =>
                    {
                        using (x.EnterRead())
                        {
                            Interlocked.Increment(ref enterCount);
                            w.WaitOne();
                            // If a thread is a reader it can become a writer as long as there are no other readers
                            // When we have multiple readers trying to become a writer...
                            IDisposable engagement = null;
                            try
                            {
                                // Write mode is not requested - there are other readers which we don't wait to leave
                                if (x.TryEnterWrite(out engagement))
                                {
                                    Interlocked.Increment(ref successCount);
                                }
                                else
                                {
                                    Interlocked.Increment(ref errorCount);
                                }
                            }
                            finally
                            {
                                if (engagement != null)
                                {
                                    engagement.Dispose();
                                }
                            }
                        }
                        Interlocked.Increment(ref doneCount);
                    };
                    var threads = new Thread[5];
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index] = new Thread(tmp);
                    }
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Start();
                    }
                    Thread.Sleep(10);
                    Assert.AreEqual(5, enterCount);
                    w.Set();
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Join();
                    }
                    Assert.AreEqual(5, doneCount);
                    Assert.AreEqual(0, successCount); // None succeds
                    Assert.AreEqual(5, errorCount); // All fail
                }
            }
        }

        [Test]
        public void ReentryReadToWriteRaceCondition()
        {
            using (var w = new ManualResetEvent(false))
            {
                using (var x = new ReadWriteLock(true))
                {
                    var enterCount = 0;
                    var doneCount = 0;
                    var errorCount = 0;
                    var successCount = 0;
                    ThreadStart tmp = () =>
                    {
                        using (x.EnterRead())
                        {
                            Interlocked.Increment(ref enterCount);
                            w.WaitOne();
                            // If a thread is a reader it can become a writer as long as there are no other readers
                            // When we have multiple readers trying to become a writer...
                            try
                            {
                                // write mode is requested and reserved by one thread - others fail
                                using (x.EnterWrite())
                                {
                                    Interlocked.Increment(ref successCount);
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                Interlocked.Increment(ref errorCount);
                            }
                        }
                        Interlocked.Increment(ref doneCount);
                    };
                    var threads = new Thread[5];
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index] = new Thread(tmp);
                    }
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Start();
                    }
                    do
                    {
                        Thread.Sleep(10);
                    } while (enterCount < 5);
                    w.Set();
                    for (int index = 0; index < 5; index++)
                    {
                        threads[index].Join();
                    }
                    Assert.AreEqual(5, doneCount);
                    Assert.AreEqual(1, successCount); // One succeds - the thread that succeds to reserve write waits for others to leave
                    Assert.AreEqual(4, errorCount); // The others get InvalidOperationException - the threads that fail the reserve fail
                } // This code results in a dead lock in a not reentrant ReadWriteLock
            }
        }

        [Test]
        public void WriteWaitsMultipleReadsToFinish()
        {
            using (var w0 = new ManualResetEvent(false))
            {
                using (var w1 = new ManualResetEvent(false))
                {
                    using (var x = new ReadWriteLock())
                    {
                        var ok = false;
                        int[] z = { 0 };
                        var threads = new Thread[5];
                        for (int index = 0; index < 5; index++)
                        {
                            threads[index] = new Thread
                            (
                                () =>
                                {
                                    w0.WaitOne();
                                    using (x.EnterRead())
                                    {
                                        w1.Set();
                                        Interlocked.Increment(ref z[0]);
                                        Thread.Sleep(10);
                                    }
                                }
                            );
                        }
                        var a = new Thread
                        (
                            () =>
                            {
                                w1.WaitOne();
                                using (x.EnterWrite())
                                {
                                    Assert.IsTrue(x.IsCurrentThreadWriter);
                                    ok = Interlocked.Increment(ref z[0]) == 6;
                                }
                            }
                        );
                        for (int index = 0; index < 5; index++)
                        {
                            threads[index].Start();
                        }
                        a.Start();
                        w0.Set();
                        for (int index = 0; index < 5; index++)
                        {
                            threads[index].Join();
                        }
                        a.Join();
                        Assert.IsTrue(ok);
                        Assert.AreEqual(7, Interlocked.Increment(ref z[0]));
                    }
                }
            }
        }

        [Test]
        public void WriteWaitsReadToFinish()
        {
            using (var w = new ManualResetEvent(false))
            {
                using (var x = new ReadWriteLock())
                {
                    int[] z = { 0 };
                    var ok = true;
                    var a = new Thread
                    (
                        () =>
                        {
                            w.WaitOne();
                            using (x.EnterWrite())
                            {
                                ok = ok && Interlocked.Increment(ref z[0]) == 2;
                            }
                        }
                    );
                    var b = new Thread
                    (
                        () =>
                        {
                            using (x.EnterRead())
                            {
                                w.Set();
                                Thread.Sleep(10);
                                ok = ok && Interlocked.Increment(ref z[0]) == 1;
                            }
                        }
                    );
                    a.Start();
                    b.Start();
                    a.Join();
                    b.Join();
                    Assert.IsTrue(ok);
                    Assert.AreEqual(3, Interlocked.Increment(ref z[0]));
                }
            }
        }
    }
}

#endif