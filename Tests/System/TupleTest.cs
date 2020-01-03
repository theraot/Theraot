//
// TupleTest.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2014 Xamarin Inc (http://www.xamarin.com)
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

namespace MonoTests.System
{
    [TestFixture]
    public class TupleTest
    {
        [Test]
        public void ToStringTest()
        {
            var t1 = new Tuple<int, object, int, int, int, int, int, Tuple<string, string>>(1, null, 3, 4, 5, 6, 7, new Tuple<string, string>(null, null));

            Assert.AreEqual("(1, , 3, 4, 5, 6, 7, , )", t1.ToString(), "#1");
        }

        [Test]
        public void TupleWithRest_Invalid()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var tuple = new Tuple<int, int, int, int, int, int, int, int>(1, 2, 3, 4, 5, 6, 7, 8);
                    GC.KeepAlive(tuple);
                }
            );
        }

        [Test]
        public void TupleWithRest_InvalidDueToNull()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var tuple = new Tuple<int, object, int, int, int, int, int, Tuple<string, string>>(1, null, 3, 4, 5, 6,
                        7, null);
                    GC.KeepAlive(tuple);
                }
            );
        }
    }
}