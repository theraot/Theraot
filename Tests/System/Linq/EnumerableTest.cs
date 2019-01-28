#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// EnumerableTest.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2007 Novell, Inc. (http://www.novell.com)
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
using System.IO;
using System.Linq;
using Theraot.Collections;

namespace MonoTests.System.Linq
{
    [TestFixture]
    public class EnumerableTest
    {
        [Test]
        public void TestSimpleExcept()
        {
            int[] first = { 0, 1, 2, 3, 4, 5 };
            int[] second = { 2, 4, 6 };
            int[] result = { 0, 1, 3, 5 };

            AssertAreSame(result, first.Except(second));
        }

        [Test]
        public void TestSimpleIntersect()
        {
            int[] first = { 0, 1, 2, 3, 4, 5 };
            int[] second = { 2, 4, 6 };
            int[] result = { 2, 4 };

            AssertAreSame(result, first.Intersect(second));
        }

        [Test]
        public void TestSimpleUnion()
        {
            int[] first = { 0, 1, 2, 3, 4, 5 };
            int[] second = { 2, 4, 6 };
            int[] result = { 0, 1, 2, 3, 4, 5, 6 };

            AssertAreSame(result, first.Union(second));
        }

        private class Foo
        {
            // Empty
        }

        private class Bar : Foo
        {
            // Empty
        }

        [Test]
        public void TestCast()
        {
            var a = new Bar();
            var b = new Bar();
            var c = new Bar();

            var foos = new Foo[] { a, b, c };
            var result = new[] { a, b, c };

            AssertAreSame(result, foos.Cast<Bar>());
        }

        private class Bingo : IEnumerable<int>, IEnumerable<string>
        {
            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                yield return 42;
                yield return 12;
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                yield return "foo";
                yield return "bar";
            }

            public IEnumerator GetEnumerator()
            {
                return (this as IEnumerable<int>).GetEnumerator();
            }
        }

        [Test]
        public void TestCastToImplementedType()
        {
            var ints = new[] { 42, 12 };
            var strs = new[] { "foo", "bar" };

            var bingo = new Bingo();

            // Note: we are testing Cast
            AssertAreSame(ints, bingo.Cast<int>());
            AssertAreSame(strs, bingo.Cast<string>());
        }

        [Test]
        public void TestLast()
        {
            int[] data = { 1, 2, 3 };

            Assert.AreEqual(3, data.Last());
        }

        [Test]
        public void TestLastOrDefault()
        {
            int[] data = { };

            Assert.AreEqual(default(int), data.LastOrDefault());
        }

        [Test]
        public void TestFirst()
        {
            int[] data = { 1, 2, 3 };

            Assert.AreEqual(1, data.First());
        }

        [Test]
        public void TestFirstOrDefault()
        {
            int[] data = { };

            Assert.AreEqual(default(int), data.FirstOrDefault());
        }

        [Test]
        public void TestSequenceEqual()
        {
            int[] first = { 0, 1, 2, 3, 4, 5 };
            int[] second = { 0, 1, 2 };
            int[] third = { 0, 1, 2, 3, 4, 5 };

            Assert.IsFalse(first.SequenceEqual(second));
            Assert.IsTrue(first.SequenceEqual(third));
        }

        [Test]
        public void TestSkip()
        {
            int[] data = { 0, 1, 2, 3, 4, 5 };
            int[] result = { 3, 4, 5 };

            AssertAreSame(result, data.Skip(3));
        }

        [Test]
        public void TestSkipWhile()
        {
            int[] data = { 0, 1, 2, 3, 4, 5 };
            int[] result = { 3, 4, 5 };

            AssertAreSame(result, data.SkipWhile(i => i < 3));
        }

        [Test]
        public void TestTake()
        {
            int[] data = { 0, 1, 2, 3, 4, 5 };
            int[] result = { 0, 1, 2 };

            AssertAreSame(result, data.Take(3));
        }

        [Test]
        public void TestTakeWhile()
        {
            int[] data = { 0, 1, 2, 3, 4, 5 };
            int[] result = { 0, 1, 2 };

            AssertAreSame(result, data.TakeWhile(i => i < 3));
        }

        [Test]
        public void TestSelect()
        {
            int[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] result = { 1, 3, 5, 7, 9 };

            AssertAreSame(result, data.Where(i => i % 2 != 0));
        }

