#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

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

using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestLambda
    {
        private delegate object DelegateObjectEmtpy();

        private delegate object DelegateObjectINT(int a);

        private delegate object DelegateObjectString(string s);

        private delegate object DelegateObjectObject(object s);

        public static void Foo()
        {
            // Empty
        }

        public static int CallDelegate(Func<int, int> e)
        {
            if (e == null)
            {
                return 0;
            }

            return e(42);
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
        public void Compile()
        {
            var lambda = Expression.Lambda<Func<int>>(Expression.Constant(1));
            Assert.AreEqual(typeof(Func<int>), lambda.Type);
            Assert.AreEqual("() => 1", lambda.ToString());

            var compiled = lambda.Compile();
            compiled();
        }

        [Test]
        public void InvalidArgCount()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // missing a parameter
                    Expression.Lambda(typeof(DelegateObjectINT), Expression.Constant("foo"));
                }
            );
        }

        [Test]
        public void InvalidArgCount2()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // extra parameter
                    var p = Expression.Parameter(typeof(int), "AAA");
                    Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant("foo"), p);
                }
            );
        }

        [Test]
        public void InvalidArgType()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // invalid argument type
                    var p = Expression.Parameter(typeof(string), "AAA");
                    Expression.Lambda(typeof(DelegateObjectINT), Expression.Constant("foo"), p);
                }
            );
        }

        [Test]
        public void InvalidArgType2()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // invalid argument type
                    var p = Expression.Parameter(typeof(string), "AAA");
                    Expression.Lambda(typeof(DelegateObjectObject), Expression.Constant("foo"), p);
                }
            );
        }

        [Test]
        public void InvalidConversion()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // float to object, invalid
                    Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant(1.0));
                }
            );
        }

        [Test]
        public void InvalidConversion2()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // float to object, invalid
                    Expression.Lambda(typeof(DelegateObjectEmtpy), Expression.Constant(1));
                }
            );
        }

        [Test]
        public void LambdaPassedAsDelegate()
        {
            var pi = Expression.Parameter(typeof(int), "i");
            var identity = Expression.Lambda<Func<int, int>>(pi, pi);

            var compiled = Expression.Lambda<Func<int>>
            (
                Expression.Call
                (
                    GetType().GetMethod("CallDelegate"),
                    identity
                )
            ).Compile();

            Assert.AreEqual(42, compiled());
        }

        [Test]
        public void LambdaPassedAsDelegateUsingParentParameter()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");

            var compiled = Expression.Lambda<Func<int, int>>
            (
                Expression.Call
                (
                    GetType().GetMethod("CallDelegate"),
                    Expression.Lambda<Func<int, int>>
                    (
                        Expression.Multiply(a, b), b
                    )
                ),
                a
            ).Compile();

            Assert.AreEqual(84, compiled(2));
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
        public void NestedParentParameterUse()
        {
            var a = Expression.Parameter(typeof(int), null);
            var b = Expression.Parameter(typeof(int), null);
            var c = Expression.Parameter(typeof(int), null);
            var d = Expression.Parameter(typeof(int), null);

            var compiled = Expression.Lambda<Func<int, int>>
            (
                Expression.Call
                (
                    GetType().GetMethod("CallFunc"),
                    Expression.Lambda<Func<int, int>>
                    (
                        Expression.Call
                        (
                            GetType().GetMethod("CallFunc"),
                            Expression.Lambda<Func<int, int>>
                            (
                                Expression.Call
                                (
                                    GetType().GetMethod("CallFunc"),
                                    Expression.Lambda<Func<int, int>>
                                    (
                                        Expression.Add(c, d),
                                        d
                                    ),
                                    Expression.Add(b, c)
                                ),
                                c
                            ),
                            Expression.Add(a, b)
                        ),
                        b
                    ),
                    a
                ),
                a
            ).Compile();

            Assert.AreEqual(5, compiled(1));
        }

        [Test]
        public void NonDelegateTypeInCtor()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // The first parameter must be a delegate type
                    Expression.Lambda(typeof(string), Expression.Constant(1));
                }
            );
        }

        [Test]
        public void NullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Lambda<Func<int, int>>(Expression.Constant(1), new ParameterExpression[] {null}));
        }

        [Test]
        public void ParameterOutOfScope()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    var a = Expression.Parameter(typeof(int), "a");
                    var secondA = Expression.Parameter(typeof(int), "a");

                    // Here we have the same name for the parameter expression, but
                    // we pass a different object to the Lambda expression, so they are
                    // different, this should throw
                    var lambda = Expression.Lambda<Func<int, int>>(a, secondA);
                    lambda.Compile();
                }
            );
        }

        [Test]
        public void ParameterRefTest()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");

            var lambda = Expression.Lambda<Func<int, int, int>>
            (
                Expression.Add(a, b), a, b
            );

            Assert.AreEqual(typeof(Func<int, int, int>), lambda.Type);
            Assert.AreEqual("(a, b) => (a + b)", lambda.ToString());

            var compiled = lambda.Compile();
            var res = compiled(10, 20);
            Assert.AreEqual(res, 30);
        }

        [Test]
        public void ReturnValueCheck()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var p1 = Expression.Parameter(typeof(int?), "va");
                    var p2 = Expression.Parameter(typeof(int?), "vb");
                    Expression add = Expression.Add(p1, p2);

                    // This should throw, since the add.Type is "int?" and the return
                    // type we have here is int.
                    Expression.Lambda<Func<int?, int?, int>>(add, p1, p2);
                }
            );
        }

        [Test]
        public void UnTypedLambdaReturnsExpressionOfDelegateType()
        {
            var l = Expression.Lambda("foo".ToConstant());

            Assert.IsTrue(l is Expression<Func<string>>);
        }
    }
}