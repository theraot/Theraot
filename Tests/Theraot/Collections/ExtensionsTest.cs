using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Theraot.Collections;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    class ExtensionsTest
    {
        [Test]
        public void AddRange()
        {
            var collection = new List<int>();
            // Adds and returns the number of added items
            Assert.AreEqual(4, Extensions.AddRange(collection, new[] { 0, 1, 2, 3 }));
            Assert.AreEqual(4, collection.Count);
            // Adds and allows to iterate over the items as they are added
            Assert.IsTrue(collection.AddRangeEnumerable(new[] { 10, 20, 30 }).SequenceEqual(new[] { 10, 20, 30 }));
            var count = collection.Count;
            Assert.AreEqual(7, count);
            foreach (var item in collection.AddRangeEnumerable(new[] { 100, 200, 300 }))
            {
                GC.KeepAlive(item);
                count++;
                Assert.AreEqual(count, collection.Count);
            }
            Assert.IsTrue(collection.SequenceEqual(new[] { 0, 1, 2, 3, 10, 20, 30, 100, 200, 300 }));
        }

        [Test]
        public void After()
        {
            var check = false;
            var index = 0;
            var array = new List<int> { 0, 1, 2, 3 };

            // Does something after the iteration is over
            foreach (var item in array.After(() => check = true))
            {
                Assert.AreEqual(index, item);
                index++;
                Assert.IsFalse(check);
            }
            Assert.IsTrue(check);

            check = false;
            foreach (var item in (new int[] { }).After(() => check = true))
            {
                GC.KeepAlive(item);
                // There should be no items
                Assert.Fail();
            }
            Assert.IsTrue(check);

            index = 0;
            check = false;
            foreach (var item in array.AfterAny(() => check = true))
            {
                Assert.AreEqual(index, item);
                index++;
                Assert.IsFalse(check);
            }
            Assert.IsTrue(check);

            check = false;
            foreach (var item in (new int[] { }).AfterAny(() => check = true))
            {
                GC.KeepAlive(item);
                // There should be no items
                Assert.Fail();
            }
            Assert.IsFalse(check);

            // Does something after the iteration is over - taking the count
            index = 0;
            foreach (var item in array.AfterCounted(count => index = count))
            {
                GC.KeepAlive(item);
                Assert.AreEqual(0, index);
            }
            Assert.AreEqual(4, index);

            index = 0;
            foreach (var item in (new int[] { }).AfterCounted(count => index = count))
            {
                GC.KeepAlive(item);
                // There should be no items
                Assert.Fail();
            }
            Assert.AreEqual(0, index);

            index = 0;
            foreach (var item in array.AfterLastCounted(count => index = count))
            {
                GC.KeepAlive(item);
                Assert.AreEqual(0, index);
            }
            Assert.AreEqual(4, index);

            index = 42;
            foreach (var item in (new int[] { }).AfterLastCounted(count => index = count))
            {
                GC.KeepAlive(item);
                // There should be no items
                Assert.Fail();
            }
            // Not changed
            Assert.AreEqual(42, index);

            // Does something after each item
            index = 0;
            foreach (var item in array.AfterEach(() => index++))
            {
                Assert.AreEqual(index, item);
            }

            // Does something after each item - taking the position that has just passed
            index = 0;
            foreach (var item in array.AfterEachCounted(position => index = position + 1))
            {
                Assert.AreEqual(index, item);
            }
        }

        [Test]
        public void Append()
        {
            var left = new[] { 0, 1, 2, 3 };
            var right = new[] { 0, 1, 2, 3, 4 };
            var all = new[] { 0, 1, 2, 3, 0, 1, 2, 3, 4 };
            Assert.IsTrue(left.Append(4).SequenceEqual(right));
            Assert.IsTrue(left.Append(right).SequenceEqual(all));
        }

        [Test]
        public void Before()
        {
            var check = false;
            var index = 0;
            var array = new List<int> { 0, 1, 2, 3 };

            // Does something before the iteration starts
            foreach (var item in array.Before(() => check = true))
            {
                Assert.AreEqual(index, item);
                index++;
                Assert.IsTrue(check);
            }
            Assert.IsTrue(check);

            check = false;
            foreach (var item in (new int[] { }).Before(() => check = true))
            {
                GC.KeepAlive(item);
                // There should be no items
                Assert.Fail();
            }
            Assert.IsTrue(check);

            index = 0;
            foreach (var item in array.BeforeAny(() => check = true))
            {
                Assert.AreEqual(index, item);
                index++;
                Assert.IsTrue(check);
            }
            Assert.IsTrue(check);

            check = false;
            foreach (var item in (new int[] { }).BeforeAny(() => check = true))
            {
                GC.KeepAlive(item);
                // There should be no items
                Assert.Fail();
            }
            Assert.IsFalse(check);

            // Does something after each item
            index = 0;
            foreach (var item in array.BeforeEach(() => index++))
            {
                // Note: the index has been already incremented
                Assert.AreEqual(index, item + 1);
            }

            // Does something after each item - taking the position that is about to pass
            foreach (var item in array.BeforeEachCounted(position => index = position))
            {
                Assert.AreEqual(index, item);
            }
        }

        [Test]
        public void CanCopyTo()
        {
            // You cannot copy to null
            Assert.Throws<ArgumentNullException>(() => Extensions.CanCopyTo(0, null));
            // You can copy zero elements to a zero length array
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(0, new int[] { }));
            // You cannot copy one elements to a no length array
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(1, new int[] { }));
            // You can copy zero elements to a zero length array
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(0, new[] { 0 }));
            // You can copy one element to an unary array
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(1, new[] { 0 }));

            // You cannot copy to null
            Assert.Throws<ArgumentNullException>(() => Extensions.CanCopyTo(0, null, 0));
            // You can copy zero elements to a zero length array in offset 0
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(0, new int[] { }, 0));
            // You cannot copy one elements to a no length array in offset 0
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(1, new int[] { }, 0));
            // You can copy zero elements to a zero length array in offset 0
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(0, new[] { 0 }, 0));
            // You can copy one element to an unary array in offset 0
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(1, new[] { 0 }, 0));
            // ---
            // You cannot copy zero elements to a zero length array in offset 1
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(0, new int[] { }, 1));
            // You cannot copy one elements to a no length array in offset 1
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(1, new int[] { }, 1));
            // You can copy zero elements to a zero length array in offset 1
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(0, new[] { 0 }, 1));
            // You cannot copy one element to an unary array in offset 1
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(1, new[] { 0 }, 1));
            // You cannot copy in negative offset
            Assert.Throws<ArgumentOutOfRangeException>(() => Extensions.CanCopyTo(0, new[] { 0 }, -1));

            // You cannot copy to null
            Assert.Throws<ArgumentNullException>(() => Extensions.CanCopyTo((int[])null, 0, 0));
            // You can copy zero elements to a zero length array in offset 0
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(new int[] { }, 0, 0));
            // You cannot copy one elements to a no length array in offset 0
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(new int[] { }, 0, 1));
            // You can copy zero elements to a zero length array in offset 0
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(new[] { 0 }, 0, 0));
            // You can copy one element to an unary array in offset 0
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(new[] { 0 }, 0, 1));
            // ---
            // You cannot copy zero elements to a zero length array in offset 1
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(new int[] { }, 1, 0));
            // You cannot copy one elements to a no length array in offset 1
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(new int[] { }, 1, 1));
            // You can copy zero elements to a zero length array in offset 1
            Assert.DoesNotThrow(() => Extensions.CanCopyTo(new[] { 0 }, 1, 0));
            // You cannot copy one element to an unary array in offset 1
            Assert.Throws<ArgumentException>(() => Extensions.CanCopyTo(new[] { 0 }, 1, 1));
            // You cannot copy in negative offset
            Assert.Throws<ArgumentOutOfRangeException>(() => Extensions.CanCopyTo(new[] { 0 }, -1, 0));
            // You cannot copy in negative cuantities
            Assert.Throws<ArgumentOutOfRangeException>(() => Extensions.CanCopyTo(new[] { 0 }, 0, -1));
        }

        [Test]
        public void Cycle()
        {
            var array = new[] { 0, 1, 2, 3 };
            var index = 0;
            foreach (var item in array.Cycle())
            {
                Assert.AreEqual(item, index % 4);
                index++;
                // Cycle is an infinite loop - we need a way out
                if (index > 20)
                {
                    break;
                }
            }
        }

        [Test]
        public void HasAtLeast()
        {
            var emptyCollection = new int []{};
            Assert.IsTrue(emptyCollection.HasAtLeast(0));
            Assert.IsFalse(emptyCollection.HasAtLeast(1));
            var unaryCollection = new[] { 0 };
            Assert.IsTrue(unaryCollection.HasAtLeast(0));
            Assert.IsTrue(unaryCollection.HasAtLeast(1));
            Assert.IsFalse(unaryCollection.HasAtLeast(2));
            var arrayCollection = new [] {0, 1, 2, 3};
            Assert.IsTrue(arrayCollection.HasAtLeast(0));
            Assert.IsTrue(arrayCollection.HasAtLeast(1));
            Assert.IsTrue(arrayCollection.HasAtLeast(2));
            Assert.IsTrue(arrayCollection.HasAtLeast(3));
            Assert.IsTrue(arrayCollection.HasAtLeast(4));
            Assert.IsFalse(arrayCollection.HasAtLeast(5));

            Assert.IsTrue(Enumerable.Empty<int>().HasAtLeast(0));
            Assert.IsFalse(Enumerable.Empty<int>().HasAtLeast(1));
            Assert.IsTrue(Enumerable.Range(0, 1).HasAtLeast(0));
            Assert.IsTrue(Enumerable.Range(0, 1).HasAtLeast(1));
            Assert.IsFalse(Enumerable.Range(0, 1).HasAtLeast(2));
            Assert.IsTrue(Enumerable.Range(0, 4).HasAtLeast(0));
            Assert.IsTrue(Enumerable.Range(0, 4).HasAtLeast(1));
            Assert.IsTrue(Enumerable.Range(0, 4).HasAtLeast(2));
            Assert.IsTrue(Enumerable.Range(0, 4).HasAtLeast(3));
            Assert.IsTrue(Enumerable.Range(0, 4).HasAtLeast(4));
            Assert.IsFalse(Enumerable.Range(0, 4).HasAtLeast(5));
        }
        [Test]
        public void Prepend()
        {
            var left = new[] { 0, 1, 2, 3 };
            var right = new[] { 4, 0, 1, 2, 3 };
            var all = new[] { 4, 0, 1, 2, 3, 0, 1, 2, 3 };
            Assert.IsTrue(left.Prepend(4).SequenceEqual(right));
            Assert.IsTrue(left.Prepend(right).SequenceEqual(all));
        }
    }
}