        [Test]
        public void TestReverse()
        {
            int[] data = { 0, 1, 2, 3, 4 };
            int[] result = { 4, 3, 2, 1, 0 };

            AssertAreSame(result, data.Reverse());
            AssertAreSame(result, Enumerable.Range(0, 5).Reverse());
        }

        [Test]
        public void ReverseArrays()
        {
            int[] source = { 1, 2, 3 };

            var query = source.Reverse();
            using (var enumerator = query.GetEnumerator())
            {
                enumerator.MoveNext();
                Assert.AreEqual(3, enumerator.Current);

                source[1] = 42;
                enumerator.MoveNext();
                Assert.AreEqual(2, enumerator.Current);

                enumerator.MoveNext();
                Assert.AreEqual(1, enumerator.Current);
            }
        }

        [Test]
        public void TestSum()
        {
            int[] data = { 1, 2, 3, 4 };

            Assert.AreEqual(10, data.Sum());
        }

        [Test]
        public void SumOnEmpty()
        {
            int[] data = { };

            Assert.AreEqual(0, data.Sum());
        }

        [Test]
        public void TestMax()
        {
            int[] data = { 1, 3, 5, 2 };

            Assert.AreEqual(5, data.Max());
        }

        [Test]
        public void TestMaxNullableInt32()
        {
            int?[] data = { null, null, null };

            Assert.IsNull(data.Max(x => -x));

            data = new int?[] { null, 1, 2 };

            Assert.AreEqual(-1, data.Max(x => -x));
        }

        [Test]
        public void TestMin()
        {
            int[] data = { 3, 5, 2, 6, 1, 7 };

            Assert.AreEqual(1, data.Min());
        }

        [Test]
        public void TestMinNullableInt32()
        {
            int?[] data = { null, null, null };

            Assert.IsNull(data.Min(x => -x));

            data = new int?[] { null, 1, 2 };

            Assert.AreEqual(-2, data.Min(x => -x));
        }

        [Test]
        public void TestMinStringEmpty()
        {
            Assert.IsNull((new string[0]).Min());
        }

        [Test]
        public void TestMaxStringEmpty()
        {
            Assert.IsNull((new string[0]).Max());
        }

        [Test]
        public void TestToList()
        {
            int[] data = { 3, 5, 2 };

            var list = data.ToList();

            AssertAreSame(data, list);

            Assert.AreEqual(typeof(List<int>), list.GetType());
        }

        [Test]
        public void TestToArray()
        {
            ICollection<int> coll = new List<int>
            {
                0,
                1,
                2
            };
            int[] result = { 0, 1, 2 };

            var array = coll.ToArray();

            AssertAreSame(result, array);

            Assert.AreEqual(typeof(int[]), array.GetType());
        }

        [Test]
        public void TestIntersect()
        {
            int[] left = { 1, 1 }, right = { 1, 1 };
            int[] result = { 1 };

            AssertAreSame(result, left.Intersect(right));
        }

        [Test]
        public void TestAverageOnInt32()
        {
            Assert.AreEqual(23.25, (new[] { 24, 7, 28, 34 }).Average());
        }

        [Test]
        public void TestAverageOnInt64()
        {
            Assert.AreEqual(23.25, (new long[] { 24, 7, 28, 34 }).Average());
        }

        [Test]
        public void TestAverageInt32()
        {
            // This does not overflow, computation is done with longs
            var x = new[] { int.MaxValue, int.MaxValue };
            Assert.AreEqual((double)int.MaxValue, x.Average());
        }

        [Test]
        [Category("NotDotNet")] // Mirosoft is failing at this, from .NET 3.5 on :/
        [Ignore("Not working")]
        public void TestAverageOverflowOnInt64()
        {
            var x = new[] { long.MaxValue, long.MaxValue };
            x.Average();
        }

        [Test]
        public void TestAverageOnLongNullable()
        {
            var list = new List<long?>
            {
                2,
                3
            };
            Assert.AreEqual(2.5d, list.Average());
        }

        [Test]
        public void TestRange()
        {
            AssertAreSame(new[] { 1, 2, 3, 4 }, Enumerable.Range(1, 4));
            AssertAreSame(new[] { 0, 1, 2, 3 }, Enumerable.Range(0, 4));
        }

        [Test]
        public void SingleValueOfMaxInt32()
        {
            AssertAreSame(new[] { int.MaxValue }, Enumerable.Range(int.MaxValue, 1));
        }

        [Test]
        public void EmptyRangeStartingAtMinInt32()
        {
            AssertAreSame(new int[0], Enumerable.Range(int.MinValue, 0));
        }

