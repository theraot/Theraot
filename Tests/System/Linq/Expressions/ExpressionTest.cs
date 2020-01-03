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

        [Test]
        public void CompileActionDiscardingRetValue()
        {
            const string name = "i";
            const int value = 42;

            var parameter = Expression.Parameter(typeof(int), name);
            var identity = new Func<int, int>(Identity).GetMethodInfo();
            Assert.IsNotNull(identity);

            var lambda = Expression.Lambda<Action<int>>(Expression.Call(identity, parameter), parameter);

            var compiled = lambda.Compile();

            _buffer = 0;

            compiled(value);
            Assert.AreEqual(value, _buffer);
        }

        [Test]
        public void ExpressionDelegateTarget()
        {
            const string name = "str";

            var parameter = Expression.Parameter(typeof(string), name);
            var compiled = Expression.Lambda<Func<string, string>>(parameter, parameter).Compile();

            Assert.AreEqual(typeof(Func<string, string>), compiled.GetType());
            Assert.IsNotNull(compiled.Target);
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
            const int value = 45; // > 16

            AssertEx.Throws<ArgumentException>(() => Expression.GetActionType(GetTestTypeArray(value)));
        }

        [Test]
        public void GetActionTypeTest()
        {
            // Note: Testing with Action<int, int, int, int, int> on a .NET 3.5 build running on .NET 4.0 runtime fails
            //       On a .NET 4.0 runtime Expression.GetActionType will return the type from mscorlib..
            //       But give it is a .NET 3.5 build, it would be comparing against the type in Theraot.Core
            var instances = new[]
            {
                typeof(Action),
                typeof(Action<int>),
                typeof(Action<int, int>),
                typeof(Action<int, int, int>),
                typeof(Action<int, int, int, int>)
            };

            foreach (var instance in instances)
            {
                var action = Expression.GetActionType(instance.GetGenericArguments());
                Assert.AreEqual(instance, action, instance.Name);
            }
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
            const int value = 64; // > 17

            AssertEx.Throws<ArgumentException>(() => Expression.GetFuncType(GetTestTypeArray(value)));
        }

        [Test]
        public void GetFuncTypeTest()
        {
            // Note: Testing with Func<int, int, int, int, int, int> on a .NET 3.5 build running on .NET 4.0 runtime fails
            //       On a .NET 4.0 runtime Expression.GetFuncType will return the type from mscorlib..
            //       But give it is a .NET 3.5 build, it would be comparing against the type in Theraot.Core
            var instances = new[]
            {
                typeof(Func<int>),
                typeof(Func<int, int>),
                typeof(Func<int, int, int>),
                typeof(Func<int, int, int, int>),
                typeof(Func<int, int, int, int, int>)
            };

            foreach (var instance in instances)
            {
                var func = Expression.GetFuncType(instance.GetGenericArguments());
                Assert.AreEqual(instance, func, instance.Name);
            }
        }

        [Test]
        public void HoistedParameter()
        {
            const string name = "i";
            const int value = 42;

            var parameter = Expression.Parameter(typeof(int), name);

            var compiled = Expression.Lambda<Func<int, string>>
            (
                Expression.Invoke
                (
                    Expression.Lambda<Func<string>>
                    (
                        Expression.Call(parameter, new Func<string>(default(int).ToString).GetMethodInfo())
                    )
                ),
                parameter
            ).Compile();

            Assert.AreEqual($"{value}", compiled(value));
        }

        [Test]
        public void Parameter()
        {
            const string name = "foo";
            var type = typeof(string);

            var parameter = Expression.Parameter(type, name);
            Assert.AreEqual(name, parameter.Name);
            Assert.AreEqual(type, parameter.Type);
            Assert.AreEqual(name, parameter.ToString());
        }

        [Test]
        public void ParameterEmptyName()
        {
            const string name = ""; // ""
            var type = typeof(string);

            var parameter = Expression.Parameter(type, name);
            Assert.AreEqual(name, parameter.Name);
            Assert.AreEqual(type, parameter.Type);
        }

        [Test]
        public void ParameterNullName()
        {
            const string name = null; // null
            var type = typeof(string);

            var parameter = Expression.Parameter(type, name);
            Assert.AreEqual(name, parameter.Name);
            Assert.AreEqual(type, parameter.Type);
        }

        [Test]
        public void ParameterNullType()
        {
            const string name = "foo";
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.Parameter(null, name));
        }

        [Test]
        public void SimpleHoistedParameter()
        {
            const string name = "s";
            const string value = "x";

            var parameter = Expression.Parameter(typeof(string), name);

            var compiled = Expression.Lambda<Func<string, Func<string>>>
            (
                Expression.Lambda<Func<string>>(parameter),
                parameter
            ).Compile();

            var func = compiled(value);

            Assert.AreEqual(value, func());
        }

        [Test]
        public void TwoHoistingLevels()
        {
            const string nameLeft = "x";
            const string nameRight = "y";
            const string valueLeft = "Hello ";
            const string valueRight = "World !";
            const string result = valueLeft + valueRight;

            var parameterLeft = Expression.Parameter(typeof(string), nameLeft);
            var parameterRight = Expression.Parameter(typeof(string), nameRight);

            var lambda = Expression.Lambda<Func<string, Func<string, Func<string>>>>
            (
                Expression.Lambda<Func<string, Func<string>>>
                (
                    Expression.Lambda<Func<string>>
                    (
                        Expression.Call
                        (
                            new Func<string, string, string>(string.Concat).GetMethodInfo(),
                            parameterLeft,
                            parameterRight
                        )
                    ),
                    parameterRight
                ),
                parameterLeft
            );

            var compiled = lambda.Compile();
            var func = compiled(valueLeft);
            var innerFunc = func(valueRight);

            Assert.AreEqual(result, innerFunc());
        }

        [Test]
        public void VoidParameter()
        {
            const string name = "hello";

            AssertEx.Throws<ArgumentException>(() => Expression.Parameter(typeof(void), name));
        }

        private static Type[] GetTestTypeArray(int length)
        {
            return Enumerable.Range(0, length - 1)
                .Select(_ => typeof(int))
                .ToArray();
        }
    }
}