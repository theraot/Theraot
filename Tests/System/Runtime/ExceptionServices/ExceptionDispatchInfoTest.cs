//
// ExceptionDispatchInfoTest.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2011 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace MonoTests.System.Runtime.ExceptionServices
{
    [TestFixture]
    public class ExceptionDispatchInfoTest
    {
        [Test]
        public void Capture_InvalidArguments()
        {
            try
            {
                ExceptionDispatchInfo.Capture(null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void Capture()
        {
            var e = new ApplicationException("test");
            var edi = ExceptionDispatchInfo.Capture(e);
            Assert.AreEqual(e, edi.SourceException);
        }

#if FAT

        [Test]
        public void Throw()
        {
            Exception orig = null;
            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    throw new ApplicationException("aaa");
                }
                catch (Exception e)
                {
                    orig = e;
                    return ExceptionDispatchInfo.Capture(e);
                }
            });

            var ed = t.Result;
            var origStack = orig.StackTrace;
            try
            {
                ed.Throw();
                Assert.Fail("#0");
            }
            catch (Exception e)
            {
                var s = e.StackTrace.Split('\n');
                Assert.AreEqual(4, s.Length, "#1");
                Assert.AreEqual(orig, e, "#2");
                Assert.AreNotEqual(origStack, e.StackTrace, "#3");
            }
        }

#endif

        [Test]
        [Category("NotDotNet")] // Failing in .NET 4.5
        public void ThrowWithEmptyFrames()
        {
            var edi = ExceptionDispatchInfo.Capture(new OperationCanceledException());
            try
            {
                edi.Throw();
                Assert.Fail("#0");
            }
            catch (OperationCanceledException e)
            {
                Assert.IsFalse(e.StackTrace.Contains("---"));
                Assert.AreEqual(2, e.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length);
            }
        }

        [Test]
        [Category("NotDotNet")] // Failing in .NET 4.5
        public void LastThrowWins()
        {
            Exception e;
            try
            {
                throw new Exception("test");
            }
            catch (Exception e2)
            {
                e = e2;
            }

            var edi = ExceptionDispatchInfo.Capture(e);

            try
            {
                edi.Throw();
            }
            catch (Exception ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                edi.Throw();
            }
            catch (Exception ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                edi.Throw();
            }
            catch (Exception ex)
            {
                var split = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Assert.AreEqual(4, split.Length, "#1");
                Assert.IsTrue(split[1].Contains("---"), "#2");
            }
        }

        [Test]
        [Category("NotDotNet")] // Failing in .NET 4.5
        public void ThrowMultipleCaptures()
        {
            Exception e;
            try
            {
                throw new Exception("test");
            }
            catch (Exception e2)
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine(e2.StackTrace);
                e = e2;
            }

            var edi = ExceptionDispatchInfo.Capture(e);

            try
            {
                edi.Throw();
            }
            catch (Exception e3)
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine(e3.StackTrace);
                edi = ExceptionDispatchInfo.Capture(e3);
            }

            try
            {
                edi.Throw();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine(ex.StackTrace);
                var split = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Assert.AreEqual(7, split.Length, "#1");
                Assert.IsTrue(split[1].Contains("---"), "#2");
                Assert.IsTrue(split[4].Contains("---"), "#3");
            }
        }
    }
}