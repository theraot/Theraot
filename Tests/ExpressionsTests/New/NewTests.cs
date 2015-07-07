// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Tests.ExpressionCompiler.New
{
    public static class NewTests
    {
        #region Test methods

        [Test]
        public static void CheckNewCustomTest()
        {
            Expression<Func<C>> e =
                Expression.Lambda<Func<C>>(
                    Expression.New(typeof(C)),
                    Enumerable.Empty<ParameterExpression>());
            Func<C> f = e.Compile();

            Assert.AreEqual(new C(), f());
        }

        [Test]
        public static void CheckNewEnumTest()
        {
            Expression<Func<E>> e =
                Expression.Lambda<Func<E>>(
                    Expression.New(typeof(E)),
                    Enumerable.Empty<ParameterExpression>());
            Func<E> f = e.Compile();

            Assert.AreEqual(new E(), f());
        }

        [Test]
        public static void CheckNewNullableEnumTest()
        {
            Expression<Func<E?>> e =
                Expression.Lambda<Func<E?>>(
                    Expression.New(typeof(E?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<E?> f = e.Compile();

            Assert.AreEqual(new E?(), f());
        }

        [Test]
        public static void CheckNewNullableIntTest()
        {
            Expression<Func<int?>> e =
                Expression.Lambda<Func<int?>>(
                    Expression.New(typeof(int?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<int?> f = e.Compile();

            Assert.AreEqual(new int?(), f());
        }

        [Test]
        public static void CheckNewStructTest()
        {
            Expression<Func<S>> e =
                Expression.Lambda<Func<S>>(
                    Expression.New(typeof(S)),
                    Enumerable.Empty<ParameterExpression>());
            Func<S> f = e.Compile();

            Assert.AreEqual(new S(), f());
        }

        [Test]
        public static void CheckNewNullableStructTest()
        {
            Expression<Func<S?>> e =
                Expression.Lambda<Func<S?>>(
                    Expression.New(typeof(S?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<S?> f = e.Compile();

            Assert.AreEqual(new S?(), f());
        }

        [Test]
        public static void CheckNewStructWithStringTest()
        {
            Expression<Func<Sc>> e =
                Expression.Lambda<Func<Sc>>(
                    Expression.New(typeof(Sc)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc> f = e.Compile();

            Assert.AreEqual(new Sc(), f());
        }

        [Test]
        public static void CheckNewNullableStructWithStringTest()
        {
            Expression<Func<Sc?>> e =
                Expression.Lambda<Func<Sc?>>(
                    Expression.New(typeof(Sc?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc?> f = e.Compile();

            Assert.AreEqual(new Sc?(), f());
        }

        [Test]
        public static void CheckNewStructWithStringAndFieldTest()
        {
            Expression<Func<Scs>> e =
                Expression.Lambda<Func<Scs>>(
                    Expression.New(typeof(Scs)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs> f = e.Compile();

            Assert.AreEqual(new Scs(), f());
        }

        [Test]
        public static void CheckNewNullableStructWithStringAndFieldTest()
        {
            Expression<Func<Scs?>> e =
                Expression.Lambda<Func<Scs?>>(
                    Expression.New(typeof(Scs?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs?> f = e.Compile();

            Assert.AreEqual(new Scs?(), f());
        }

        [Test]
        public static void CheckNewStructWithTwoValuesTest()
        {
            Expression<Func<Sp>> e =
                Expression.Lambda<Func<Sp>>(
                    Expression.New(typeof(Sp)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp> f = e.Compile();

            Assert.AreEqual(new Sp(), f());
        }

        [Test]
        public static void CheckNewNullableStructWithTwoValuesTest()
        {
            Expression<Func<Sp?>> e =
                Expression.Lambda<Func<Sp?>>(
                    Expression.New(typeof(Sp?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp?> f = e.Compile();

            Assert.AreEqual(new Sp?(), f());
        }

        [Test]
        public static void CheckNewGenericWithStructRestrictionWithEnumTest()
        {
            CheckNewGenericWithStructRestrictionHelper<E>();
        }

        [Test]
        public static void CheckNewGenericWithStructRestrictionWithStructTest()
        {
            CheckNewGenericWithStructRestrictionHelper<S>();
        }

        [Test]
        public static void CheckNewGenericWithStructRestrictionWithStructWithStringAndFieldTest()
        {
            CheckNewGenericWithStructRestrictionHelper<Scs>();
        }

        [Test]
        public static void CheckNewNullableGenericWithStructRestrictionWithEnumTest()
        {
            CheckNewNullableGenericWithStructRestrictionHelper<E>();
        }

        [Test]
        public static void CheckNewNullableGenericWithStructRestrictionWithStructTest()
        {
            CheckNewNullableGenericWithStructRestrictionHelper<S>();
        }

        [Test]
        public static void CheckNewNullableGenericWithStructRestrictionWithStructWithStringAndFieldTest()
        {
            CheckNewNullableGenericWithStructRestrictionHelper<Scs>();
        }

        #endregion

        #region Generic helpers

        private static void CheckNewGenericWithStructRestrictionHelper<Ts>() where Ts : struct
        {
            Expression<Func<Ts>> e =
                Expression.Lambda<Func<Ts>>(
                    Expression.New(typeof(Ts)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts> f = e.Compile();

            Assert.AreEqual(new Ts(), f());
        }

        private static void CheckNewNullableGenericWithStructRestrictionHelper<Ts>() where Ts : struct
        {
            Expression<Func<Ts?>> e =
                Expression.Lambda<Func<Ts?>>(
                    Expression.New(typeof(Ts?)),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts?> f = e.Compile();

            Assert.AreEqual(new Ts?(), f());
        }

        #endregion

        [Test]
        public static void PrivateDefaultConstructor()
        {
            Assert.AreEqual("Test instance", TestPrivateDefaultConstructor.GetInstanceFunc()().ToString());
        }

        class TestPrivateDefaultConstructor
        {
            private TestPrivateDefaultConstructor()
            { 
            }

            public static Func<TestPrivateDefaultConstructor> GetInstanceFunc()
            {
                var lambda = Expression.Lambda<Func<TestPrivateDefaultConstructor>>(Expression.New(typeof(TestPrivateDefaultConstructor)), new ParameterExpression[] { });
                return lambda.Compile();
            }

            public override string ToString()
            {
                return "Test instance";
            }
        }
    }
}
