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
//
// Authors:
//   Miguel de Icaza (miguel@novell.com)
//   Jb Evain (jbevain@novell.com)
//

using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestLambda
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NonDelegateTypeInCtor()
        {
            // The first parameter must be a delegate type
            Expression.Lambda(typeof(string), Expression.Constant(1));
        }

        private delegate object DelegateObjectEmtpy();

        private delegate object DelegateObjectINT(int a);

        private delegate object DelegateObjectString(string s);

        private delegate object DelegateObjectObject(object s);

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidConversion()
        {
            // float to object, invalid
            Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant(1.0));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidConversion2()
        {
            // float to object, invalid
            Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant(1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgCount()
        {
            // missing a parameter
            Expression.Lambda(typeof(DelegateObjectINT), Expression.Constant("foo"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgCount2()
        {
            // extra parameter
            var p = Expression.Parameter(typeof(int), "AAA");
            Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant("foo"), p);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgType()
        {
            // invalid argument type
            var p = Expression.Parameter(typeof(string), "AAA");
            Expression.Lambda(typeof(DelegateObjectINT), Expression.Constant("foo"), p);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgType2()
        {
            // invalid argument type

            var p = Expression.Parameter(typeof(string), "AAA");
            Expression.Lambda(typeof(DelegateObjectObject), Expression.Constant("foo"), p);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParameter()
        {
            Expression.Lambda<Func<int, int>>(Expression.Constant(1), new ParameterExpression[] { null });
        }

        [Test]
        public void Assignability()
        {
            // allowed: string to object
            Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant("string"));

            // allowed delegate has string, delegate has base class (object)
            var p = Expression.Parameter(typeof(object), "ParObject");
            var l = Expression.Lambda(typeof(DelegateObjectString), Expression.Constant(""), p);

            Assert.AreEqual("ParObject => \"\"", l.ToString());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParameterOutOfScope()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var secondA = Expression.Parameter(typeof(int), "a");

            // Here we have the same name for the parameter expression, but
            // we pass a different object to the Lambda expression, so they are
            // different, this should throw
            var l = Expression.Lambda<Func<int, int>>(a, secondA);
            l.Compile();
        }

        [Test]
        public void ParameterRefTest()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");

            var l = Expression.Lambda<Func<int, int, int>>(
                Expression.Add(a, b), a, b);

            Assert.AreEqual(typeof(Func<int, int, int>), l.Type);
            Assert.AreEqual("(a, b) => (a + b)", l.ToString());

            var xx = l.Compile();
            var res = xx(10, 20);
            Assert.AreEqual(res, 30);
        }

        [Test]
        public void Compile()
        {
            var l = Expression.Lambda<Func<int>>(Expression.Constant(1));
            Assert.AreEqual(typeof(Func<int>), l.Type);
            Assert.AreEqual("() => 1", l.ToString());

            var fi = l.Compile();
            fi();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ReturnValueCheck()
        {
            var p1 = Expression.Parameter(typeof(int?), "va");
            var p2 = Expression.Parameter(typeof(int?), "vb");
            Expression add = Expression.Add(p1, p2);

            // This should throw, since the add.Type is "int?" and the return
            // type we have here is int.
            Expression.Lambda<Func<int?, int?, int>>(add, p1, p2);
        }

        public static void Foo()
        {
        }

        [Test]
        public void LambdaType()
        {
            var l = Expression.Lambda(Expression.Constant(1), Expression.Parameter(typeof(int), "foo"));

            Assert.AreEqual(typeof(Func<int, int>), l.Type);

            l = Expression.Lambda(Expression.Call(null, GetType().GetMethod("Foo")), Expression.Parameter(typeof(string), "foofoo"));

            Assert.AreEqual(typeof(Action<string>), l.Type);
        }

        [Test]
        public void UnTypedLambdaReturnsExpressionOfDelegateType()
        {
            var l = Expression.Lambda("foo".ToConstant());

            Assert.IsTrue(l is Expression<Func<string>>);
        }

        public static int CallDelegate(Func<int, int> e)
        {
            if (e == null)
            {
                return 0;
            }
            return e(42);
        }

        [Test]
        public void LambdaPassedAsDelegate()
        {
            var pi = Expression.Parameter(typeof(int), "i");
            var identity = Expression.Lambda<Func<int, int>>(pi, pi);

            var l = Expression.Lambda<Func<int>>(
                Expression.Call(
                    GetType().GetMethod("CallDelegate"),
                    identity)).Compile();

            Assert.AreEqual(42, l());
        }

        [Test]
        public void LambdaPassedAsDelegateUsingParentParameter()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");

            var l = Expression.Lambda<Func<int, int>>(
                Expression.Call(
                    GetType().GetMethod("CallDelegate"),
                    Expression.Lambda<Func<int, int>>(
                            Expression.Multiply(a, b), b)),
                a).Compile();

            Assert.AreEqual(84, l(2));
        }

        public static int CallFunc(Func<int, int> e, int i)
        {
            if (e == null)
            {
                return 0;
            }
            return e(i);
        }

        [Test]
        public void NestedParentParameterUse()
        {
            var a = Expression.Parameter(typeof(int), null);
            var b = Expression.Parameter(typeof(int), null);
            var c = Expression.Parameter(typeof(int), null);
            var d = Expression.Parameter(typeof(int), null);

            var l = Expression.Lambda<Func<int, int>>(
                Expression.Call(
                    GetType().GetMethod("CallFunc"),
                    Expression.Lambda<Func<int, int>>(
                        Expression.Call(
                            GetType().GetMethod("CallFunc"),
                            Expression.Lambda<Func<int, int>>(
                                Expression.Call(
                                    GetType().GetMethod("CallFunc"),
                                    Expression.Lambda<Func<int, int>>(
                                        Expression.Add(c, d),
                                        d),
                                    Expression.Add(b, c)),
                                c),
                            Expression.Add(a, b)),
                        b),
                    a),
                a).Compile();

            Assert.AreEqual(5, l(1));
        }
    }
}