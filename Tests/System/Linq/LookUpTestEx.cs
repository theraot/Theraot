﻿#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Theraot.Collections;

namespace MonoTests.System.Linq
{
    [TestFixture]
    public class LookUpTestEx
    {
        [Test]
        public void ToLookupIsImmediate()
        {
            var src = new IterateAndCount(10);
            var a = src.ToLookup(i => i > 5, null);
            var b = src.ToLookup(i => i > 5, j => $"str: {j.ToString(CultureInfo.InvariantCulture)}", null);
            Assert.AreEqual(src.Total, 20);
            a.Consume();
            b.Consume();
            Assert.AreEqual(src.Total, 20);
        }

        [Test]
        public void ToLookupOverloadA()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.ToLookup(i => i > 5, null);
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
        public void ToLookupOverloadAEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.ToLookup(i => i > 5, null);
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
        public void ToLookupOverloadB()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.ToLookup(i => i > 5, j => $"str: {j.ToString(CultureInfo.InvariantCulture)}", null);
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
                    Assert.AreEqual(item, $"str: {(index + 1).ToString(CultureInfo.InvariantCulture)}");
                    index++;
                    count++;
                }

                Assert.AreEqual(count, 5);
                first = false;
            }

            Assert.AreEqual(index, 10);
        }

        [Test]
        public void ToLookupOverloadBEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.ToLookup(i => i > 5, j => $"str: {j.ToString(CultureInfo.InvariantCulture)}", null);
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
                    Total++;
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