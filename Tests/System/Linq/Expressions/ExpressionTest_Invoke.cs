#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_Invoke.cs
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

using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestInvoke
    {
        private delegate string StringAction(string s);

        [Test]
        public void ArgumentCountDoesNotMatchParametersLength()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Invoke(CreateInvokable<Action<int>>(), 1.ToConstant(), 2.ToConstant()));
        }

        [Test]
        public void ArgumentNotAssignableToParameterType()
        {
            Assert.Throws<ArgumentException>(() => Expression.Invoke(CreateInvokable<Action<int>>(), "".ToConstant()));
        }

        [Test]
        public void EmptyArguments()
        {
            var invoke = Expression.Invoke(CreateInvokable<Action>(), null);
            Assert.AreEqual(typeof(void), invoke.Type);
            Assert.IsNotNull(invoke.Arguments);
            Assert.AreEqual(0, invoke.Arguments.Count);
            Assert.AreEqual("Invoke(invokable)", invoke.ToString());
        }

        [Test]
        [Category("NotDotNet")] // https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=352402
        public void InvokeFunc()
        {
            var invoke = Expression.Invoke(CreateInvokable<Func<string, string, int>>(), "foo".ToConstant(), "bar".ToConstant());
            Assert.AreEqual(typeof(int), invoke.Type);
            Assert.AreEqual(2, invoke.Arguments.Count);
            Assert.AreEqual("Invoke(invokable, \"foo\", \"bar\")", invoke.ToString());
        }

        [Test]
        [Category("NotDotNet")] // https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=352402
        public void InvokeLambda()
        {
            var p = Expression.Parameter(typeof(int), "i");
            var lambda = Expression.Lambda<Func<int, int>>(p, p);

            var invoke = Expression.Invoke(lambda, 1.ToConstant());
            Assert.AreEqual(typeof(int), invoke.Type);
            Assert.AreEqual(1, invoke.Arguments.Count);
            Assert.AreEqual("Invoke(i => i, 1)", invoke.ToString());
        }

        [Test]
        public void InvokeWithExpressionLambdaAsArguments()
        {
            var p = Expression.Parameter(typeof(string), "s");

            string Caller(string s, Expression<Func<string, string>> f)
            {
                return f.Compile().Invoke(s);
            }

            var invoke = Expression.Lambda<Func<string>>
            (
                Expression.Invoke
                (
                    Expression.Constant((Func<string, Expression<Func<string, string>>, string>)Caller),
                    Expression.Constant("KABOOM!"),
                    Expression.Lambda<Func<string, string>>
                    (
                        Expression.Call(p, typeof(string).GetMethod("ToLowerInvariant")), p
                    )
                )
            );

            Assert.AreEqual
            (
                ExpressionType.Quote,
                (invoke.Body as InvocationExpression).Arguments[1].NodeType
            );

            Assert.AreEqual("kaboom!", invoke.Compile().DynamicInvoke());
        }

        [Test]
        public void NonInvokableExpressionType()
        {
            Assert.Throws<ArgumentException>(() => Expression.Invoke(CreateInvokable<int>(), null));
        }

        [Test]
        public void NullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Invoke(CreateInvokable<Action<int>>(), null as Expression));
        }

        [Test]
        public void NullExpression()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Invoke(null));
        }

        [Test]
        public void TestCompileInvokePrivateDelegate()
        {
            var action = Expression.Parameter(typeof(StringAction), "action");
            var str = Expression.Parameter(typeof(string), "str");
            var compiled = Expression.Lambda<Func<StringAction, string, string>>
            (
                Expression.Invoke(action, str), action, str
            ).Compile();

            Assert.AreEqual("foo", compiled(Identity, "foo"));
        }

        private static Expression CreateInvokable<T>()
        {
            return Expression.Parameter(typeof(T), "invokable");
        }

        private static string Identity(string s)
        {
            return s;
        }
    }
}