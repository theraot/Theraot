#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Theraot;
using Theraot.Collections;
using Theraot.Core;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    internal class ExtensionsGroupProgressiveBy
    {
        public static void AssertException<T>(Action action) where T : Exception
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action();
                Assert.Fail();
            }
            catch (T)
            {
                // Empty
            }
            catch (Exception exception)
            {
                No.Op(exception);
                Assert.Fail($"Expected: {typeof(T).Name}");
            }
        }

        [Test]
        public void GroupProgressiveByArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // GroupProgressiveBy<string,string> (Func<string, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string>(null));

            // GroupProgressiveBy<string,string> (Func<string, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(null, EqualityComparer<string>.Default));

            // GroupProgressiveBy<string,string,string> (Func<string, string>, Func<string, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", _ => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string>(null, _ => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(_ => "test", (Func<string, string>)null));

            // GroupProgressiveBy<string,string,string> (Func<string, string>, Func<string, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", _ => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(null, _ => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(_ => "test", (Func<string, string>)null, EqualityComparer<string>.Default));

            // GroupProgressiveBy<string,string,string> (Func<string, string>, Func<string, IEnumerable<string>, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", (_, _) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string>(null, (_, _) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(_ => "test", (Func<string, IEnumerable<string>, string>)null));

            // GroupProgressiveBy<string,string,string,string> (Func<string, string>, Func<string, string>, Func<string, IEnumerable<string>, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", _ => "test", (_, _) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string, string>(null, _ => "test", (_, _) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string, string>(_ => "test", null, (_, _) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string, string>(_ => "test", _ => "test", null));

            // GroupProgressiveBy<string,string,string> (Func<string, string>, Func<string, IEnumerable<string>, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", (_, _) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(null, (_, _) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(_ => "test", (Func<string, IEnumerable<string>, string>)null, EqualityComparer<string>.Default));

            // GroupProgressiveBy<string,string,string,string> (Func<string, string>, Func<string, string>, Func<string, IEnumerable<string>, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupProgressiveBy(_ => "test", _ => "test", (_, _) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy(null, _ => "test", (_, _) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string, string>(_ => "test", null, (_, _) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupProgressiveBy<string, string, string, string>(_ => "test", _ => "test", null, EqualityComparer<string>.Default));
        }

        [Test]
        public void GroupProgressiveByIsDeferred()
        {
            var src = new IterateAndCount(10);
            var a = src.GroupProgressiveBy(i => i > 5, null);
            var b = src.GroupProgressiveBy(i => i > 5, j => $"str: {j.ToString(CultureInfo.InvariantCulture)}", null);
            var c = src.GroupProgressiveBy(i => i > 5, (_, group) => StringEx.Concat(group.ToArray()), null);
            var d = src.GroupProgressiveBy(i => i > 5, j => j + 1, (_, group) => StringEx.Concat(group.ToArray()), null);
            Assert.AreEqual(src.Total, 0);
            a.Consume();
            b.Consume();
            c.Consume();
            d.Consume();
            Assert.AreEqual(src.Total, 40);
        }

        [Test]
        public void GroupProgressiveByIsDeferredToGetEnumerator()
        {
            var src = new IterateAndCount(10);
            var a = src.GroupProgressiveBy(i => i > 5, null);
            Assert.AreEqual(src.Total, 0);
            using (var enumerator = a.GetEnumerator())
            {
                // GroupProgressiveBy is truly deferred
                GC.KeepAlive(enumerator);
                Assert.AreEqual(src.Total, 0);
            }

            Assert.AreEqual(src.Total, 0);
            using (var enumerator = a.GetEnumerator())
            {
                // GroupProgressiveBy is truly deferred
                GC.KeepAlive(enumerator);
                Assert.AreEqual(src.Total, 0);
            }

            Assert.AreEqual(src.Total, 0);
        }

        [Test]
        public void GroupProgressiveByLastNullGroup()
        {
            var values = new List<Data>
            {
                new Data(0, "a"),
                new Data(1, "a"),
                new Data(2, "b"),
                new Data(3, "b"),
                new Data(4, null)
            };
            var groups = values.GroupProgressiveBy(d => d.String);

            var count = 0;
            foreach (var group in groups)
            {
                switch (group.Key)
                {
                    case "a":
                        Assert.IsTrue(group.Select(item => item.Number).ToArray().SetEquals(new[] { 0, 1 }));
                        break;

                    case "b":
                        Assert.IsTrue(group.Select(item => item.Number).ToArray().SetEquals(new[] { 2, 3 }));
                        break;

                    case null:
                        Assert.IsTrue(group.Select(item => item.Number).ToArray().SetEquals(new[] { 4 }));
                        break;

                    default:
                        Assert.Fail();
                        break;
                }

                count++;
            }

            Assert.AreEqual(3, count);
        }

        [Test]
        public void GroupProgressiveByOverloadA()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, null);
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
        public void GroupProgressiveByOverloadAEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, null);
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
        public void GroupProgressiveByOverloadB()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, j => $"str: {j.ToString(CultureInfo.InvariantCulture)}", null);
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
        public void GroupProgressiveByOverloadBEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, j => $"str: {j.ToString(CultureInfo.InvariantCulture)}", null);
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
        public void GroupProgressiveByOverloadC()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, (_, group) => StringEx.Concat(group.ToArray()), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            Assert.AreEqual(rArray[0], "12345");
            Assert.AreEqual(rArray[1], "678910");
        }

        [Test]
        public void GroupProgressiveByOverloadD()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, j => j + 1, (_, group) => StringEx.Concat(group.ToArray()), null);
            var rArray = r.ToArray();
            Assert.AreEqual(rArray.Length, 2);
            Assert.AreEqual(rArray[0], "23456");
            Assert.AreEqual(rArray[1], "7891011");
        }

        [Test]
        public void GroupProgressiveByOverloadDEx()
        {
            var src = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var r = src.GroupProgressiveBy(i => i > 5, FuncHelper.GetIdentityFunc<int>(), (_, group) => StringEx.Concat(group.ToArray()), null);
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
                    Total++;
                    yield return Total;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class Data
        {
            public readonly int Number;
            public readonly string String;

            public Data(int number, string str)
            {
                Number = number;
                String = str;
            }
        }
    }
}