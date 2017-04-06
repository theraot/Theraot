#define NET_4_0
#if NET_4_0
//
// SpinLockTests.cs
//
// Author:
//       Jérémie "Garuma" Laval <jeremie.laval@gmail.com>
//
// Copyright (c) 2010 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public class SpinLockTests
    {
        private SpinLock _sl;

        [SetUp]
        public void Setup()
        {
            _sl = new SpinLock(true);
        }

        [Test, ExpectedException(typeof(LockRecursionException))]
        public void RecursionExceptionTest()
        {
            _sl = new SpinLock(true);
            var taken = false;
            var taken2 = false;

            _sl.Enter(ref taken);
            Assert.IsTrue(taken, "#1");
            _sl.Enter(ref taken2);
        }

        [Test]
        public void SimpleEnterExitSchemeTest()
        {
            var taken = false;

            for (int i = 0; i < 50000; i++)
            {
                _sl.Enter(ref taken);
                Assert.IsTrue(taken, "#" + i.ToString());
                _sl.Exit();
                taken = false;
            }
        }

        [Test]
        public void SemanticCorrectnessTest()
        {
            _sl = new SpinLock(false);

            var taken = false;
            var taken2 = false;

            _sl.Enter(ref taken);
            Assert.IsTrue(taken, "#1");
            _sl.TryEnter(ref taken2);
            Assert.IsFalse(taken2, "#2");
            _sl.Exit();

            _sl.TryEnter(ref taken2);
            Assert.IsTrue(taken2, "#3");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void FirstTakenParameterTest()
        {
            var taken = true;

            _sl.Enter(ref taken);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void SecondTakenParameterTest()
        {
            var taken = true;

            _sl.TryEnter(ref taken);
        }

        internal class SpinLockWrapper
        {
            public SpinLock Lock = new SpinLock(false);
        }

        [Test]
        public void LockUnicityTest()
        {
            ParallelTestHelper.Repeat(delegate
            {
                var currentCount = 0;
                var fail = false;
                var wrapper = new SpinLockWrapper();

                ParallelTestHelper.ParallelStressTest(wrapper, delegate
                {
                    var taken = false;
                    wrapper.Lock.Enter(ref taken);
                    var current = currentCount++;
                    if (current != 0)
                    {
                        fail = true;
                    }

                    var sw = new SpinWait();
                    for (int i = 0; i < 200; i++)
                    {
                        sw.SpinOnce();
                    }

                    currentCount--;

                    wrapper.Lock.Exit();
                }, 4);

                Assert.IsFalse(fail);
            }, 200);
        }

        [Test]
        public void IsHeldByCurrentThreadTest()
        {
            var lockTaken = false;

            _sl.Enter(ref lockTaken);
            Assert.IsTrue(lockTaken, "#1");
            Assert.IsTrue(_sl.IsHeldByCurrentThread, "#2");

            lockTaken = false;
            _sl = new SpinLock(true);

            _sl.Enter(ref lockTaken);
            Assert.IsTrue(lockTaken, "#3");
            Assert.IsTrue(_sl.IsHeldByCurrentThread, "#4");
        }
    }
}

#endif