// AggregateExceptionTests.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
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
//
//

#define NET_4_0
#if NET_4_0

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoTests.System
{
    [TestFixture]
    public class AggregateExceptionTest
    {
        private AggregateException _e;

        [SetUpAttribute]
        public void Setup()
        {
            _e = new AggregateException(new Exception("foo"), new AggregateException(new Exception("bar"), new Exception("foobar")));
        }

        [Test]
        public void SimpleInnerExceptionTestCase()
        {
            const string message = "Foo";
            var inner = new ApplicationException(message);
            var ex = new AggregateException(inner);

            Assert.IsNotNull(ex.InnerException);
            Assert.IsNotNull(ex.InnerExceptions);

            Assert.AreEqual(inner, ex.InnerException);
            Assert.AreEqual(1, ex.InnerExceptions.Count);
            Assert.AreEqual(inner, ex.InnerExceptions[0]);
            Assert.AreEqual(message, ex.InnerException.Message);
            Assert.AreEqual(inner, ex.GetBaseException());
        }

        [TestAttribute]
        public void FlattenTestCase()
        {
            var ex = _e.Flatten();

            Assert.AreEqual(3, ex.InnerExceptions.Count, "#1");
            Assert.AreEqual(3, ex.InnerExceptions.Count((exception) => !(exception is AggregateException)), "#2");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void InitializationWithNullInnerValuesTest()
        {
            var foo = new AggregateException(new[] { new Exception(), null, new ApplicationException() });
        }

        [Test]
        public void InitializationWithNullValuesTest()
        {
            Throws(typeof(ArgumentNullException), () => new AggregateException((IEnumerable<Exception>)null));
            Throws(typeof(ArgumentNullException), () => new AggregateException((Exception[])null));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_Invalid()
        {
            _e.Handle(null);
        }

        [Test]
        public void Handle_AllHandled()
        {
            _e.Handle(l => true);
        }

        [Test]
        public void Handle_Unhandled()
        {
            try
            {
                _e.Handle(l => l is AggregateException);
                Assert.Fail();
            }
            catch (AggregateException e)
            {
                Assert.AreEqual(1, e.InnerExceptions.Count);
            }
        }

        [Test]
        public void GetBaseWithInner()
        {
            var ae = new AggregateException("x", new[] { new ArgumentException(), new ArgumentNullException() });
            Assert.AreEqual(ae, ae.GetBaseException(), "#1");

            var expected = new ArgumentException();
            var ae2 = new AggregateException("x", new AggregateException(expected, new Exception()));
            Assert.AreEqual(expected, ae2.GetBaseException().InnerException, "#2");
        }

        [Test]
        public void GetBaseException_stops_at_first_inner_exception_that_is_not_AggregateException()
        {
            var inner = new ArgumentNullException();
            var outer = new InvalidOperationException("x", inner);
            Assert.AreEqual(outer, new AggregateException(outer).GetBaseException());
        }

        private static void Throws(Type t, Action action)
        {
            if (action == null)
            {
                return;
            }
            Exception e = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                e = ex;
            }

            if (e == null || e.GetType() != t)
            {
                Assert.Fail();
            }
        }
    }
}

#endif