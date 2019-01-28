#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Tests.System.Linq.Expressions
{
    [TestFixture]
    internal static class ExpressionTestEx
    {
        [Test]
        public static void TestLamdaCompilation()
        {
            var paramExpr = Expression.Parameter(typeof(int), "arg");
            var lambdaExpr = Expression.Lambda(
                Expression.Add(
                    paramExpr,
                    Expression.Constant(1)
                ),
                paramExpr
            );
            Assert.AreEqual(2, lambdaExpr.Compile().DynamicInvoke(1));
        }

        [Test]
        public static void TestLamdaCompilationWithDelegateType()
        {
            var paramExpr = Expression.Parameter(typeof(int), "arg");
            var lambdaExpr = Expression.Lambda(
                typeof(Func<int, int>),
                Expression.Add(
                    paramExpr,
                    Expression.Constant(1)
                ),
                paramExpr
            );
            Assert.AreEqual(2, lambdaExpr.Compile().DynamicInvoke(1));
        }

        [Test]
        public static void TestLamdaCompilationWithDelegateTypeNull()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    var paramExpr = Expression.Parameter(typeof(int), "arg");
                    var lambdaExpr = Expression.Lambda(
                        null,
                        Expression.Add(
                            paramExpr,
                            Expression.Constant(1)
                        ),
                        paramExpr
                    );
                    GC.KeepAlive(lambdaExpr);
                }
            );
        }

        [Test]
        public static void TestLamdaCompilationWithDelegateTypeWrong()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var paramExpr = Expression.Parameter(typeof(int), "arg");
                    var lambdaExpr = Expression.Lambda(
                        typeof(Func<string, string>),
                        Expression.Add(
                            paramExpr,
                            Expression.Constant(1)
                        ),
                        paramExpr
                    );
                    GC.KeepAlive(lambdaExpr);
                }
            );
        }

        [Test]
        public static void TestLamdaCompilationWithDelegateTypeInvalid()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var paramExpr = Expression.Parameter(typeof(int), "arg");
                    var lambdaExpr = Expression.Lambda(
                        typeof(string),
                        Expression.Add(
                            paramExpr,
                            Expression.Constant(1)
                        ),
                        paramExpr
                    );
                    GC.KeepAlive(lambdaExpr);
                }
            );
        }

        [Test]
        public static void TestLamdaCompilationGenericDelegateType()
        {
            var paramExpr = Expression.Parameter(typeof(int), "arg");
            var lambdaExpr = Expression.Lambda<Func<int, int>>(
                Expression.Add(
                    paramExpr,
                    Expression.Constant(1)
                ),
                paramExpr
            );
            Assert.AreEqual(2, lambdaExpr.Compile().DynamicInvoke(1));
        }

        [Test]
        public static void TestLamdaCompilationGenericDelegateTypeWrong()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var paramExpr = Expression.Parameter(typeof(int), "arg");
                    var lambdaExpr = Expression.Lambda<Func<string, string>>(
                        Expression.Add(
                            paramExpr,
                            Expression.Constant(1)
                        ),
                        paramExpr
                    );
                    GC.KeepAlive(lambdaExpr);
                }
            );
        }

        [Test]
        public static void TestLamdaCompilationGenericDelegateTypeInvalid()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var paramExpr = Expression.Parameter(typeof(int), "arg");
                    var lambdaExpr = Expression.Lambda<string>(
                        Expression.Add(
                            paramExpr,
                            Expression.Constant(1)
                        ),
                        paramExpr
                    );
                    GC.KeepAlive(lambdaExpr);
                }
            );
        }
    }
}