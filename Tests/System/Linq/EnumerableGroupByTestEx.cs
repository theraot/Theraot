using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Theraot.Collections;
using Theraot.Core;

namespace MonoTests.System.Linq
{
    [TestFixture]
    public class EnumerableAsQueryableTestEx
    {
        [Test]
        public void GroupByIsDeferred()
        {
            var src = new IterateAndCount(10);
            var a = src.GroupBy(i => i > 5, null);
            var b = src.GroupBy(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            var c = src.GroupBy(i => i > 5, (key, group) => StringEx.Concat(group.ToArray()), null);
            var d = src.GroupBy(i => i > 5, j => j + 1, (key, group) => StringEx.Concat(group.ToArray()), null);
            Assert.AreEqual(src.Total, 0);
            a.Consume();
            b.Consume();
            c.Consume();
            d.Consume();
            Assert.AreEqual(src.Total, 40);
        }

        [Test]
        public void GroupByIsDeferredToGetEnumerator()
        {
            var src = new IterateAndCount(10);
            var a = src.GroupBy(i => i > 5, null);
            Assert.AreEqual(src.Total, 0);
            using (var enumerator = a.GetEnumerator())
            {
                // This is a shame, GroupBy is not really deferred
                GC.KeepAlive(enumerator);
                Assert.AreEqual(src.Total, 10);
            }
            Assert.AreEqual(src.Total, 10);
            using (var enumerator = a.GetEnumerator())
            {
                // This is a shame, GroupBy is not really deferred
                GC.KeepAlive(enumerator);
                Assert.AreEqual(src.Total, 20);
            }
            Assert.AreEqual(src.Total, 20);
        }

        [Test]
        public void GroupByOverloadA()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            var index = 0;
            var first = true;
            foreach (var g in rArray)
            {
                Assert.AreEqual(g.Key, !first);
                var count = 0;
                foreach (var item in g)
                {
                    Assert.AreEqual(item, index + 1);
                    index++;
                    count++;
                }
                Assert.AreEqual(count, 5);
                first = false;
            }
            Assert.AreEqual(index, 10);
        }

        [Test]
        public void GroupByOverloadAEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            var first = true;
            foreach (var g in rArray)
            {
                Assert.AreEqual(g.Key, !first);
                Assert.AreEqual(g.Count(), 5);
                first = false;
            }
        }

        [Test]
        public void GroupByOverloadB()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            var index = 0;
            var first = true;
            foreach (var g in rArray)
            {
                Assert.AreEqual(g.Key, !first);
                var count = 0;
                foreach (var item in g)
                {
                    Assert.AreEqual(item, "str: " + (index + 1).ToString(CultureInfo.InvariantCulture));
                    index++;
                    count++;
                }
                Assert.AreEqual(count, 5);
                first = false;
            }
            Assert.AreEqual(index, 10);
        }

        [Test]
        public void GroupByOverloadBEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            var first = true;
            foreach (var g in rArray)
            {
                Assert.AreEqual(g.Key, !first);
                Assert.AreEqual(g.Count(), 5);
                first = false;
            }
        }

        [Test]
        public void GroupByOverloadC()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, (key, group) => StringEx.Concat(group.ToArray()), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            Assert.AreEqual(rArray[0], "12345");
            Assert.AreEqual(rArray[1], "678910");
        }

        [Test]
        public void GroupByOverloadD()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, j => j + 1, (key, group) => StringEx.Concat(group.ToArray()), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            Assert.AreEqual(rArray[0], "23456");
            Assert.AreEqual(rArray[1], "7891011");
        }

        [Test]
        public void GroupByOverloadDEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupBy(i => i > 5, FuncHelper.GetIdentityFunc<int>(), (key, group) => StringEx.Concat(group.ToArray()), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            Assert.AreEqual(rArray[0], "12345");
            Assert.AreEqual(rArray[1], "678910");
        }

        public class IterateAndCount : IEnumerable<int>
        {
            private readonly int _count;

            public IterateAndCount(int count)
            {
                _count = count;
            }

            public int Total { get; private set; }

            public IEnumerator<int> GetEnumerator()
            {
                for (var index = 0; index < _count; index++)
                {
                    Total = Total + 1;
                    yield return Total;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}