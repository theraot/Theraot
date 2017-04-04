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
								Assert.AreEqual(expected[index], item.Key);
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
						Assert.AreEqual(expected.Length, avl.Count);
						var index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
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
								var duplicate = expected.Contains(item);
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
						Assert.AreEqual(expected.Count, avl.Count);
						expected.Sort();
						var index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						foreach (var duplicate in duplicates)
						{
								Assert.IsTrue(avl.Remove(duplicate));
								expected.Remove(duplicate);
						}
						Assert.AreEqual(expected.Count, avl.Count);
						expected.Sort();
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
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
						Assert.AreEqual(expected.Length, avl.Count);
						var index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
				}

				[Test]
				public void LargeTreeTest()
				{
						var avl = new AVLTree<int, int> { { 4, 0 }, { 8, 0 }, { 12, 0 }, { 16, 0 }, { 18, 0 }, { 19, 0 }, { 21, 0 } };
						Assert.AreEqual(avl.Count, 7);
						var expected = new[] { 4, 8, 12, 16, 18, 19, 21 };
						var index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(2, 0);
						avl.Add(6, 0);
						avl.Add(10, 0);
						avl.Add(14, 0);
						avl.Add(17, 0);
						avl.Add(20, 0);
						avl.Add(22, 0);
						expected = new[] { 2, 4, 6, 8, 10, 12, 14, 16, 17, 18, 19, 20, 21, 22 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(1, 0);
						avl.Add(3, 0);
						avl.Add(5, 0);
						avl.Add(7, 0);
						avl.Add(9, 0);
						avl.Add(11, 0);
						avl.Add(13, 0);
						avl.Add(15, 0);
						avl.Add(23, 0);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Remove(23);
						avl.Remove(20);
						avl.Remove(17);
						avl.Remove(22);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 21 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(9, 0);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 21 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(31, 0);
						avl.Add(27, 0);
						avl.Add(26, 0);
						avl.Add(24, 0);
						avl.Add(28, 0);
						avl.Add(30, 0);
						avl.Add(32, 0);
						avl.Add(29, 0);
						avl.Add(25, 0);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 21, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Remove(31);
						avl.Remove(30);
						avl.Remove(25);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 21, 24, 26, 27, 28, 29, 32 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(20, 0);
						avl.Add(10, 0);
						avl.Add(30, 0);
						avl.Add(35, 0);
						avl.Add(31, 0);
						avl.Add(33, 0);
						avl.Add(32, 0);
						avl.Add(34, 0);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 24, 26, 27, 28, 29, 30, 31, 32, 32, 33, 34, 35 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(39, 0);
						avl.Add(36, 0);
						avl.Add(46, 0);
						avl.Add(42, 0);
						avl.Add(38, 0);
						avl.Add(29, 0);
						avl.Add(21, 0);
						avl.Add(48, 0);
						avl.Add(44, 0);
						avl.Add(40, 0);
						avl.Add(47, 0);
						avl.Add(49, 0);
						avl.Add(41, 0);
						avl.Add(1, 0);
						avl.Add(28, 0);
						avl.Add(7, 0);
						avl.Add(43, 0);
						avl.Add(45, 0);
						avl.Add(37, 0);
						expected = new[] { 1, 1, 2, 3, 4, 5, 6, 7, 7, 8, 9, 9, 10, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 21, 24, 26, 27, 28, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Remove(1);
						avl.Remove(7);
						avl.Remove(9);
						avl.Remove(10);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 21, 24, 26, 27, 28, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(17, 0);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 21, 24, 26, 27, 28, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Remove(21);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24, 26, 27, 28, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(22, 0);
						avl.Add(23, 0);
						avl.Add(25, 0);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Remove(28);
						avl.Remove(29);
						avl.Remove(32);
						expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
						avl.Add(50, 0);
						avl.Add(51, 0);
						avl.Add(52, 0);
						avl.Add(53, 0);
						avl.Add(54, 0);
						avl.Add(55, 0);
						avl.Add(56, 0);
						avl.Add(57, 0);
						avl.Add(58, 0);
						avl.Add(59, 0);
						avl.Add(60, 0);
						avl.Add(61, 0);
						avl.Add(62, 0);
						avl.Add(63, 0);
						avl.Add(64, 0);
						avl.Add(65, 0);
						avl.Add(66, 0);
						avl.Add(67, 0);
						avl.Add(68, 0);
						avl.Add(69, 0);
						avl.Add(70, 0);
						avl.Add(71, 0);
						avl.Add(72, 0);
						avl.Add(73, 0);
						avl.Add(74, 0);
						avl.Add(75, 0);
						avl.Add(76, 0);
						avl.Add(77, 0);
						avl.Add(78, 0);
						avl.Add(79, 0);
						avl.Add(80, 0);
						avl.Add(81, 0);
						avl.Add(82, 0);
						avl.Add(83, 0);
						avl.Add(84, 0);
						avl.Add(85, 0);
						avl.Add(86, 0);
						avl.Add(87, 0);
						avl.Add(88, 0);
						avl.Add(89, 0);
						avl.Add(90, 0);
						avl.Add(91, 0);
						avl.Add(92, 0);
						avl.Add(93, 0);
						avl.Add(94, 0);
						avl.Add(95, 0);
						avl.Add(96, 0);
						avl.Add(97, 0);
						avl.Add(98, 0);
						avl.Add(99, 0);
						avl.Add(0, 0);
						expected = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
						index = 0;
						foreach (var item in avl)
						{
								Assert.AreEqual(expected[index], item.Key);
								index++;
						}
				}

				[Test]
				public void SearchLeft()
				{
						var avl = new AVLTree<int, int> { { 10, 10 }, { 20, 20 }, { 30, 30 }, { 40, 40 }, { 50, 50 } };
						Assert.AreEqual(50, avl.GetNearestLeft(100).Key);
						Assert.AreEqual(30, avl.GetNearestLeft(30).Key);
						Assert.AreEqual(20, avl.GetNearestLeft(25).Key);
						Assert.AreEqual(20, avl.GetNearestLeft(20).Key);
						Assert.IsNull(avl.GetNearestLeft(5));

						avl = new AVLTree<int, int> { { 10, 10 }, { 10, 20 }, { 30, 30 }, { 30, 40 }, { 50, 50 }, { 50, 60 } };
						Assert.AreEqual(50, avl.GetNearestLeft(100).Key);
						Assert.AreEqual(60, avl.GetNearestLeft(100).Value);
						Assert.AreEqual(30, avl.GetNearestLeft(30).Key);
						Assert.AreEqual(30, avl.GetNearestLeft(30).Value);
						Assert.AreEqual(10, avl.GetNearestLeft(25).Key);
						Assert.AreEqual(20, avl.GetNearestLeft(25).Value);
						Assert.AreEqual(10, avl.GetNearestLeft(20).Key);
						Assert.AreEqual(20, avl.GetNearestLeft(20).Value);
						Assert.IsNull(avl.GetNearestLeft(5));

						Assert.AreEqual(avl.GetNearestLeft(100).Value, avl.RemoveNearestLeft(100).Value);
						Assert.AreEqual(avl.GetNearestLeft(30).Value, avl.RemoveNearestLeft(30).Value);
						Assert.AreEqual(avl.GetNearestLeft(20).Value, avl.RemoveNearestLeft(25).Value);
						Assert.IsNull(avl.RemoveNearestLeft(5));
				}

				[Test]
				public void SearchRight()
				{
						var avl = new AVLTree<int, int> { { 10, 10 }, { 20, 20 }, { 30, 30 }, { 40, 40 }, { 50, 50 } };
						Assert.IsNull(avl.GetNearestRight(100));
						Assert.AreEqual(30, avl.GetNearestRight(30).Key);
						Assert.AreEqual(30, avl.GetNearestRight(25).Key);
						Assert.AreEqual(20, avl.GetNearestRight(20).Key);
						Assert.AreEqual(10, avl.GetNearestRight(5).Key);

						avl = new AVLTree<int, int> { { 10, 10 }, { 10, 20 }, { 30, 30 }, { 30, 40 }, { 50, 50 }, { 50, 60 } };
						Assert.IsNull(avl.GetNearestRight(100));
						Assert.AreEqual(30, avl.GetNearestRight(30).Key);
						Assert.AreEqual(30, avl.GetNearestRight(30).Value);
						Assert.AreEqual(30, avl.GetNearestRight(25).Key);
						Assert.AreEqual(30, avl.GetNearestRight(25).Value);
						Assert.AreEqual(30, avl.GetNearestRight(20).Key);
						Assert.AreEqual(30, avl.GetNearestRight(20).Value);
						Assert.AreEqual(10, avl.GetNearestRight(5).Key);
						Assert.AreEqual(10, avl.GetNearestRight(5).Value);

						Assert.IsNull(avl.RemoveNearestRight(100));
						Assert.AreEqual(avl.GetNearestRight(30).Value, avl.RemoveNearestRight(30).Value);
						Assert.AreEqual(avl.GetNearestRight(5).Value, avl.RemoveNearestRight(5).Value);
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
								var duplicate = copy.Contains(item);
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
