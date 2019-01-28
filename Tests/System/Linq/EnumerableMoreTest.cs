#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// EnumerableMoreTest.cs
//
// Author:
//  Andreas Noever <andreas.noever@gmail.com>
//
// (C) 2007 Andreas Noever
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
using Theraot.Collections;

namespace MonoTests.System.Linq
{
    [TestFixture]
    public class EnumerableMoreTest
    {
        private class BigEnumerable : IEnumerable<int>
        {
            public readonly ulong Count;

            public BigEnumerable(ulong count)
            {
                Count = count;
            }

            #region IEnumerable<int> Members

            public IEnumerator<int> GetEnumerator()
            {
                return new BigEnumerator(this);
            }

            #endregion IEnumerable<int> Members

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion IEnumerable Members
        }

        private class BigEnumerator : IEnumerator<int>
        {
            private readonly BigEnumerable _parent;
            private ulong _current;

            public BigEnumerator(BigEnumerable parent)
            {
                _parent = parent;
            }

            public int Current => 3;

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get { throw new NotImplementedException(); }
            }

            public bool MoveNext()
            {
                if (_current == _parent.Count)
                {
                    return false;
                }

                _current++;
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public static void AssertException<T>(Action action) where T : Exception
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
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
                Theraot.No.Op(exception);
                Assert.Fail("Expected: " + typeof(T).Name);
            }
        }

        private static void AssertAreSame<TK, TV>(TK expectedKey, IEnumerable<TV> expectedValues, IGrouping<TK, TV> actual)
        {
            if (expectedValues == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);

            Assert.AreEqual(expectedKey, actual.Key);

            var ee = expectedValues.GetEnumerator(); // TODO: Review
            var ea = actual.GetEnumerator();

            while (ee.MoveNext())
            {
                Assert.IsTrue(ea.MoveNext(), "'" + ee.Current + "' expected.");
                Assert.AreEqual(ee.Current, ea.Current);
            }

            if (ea.MoveNext())
            {
                Assert.Fail("Unexpected element: " + ee.Current);
            }
        }

        private static void AssertAreSame<TK, TV>(IDictionary<TK, IEnumerable<TV>> expected, IEnumerable<IGrouping<TK, TV>> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);

            var ee = expected.GetEnumerator(); // TODO: Review
            var ea = actual.GetEnumerator();

            while (ee.MoveNext())
            {
                Assert.IsTrue(ea.MoveNext(), "'" + ee.Current.Key + "' expected.");
                AssertAreSame(ee.Current.Key, ee.Current.Value, ea.Current);
            }

