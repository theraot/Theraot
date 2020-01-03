#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// EnumerableAsQueryableTest.cs
//
// Authors:
//  Roei Erez (roeie@mainsoft.com)
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MonoTests.System.Linq
{
    public static class Ext
    {
        public static string InstantiatedGenericMethod<T>(this IQueryable<int> iq, T t)
        {
            Theraot.No.Op(iq);
            Theraot.No.Op(t);
            return "QueryableInstantiatedGenericMethod";
        }

        public static string InstantiatedGenericMethod(this IEnumerable<int> ie, int t)
        {
            Theraot.No.Op(ie);
            Theraot.No.Op(t);
            return "EnumerableInstantiatedGenericMethod";
        }

        public static string NonGenericMethod(this IQueryable<int> iq)
        {
            Theraot.No.Op(iq);
            return "QueryableNonGenericMethod";
        }

        public static string NonGenericMethod(this IEnumerable<int> iq)
        {
            Theraot.No.Op(iq);
            return "EnumerableNonGenericMethod";
        }

        public static string UserQueryableExt1<T>(this IQueryable<T> e, Expression<Func<int, int>> ex)
        {
            Theraot.No.Op(e);
            Theraot.No.Op(ex);
            return "UserQueryableExt1";
        }

        public static string UserQueryableExt1<T>(this IEnumerable<T> e, Expression<Func<int, int>> ex)
        {
            Theraot.No.Op(e);
            Theraot.No.Op(ex);
            return "UserEnumerableExt1";
        }

        public static string UserQueryableExt2<T>(this IQueryable<T> e, Expression<Func<int, int>> ex)
        {
            Theraot.No.Op(e);
            Theraot.No.Op(ex);
            return "UserQueryableExt2";
        }

        public static string UserQueryableExt2<T>(this IEnumerable<T> e, Func<int, int> ex)
        {
            Theraot.No.Op(e);
            Theraot.No.Op(ex);
            return "UserEnumerableExt2";
        }

        public static string UserQueryableExt3<T>(this IQueryable<T> e, Expression<Func<int, int>> ex, int dummy)
        {
            Theraot.No.Op(e);
            Theraot.No.Op(ex);
            Theraot.No.Op(dummy);
            return "UserQueryableExt3";
        }
    }

    [TestFixture]
    public class EnumerableAsQueryableTest
    {
        private int[] _array;
        private IQueryable<int> _src;

        [Test]
        public void Aggregate()
        {
            Assert.AreEqual(_src.Aggregate((n, m) => n + m), _array.Aggregate((n, m) => n + m));
        }

        [Test]
        public void All()
        {
            Assert.AreEqual(_src.All(n => n < 11), _array.All(n => n < 11));
            Assert.AreEqual(_src.All(n => n < 10), _array.All(n => n < 10));
        }

        [Test]
        public void Any()
        {
            Assert.AreEqual(_src.Any(i => i > 5), _array.Any(i => i > 5));
        }

        [Test]
        public void Average()
        {
            Assert.AreEqual(_src.Average(n => 11), _array.Average(n => 11));
        }

        [Test]
        public void Concat()
        {
            Assert.AreEqual(_src.Concat(_src).Count(), _array.Concat(_src).Count());
        }

        [Test]
        public void Contains()
        {
            for (var i = 1; i < 20; ++i)
            {
                Assert.AreEqual(_src.Contains(i), _array.Contains(i));
            }
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(_src.Count(), _array.Count());
        }

        [Test]
        public void Distinct()
        {
            Assert.AreEqual(_src.Distinct().Count(), _array.Distinct().Count());
            Assert.AreEqual(_src.Distinct(new CustomEqualityComparer()).Count(), _array.Distinct(new CustomEqualityComparer()).Count());
        }

        [Test]
        public void ElementAt()
        {
            for (var i = 0; i < 10; ++i)
            {
                Assert.AreEqual(_src.ElementAt(i), _array.ElementAt(i));
            }
        }

        [Test]
        public void ElementAtOrDefault()
        {
            for (var i = 0; i < 10; ++i)
            {
                Assert.AreEqual(_src.ElementAtOrDefault(i), _array.ElementAtOrDefault(i));
            }

            Assert.AreEqual(_src.ElementAtOrDefault(100), _array.ElementAtOrDefault(100));
        }

        [Test]
        public void Except()
        {
            int[] except = { 1, 2, 3 };
            Assert.AreEqual(_src.Except(except.AsQueryable()).Count(), _array.Except(except).Count());
        }

        [Test]
        public void First()
        {
            Assert.AreEqual(_src.First(), _array.First());
        }

        [Test]
        public void FirstOrDefault()
        {
            Assert.AreEqual(_src.FirstOrDefault(n => n > 5), _array.FirstOrDefault(n => n > 5));
            Assert.AreEqual(_src.FirstOrDefault(n => n > 10), _array.FirstOrDefault(n => n > 10));
        }

        [Test]
        public void GroupBy()
        {
            var grouping = _src.GroupBy(n => n > 5);
            Assert.AreEqual(grouping.Count(), 2);
            foreach (var group in grouping)
            {
                Assert.AreEqual(group.Count(), 5);
            }
        }

        [Test]
        public void InstantiatedGenericMethod()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    const BindingFlags extensionFlags = BindingFlags.Static | BindingFlags.Public;
                    var method =
                    (
                        from m in typeof(Ext).GetMethods(extensionFlags)
                        where
                        (
                            m.Name == "InstantiatedGenericMethod"
                            && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                        )
                        select m
                    ).FirstOrDefault().MakeGenericMethod(typeof(int));

                    Expression e = Expression.Call(method, _src.Expression, Expression.Constant(0));
                    _src.Provider.Execute(e);
                }
            );
        }

        [Test]
        public void Intersect()
        {
            int[] subset = { 1, 2, 3 };
            var intersection = _src.Intersect(subset.AsQueryable()).ToArray();
            Assert.AreEqual(subset, intersection);
        }

        [Test]
        public void Last()
        {
            Assert.AreEqual(_src.Last(n => n > 1), _array.Last(n => n > 1));
        }

        [Test]
        public void LastOrDefault()
        {
            Assert.AreEqual(_src.LastOrDefault(), _array.LastOrDefault());
        }

        [Test]
        public void LongCount()
        {
            Assert.AreEqual(_src.LongCount(), _array.LongCount());
        }

        [Test]
        public void Max()
        {
            Assert.AreEqual(_src.Max(), _array.Max<int>());
        }

        [Test]
        public void Min()
        {
            Assert.AreEqual(_src.Min(), _array.Min<int>());
        }

        [SetUp]
        public void MyTestCleanup()
        {
            _array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _src = _array.AsQueryable();
        }

        [Test]
        public void NewQueryableExpression()
        {
            var queryable = _array.AsQueryable();
            var expression = queryable.Expression;

            Assert.AreEqual(ExpressionType.Constant, expression.NodeType);

            var constant = (ConstantExpression)expression;

            Assert.AreEqual(queryable, constant.Value);
        }

        [Test]
        public void NonGenericAsQueryableInstantiateProperQueryable()
        {
            IEnumerable bar = new Bar<int, string>();
            var queryable = bar.AsQueryable();

            Assert.IsTrue(queryable is IQueryable<string>);
        }

        [Test]
        public void NonGenericEnumerable1()
        {
            Assert.Throws<ArgumentException>(() => new MyEnum().AsQueryable());
        }

        [Test]
        public void NonGenericEnumerable2()
        {
            IEnumerable<int> nonGen = new[] { 1, 2, 3 };
            Assert.IsTrue(nonGen.AsQueryable() != null);
        }

        [Test]
        public void NonGenericMethod()
        {
            const BindingFlags extensionFlags = BindingFlags.Static | BindingFlags.Public;
            var method =
            (
                from m in typeof(Ext).GetMethods(extensionFlags)
                where
                (
                    m.Name == "NonGenericMethod"
                    && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                )
                select m
            ).FirstOrDefault();

            Expression e = Expression.Call(method, _src.Expression);
            Assert.AreEqual(_src.Provider.Execute(e), "EnumerableNonGenericMethod", "NonGenericMethod");
        }

        [Test]
        public void NullEnumerable()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    const IEnumerable<int> a = null;
                    a.AsQueryable();
                }
            );
        }

        [Test]
        public void OfType()
        {
            Assert.AreEqual(_src.OfType<int>().Count(), _array.Count());
        }

        [Test]
        public void OrderBy()
        {
            var arr1 = _array.OrderBy(n => n * -1).ToArray();
            var arr2 = _src.OrderBy(n => n * -1).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void OrderByDescending()
        {
            var arr1 = _array.OrderBy(n => n).ToArray();
            var arr2 = _src.OrderBy(n => n).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void Reverse()
        {
            var arr1 = _array.Reverse().Reverse().ToArray();
            var arr2 = _src.Reverse().Reverse().ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void Select()
        {
            var arr1 = _array.Select(n => n - 1).ToArray();
            var arr2 = _src.Select(n => n - 1).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void SelectMany()
        {
            var arr1 = _array.SelectMany(n => new[] { n, n, n }).ToArray();
            var arr2 = _src.SelectMany(n => new[] { n, n, n }).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void SequenceEqual()
        {
            Assert.IsTrue(_src.SequenceEqual(_src));
        }

        [Test]
        public void Single()
        {
            Assert.AreEqual(_src.Single(n => n == 10), 10);
        }

        [Test]
        public void SingleOrDefault()
        {
            Assert.AreEqual(_src.SingleOrDefault(n => n == 10), 10);
            Assert.AreEqual(_src.SingleOrDefault(n => n == 11), 0);
        }

        [Test]
        public void Skip()
        {
            var arr1 = _array.Skip(5).ToArray();
            var arr2 = _src.Skip(5).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void SkipWhile()
        {
            var arr1 = _src.SkipWhile(n => n < 6).ToArray();
            var arr2 = _src.Skip(5).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void Sum()
        {
            Assert.AreEqual(_src.Sum(n => n), _array.Sum(n => n));
            Assert.AreEqual(_src.Sum(n => n + 1), _array.Sum(n => n + 1));
        }

        [Test]
        public void Take()
        {
            var arr1 = _array.Take(3).ToArray();
            var arr2 = _src.Take(3).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void TakeWhile()
        {
            var arr1 = _array.TakeWhile(n => n < 6).ToArray();
            var arr2 = _src.TakeWhile(n => n < 6).ToArray();
            Assert.AreEqual(arr1, arr2);
        }

        [Test]
        public void Union()
        {
            var arr1 = _src.ToArray();
            var arr2 = _src.Union(_src).ToArray();
            Assert.AreEqual(arr1, arr2);

            int[] arr = { 11, 12, 13 };
            Assert.AreEqual(_src.Union(arr).ToArray(), _array.Union(arr).ToArray());
        }

        [Test]
        public void UserExtensionMethod()
        {
            const BindingFlags extensionFlags = BindingFlags.Static | BindingFlags.Public;
            var method =
            (
                from m in typeof(Ext).GetMethods(extensionFlags)
                where
                (
                    m.Name == "UserQueryableExt1"
                    && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                )
                select m
            ).FirstOrDefault().MakeGenericMethod(typeof(int));
            Expression<Func<int, int>> exp = i => i;
            Expression expression = Expression.Equal
            (
                Expression.Constant("UserEnumerableExt1"),
                Expression.Call(method, _src.Expression, Expression.Quote(exp))
            );
            Assert.AreEqual(_src.Provider.Execute<bool>(expression), true, "UserQueryableExt1");

            method =
            (
                from m in typeof(Ext).GetMethods(extensionFlags)
                where
                (
                    m.Name == "UserQueryableExt2" && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                )
                select m
            ).FirstOrDefault().MakeGenericMethod(typeof(int));
            expression = Expression.Equal
            (
                Expression.Constant("UserEnumerableExt2"),
                Expression.Call(method, _src.Expression, Expression.Quote(exp))
            );
            Assert.AreEqual(_src.Provider.Execute<bool>(expression), true, "UserQueryableExt2");
        }

        [Test]
        public void UserExtensionMethodNegative()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    const BindingFlags extensionFlags = BindingFlags.Static | BindingFlags.Public;
                    var method =
                    (
                        from m in typeof(Ext).GetMethods(extensionFlags)
                        where
                        (
                            m.Name == "UserQueryableExt3"
                            && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                        )
                        select m
                    ).FirstOrDefault().MakeGenericMethod(typeof(int));
                    Expression<Func<int, int>> exp = i => i;
                    Expression e = Expression.Call(method, _src.Expression, Expression.Quote(exp), Expression.Constant(10));
                    _src.Provider.Execute(e);
                }
            );
        }

        [Test]
        public void Where()
        {
            var oddArray1 = _array.Where(n => (n % 2) == 1).ToArray();
            var oddArray2 = _src.Where(n => (n % 2) == 1).ToArray();
            Assert.AreEqual(oddArray1, oddArray2);
        }

        // ReSharper disable once UnusedTypeParameter
        private sealed class Bar<T1, T2> : IEnumerable<T2>
        {
            public IEnumerator<T2> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

    internal class CustomEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return true;
        }

        public int GetHashCode(int obj)
        {
            return 0;
        }
    }

    internal class MyEnum : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}