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

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Tests.Helpers;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTest
    {
        private static int _buffer;

        public static int Identity(int i)
        {
            _buffer = i;
            return i;
        }

        private static Type[] GetTestTypeArray(int length)
        {
            return Enumerable.Range(0, length - 1)
                .Select(_ => typeof(int))
                .ToArray();
        }

        [Test]
        public void CompileActionDiscardingRetValue()
        {
            const string Name = "i";
            const int Value = 42;

            var parameter = Expression.Parameter(typeof(int), Name);
            var identity = GetType().GetMethod(nameof(Identity), BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(identity);

            var lambda = Expression.Lambda<Action<int>>(Expression.Call(identity, parameter), parameter);

            var method = lambda.Compile();

            _buffer = 0;

            method(Value);
            Assert.AreEqual(Value, _buffer);
        }

        [Test]
        public void ExpressionDelegateTarget()
        {
            const string Name = "str";

            var parameter = Expression.Parameter(typeof(string), Name);
            var method = Expression.Lambda<Func<string, string>>(parameter, parameter).Compile();

            Assert.AreEqual(typeof(Func<string, string>), method.GetType());
            Assert.IsNotNull(method.Target);
        }

        [Test]
        public void GetActionTypeArgNull()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.GetActionType(null));
        }

        [Test]
        public void GetActionTypeArgTooBig()
        {
            const int Value = 45; // > 16

            AssertEx.Throws<ArgumentException>(() => Expression.GetActionType(GetTestTypeArray(Value)));
        }

        [Test]
        public void GetActionTypeTest()
        {
            var action = Expression.GetActionType();
            Assert.AreEqual(typeof(Action), action);

            action = Expression.GetActionType(typeof(int));
            Assert.AreEqual(typeof(Action<int>), action);

            action = Expression.GetActionType(typeof(int), typeof(int));
            Assert.AreEqual(typeof(Action<int, int>), action);

            action = Expression.GetActionType(typeof(int), typeof(int), typeof(int));
            Assert.AreEqual(typeof(Action<int, int, int>), action);

            action = Expression.GetActionType(typeof(int), typeof(int), typeof(int), typeof(int));
            Assert.AreEqual(typeof(Action<int, int, int, int>), action);
        }

        [Test]
        public void GetFuncTypeArgEmpty()
        {
            AssertEx.Throws<ArgumentException>(() => Expression.GetFuncType(ArrayEx.Empty<Type>()));
        }

        [Test]
        public void GetFuncTypeArgNull()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.GetFuncType(null));
        }

        [Test]
        public void GetFuncTypeArgTooBig()
        {
            const int Value = 64; // > 17

            AssertEx.Throws<ArgumentException>(() => Expression.GetFuncType(GetTestTypeArray(Value)));
        }

        [Test]
        public void GetFuncTypeTest()
        {
            var func = Expression.GetFuncType(typeof(int));
            Assert.AreEqual(typeof(Func<int>), func);

            func = Expression.GetFuncType(typeof(int), typeof(int));
            Assert.AreEqual(typeof(Func<int, int>), func);

            func = Expression.GetFuncType(typeof(int), typeof(int), typeof(int));
            Assert.AreEqual(typeof(Func<int, int, int>), func);

            func = Expression.GetFuncType(typeof(int), typeof(int), typeof(int), typeof(int));
            Assert.AreEqual(typeof(Func<int, int, int, int>), func);

            func = Expression.GetFuncType(typeof(int), typeof(int), typeof(int), typeof(int), typeof(int));
            Assert.AreEqual(typeof(Func<int, int, int, int, int>), func);
        }

        [Test]
        public void HoistedParameter()
        {
            const string Name = "i";
            const int Value = 42;

            var parameter = Expression.Parameter(typeof(int), Name);

            var method = Expression.Lambda<Func<int, string>>
            (
                Expression.Invoke
                (
                    Expression.Lambda<Func<string>>
                    (
                        Expression.Call(parameter, typeof(int).GetMethod(nameof(int.ToString), ArrayEx.Empty<Type>()))
                    )
                ),
                parameter
            ).Compile();

            Assert.AreEqual($"{Value}", method(Value));
        }

        [Test]
        public void Parameter()
        {
            const string Name = "foo";

            var parameter = Expression.Parameter(typeof(string), Name);
            Assert.AreEqual(Name, parameter.Name);
            Assert.AreEqual(typeof(string), parameter.Type);
            Assert.AreEqual(Name, parameter.ToString());
        }

        [Test]
        public void ParameterEmptyName()
        {
            const string Name = ""; // ""

            var parameter = Expression.Parameter(typeof(string), Name);
            Assert.AreEqual(Name, parameter.Name);
            Assert.AreEqual(typeof(string), parameter.Type);
        }

        [Test]
        public void ParameterNullName()
        {
            const string Name = null; // null

            var parameter = Expression.Parameter(typeof(string), Name);
            Assert.AreEqual(Name, parameter.Name);
            Assert.AreEqual(typeof(string), parameter.Type);
        }

        [Test]
        public void ParameterNullType()
        {
            const string Name = "foo";
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.Parameter(null, Name));
        }

        [Test]
        public void SimpleHoistedParameter()
        {
            const string Name = "s";
            const string Value = "x";

            var parameter = Expression.Parameter(typeof(string), Name);

            var method = Expression.Lambda<Func<string, Func<string>>>
            (
                Expression.Lambda<Func<string>>(parameter),
                parameter
            ).Compile();

            var func = method(Value);

            Assert.AreEqual(Value, func());
        }

        [Test]
        public void TwoHoistingLevels()
        {
            const string NameLeft = "x";
            const string NameRight = "y";
            const string ValueLeft = "Hello ";
            const string ValueRight = "World !";
            const string Result = ValueLeft + ValueRight;

            var parameterLeft = Expression.Parameter(typeof(string), NameLeft);
            var parameterRight = Expression.Parameter(typeof(string), NameRight);

            var lambda = Expression.Lambda<Func<string, Func<string, Func<string>>>>
                (
                    Expression.Lambda<Func<string, Func<string>>>
                    (
                        Expression.Lambda<Func<string>>
                        (
                            Expression.Call
                            (
                                typeof(string).GetMethod(nameof(string.Concat), new[] {typeof(string), typeof(string)}),
                                parameterLeft,
                                parameterRight
                            )
                        ),
                        parameterRight
                    ),
                    parameterLeft
                );

            var method = lambda.Compile();
            var func = method(ValueLeft);
            var innerFunc = func(ValueRight);

            Assert.AreEqual(Result, innerFunc());
        }

        [Test]
        public void VoidParameter()
        {
            const string Name = "hello";

            AssertEx.Throws<ArgumentException>(() => Expression.Parameter(typeof(void), Name));
        }
    }
}