            if (ea.MoveNext())
            {
                Assert.Fail("Unexpected element: " + ee.Current.Key);
            }
        }

        private static void AssertAreSame<TK, TV>(IDictionary<TK, IEnumerable<TV>> expected, ILookup<TK, TV> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);

            var ee = expected.GetEnumerator(); // TODO: Review
            var ea = actual.GetEnumerator();

            while (ee.MoveNext())
            {
                Assert.IsTrue(ea.MoveNext(), "'" + ee.Current.Key + "' expected.");
                AssertAreSame(ee.Current.Key, ee.Current.Value, ea.Current);
            }

            if (ea.MoveNext())
            {
                Assert.Fail("Unexpected element: " + ee.Current.Key);
            }
        }

        private static void AssertAreSame<TK, TV>(IDictionary<TK, TV> expected, IDictionary<TK, TV> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);

            var ee = expected.GetEnumerator(); // TODO: Review
            var ea = actual.GetEnumerator();

            while (ee.MoveNext())
            {
                Assert.IsTrue(ea.MoveNext(), "'" + ee.Current.Key + ", " + ee.Current.Value + "' expected.");
                Assert.AreEqual(ee.Current.Key, ea.Current.Key);
                Assert.AreEqual(ee.Current.Value, ea.Current.Value);
            }

            if (ea.MoveNext())
            {
                Assert.Fail("Unexpected element: " + ee.Current.Key + ", " + ee.Current.Value);
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

            var ee = expected.GetEnumerator(); // TODO: Review
            var ea = actual.GetEnumerator();

            while (ee.MoveNext())
            {
                Assert.IsTrue(ea.MoveNext(), "'" + ee.Current + "' expected.");
                Assert.AreEqual(ee.Current, ea.Current);
            }

            if (ea.MoveNext())
            {
                Assert.Fail("Unexpected element: " + ea.Current);
            }
        }

        [Test]
        public void FirstArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // First<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).First());

            // First<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).First(x => true));
            AssertException<ArgumentNullException>(() => data.First(null));
        }

        [Test]
        public void FirstTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] empty = { };

            // First<TSource> ()
            Assert.AreEqual(2, data.First());
            AssertException<InvalidOperationException>(() => empty.First());

            // First<TSource> (Func<TSource, bool>)
            Assert.AreEqual(5, data.First(x => x == 5));
            AssertException<InvalidOperationException>(() => empty.First(x => x == 5));
            AssertException<InvalidOperationException>(() => data.First(x => x == 6));
        }

        [Test]
        public void FirstOrDefaultArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // FirstOrDefault<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).FirstOrDefault());

            // FirstOrDefault<TSource> (Func<string, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).FirstOrDefault(x => true));
            AssertException<ArgumentNullException>(() => data.FirstOrDefault(null));
        }

        [Test]
        public void FirstOrDefaultTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] empty = { };

            // FirstOrDefault<TSource> ()
            Assert.AreEqual(2, data.FirstOrDefault());
            Assert.AreEqual(0, empty.FirstOrDefault());

            // FirstOrDefault<TSource> (Func<TSource, bool>)
            Assert.AreEqual(5, data.FirstOrDefault(x => x == 5));
            Assert.AreEqual(0, empty.FirstOrDefault(x => x == 5));
            Assert.AreEqual(0, data.FirstOrDefault(x => x == 6));
        }

        [Test]
        public void LastArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Last<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Last());

            // Last<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Last(x => true));
            AssertException<ArgumentNullException>(() => data.Last(null));
        }

        [Test]
        public void LastTest()
        {
            int[] data = { 2, 1, 1, 3, 4, 5 };
            int[] empty = { };

            // Last<TSource> ()
            Assert.AreEqual(5, data.Last());
            AssertException<InvalidOperationException>(() => empty.Last());

            // Last<TSource> (Func<TSource, bool>)
            Assert.AreEqual(4, data.Last(x => x < 5));
            AssertException<InvalidOperationException>(() => empty.Last(x => x == 5));
            AssertException<InvalidOperationException>(() => data.Last(x => x == 6));
        }

        [Test]
        public void LastOrDefaultArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // LastOrDefault<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).LastOrDefault());

            // LastOrDefault<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).LastOrDefault(x => true));
            AssertException<ArgumentNullException>(() => data.LastOrDefault(null));
        }

        [Test]
        public void LastOrDefaultTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] empty = { };

            // LastOrDefault<TSource> ()
            Assert.AreEqual(4, data.LastOrDefault());
            Assert.AreEqual(0, empty.LastOrDefault());

            // LastOrDefault<TSource> (Func<TSource, bool>)
            Assert.AreEqual(3, data.LastOrDefault(x => x < 4));
            Assert.AreEqual(0, empty.LastOrDefault(x => x == 5));
            Assert.AreEqual(0, data.LastOrDefault(x => x == 6));
        }

        [Test]
        public void SingleArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Single<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Single());

            // Single<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Single(x => true));
            AssertException<ArgumentNullException>(() => data.Single(null));
        }

        [Test]
        public void SingleTest()
        {
            int[] data = { 2 };
            int[] data2 = { 2, 3, 5 };
            int[] empty = { };

            // Single<TSource> ()
            Assert.AreEqual(2, data.Single());
            AssertException<InvalidOperationException>(() => data2.Single());
            AssertException<InvalidOperationException>(() => empty.Single());

            // Single<TSource> (Func<TSource, bool>)
            Assert.AreEqual(5, data2.Single(x => x == 5));
            AssertException<InvalidOperationException>(() => data2.Single(x => false));
            AssertException<InvalidOperationException>(() => data2.Single(x => true));
            AssertException<InvalidOperationException>(() => empty.Single(x => true));
        }

        [Test]
        public void SingleOrDefaultArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // SingleOrDefault<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SingleOrDefault());

            // SingleOrDefault<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SingleOrDefault(x => true));
            AssertException<ArgumentNullException>(() => data.SingleOrDefault(null));
        }

        [Test]
        public void SingleOrDefaultTest()
        {
            int[] data = { 2 };
            int[] data2 = { 2, 3, 5 };
            int[] empty = { };

            // SingleOrDefault<TSource> ()
            Assert.AreEqual(2, data.SingleOrDefault());
            Assert.AreEqual(0, empty.SingleOrDefault());
            AssertException<InvalidOperationException>(() => data2.SingleOrDefault());

            // SingleOrDefault<TSource> (Func<TSource, bool>)
            Assert.AreEqual(3, data2.SingleOrDefault(x => x == 3));
            Assert.AreEqual(0, data2.SingleOrDefault(x => false));
            AssertException<InvalidOperationException>(() => data2.SingleOrDefault(x => true));
        }

        [Test]
        public void ElementAtArgumentNullTest()
        {
            // ElementAt<TSource> (int)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ElementAt(0));
        }

        [Test]
        public void ElementAtTest()
        {
            int[] data = { 2, 3, 4, 5 };

            // ElementAt<string> (int)
            Assert.AreEqual(2, data.ElementAt(0));
            Assert.AreEqual(4, data.ElementAt(2));
            AssertException<ArgumentOutOfRangeException>(() => data.ElementAt(-1));
            AssertException<ArgumentOutOfRangeException>(() => data.ElementAt(4));
            AssertException<ArgumentOutOfRangeException>(() => data.ElementAt(6));
        }

        [Test]
        public void ElementAtOrDefaultArgumentNullTest()
        {
            // ElementAtOrDefault<TSource> (int)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ElementAtOrDefault(0));
        }

        [Test]
        public void ElementAtOrDefaultTest()
        {
            int[] data = { 2, 3, 4, 5 };
            int[] empty = { };

            // ElementAtOrDefault<TSource> (int)
            Assert.AreEqual(2, data.ElementAtOrDefault(0));
            Assert.AreEqual(4, data.ElementAtOrDefault(2));
            Assert.AreEqual(0, data.ElementAtOrDefault(-1));
            Assert.AreEqual(0, data.ElementAtOrDefault(4));
            Assert.AreEqual(0, empty.ElementAtOrDefault(4));
        }

        [Test]
        public void EmptyTest()
        {
            var empty = Enumerable.Empty<string>();
            Assert.IsFalse(empty.GetEnumerator().MoveNext());
        }

        [Test]
        public void AnyArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Any<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Any());

            // Any<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Any(x => true));
            AssertException<ArgumentNullException>(() => data.Any(null));
        }

        [Test]
        public void AnyTest()
        {
            int[] data = { 5, 2, 3, 1, 6 };
            int[] empty = { };

            // Any<TSource> ()
            Assert.IsTrue(data.Any());
            Assert.IsFalse(empty.Any());

            // Any<TSource> (Func<TSource, bool>)
            Assert.IsTrue(data.Any(x => x == 5));
            Assert.IsFalse(data.Any(x => x == 9));
            Assert.IsFalse(empty.Any(x => true));
        }

        [Test]
        public void AllArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // All<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).All(x => true));
            AssertException<ArgumentNullException>(() => data.All(null));
        }

        [Test]
        public void AllTest()
        {
            int[] data = { 5, 2, 3, 1, 6 };
            int[] empty = { };

            // All<TSource> (Func<TSource, bool>)
            Assert.IsTrue(data.All(x => true));
            Assert.IsFalse(data.All(x => x != 1));
            Assert.IsTrue(empty.All(x => false));
        }

        [Test]
        public void CountArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Count<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Count());

            // Count<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Count(x => true));
            AssertException<ArgumentNullException>(() => data.Count(null));
        }

        [Test]
        public void CountTest()
        {
            int[] data = { 5, 2, 3, 1, 6 };

            // Count<TSource> ()
            Assert.AreEqual(5, data.Count());

            // Count<TSource> (Func<TSource, bool>)
            Assert.AreEqual(3, data.Count(x => x < 5));
        }

        //[Test]
        public void CountOverflowTest()
        {
            // Count<TSource> ()
            //AssertException<OverflowException> (delegate () { data.Count (); });

            // Count<TSource> (Func<TSource, bool>)
            //AssertException<OverflowException> (delegate () { data.Count (x => 3 == x); });

            // Documentation error: http://msdn2.microsoft.com/en-us/library/bb535181.aspx
            // An exception is only rasied if count > int.MaxValue. Not if source contains more than int.MaxValue elements.
            // AssertException<OverflowException> (delegate () { data.Count (x => 5 == x); });
        }

        [Test]
        public void LongCountArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // LongCount<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).LongCount());

            // LongCount<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).LongCount(x => true));
            AssertException<ArgumentNullException>(() => data.LongCount(null));
        }

        [Test]
        public void LongCountTest()
        {
            int[] data = { 5, 2, 3, 1, 6 };

            //TODO: Overflow test...

            // LongCount<TSource> ()
            Assert.AreEqual(5, data.LongCount());
            Assert.AreEqual(5, Enumerable.Range(0, 5).LongCount());

            // LongCount<TSource> (Func<TSource, bool>)
            Assert.AreEqual(3, data.LongCount(x => x < 5));
        }

        [Test]
        public void ContainsArgumentNullTest()
        {
            // Contains<TSource> (TSource)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Contains("2"));

            // Contains<TSource> (TSource, IEqualityComparer<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Contains("2", EqualityComparer<string>.Default));
        }

        private static void IsFalse(bool b, int[] data)
        {
            if (b)
            {
                Console.WriteLine(data.Contains(0));
                const object O = null;
                GC.KeepAlive(O.ToString());
                Assert.IsFalse(true);
            }
            //Console.WriteLine ("HIT!");
        }

        [Test]
        public void ContainsTest()
        {
            int[] data = { 5, 2, 3, 1, 6 };
            ICollection<int> icoll = data;

            // Contains<TSource> (TSource)
            Assert.IsTrue(data.Contains(2));
            for (var i = 0; i < 50; ++i)
            {
                Console.WriteLine(icoll.Contains(0));//Console.WriteLine (data.Contains (0));
            }

            IsFalse(data.Contains(0), data);

            // Contains<TSource> (TSource, IEqualityComparer<TSource>)
            Assert.IsTrue(data.Contains(2, EqualityComparer<int>.Default));
            Assert.IsFalse(data.Contains(0, EqualityComparer<int>.Default));
        }

        [Test]
        public void AggregateArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Aggregate<TSource> (Func<TSource, TSource, TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Aggregate((x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Aggregate(null));

            // Aggregate<TSource,TAccumulate> (TAccumulate, Func<TAccumulate, TSource, TAccumulate>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Aggregate("initial", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Aggregate("initial", null));

            // Aggregate<TSource,TAccumulate,TResult> (TAccumulate, Func<TAccumulate, TSource, TAccumulate>, Func<TAccumulate, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Aggregate("initial", (x, y) => "test", x => "test"));
            AssertException<ArgumentNullException>(() => data.Aggregate("initial", null, x => "test"));
            AssertException<ArgumentNullException>(() => data.Aggregate("initial", (x, y) => "test", (Func<string, string>)null));
        }

        [Test]
        public void AggregateTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };
            string[] empty = { };

            // Aggregate<TSource> (Func<TSource, TSource, TSource>)
            Assert.AreEqual("21534", data.Aggregate((x, y) => x + y));
            AssertException<InvalidOperationException>(() => empty.Aggregate((x, y) => x + y)); //only this overload throws

            // Aggregate<TSource,TAccumulate> (TAccumulate, Func<TAccumulate, TSource, TAccumulate>)
            Assert.AreEqual("initial21534", data.Aggregate("initial", (x, y) => x + y));

            // Aggregate<TSource,TAccumulate,TResult> (TAccumulate, Func<TAccumulate, TSource, TAccumulate>, Func<TAccumulate, TResult>)
            Assert.AreEqual("INITIAL21534", data.Aggregate("initial", (x, y) => x + y, x => x.ToUpper()));
        }

        [Test]
        public void SumArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Sum<TSource> (Func<TSource, int>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum(x => 0));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, int>)null));

            // Sum<TSource> (Func<TSource, Nullable<int>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum(x => (int?)0));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, int?>)null));

            // Sum<TSource> (Func<TSource, Int64>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum(x => 0L));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, long>)null));

            // Sum<TSource> (Func<TSource, Nullable<Int64>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum((Func<string, long?>)(x => (int?)0L)));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, long?>)null));

            // Sum<TSource> (Func<TSource, Single>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum(x => 0f));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, float>)null));

            // Sum<TSource> (Func<TSource, Nullable<Single>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum((Func<string, float?>)(x => (int?)0f)));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, float?>)null));

            // Sum<TSource> (Func<TSource, Double>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum(x => 0d));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, double>)null));

            // Sum<TSource> (Func<TSource, Nullable<Double>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum((Func<string, double?>)(x => (int?)0d)));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, double?>)null));

            // Sum<TSource> (Func<TSource, Decimal>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum(x => 0m));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, decimal>)null));

            // Sum<TSource> (Func<TSource, Nullable<Decimal>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Sum((Func<string, decimal?>)(x => (int?)0m)));
            AssertException<ArgumentNullException>(() => data.Sum((Func<string, decimal?>)null));

            // Sum (IEnumerable<int>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<int>)null).Sum());

            // Sum (IEnumerable<int?>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<int?>)null).Sum());

            // Sum (IEnumerable<long>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<long>)null).Sum());

            // Sum (IEnumerable<long?>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<long?>)null).Sum());

            // Sum (IEnumerable<float>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<float>)null).Sum());

            // Sum (IEnumerable<float?>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<float?>)null).Sum());

            // Sum (IEnumerable<double>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<double>)null).Sum());

            // Sum (IEnumerable<double?>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<double?>)null).Sum());

            // Sum (IEnumerable<decimal>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal>)null).Sum());

            // Sum (IEnumerable<decimal?>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal?>)null).Sum());
        }

        [Test]
        public void SumTest()
        {
            string[] data = { "2", "3", "5", "5" };

            //TODO: OverflowException

            // Sum<TSource> (Func<TSource, int>)
            Assert.AreEqual(15, data.Sum(x => int.Parse(x)));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum(x => int.Parse(x)));

            // Sum<TSource> (Func<TSource, Nullable<int>>)
            Assert.AreEqual(15, data.Sum(x => (int?)int.Parse(x)));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum(x => (int?)int.Parse(x)));

            // Sum<TSource> (Func<TSource, Int64>)
            Assert.AreEqual(15, data.Sum((Func<string, long>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, long>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Nullable<Int64>>)
            Assert.AreEqual(15, data.Sum((Func<string, long?>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, long?>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Single>)
            Assert.AreEqual(15, data.Sum((Func<string, float>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, float>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Nullable<Single>>)
            Assert.AreEqual(15, data.Sum((Func<string, float?>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, float?>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Double>)
            Assert.AreEqual(15, data.Sum((Func<string, double>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, double>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Nullable<Double>>)
            Assert.AreEqual(15, data.Sum((Func<string, double?>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, double?>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Decimal>)
            Assert.AreEqual(15, data.Sum((Func<string, decimal>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, decimal>)(x => int.Parse(x))));

            // Sum<TSource> (Func<TSource, Nullable<Decimal>>)
            Assert.AreEqual(15, data.Sum((Func<string, decimal?>)(x => int.Parse(x))));
            Assert.AreEqual(0, Enumerable.Empty<string>().Sum((Func<string, decimal?>)(x => int.Parse(x))));

            // Sum<> ()
            Assert.AreEqual(6, new[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<int>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new int?[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<int?>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new long[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<long>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new long?[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<long?>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new float[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<float>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new float?[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<float?>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new double[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<double>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new double?[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<double?>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new decimal[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<decimal>().Sum());

            // Sum<> ()
            Assert.AreEqual(6, new decimal?[] { 1, 2, 3 }.Sum());
            Assert.AreEqual(0, Enumerable.Empty<decimal?>().Sum());
        }

        [Test]
        public void MinArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Min<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min());

            // Min<TSource> (Func<TSource, int>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => 0));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, int>)null));

            // Min<TSource> (Func<TSource, Nullable<int>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => (int?)0));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, int?>)null));

            // Min<TSource> (Func<TSource, Int64>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => 0L));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, long>)null));

            // Min<TSource> (Func<TSource, Nullable<Int64>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min((Func<string, long?>)(x => (int?)0L)));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, long?>)null));

            // Min<TSource> (Func<TSource, Single>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => 0f));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, float>)null));

            // Min<TSource> (Func<TSource, Nullable<Single>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min((Func<string, float?>)(x => (int?)0f)));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, float?>)null));

            // Min<TSource> (Func<TSource, Double>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => 0d));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, double>)null));

            // Min<TSource> (Func<TSource, Nullable<Double>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min((Func<string, double?>)(x => (int?)0d)));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, double?>)null));

            // Min<TSource> (Func<TSource, Decimal>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => 0m));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, decimal>)null));

            // Min<TSource> (Func<TSource, Nullable<Decimal>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min((Func<string, decimal?>)(x => (int?)0m)));
            AssertException<ArgumentNullException>(() => data.Min((Func<string, decimal?>)null));

            // Min<TSource,TSource> (Func<TSource, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Min(x => "test"));
            AssertException<ArgumentNullException>(() => data.Min<string, string>(null));

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<int>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<int?>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<long>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<long?>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<float>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<float?>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<double>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<double?>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal>)null).Min());

            // Min<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal?>)null).Min());
        }

        [Test]
        public void MinTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Min<TSource> ()
            Assert.AreEqual("1", data.Min());

            // Min<TSource> (Func<TSource, int>)
            Assert.AreEqual(1, data.Min(x => int.Parse(x)));

            // Min<TSource> (Func<TSource, Nullable<int>>)
            Assert.AreEqual(1, data.Min(x => (int?)int.Parse(x)));

            // Min<TSource> (Func<TSource, Int64>)
            Assert.AreEqual(1, data.Min((Func<string, long>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Nullable<Int64>>)
            Assert.AreEqual(1, data.Min((Func<string, long?>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Single>)
            Assert.AreEqual(1, data.Min((Func<string, float>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Nullable<Single>>)
            Assert.AreEqual(1, data.Min((Func<string, float?>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Double>)
            Assert.AreEqual(1, data.Min((Func<string, double>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Nullable<Double>>)
            Assert.AreEqual(1, data.Min((Func<string, double?>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Decimal>)
            Assert.AreEqual(1, data.Min((Func<string, decimal>)(x => int.Parse(x))));

            // Min<TSource> (Func<TSource, Nullable<Decimal>>)
            Assert.AreEqual(1, data.Min((Func<string, decimal?>)(x => int.Parse(x))));

            // Min<TSource,TSource> (Func<TSource, TSource>)
            Assert.AreEqual("1", data.Min(x => x));

            // Min<> ()
            Assert.AreEqual(2, new[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new int?[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new long[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new long?[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new float[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new float?[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new double[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new double?[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new decimal[] { 2, 3, 4 }.Min());

            // Min<> ()
            Assert.AreEqual(2, new decimal?[] { 2, 3, 4 }.Min());
        }

        [Test]
        public void MaxArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Max<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max());

            // Max<TSource> (Func<TSource, int>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => 0));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, int>)null));

            // Max<TSource> (Func<TSource, Nullable<int>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => (int?)0));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, int?>)null));

            // Max<TSource> (Func<TSource, Int64>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => 0L));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, long>)null));

            // Max<TSource> (Func<TSource, Nullable<Int64>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max((Func<string, long?>)(x => (int?)0L)));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, long?>)null));

            // Max<TSource> (Func<TSource, Single>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => 0f));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, float>)null));

            // Max<TSource> (Func<TSource, Nullable<Single>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max((Func<string, float?>)(x => (int?)0f)));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, float?>)null));

            // Max<TSource> (Func<TSource, Double>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => 0d));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, double>)null));

            // Max<TSource> (Func<TSource, Nullable<Double>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max((Func<string, double?>)(x => (int?)0d)));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, double?>)null));

            // Max<TSource> (Func<TSource, Decimal>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => 0m));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, decimal>)null));

            // Max<TSource> (Func<TSource, Nullable<Decimal>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max((Func<string, decimal?>)(x => (int?)0m)));
            AssertException<ArgumentNullException>(() => data.Max((Func<string, decimal?>)null));

            // Max<TSource,TSource> (Func<TSource, TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Max(x => "test"));
            AssertException<ArgumentNullException>(() => data.Max<string, string>(null));

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<int>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<int?>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<long>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<long?>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<double>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<double?>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<float>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<float?>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal>)null).Max());

            // Max<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal?>)null).Max());
        }

        [Test]
        public void MaxTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Max<string> ()
            Assert.AreEqual("5", data.Max());

            // Max<TSource> (Func<TSource, int>)
            Assert.AreEqual(5, data.Max(x => int.Parse(x)));

            // Max<TSource> (Func<TSource, Nullable<int>>)
            Assert.AreEqual(5, data.Max(x => (int?)int.Parse(x)));

            // Max<TSource> (Func<TSource, Int64>)
            Assert.AreEqual(5, data.Max((Func<string, long>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Nullable<Int64>>)
            Assert.AreEqual(5, data.Max((Func<string, long?>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Single>)
            Assert.AreEqual(5, data.Max((Func<string, float>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Nullable<Single>>)
            Assert.AreEqual(5, data.Max((Func<string, float?>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Double>)
            Assert.AreEqual(5, data.Max((Func<string, double>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Nullable<Double>>)
            Assert.AreEqual(5, data.Max((Func<string, double?>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Decimal>)
            Assert.AreEqual(5, data.Max((Func<string, decimal>)(x => int.Parse(x))));

            // Max<TSource> (Func<TSource, Nullable<Decimal>>)
            Assert.AreEqual(5, data.Max((Func<string, decimal?>)(x => int.Parse(x))));

            // Max<TSource,TSource> (Func<TSource, TSource>)
            Assert.AreEqual("5", data.Max(x => x));

            // Max<> ()
            Assert.AreEqual(4, new[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new int?[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new long[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new long?[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new float[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new float?[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new double[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new double?[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new decimal[] { 2, 3, 4 }.Max());

            // Max<> ()
            Assert.AreEqual(4, new decimal?[] { 2, 3, 4 }.Max());
        }

        [Test]
        public void AverageArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Average<TSource> (Func<TSource, int>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average(x => 0));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, int>)null));

            // Average<TSource> (Func<TSource, Nullable<int>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average(x => (int?)0));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, int?>)null));

            // Average<TSource> (Func<TSource, Int64>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average(x => 0L));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, long>)null));

            // Average<TSource> (Func<TSource, Nullable<Int64>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average((Func<string, long?>)(x => (int?)0L)));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, long?>)null));

            // Average<TSource> (Func<TSource, Single>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average(x => 0f));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, float>)null));

            // Average<TSource> (Func<TSource, Nullable<Single>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average((Func<string, float?>)(x => (int?)0f)));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, float?>)null));

            // Average<TSource> (Func<TSource, Double>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average(x => 0d));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, double>)null));

            // Average<TSource> (Func<TSource, Nullable<Double>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average((Func<string, double?>)(x => (int?)0d)));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, double?>)null));

            // Average<TSource> (Func<TSource, Decimal>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average(x => 0m));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, decimal>)null));

            // Average<TSource> (Func<TSource, Nullable<Decimal>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Average((Func<string, decimal?>)(x => (int?)0m)));
            AssertException<ArgumentNullException>(() => data.Average((Func<string, decimal?>)null));

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<int>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<int?>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<long>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<long?>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<float>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<float?>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<double>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<double?>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal>)null).Average());

            // Average<> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<decimal?>)null).Average());
        }

        [Test]
        public void AverageTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };
            string[] empty = { };

            // Average<string> (Func<string, int>)
            Assert.AreEqual(3, data.Average(x => int.Parse(x)));
            AssertException<InvalidOperationException>(() => empty.Average(x => int.Parse(x)));

            // Average<TSource> (Func<TSource, Nullable<int>>)
            Assert.AreEqual(3, data.Average(x => (int?)int.Parse(x)));

            // Average<TSource> (Func<TSource, Int64>)
            Assert.AreEqual(3, data.Average((Func<string, long>)(x => int.Parse(x))));
            AssertException<InvalidOperationException>(() => empty.Average((Func<string, long>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Nullable<Int64>>)
            Assert.AreEqual(3, data.Average((Func<string, long?>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Single>)
            Assert.AreEqual(3, data.Average((Func<string, float>)(x => int.Parse(x))));
            AssertException<InvalidOperationException>(() => empty.Average((Func<string, float>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Nullable<Single>>)
            Assert.AreEqual(3, data.Average((Func<string, float?>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Double>)
            Assert.AreEqual(3, data.Average((Func<string, double>)(x => int.Parse(x))));
            AssertException<InvalidOperationException>(() => empty.Average((Func<string, double>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Nullable<Double>>)
            Assert.AreEqual(3, data.Average((Func<string, double?>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Decimal>)
            Assert.AreEqual(3, data.Average((Func<string, decimal>)(x => int.Parse(x))));
            AssertException<InvalidOperationException>(() => empty.Average((Func<string, decimal>)(x => int.Parse(x))));

            // Average<TSource> (Func<TSource, Nullable<Decimal>>)
            Assert.AreEqual(3, data.Average((Func<string, decimal?>)(x => int.Parse(x))));

            // Average<> ()
            Assert.AreEqual(3, new[] { 2, 3, 4 }.Average());
            AssertException<InvalidOperationException>(() => new int[0].Average());

            // Average<> ()
            Assert.AreEqual(3, new int?[] { 2, 3, 4 }.Average());

            // Average<> ()
            Assert.AreEqual(3, new long[] { 2, 3, 4 }.Average());
            AssertException<InvalidOperationException>(() => new long[0].Average());

            // Average<> ()
            Assert.AreEqual(3, new long?[] { 2, 3, 4 }.Average());

            // Average<> ()
            Assert.AreEqual(3, new float[] { 2, 3, 4 }.Average());
            AssertException<InvalidOperationException>(() => new float[0].Average());

            // Average<> ()
            Assert.AreEqual(3, new float?[] { 2, 3, 4 }.Average());

            // Average<> ()
            Assert.AreEqual(3, new double[] { 2, 3, 4 }.Average());
            AssertException<InvalidOperationException>(() => new double[0].Average());

            // Average<> ()
            Assert.AreEqual(3, new double?[] { 2, 3, 4 }.Average());

            // Average<> ()
            Assert.AreEqual(3, new decimal[] { 2, 3, 4 }.Average());
            AssertException<InvalidOperationException>(() => new decimal[0].Average());

            // Average<> ()
            Assert.AreEqual(3, new decimal?[] { 2, 3, 4 }.Average());
        }

        [Test]
        public void WhereArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Where<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Where(x => true));
            AssertException<ArgumentNullException>(() => data.Where((Func<string, bool>)null));

            // Where<TSource> (Func<TSource, int, bool>)
            AssertException<ArgumentNullException>(() => Enumerable.Where(((IEnumerable<string>)null), (x, y) => true));
            AssertException<ArgumentNullException>(() => Enumerable.Where(data, (Func<string, int, bool>)null));
        }

        [Test]
        public void WhereTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] expected1 = { 2, 1 };
            int[] expected2 = { 2 };

            // Where<TSource> (Func<TSource, bool>)
            AssertAreSame(expected1, data.Where(x => x < 3));

            // Where<TSource> (Func<TSource, int, bool>)
            AssertAreSame(expected2, Enumerable.Where(data, (x, y) => x < 3 && y != 1));
        }

        [Test]
        public void SelectArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Select<TSource,TResult> (Func<TSource, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Select(x => "test"));
            AssertException<ArgumentNullException>(() => data.Select((Func<string, string>)null));

            // Select<TSource,TResult> (Func<TSource, int, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Select((x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Select((Func<string, int, string>)null));
        }

        [Test]
        public void SelectTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };
            string[] expected1 = { "2x", "1x", "5x", "3x", "4x" };
            string[] expected2 = { "2x0", "1x1", "5x2", "3x3", "4x4" };

            // Select<TSource,TResult> (Func<TSource, TResult>)
            AssertAreSame(expected1, data.Select(x => x + "x"));

            // Select<TSource,TResult> (Func<TSource, int, TResult>)
            AssertAreSame(expected2, data.Select((x, y) => x + "x" + y.ToString()));
        }

        [Test]
        public void SelectManyArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // SelectMany<TSource,TResult> (Func<TSource, IEnumerable<TResult>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SelectMany(x => data));
            AssertException<ArgumentNullException>(() => data.SelectMany((Func<string, IEnumerable<string>>)null));

            // SelectMany<TSource,TResult> (Func<TSource, int, IEnumerable<TResult>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SelectMany((x, y) => data));
            AssertException<ArgumentNullException>(() => data.SelectMany((Func<string, int, IEnumerable<string>>)null));

            // SelectMany<TSource,TCollection,TResult> (Func<string, int, IEnumerable<TCollection>>, Func<TSource, TCollection, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SelectMany((x, y) => data, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.SelectMany((Func<string, int, IEnumerable<string>>)null, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.SelectMany((x, y) => data, (Func<string, string, string>)null));

            // SelectMany<TSource,TCollection,TResult> (Func<TSource, IEnumerable<TCollection>>, Func<TSource, TCollection, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SelectMany(x => data, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.SelectMany((Func<string, IEnumerable<string>>)null, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.SelectMany(x => data, (Func<string, string, string>)null));
        }

        [Test]
        public void SelectManyTest()
        {
            string[] data = { "0", "1" };
            string[] expected = { "0", "00", "1", "11" };

            // SelectMany<TSource,TResult> (Func<TSource, IEnumerable<TResult>>)
            AssertAreSame(expected, data.SelectMany(x => new[] { x, x + x }));

            // SelectMany<TSource,TResult> (Func<TSource, int, IEnumerable<TResult>>)
            AssertAreSame(expected, data.SelectMany((x, y) => new[] { x, x + y.ToString() }));

            // SelectMany<TSource,TCollection,TResult> (Func<string, int, IEnumerable<TCollection>>, Func<TSource, TCollection, TResult>)
            AssertAreSame(expected, data.SelectMany((x, y) => new[] { x, x + y.ToString() }, (x, y) => y));

            // SelectMany<TSource,TCollection,TResult> (Func<TSource, IEnumerable<TCollection>>, Func<TSource, TCollection, TResult>)
            AssertAreSame(expected, data.SelectMany(x => new[] { x, x + x }, (x, y) => y));
        }

        [Test]
        public void TakeArgumentNullTest()
        {
            // Take<TSource> (int)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Take(0));
        }

        [Test]
        public void TakeTest()
        {
            int[] data = { 2, 1, 5, 3, 1 };
            int[] expected = { 2, 1 };
            int[] empty = { };

            // Take<TSource> (int)
            AssertAreSame(expected, data.Take(2));
            AssertAreSame(empty, data.Take(-2));
        }

        [Test]
        public void TakeWhileArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // TakeWhile<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).TakeWhile(x => true));
            AssertException<ArgumentNullException>(() => data.TakeWhile((Func<string, bool>)null));

            // TakeWhile<TSource> (Func<TSource, int, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).TakeWhile((x, y) => true));
            AssertException<ArgumentNullException>(() => data.TakeWhile((Func<string, int, bool>)null));
        }

        [Test]
        public void TakeWhileTest()
        {
            int[] data = { 2, 1, 5, 3, 1 };
            int[] expected = { 2, 1 };

            // TakeWhile<TSource> (Func<TSource, bool>)
            AssertAreSame(expected, data.TakeWhile(x => x != 5));

            // TakeWhile<TSource> (Func<TSource, int, bool>)
            AssertAreSame(expected, data.TakeWhile((x, y) => y != 2));
        }

        [Test]
        public void SkipArgumentNullTest()
        {
            // Skip<TSource> (int)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Skip(0));
        }

        [Test]
        public void SkipTest()
        {
            int[] data = { 2, 1, 5, 3, 1 };
            int[] expected = { 5, 3, 1 };

            // Skip<string> (TSource)
            AssertAreSame(expected, data.Skip(2));
            AssertAreSame(data, data.Skip(-2));
        }

        [Test]
        public void SkipWhileArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // SkipWhile<TSource> (Func<TSource, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SkipWhile(x => true));
            AssertException<ArgumentNullException>(() => data.SkipWhile((Func<string, bool>)null));

            // SkipWhile<TSource> (Func<TSource, int, bool>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SkipWhile((x, y) => true));
            AssertException<ArgumentNullException>(() => data.SkipWhile((Func<string, int, bool>)null));
        }

        [Test]
        public void SkipWhileTest()
        {
            int[] data = { 2, 1, 5, 3, 1 };
            int[] expected = { 5, 3, 1 };

            // SkipWhile<TSource> (Func<TSource, bool>)
            AssertAreSame(expected, data.SkipWhile(x => x != 5));

            // SkipWhile<TSource> (Func<TSource, int, bool>)
            AssertAreSame(expected, data.SkipWhile((x, y) => y != 2));
        }

        [Test]
        public void JoinArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Join<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, TInner, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Join(data, x => "test", x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Join((IEnumerable<string>)null, x => "test", x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Join(data, null, x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Join(data, x => "test", null, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.Join(data, x => "test", x => "test", (Func<string, string, string>)null));

            // Join<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, TInner, TResult>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Join(data, x => "test", x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Join((IEnumerable<string>)null, x => "test", x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Join(data, null, x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Join(data, x => "test", null, (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Join(data, x => "test", x => "test", (Func<string, string, string>)null, EqualityComparer<string>.Default));
        }

        [Test]
        public void JoinTest()
        {
            string[] dataOuter1 = { "2", "1", "5", "3", "4" };
            string[] dataInner1 = { "7", "3", "5", "8", "9" };
            string[] expected1 = { "55", "33" };

            string[] dataOuter2 = { "2", "1", "3", "4" };
            string[] dataInner2 = { "7", "5", "8", "9" };
            string[] expected2 = { };

            // Join<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, TInner, TResult>)
            AssertAreSame(expected1, dataOuter1.Join(dataInner1, x => x, x => x, (x, y) => x + y));
            AssertAreSame(expected2, dataOuter2.Join(dataInner2, x => x, x => x, (x, y) => x + y));

            // Join<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, TInner, TResult>, IEqualityComparer<string>)
            AssertAreSame(expected1, dataOuter1.Join(dataInner1, x => x, x => x, (x, y) => x + y, EqualityComparer<string>.Default));
            AssertAreSame(expected2, dataOuter2.Join(dataInner2, x => x, x => x, (x, y) => x + y, EqualityComparer<string>.Default));
        }

        [Test]
        public void JoinTestNullKeys()
        {
            var l1 = new[] {
                new { Name = "name1", Nullable = (int?) null },
                new { Name = "name2", Nullable = (int?) null }
            };

            var count = l1.Join(l1, i => i.Nullable, i => i.Nullable, (x, y) => x.Name).Count();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void GroupJoinArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // GroupJoin<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, IEnumerable<TInner>, TResult>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupJoin(data, x => "test", x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupJoin((IEnumerable<string>)null, x => "test", x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupJoin(data, null, x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupJoin(data, x => "test", null, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupJoin(data, x => "test", x => "test", (Func<string, IEnumerable<string>, string>)null));

            // GroupJoin<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, IEnumerable<TInner>, TResult, IEqualityComparer<TKey>>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupJoin(data, x => "test", x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupJoin((IEnumerable<string>)null, x => "test", x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupJoin(data, null, x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupJoin(data, x => "test", null, (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupJoin(data, x => "test", x => "test", (Func<string, IEnumerable<string>, string>)null, EqualityComparer<string>.Default));
        }

        [Test]
        public void GroupJoinTest()
        {
            string[] dataOuter1 = { "2", "1", "5", "3", "4" };
            string[] dataInner1 = { "7", "3", "5", "3", "9" };
            string[] expected1 = { "2", "1", "55", "333", "4" };

            string[] dataOuter2 = { "2", "1", "5", "8", "4" };
            string[] dataInner2 = { "7", "3", "6", "3", "9" };
            string[] expected2 = { "2", "1", "5", "8", "4" };

            // GroupJoin<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, IEnumerable<TInner>, TResult>)
            AssertAreSame(expected1, dataOuter1.GroupJoin(dataInner1, x => x, x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }));
            AssertAreSame(expected2, dataOuter2.GroupJoin(dataInner2, x => x, x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }));

            // GroupJoin<TOuter,TInner,TKey,TResult> (IEnumerable<TInner>, Func<TOuter, TKey>, Func<TInner, TKey>, Func<TOuter, IEnumerable<TInner>, TResult, IEqualityComparer<TKey>>)
            AssertAreSame(expected1, dataOuter1.GroupJoin(dataInner1, x => x, x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }, EqualityComparer<string>.Default));
            AssertAreSame(expected2, dataOuter2.GroupJoin(dataInner2, x => x, x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }, EqualityComparer<string>.Default));
        }

        [Test]
        public void GroupJoinWithNullKeys()
        {
            string[] l1 = { null };
            string[] l2 = { null, null };
            var res = l1.GroupJoin(l2, x => x, y => y, (a, b) => new
            {
                Key = a,
                Count = b.Count()
            }).ToArray();
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(0, res[0].Count);
        }

        [Test]
        public void OrderByArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // OrderBy<TSource,TKey> (Func<TSource, TKey>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).OrderBy(x => "test"));
            AssertException<ArgumentNullException>(() => data.OrderBy((Func<string, string>)null));

            // OrderBy<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).OrderBy(x => "test", Comparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.OrderBy(null, Comparer<string>.Default));
        }

        [Test]
        public void OrderByTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] expected = { 1, 2, 3, 4, 5 };

            // OrderBy<TSource,TKey> (Func<TSource, TKey>)
            AssertAreSame(expected, data.OrderBy(x => x));

            // OrderBy<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertAreSame(expected, data.OrderBy(x => x, Comparer<int>.Default));
        }

        [Test]
        public void OrderByDescendingArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // OrderByDescending<TSource,TKey> (Func<TSource, TKey>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).OrderByDescending(x => "test"));
            AssertException<ArgumentNullException>(() => data.OrderByDescending((Func<string, string>)null));

            // OrderByDescending<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).OrderByDescending(x => "test", Comparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.OrderByDescending(null, Comparer<string>.Default));
        }

        [Test]
        public void OrderByDescendingTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] expected = { 5, 4, 3, 2, 1 };

            // OrderByDescending<TSource,TKey> (Func<TSource, TKey>)
            AssertAreSame(expected, data.OrderByDescending(x => x));

            // OrderByDescending<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertAreSame(expected, data.OrderByDescending(x => x, Comparer<int>.Default));
        }

        [Test]
        public void ThenByArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // ThenBy<TSource,TKey> (Func<TSource, TKey>)
            AssertException<ArgumentNullException>(() => ((IOrderedEnumerable<string>)null).ThenBy(x => "test"));
            AssertException<ArgumentNullException>(() => data.OrderBy(x => x).ThenBy((Func<string, string>)null));

            // ThenBy<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertException<ArgumentNullException>(() => ((IOrderedEnumerable<string>)null).ThenBy(x => "test", Comparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.OrderBy(x => x).ThenBy(null, Comparer<string>.Default));
        }

        [Test]
        public void ThenByTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] expected = { 1, 2, 3, 4, 5 };

            // ThenBy<TSource,TKey> (Func<TSource, TKey>)
            AssertAreSame(expected, data.OrderBy(x => x).ThenBy(x => x));

            // ThenBy<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertAreSame(expected, data.OrderBy(x => x).ThenBy(x => x, Comparer<int>.Default));
        }

        [Test]
        public void ThenByDescendingArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // ThenByDescending<TSource,TKey> (Func<TSource, TKey>)
            AssertException<ArgumentNullException>(() => ((IOrderedEnumerable<string>)null).ThenByDescending(x => "test"));
            AssertException<ArgumentNullException>(() => data.OrderBy(x => x).ThenByDescending((Func<string, string>)null));

            // ThenByDescending<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertException<ArgumentNullException>(() => ((IOrderedEnumerable<string>)null).ThenByDescending(x => "test", Comparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.OrderBy(x => x).ThenByDescending(null, Comparer<string>.Default));
        }

        [Test]
        public void ThenByDescendingTest()
        {
            int[] data = { 2, 1, 5, 3, 4 };
            int[] expected = { 5, 4, 3, 2, 1 };

            // ThenByDescending<TSource,TKey> (Func<TSource, TKey>)
            AssertAreSame(expected, data.OrderBy(x => 0).ThenByDescending(x => x));

            // ThenByDescending<TSource,TKey> (Func<TSource, TKey>, IComparer<string>)
            AssertAreSame(expected, data.OrderBy(x => 0).ThenByDescending(x => x, Comparer<int>.Default));
        }

        [Test]
        public void GroupByArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // GroupBy<string,string> (Func<string, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string>(null));

            // GroupBy<string,string> (Func<string, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy(null, EqualityComparer<string>.Default));

            // GroupBy<string,string,string> (Func<string, string>, Func<string, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", x => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string>(null, x => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy(x => "test", (Func<string, string>)null));

            // GroupBy<string,string,string> (Func<string, string>, Func<string, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy(null, x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy(x => "test", (Func<string, string>)null, EqualityComparer<string>.Default));

            // GroupBy<string,string,string> (Func<string, string>, Func<string, IEnumerable<string>, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string>(null, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy(x => "test", (Func<string, IEnumerable<string>, string>)null));

            // GroupBy<string,string,string,string> (Func<string, string>, Func<string, string>, Func<string, IEnumerable<string>, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string, string>(null, x => "test", (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string, string>(x => "test", null, (x, y) => "test"));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string, string>(x => "test", x => "test", null));

            // GroupBy<string,string,string> (Func<string, string>, Func<string, IEnumerable<string>, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy(null, (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy(x => "test", (Func<string, IEnumerable<string>, string>)null, EqualityComparer<string>.Default));

            // GroupBy<string,string,string,string> (Func<string, string>, Func<string, string>, Func<string, IEnumerable<string>, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).GroupBy(x => "test", x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy(null, x => "test", (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string, string>(x => "test", null, (x, y) => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.GroupBy<string, string, string, string>(x => "test", x => "test", null, EqualityComparer<string>.Default));
        }

        [Test]
        public void GroupByTest()
        {
            string[] data = { "2", "1", "5", "3", "4", "3" };

            var expected = new Dictionary<string, IEnumerable<string>>
            {
                { "2", new List<string> { "2" } },
                { "1", new List<string> { "1" } },
                { "5", new List<string> { "5" } },
                { "3", new List<string> { "3", "3" } },
                { "4", new List<string> { "4" } }
            };
            var expected2 = new Dictionary<string, IEnumerable<string>>
            {
                { "2", new List<string> { "22" } },
                { "1", new List<string> { "11" } },
                { "5", new List<string> { "55" } },
                { "3", new List<string> { "33", "33" } },
                { "4", new List<string> { "44" } }
            };
            var expected3 = new[] { "22", "11", "55", "333", "44" };

            // GroupBy<int,int> (Func<int, int>)
            AssertAreSame(expected, data.GroupBy(x => x));

            // GroupBy<int,int> (Func<int, int>, IEqualityComparer<int>)
            AssertAreSame(expected, data.GroupBy(x => x, EqualityComparer<string>.Default));

            // GroupBy<int,int,int> (Func<int, int>, Func<int, int>)
            AssertAreSame(expected2, data.GroupBy(x => x, x => x + x));

            // GroupBy<int,int,int> (Func<int, int>, Func<int, int>, IEqualityComparer<int>)
            AssertAreSame(expected2, data.GroupBy(x => x, x => x + x, EqualityComparer<string>.Default));

            // GroupBy<int,int,int> (Func<int, int>, Func<int, IEnumerable<int>, int>)
            AssertAreSame(expected3, data.GroupBy(x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }));

            // GroupBy<int,int,int,int> (Func<int, int>, Func<int, int>, Func<int, IEnumerable<int>, int>)
            AssertAreSame(expected3, data.GroupBy(x => x, x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }));

            // GroupBy<int,int,int> (Func<int, int>, Func<int, IEnumerable<int>, int>, IEqualityComparer<int>)
            AssertAreSame(expected3, data.GroupBy(x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }, EqualityComparer<string>.Default));

            // GroupBy<int,int,int,int> (Func<int, int>, Func<int, int>, Func<int, IEnumerable<int>, int>, IEqualityComparer<int>)
            AssertAreSame(expected3, data.GroupBy(x => x, x => x, (x, y) =>
            {
                foreach (var s in y)
                {
                    x += s;
                }

                return x;
            }, EqualityComparer<string>.Default));
        }

        private class Data
        {
            public int Number;
            public readonly string String;

            public Data(int number, string str)
            {
                Number = number;
                String = str;
            }
        }

        [Test]
        public void GroupByLastNullGroup()
        {
            var values = new List<Data>
            {
                new Data(0, "a"),
                new Data(1, "a"),
                new Data(2, "b"),
                new Data(3, "b"),
                new Data(4, null)
            };
            var groups = values.GroupBy(d => d.String);

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
        public void ConcatArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Concat<TSource> (IEnumerable<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Concat(data));
            AssertException<ArgumentNullException>(() => data.Concat(null));
        }

        [Test]
        public void ConcatTest()
        {
            int[] data1 = { 2, 1, 5, 3, 4 };
            int[] data2 = { 1, 2, 3, 4, 5 };
            int[] expected = { 2, 1, 5, 3, 4, 1, 2, 3, 4, 5 };

            // Concat<TSource> (IEnumerable<TSource>)
            AssertAreSame(expected, data1.Concat(data2));
        }

        [Test]
        public void DistinctArgumentNullTest()
        {
            // Distinct<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Distinct());

            // Distinct<TSource> (IEqualityComparer<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Distinct(EqualityComparer<string>.Default));
        }

        [Test]
        public void DistinctTest()
        {
            int[] data = { 2, 1, 5, 3, 4, 2, 5, 3, 1, 8 };
            int[] expected = { 2, 1, 5, 3, 4, 8 };

            // Distinct<TSource> ()
            AssertAreSame(expected, data.Distinct());

            // Distinct<iTSourcent> (IEqualityComparer<TSource>)
            AssertAreSame(expected, data.Distinct(EqualityComparer<int>.Default));
        }

        [Test]
        public void UnionArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Union<TSource> (IEnumerable<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Union(data));
            AssertException<ArgumentNullException>(() => data.Union(null));

            // Union<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Union(data, EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Union(null, EqualityComparer<string>.Default));
        }

        [Test]
        public void UnionTest()
        {
            int[] data1 = { 2, 1, 5, 7, 3, 4 };
            int[] data2 = { 1, 2, 3, 8, 4, 5 };
            int[] expected = { 2, 1, 5, 7, 3, 4, 8 };

            // Union<TSource> (IEnumerable<TSource>)
            AssertAreSame(expected, data1.Union(data2));

            // Union<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            AssertAreSame(expected, data1.Union(data2, EqualityComparer<int>.Default));
        }

        [Test]
        public void IntersectArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Intersect<TSource> (IEnumerable<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Intersect(data));
            AssertException<ArgumentNullException>(() => data.Intersect(null));

            // Intersect<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Intersect(data, EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Intersect(null, EqualityComparer<string>.Default));
        }

        [Test]
        public void IntersectTest()
        {
            int[] data1 = { 2, 1, 5, 7, 3, 4 };
            int[] data2 = { 1, 2, 3, 8, 4, 5 };
            int[] expected = { 2, 1, 5, 3, 4 };

            // Intersect<TSource> (IEnumerable<TSource>)
            AssertAreSame(expected, data1.Intersect(data2));

            // Intersect<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            AssertAreSame(expected, data1.Intersect(data2, EqualityComparer<int>.Default));
        }

        [Test]
        public void ExceptArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // Except<TSource> (IEnumerable<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Except(data));
            AssertException<ArgumentNullException>(() => data.Except(null));

            // Except<TSource> (IEnumerable<string>, IEqualityComparer<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Except(data, EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.Except(null, EqualityComparer<string>.Default));
        }

        [Test]
        public void ExceptTest()
        {
            int[] data1 = { 2, 1, 5, 7, 3, 4 };
            int[] data2 = { 1, 2, 3, 8, 4, 5 };
            int[] expected = { 7 };

            // Except<TSource> (IEnumerable<TSource>)
            AssertAreSame(expected, data1.Except(data2));

            // Except<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            AssertAreSame(expected, data1.Except(data2, EqualityComparer<int>.Default));
        }

        [Test]
        public void ReverseArgumentNullTest()
        {
            // Reverse<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).Reverse());
        }

        [Test]
        public void ReverseTest()
        {
            int[] data = { 2, 1, 5, 7, 3, 4 };
            int[] expected = { 4, 3, 7, 5, 1, 2 };

            // Reverse<TSource> ()
            AssertAreSame(expected, data.Reverse());
        }

        [Test]
        public void SequenceEqualArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // SequenceEqual<TSource> (IEnumerable<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SequenceEqual(data));
            AssertException<ArgumentNullException>(() => data.SequenceEqual(null));

            // SequenceEqual<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).SequenceEqual(data, EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.SequenceEqual(null, EqualityComparer<string>.Default));
        }

        [Test]
        public void SequenceEqualTest()
        {
            int[] data1 = { 2, 1, 5, 7, 3, 4 };
            int[] data2 = { 2, 1, 5, 7, 3, 4 };
            int[] data3 = { 2, 1, 5, 7, 3, 4, 5 };
            int[] data4 = { 2, 1, 5, 7, 3 };
            int[] data5 = { 2, 1, 5, 8, 3, 4 };

            // SequenceEqual<TSource> (IEnumerable<TSource>)
            Assert.IsTrue(data1.SequenceEqual(data2));
            Assert.IsFalse(data1.SequenceEqual(data3));
            Assert.IsFalse(data1.SequenceEqual(data4));
            Assert.IsFalse(data1.SequenceEqual(data5));

            // SequenceEqual<TSource> (IEnumerable<TSource>, IEqualityComparer<TSource>)
            Assert.IsTrue(data1.SequenceEqual(data2, EqualityComparer<int>.Default));
            Assert.IsFalse(data1.SequenceEqual(data3, EqualityComparer<int>.Default));
            Assert.IsFalse(data1.SequenceEqual(data4, EqualityComparer<int>.Default));
            Assert.IsFalse(data1.SequenceEqual(data5, EqualityComparer<int>.Default));
        }

        [Test]
        public void AsEnumerableArgumentNullTest()
        {
            // Empty
        }

        [Test]
        public void AsEnumerableTest()
        {
            int[] data = { 2, 1, 5, 7, 3, 4 };

            // AsEnumerable<TSource> ()
            Assert.AreSame(data, data.AsEnumerable());
        }

        [Test]
        public void ToArrayArgumentNullTest()
        {
            // ToArray<TSource> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToArray());
        }

        [Test]
        public void ToArrayTest()
        {
            int[] data = { 2, 3, 4, 5 };
            int[] expected = { 2, 3, 4, 5 };

            // ToArray<TSource> ()
            AssertAreSame(expected, Enumerable.ToArray(data));
        }

        [Test]
        public void ToListArgumentNullTest()
        {
            // ToList<string> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToList());
        }

        [Test]
        public void ToListTest()
        {
            int[] data = { 2, 4, 5, 1 };
            int[] expected = { 2, 4, 5, 1 };

            // ToList<int> ()
            AssertAreSame(expected, data.ToList());
        }

        [Test]
        public void ToDictionaryArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // ToDictionary<TSource,TKey> (Func<TSource, TKey>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToDictionary(x => "test"));
            AssertException<ArgumentNullException>(() => data.ToDictionary((Func<string, string>)null));

            // ToDictionary<TSource,TKey> (Func<TSource, TKey>, IEqualityComparer<TKey>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToDictionary(x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.ToDictionary(null, EqualityComparer<string>.Default));

            // ToDictionary<TSource,TKey,TElement> (Func<TSource, TKey>, Func<TSource, TElement>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToDictionary(x => "test", x => "test"));
            AssertException<ArgumentNullException>(() => data.ToDictionary((Func<string, string>)null, x => "test"));
            AssertException<ArgumentNullException>(() => data.ToDictionary(x => "test", (Func<string, string>)null));

            // ToDictionary<TSource,TKey,TElement> (Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToDictionary(x => "test", x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.ToDictionary(null, x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.ToDictionary(x => "test", (Func<string, string>)null, EqualityComparer<string>.Default));
        }

        [Test]
        public void ToDictionaryTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };
            var expected = new Dictionary<string, string>
            {
                { "k2", "2" },
                { "k1", "1" },
                { "k5", "5" },
                { "k3", "3" },
                { "k4", "4" }
            };

            // ToDictionary<TSource,TKey> (Func<TSource, TKey>)
            AssertAreSame(expected, data.ToDictionary(x => "k" + x));
            AssertException<ArgumentException>(() => data.ToDictionary(x => "key"));

            // ToDictionary<TSource,TKey> (Func<TSource, TKey>, IEqualityComparer<TKey>)
            AssertAreSame(expected, data.ToDictionary(x => "k" + x, EqualityComparer<string>.Default));
            AssertException<ArgumentException>(() => data.ToDictionary(x => "key", EqualityComparer<string>.Default));

            // ToDictionary<TSource,TKey,TElement> (Func<TSource, TKey>, Func<TSource, TElement>)
            AssertAreSame(expected, data.ToDictionary(x => "k" + x, x => x));
            AssertException<ArgumentException>(() => data.ToDictionary(x => "key", x => x));

            // ToDictionary<TSource,TKey,TElement> (Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>)
            AssertAreSame(expected, data.ToDictionary(x => "k" + x, x => x, EqualityComparer<string>.Default));
            AssertException<ArgumentException>(() => data.ToDictionary(x => "key", x => x, EqualityComparer<string>.Default));
        }

        [Test]
        public void ToLookupArgumentNullTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };

            // ToLookup<string,string> (Func<string, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToLookup(x => "test"));
            AssertException<ArgumentNullException>(() => data.ToLookup<string, string>(null));

            // ToLookup<string,string> (Func<string, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToLookup(x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.ToLookup(null, EqualityComparer<string>.Default));

            // ToLookup<string,string,string> (Func<string, string>, Func<string, string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToLookup(x => "test", x => "test"));
            AssertException<ArgumentNullException>(() => data.ToLookup<string, string, string>(null, x => "test"));
            AssertException<ArgumentNullException>(() => data.ToLookup<string, string, string>(x => "test", null));

            // ToLookup<string,string,string> (Func<string, string>, Func<string, string>, IEqualityComparer<string>)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).ToLookup(x => "test", x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.ToLookup(null, x => "test", EqualityComparer<string>.Default));
            AssertException<ArgumentNullException>(() => data.ToLookup<string, string, string>(x => "test", null, EqualityComparer<string>.Default));
        }

        [Test]
        public void ToLookupTest()
        {
            string[] data = { "23", "12", "55", "42", "41" };
            var expected = new Dictionary<string, IEnumerable<string>>
            {
                { "2", new List<string> { "23" } },
                { "1", new List<string> { "12" } },
                { "5", new List<string> { "55" } },
                { "4", new List<string> { "42", "41" } }
            };
            Assert.AreEqual(expected.Count, data.ToLookup(x => x[0].ToString()).Count);

            // ToLookup<string,string> (Func<string, string>)
            AssertAreSame(expected, data.ToLookup(x => x[0].ToString()));

            // ToLookup<string,string> (Func<string, string>, IEqualityComparer<string>)
            AssertAreSame(expected, data.ToLookup(x => x[0].ToString(), EqualityComparer<string>.Default));

            // ToLookup<string,string,string> (Func<string, string>, Func<string, string>)
            AssertAreSame(expected, data.ToLookup(x => x[0].ToString(), x => x));

            // ToLookup<string,string,string> (Func<string, string>, Func<string, string>, IEqualityComparer<string>)
            AssertAreSame(expected, data.ToLookup(x => x[0].ToString(), x => x, EqualityComparer<string>.Default));
        }

        [Test]
        public void ToLookupNullKeyTest()
        {
            var strs = new[] { "one", null, "two", null, "three" };

            var i = 0;
            var l = strs.ToLookup(s => (s == null) ? null : "numbers", s => s ?? (++i).ToString());

            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l[null].Count());
            Assert.IsTrue(l[null].Contains("1"));
            Assert.IsTrue(l[null].Contains("2"));

            Assert.AreEqual(3, l["numbers"].Count());
            Assert.IsTrue(l["numbers"].Contains("one"));
            Assert.IsTrue(l["numbers"].Contains("two"));
            Assert.IsTrue(l["numbers"].Contains("three"));
        }

        [Test]
        public void DefaultIfEmptyArgumentNullTest()
        {
            // DefaultIfEmpty<string> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).DefaultIfEmpty());

            // DefaultIfEmpty<string> (string)
            AssertException<ArgumentNullException>(() => ((IEnumerable<string>)null).DefaultIfEmpty("default"));
        }

        [Test]
        public void DefaultIfEmptyTest()
        {
            string[] data = { "2", "1", "5", "3", "4" };
            string[] empty = { };
            string[] default1 = { null };
            string[] default2 = { "default" };

            // DefaultIfEmpty<string> ()
            AssertAreSame(data, data.DefaultIfEmpty());
            AssertAreSame(default1, empty.DefaultIfEmpty());

            // DefaultIfEmpty<string> (string)
            AssertAreSame(data, data.DefaultIfEmpty("default"));
            AssertAreSame(default2, empty.DefaultIfEmpty("default"));
        }

        [Test]
        public void OfTypeArgumentNullTest()
        {
            // OfType<string> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable)null).OfType<string>());
        }

        [Test]
        public void OfTypeTest()
        {
            object[] data = { "2", 2, "1", "5", "3", "4" };
            string[] expected = { "2", "1", "5", "3", "4" };

            // OfType<string> ()
            AssertAreSame(expected, data.OfType<string>());
        }

        [Test]
        public void CastArgumentNullTest()
        {
            // Cast<string> ()
            AssertException<ArgumentNullException>(() => ((IEnumerable)null).Cast<string>());
        }

        [Test]
        public void CastTest()
        {
            object[] data = { 1, 2, 3 };
            int[] expected = { 1, 2, 3 };

            // Cast<string> ()
            AssertAreSame(expected, data.Cast<int>());
            AssertException<InvalidCastException>(() => data.Cast<IEnumerable>().GetEnumerator().MoveNext());
            data.Cast<IEnumerable>();
        }

        [Test]
        public void RangeArgumentNullTest()
        {
            // Empty
        }

        [Test]
        public void RangeTest()
        {
            int[] expected = { 2, 3, 4, 5 };

            // Range<> (int)
            AssertAreSame(expected, Enumerable.Range(2, 4));
            AssertException<ArgumentOutOfRangeException>(() => Enumerable.Range(2, -3));
            AssertException<ArgumentOutOfRangeException>(() => Enumerable.Range(int.MaxValue - 5, 7));
            Enumerable.Range(int.MaxValue - 5, 6);
        }

        [Test]
        public void ExceptMultipleItems()
        {
            var data = new[] { 1, 2, 2, 2, 3, 4, 5, 5, 6, 7, 8, 8, 9, 10 };
            var expected = new[] { 2, 4, 6, 8, 10 };

            AssertAreSame(expected, data.Except(new[] { 1, 3, 5, 7, 9 }));
        }
    }
}