        [Test]
        public void TestTakeTakesProperNumberOfItems()
        {
            var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 0 });

            Assert.AreEqual(0, stream.Position);

            foreach (var b in AsEnumerable(stream).Take(2))
            {
                GC.KeepAlive(b);
            }

            Assert.AreEqual(2, stream.Position);
        }

        private static IEnumerable<byte> AsEnumerable(Stream stream)
        {
            while (true)
            {
                var b = stream.ReadByte();
                if (b < 0)
                {
                    break;
                }
                yield return (byte)b;
            }
        }

        [Test]
        public void TestOrderBy()
        {
            int[] array = { 14, 53, 3, 9, 11, 14, 5, 32, 2 };
            var q = from i in array
                    orderby i
                    select i;
            AssertIsOrdered(q);
        }

        private class Baz
        {
            private readonly string _name;
            private readonly int _age;

            public string Name
            {
                get
                {
                    if (string.IsNullOrEmpty(_name))
                    {
                        return Age.ToString();
                    }

                    return _name + " (" + Age.ToString() + ")";
                }
            }

            public int Age => _age + 1;

            public Baz(string name, int age)
            {
                _name = name;
                _age = age;
            }

            public override int GetHashCode()
            {
                return Age ^ Name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Baz b))
                {
                    return false;
                }
                return b.Age == Age && b.Name == Name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private static IEnumerable<Baz> CreateBazCollection()
        {
            return new[] {
                new Baz ("jb", 25),
                new Baz ("ana", 20),
                new Baz ("reg", 28),
                new Baz ("ro", 25),
                new Baz ("jb", 7),
            };
        }

        [Test]
        public void TestOrderByAgeAscendingTheByNameDescending()
        {
            var q = from b in CreateBazCollection()
                    orderby b.Age ascending, b.Name descending
                    select b;

            var expected = new[] {
                new Baz ("jb", 7),
                new Baz ("ana", 20),
                new Baz ("ro", 25),
                new Baz ("jb", 25),
                new Baz ("reg", 28),
            };

            AssertAreSame(expected, q);
        }

        private class Data
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return Id.ToString() + " " + Name;
            }
        }

        private IEnumerable<Data> CreateData()
        {
            return new[] {
                new Data { Id = 10, Name = "bcd" },
                new Data { Id = 20, Name = "Abcd" },
                new Data { Id = 20, Name = "Ab" },
                new Data { Id = 10, Name = "Zyx" },
            };
        }

        [Test]
        public void TestOrderByIdDescendingThenByNameAscending()
        {
            var q = from d in CreateData()
                    orderby d.Id descending, d.Name ascending
                    select d;

            var list = new List<Data>(q);

            Assert.AreEqual("Ab", list[0].Name);
            Assert.AreEqual("Abcd", list[1].Name);
            Assert.AreEqual("bcd", list[2].Name);
            Assert.AreEqual("Zyx", list[3].Name);
        }

        [Test]
        public void TestOrderByDescendingStability()
        {
            var data = new[] {
                new { Key = true, Value = 1},
                                                new { Key = false, Value = 2},
                new { Key = true, Value = 3},
                                                new { Key = false, Value = 4},
                new { Key = true, Value = 5},
                                                new { Key = false, Value = 6},
                new { Key = true, Value = 7},
                                                new { Key = false, Value = 8},
                new { Key = true, Value = 9},
                                                new { Key = false, Value = 10},
            };

            var expected = new[] {
                new { Key = true, Value = 1},
                new { Key = true, Value = 3},
                new { Key = true, Value = 5},
                new { Key = true, Value = 7},
                new { Key = true, Value = 9},
                                                new { Key = false, Value = 2},
                                                new { Key = false, Value = 4},
                                                new { Key = false, Value = 6},
                                                new { Key = false, Value = 8},
                                                new { Key = false, Value = 10},
            };

            AssertAreSame(expected, data.OrderByDescending(x => x.Key));
        }

        private static void AssertIsOrdered(IEnumerable<int> e)
        {
            var f = int.MinValue;
            foreach (var i in e)
            {
                Assert.IsTrue(f <= i);
                f = i;
            }
        }

        private static void AssertAreSame<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);

            var ee = Extensions.AsArray(expected);
            var ea = Extensions.AsArray(actual);

            if (ee.Length != ea.Length)
            {
                Assert.Fail("Wrong length");
            }

            for (int index = 0; index < ee.Length; index++)
            {
                Assert.AreEqual(ee[index], ea[index]);
            }
        }
    }
}