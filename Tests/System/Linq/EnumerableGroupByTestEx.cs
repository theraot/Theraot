using NUnit.Framework;
using System.Globalization;
using System.Linq;
using Theraot.Core;

namespace MonoTests.System.Linq
{
    [TestFixture]
    public class EnumerableAsQueryableTestEx
    {
        [Test]
        public void GroupByOverloadA()
        {
            var _src = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.GroupBy<int, bool>(i => i > 5, null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            int index = 0;
            bool first = true;
            foreach (var g in _r)
            {
                Assert.AreEqual(g.Key, !first);
                foreach (var item in g)
                {
                    Assert.AreEqual(item, index + 1);
                    index++;
                }
                first = false;
            }
            Assert.AreEqual(index, 10);
        }

        [Test]
        public void GroupByOverloadB()
        {
            var _src = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.GroupBy<int, bool, string>(i => i > 5, j => "str: " + j.ToString(CultureInfo.InvariantCulture), null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            int index = 0;
            bool first = true;
            foreach (var g in _r)
            {
                Assert.AreEqual(g.Key, !first);
                foreach (var item in g)
                {
                    Assert.AreEqual(item, "str: " + (index + 1).ToString(CultureInfo.InvariantCulture));
                    index++;
                }
                first = false;
            }
            Assert.AreEqual(index, 10);
        }

        [Test]
        public void GroupByOverloadC()
        {
            var _src = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.GroupBy<int, bool, string>(i => i > 5, (key, group) => StringHelper.Concat(group.ToArray()), null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            Assert.AreEqual(_r[0], "12345");
            Assert.AreEqual(_r[1], "678910");
        }

        [Test]
        public void GroupByOverloadD()
        {
            var _src = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = _src.GroupBy<int, bool, int, string>(i => i > 5, j => j + 1, (key, group) => StringHelper.Concat(group.ToArray()), null);
            var _r = r.ToArray();
            Assert.AreEqual(_r.Length, 2);
            Assert.AreEqual(_r[0], "23456");
            Assert.AreEqual(_r[1], "7891011");
        }
    }
}