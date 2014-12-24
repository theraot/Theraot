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
            var _src = new IterateAndCount(10);
            var a = _src.ToLookup(i => i > 5, null);
            var b = _src.ToLookup(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            Assert.AreEqual(_src.Total, 20);
            a.Consume();
            b.Consume();
            Assert.AreEqual(_src.Total, 20);
        }

        [Test]
        public void ToLookupOverloadA()
        {
            var _src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.ToLookup(i => i > 5, null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            int index = 0;
            bool first = true;
            foreach (var g in _r)
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
            var _src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.ToLookup(i => i > 5, null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            bool first = true;
            foreach (var g in _r)
            {
                Assert.AreEqual(g.Key, !first);
                Assert.AreEqual(g.Count(), 5);
                first = false;
            }
        }

        [Test]
        public void ToLookupOverloadB()
        {
            var _src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.ToLookup(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            int index = 0;
            bool first = true;
            foreach (var g in _r)
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
        public void ToLookupOverloadBEx()
        {
            var _src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.ToLookup(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            bool first = true;
            foreach (var g in _r)
            {
                Assert.AreEqual(g.Key, !first);
                Assert.AreEqual(g.Count(), 5);
                first = false;
            }
        }

        public class IterateAndCount : IEnumerable<int>
        {
            private readonly int _count;
            private int _total;

            public IterateAndCount(int count)
            {
                _count = count;
            }

            public int Total
            {
                get { return _total; }
            }

            public IEnumerator<int> GetEnumerator()
            {
                for (int index = 0; index < _count; index++)
                {
                    _total = Total + 1;
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