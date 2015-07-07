// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Tests.ExpressionCompiler.Array
{
    public static class ArrayBoundsOneOffTests
    {
        [Test]
        public static void CompileWithCastTest()
        {
            Expression<Func<object[]>> expr = () => (object[])new BaseClass[1];
            expr.Compile();
        }

        [Test]
        public static void ToStringTest()
        {
            Expression<Func<int, object>> x = c => new double[c, c];
            Assert.AreEqual("c => new System.Double[,](c, c)", x.ToString());

            object y = x.Compile()(2);
            Assert.AreEqual("System.Double[,]", y.ToString());
        }
    }
}
