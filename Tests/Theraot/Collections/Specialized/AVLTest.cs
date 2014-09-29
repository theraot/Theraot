using NUnit.Framework;
using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Tests.Theraot.Collections.Specialized
{
    [TestFixture]
    public class AVLTest
    {
        [Test]
        public void AddAndRemove()
        {
            var avl = new AVLTree<int, int> { { 10, 10 }, { 20, 20 }, { 30, 30 }, { 40, 40 }, { 50, 50 } };
            Assert.IsTrue(avl.Remove(30));
            Assert.IsFalse(avl.Remove(70));
            Assert.AreEqual(avl.Count, 4);
            var expected = new[] { 10, 20, 40, 50 };
            var index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, expected[index]);
                index++;
            }
        }

        [Test]
        public void AddRemoveAndBalance()
        {
            var avl = new AVLTree<int, int> { { 9, 9 }, { 77, 77 }, { 50, 50 }, { 48, 48 }, { 24, 24 }, { 60, 60 }, { 72, 72 }, { 95, 95 }, { 66, 66 }, { 27, 27 } };
            Assert.IsFalse(avl.Remove(30));
            Assert.IsTrue(avl.Remove(77));
            Assert.IsTrue(avl.Remove(72));
            Assert.IsTrue(avl.Remove(27));
            var expected = new[] { 9, 24, 48, 50, 60, 66, 95 };
            Assert.AreEqual(avl.Count, expected.Length);
            var index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, expected[index]);
                index++;
            }
        }

        [Test]
        public void AddRemoveAndBalanceExtended()
        {
            var avl = new AVLTree<int, int> { { 1, 1 }, { 3, 3 }, { 5, 5 }, { 7, 7 }, { 9, 9 } };
            Assert.IsTrue(avl.Remove(5));
            Assert.IsTrue(avl.Remove(7));
            Assert.IsTrue(avl.Remove(3));
            Assert.IsFalse(avl.Remove(7));
            Assert.IsFalse(avl.Remove(11));
            Assert.IsFalse(avl.Remove(7));
            Assert.IsFalse(avl.Remove(5));
            Assert.IsFalse(avl.Remove(2));
            var expected = new[] { 1, 9 };
            Assert.AreEqual(avl.Count, expected.Length);
            var index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, expected[index]);
                index++;
            }
        }

        [Test]
        public void AddRemoveAndBalanceDuplicates()
        {
            var values = new[]
            {
                9,
                77,
                50,
                48,
                24,
                60,
                72,
                95,
                66,
                27,
                28,
                33,
                50,
                65,
                37,
                75,
                58,
                36,
                74,
                60
            };
            var avl = new AVLTree<int, int>();
            var expected = new List<int>();
            var duplicates = new List<int>();
            foreach (var item in values)
            {
                bool duplicate = expected.Contains(item);
                if (duplicate)
                {
                    duplicates.Add(item);
                }
                else
                {
                    expected.Add(item);
                }
                avl.Add(item, item);
            }
            foreach (var duplicate in duplicates)
            {
                Assert.IsTrue(avl.Remove(duplicate));
            }
            Assert.AreEqual(avl.Count, expected.Count);
            expected.Sort();
            var index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, expected[index]);
                index++;
            }
            foreach (var duplicate in duplicates)
            {
                Assert.IsTrue(avl.Remove(duplicate));
                expected.Remove(duplicate);
            }
            Assert.AreEqual(avl.Count, expected.Count);
            expected.Sort();
            index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, expected[index]);
                index++;
            }
        }

        [Test]
        public void TestAdditionAndIteration()
        {
            var data = new[] { 6, 4, 9, 1, 5, 8, 7 };
            AddAndIterate(data);
        }

        [Test]
        public void TestAdditionAndIterationExtended()
        {
            AddAndIterate(new[] { 1, 2, 3, 4, 5 });
            AddAndIterate(new[] { 2, 1, 4, 3, 5 });
            AddAndIterate(new[] { 50, 20, 60, 40 });
            AddAndIterate(new[] { 50, 20, 60, 80 });
            AddAndIterate(new[] { 20, 10, 30, 80, 40, 60, 50, 70 });
            AddAndIterate(new[] { 30, 20, 80, 10, 40, 80, 50, 70 });
            AddAndIterate(new[] { 40, 20, 50, 30, 45, 60 });
            AddAndIterate(new[] { 40, 20, 50, 30, 45, 60, 55 });
            AddAndIterate(new[] { 40, 20, 50, 15, 30, 45, 60, 25 });
            AddAndIterate(new[] { 40, 20, 50, 15, 30, 45, 60, 25, 55 });
            AddAndIterate(new[] { 50, 20, 60, 10, 40, 70, 5, 30, 45 });
            AddAndIterate(new[] { 40, 20, 50, 10, 30, 45, 60, 5, 25, 70 });
            AddAndIterate(new[] { 3, 4, 7, 9, 10, 11, 12, 13 });
        }

        [Test]
        public void TestBalance()
        {
            AddAndIterate(new[] { 42, 36, 72, 54, 41, 29, 21, 83, 59, 46, 81, 96, 51, 1, 28, 42, 7, 56, 71, 40 });
        }

        [Test]
        public void TestDuplicated()
        {
            AddAndIterateNonDuplicate(new[] { 3, 5, 5, 9, 12, 14, 16, 17, 21, 25 });
            AddAndIterateNonDuplicate(new[] { 1, 1, 2, 2, 3, 3, 5, 6, 9, 10, 10, 10 });
        }

        private static void AddAndIterate(int[] data)
        {
            var avl = new AVLTree<int, int>();
            foreach (var item in data)
            {
                avl.Add(item, item);
            }
            Assert.AreEqual(avl.Count, data.Length);
            Array.Sort(data);
            var index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, data[index]);
                index++;
            }
        }

        private static void AddAndIterateNonDuplicate(int[] data)
        {
            var avl = new AVLTree<int, int>();
            var copy = new List<int>();
            foreach (var item in data)
            {
                bool duplicate = copy.Contains(item);
                Assert.AreNotEqual(avl.AddNonDuplicate(item, item), duplicate);
                if (!duplicate)
                {
                    copy.Add(item);
                }
            }
            Assert.AreEqual(avl.Count, copy.Count);
            Array.Sort(data);
            var index = 0;
            foreach (var item in avl)
            {
                Assert.AreEqual(item.Key, copy[index]);
                index++;
            }
        }
    }
}