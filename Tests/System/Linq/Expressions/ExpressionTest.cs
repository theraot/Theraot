#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTest
    {
        [Test]
        public void GetFuncTypeArgNull()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.GetFuncType(null));
        }

        private static Type[] GetTestTypeArray(int length)
        {
            return Enumerable.Range(0, length - 1)
                .Select(i => typeof(int))
                .ToArray();
        }

        [Test]
        public void GetFuncTypeArgEmpty()
        {
            Assert.Throws<ArgumentException>(() => Expression.GetFuncType(ArrayEx.Empty<Type>()));
        }

        [Test]
        public void GetFuncTypeArgTooBig()
        {
            Assert.Throws<ArgumentException>(() => Expression.GetFuncType(GetTestTypeArray(64)));
        }

        [Test]
        public void GetFuncTypeTest()
        {
            var func = Expression.GetFuncType(new[] { typeof(int) });
            Assert.AreEqual(typeof(Func<int>), func);

            func = Expression.GetFuncType(new[] { typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Func<int, int>), func);

            func = Expression.GetFuncType(new[] { typeof(int), typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Func<int, int, int>), func);

            func = Expression.GetFuncType(new[] { typeof(int), typeof(int), typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Func<int, int, int, int>), func);

            func = Expression.GetFuncType(new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Func<int, int, int, int, int>), func);
        }

        [Test]
        public void GetActionTypeArgNull()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.GetActionType(null));
        }

        [Test]
        public void GetActionTypeArgTooBig()
        {
            Assert.Throws<ArgumentException>(() => Expression.GetActionType(GetTestTypeArray(45)));
        }

        [Test]
        public void GetActionTypeTest()
        {
            var action = Expression.GetActionType(new Type[0]);
            Assert.AreEqual(typeof(Action), action);

            action = Expression.GetActionType(new[] { typeof(int) });
            Assert.AreEqual(typeof(Action<int>), action);

            action = Expression.GetActionType(new[] { typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Action<int, int>), action);

            action = Expression.GetActionType(new[] { typeof(int), typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Action<int, int, int>), action);

            action = Expression.GetActionType(new[] { typeof(int), typeof(int), typeof(int), typeof(int) });
            Assert.AreEqual(typeof(Action<int, int, int, int>), action);
        }

        [Test]
        public void ParameterNullType()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Parameter(null, "foo"));
        }

        [Test]
        public void ParameterNullName()
        {
            var p = Expression.Parameter(typeof(string), null);
            Assert.AreEqual(null, p.Name);
            Assert.AreEqual(typeof(string), p.Type);
        }

        [Test]
        public void ParameterEmptyName()
        {
            var p = Expression.Parameter(typeof(string), "");
            Assert.AreEqual("", p.Name);
            Assert.AreEqual(typeof(string), p.Type);
        }

        [Test]
        public void Parameter()
        {
            var p = Expression.Parameter(typeof(string), "foo");
            Assert.AreEqual("foo", p.Name);
            Assert.AreEqual(typeof(string), p.Type);
            Assert.AreEqual("foo", p.ToString());
        }

        [Test]
        [Category("NotDotNet")]
        public void VoidParameter()
        {
            Assert.Throws<ArgumentException>(() => Expression.Parameter(typeof(void), "hello"));
        }

        private static int _buffer;

        public static int Identity(int i)
        {
            _buffer = i;
            return i;
        }

        [Test]
        public void CompileActionDiscardingRetValue()
        {
            var p = Expression.Parameter(typeof(int), "i");
            var identity = GetType().GetMethod("Identity", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(identity);

            var lambda = Expression.Lambda<Action<int>>(Expression.Call(identity, p), p);

            var method = lambda.Compile();

            _buffer = 0;

            method(42);
            Assert.AreEqual(42, _buffer);
        }

        [Test]
        public void ExpressionDelegateTarget()
        {
            var p = Expression.Parameter(typeof(string), "str");
            var identity = Expression.Lambda<Func<string, string>>(p, p).Compile();

            Assert.AreEqual(typeof(Func<string, string>), identity.GetType());
            Assert.IsNotNull(identity.Target);
        }

        [Test]
        public void SimpleHoistedParameter()
        {
            var p = Expression.Parameter(typeof(string), "s");

            var f = Expression.Lambda<Func<string, Func<string>>>(
                Expression.Lambda<Func<string>>(
                    p,
                    new ParameterExpression[0]),
                p).Compile();

            var f2 = f("x");

            Assert.AreEqual("x", f2());
        }

        [Test]
        public void TwoHoistingLevels()
        {
            var p1 = Expression.Parameter(typeof(string), "x");
            var p2 = Expression.Parameter(typeof(string), "y");

            var e =
                Expression.Lambda<Func<string, Func<string, Func<string>>>>(
                    Expression.Lambda<Func<string, Func<string>>>(
                        Expression.Lambda<Func<string>>(
                            Expression.Call(
                                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                                new Expression[] { p1, p2 }),
                            new ParameterExpression[0]),
                        new[] { p2 }),
                    new[] { p1 });

            var f = e.Compile();
            var f2 = f("Hello ");
            var f3 = f2("World !");

            Assert.AreEqual("Hello World !", f3());
        }

        [Test]
        public void HoistedParameter()
        {
            var i = Expression.Parameter(typeof(int), "i");

            var l = Expression.Lambda<Func<int, string>>(
                Expression.Invoke(
                    Expression.Lambda<Func<string>>(
                        Expression.Call(i, typeof(int).GetMethod("ToString", ArrayEx.Empty<Type>())))), i).Compile();

            Assert.AreEqual("42", l(42));
        }
    }
}