#pragma warning disable RCS1132	// Remove redundant overriding member

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable AccessToDisposedClosure
// ReSharper disable CollectionNeverQueried.Local

#pragma warning disable CS0649 // Field is never assigned to.

namespace Tests.SystemTests.CollectionsTests.ConcurrentTests
{
    [TestFixture]
    public static class ConcurrentDictionaryDotNetTests
    {
        [Test]
        public static void TestBasicScenarios()
        {
            var cd = new ConcurrentDictionary<int, int>();

            var tks = new Task[2];
            tks[0] = TaskEx.Run(() =>
            {
                var ret = cd.TryAdd(1, 11);
                if (!ret)
                {
                    ret = cd.TryUpdate(1, 11, 111);
                    Assert.True(ret);
                }

                ret = cd.TryAdd(2, 22);
                if (!ret)
                {
                    ret = cd.TryUpdate(2, 22, 222);
                    Assert.True(ret);
                }
            });

            tks[1] = TaskEx.Run(() =>
            {
                var ret = cd.TryAdd(2, 222);
                if (!ret)
                {
                    ret = cd.TryUpdate(2, 222, 22);
                    Assert.True(ret);
                }

                ret = cd.TryAdd(1, 111);
                if (!ret)
                {
                    ret = cd.TryUpdate(1, 111, 11);
                    Assert.True(ret);
                }
            });

            Task.WaitAll(tks);
        }

        [Test]
        public static void TestAddNullValue_ConcurrentDictionaryOfString_null()
        {
            // using ConcurrentDictionary<TKey, TValue> class
            var dict1 = new ConcurrentDictionary<string, string>();
            dict1["key"] = null;
        }

        [Test]
        public static void TestAddNullValue_IDictionaryOfString_null()
        {
            // using IDictionary<TKey, TValue> interface
            IDictionary<string, string> dict2 = new ConcurrentDictionary<string, string>();
            dict2["key"] = null;
            dict2.Add("key2", null);
        }

        [Test]
        public static void TestAddNullValue_IDictionary_ReferenceType_null()
        {
            // using IDictionary interface
            IDictionary dict3 = new ConcurrentDictionary<string, string>();
#if GREATERTHAN_NET35 || (!NETCOREAPP10  && LESSTHAN_NETCOREAPP30)
            Assert.Throws<ArgumentException>(() => dict3["key"] = null);
#else
            dict3["key"] = null;
#endif
            dict3.Add("key2", null);
        }

        [Test]
        public static void TestAddNullValue_IDictionary_ValueType_null_indexer()
        {
            // using IDictionary interface and value type values
            static void action()
            {
                IDictionary dict4 = new ConcurrentDictionary<string, int>();
                dict4["key"] = null;
            }

            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public static void TestAddNullValue_IDictionary_ValueType_null_add()
        {
            static void action()
            {
                IDictionary dict5 = new ConcurrentDictionary<string, int>();
                dict5.Add("key", null);
            }

#if GREATERTHAN_NET35 || (!NETCOREAPP10 && LESSTHAN_NETCOREAPP30)
            Assert.Throws<NullReferenceException>(action);
#else
            Assert.Throws<ArgumentException>(action);
#endif
        }

        [Test]
        public static void TestAddValueOfDifferentType()
        {
            TestDelegate action = () =>
            {
                IDictionary dict = new ConcurrentDictionary<string, string>();
                dict["key"] = 1;
            };
            Assert.Throws<ArgumentException>(action);

            action = () =>
            {
                IDictionary dict = new ConcurrentDictionary<string, string>();
                dict.Add("key", 1);
            };
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        [TestCase(1, 1, 1, 10000)]
        [TestCase(5, 1, 1, 10000)]
        [TestCase(1, 1, 2, 5000)]
        [TestCase(1, 1, 5, 2000)]
        [TestCase(4, 0, 4, 2000)]
        [TestCase(16, 31, 4, 2000)]
        [TestCase(64, 5, 5, 5000)]
        [TestCase(5, 5, 5, 2500)]
        public static void TestAdd(int cLevel, int initSize, int threads, int addsPerThread)
        {
            var dictConcurrent = new ConcurrentDictionary<int, int>(cLevel, 1);
            IDictionary<int, int> dict = dictConcurrent;

            var count = threads;
            using (var mre = new ManualResetEvent(false))
            {
                for (var i = 0; i < threads; i++)
                {
                    var ii = i;
                    TaskEx.Run(
                        () =>
                        {
                            for (var j = 0; j < addsPerThread; j++)
                            {
                                dict.Add(j + (ii * addsPerThread), -(j + (ii * addsPerThread)));
                            }
                            if (Interlocked.Decrement(ref count) == 0)
                            {
                                mre.Set();
                            }
                        });
                }
                mre.WaitOne();
            }

            foreach (var pair in dict)
            {
                Assert.AreEqual(pair.Key, -pair.Value);
            }

            var gotKeys = new List<int>();
            foreach (var pair in dict)
            {
                gotKeys.Add(pair.Key);
            }

            gotKeys.Sort();

            var expectKeys = new List<int>();
            var itemCount = threads * addsPerThread;
            for (var i = 0; i < itemCount; i++)
            {
                expectKeys.Add(i);
            }

            Assert.AreEqual(expectKeys.Count, gotKeys.Count);

            for (var i = 0; i < expectKeys.Count; i++)
            {
                Assert.True(expectKeys[i].Equals(gotKeys[i]),
                    string.Format("The set of keys in the dictionary is are not the same as the expected" + Environment.NewLine +
                            "TestAdd1(cLevel={0}, initSize={1}, threads={2}, addsPerThread={3})", cLevel, initSize, threads, addsPerThread)
                   );
            }

            // Finally, let's verify that the count is reported correctly.
            var expectedCount = threads * addsPerThread;
            Assert.AreEqual(expectedCount, dict.Count);
            Assert.AreEqual(expectedCount, dictConcurrent.ToArray().Length);
        }

        [Test]
        [TestCase(1, 1, 10000)]
        [TestCase(5, 1, 10000)]
        [TestCase(1, 2, 5000)]
        [TestCase(1, 5, 2001)]
        [TestCase(4, 4, 2001)]
        [TestCase(15, 5, 2001)]
        [TestCase(64, 5, 5000)]
        [TestCase(5, 5, 25000)]
        public static void TestUpdate(int cLevel, int threads, int updatesPerThread)
        {
            IDictionary<int, int> dict = new ConcurrentDictionary<int, int>(cLevel, 1);

            for (var i = 1; i <= updatesPerThread; i++)
            {
                dict[i] = i;
            }

            var running = threads;
            using (var mre = new ManualResetEvent(false))
            {
                for (var i = 0; i < threads; i++)
                {
                    var ii = i;
                    TaskEx.Run(
                        () =>
                        {
                            for (var j = 1; j <= updatesPerThread; j++)
                            {
                                dict[j] = (ii + 2) * j;
                            }
                            if (Interlocked.Decrement(ref running) == 0)
                            {
                                mre.Set();
                            }
                        });
                }
                mre.WaitOne();
            }

            foreach (var pair in dict)
            {
                var div = pair.Value / pair.Key;
                var rem = pair.Value % pair.Key;

                Assert.AreEqual(0, rem);
                Assert.True(div > 1 && div <= threads + 1,
                    string.Format("* Invalid value={3}! TestUpdate1(cLevel={0}, threads={1}, updatesPerThread={2})", cLevel, threads, updatesPerThread, div));
            }

            var gotKeys = new List<int>();
            foreach (var pair in dict)
            {
                gotKeys.Add(pair.Key);
            }

            gotKeys.Sort();

            var expectKeys = new List<int>();
            for (var i = 1; i <= updatesPerThread; i++)
            {
                expectKeys.Add(i);
            }

            Assert.AreEqual(expectKeys.Count, gotKeys.Count);

            for (var i = 0; i < expectKeys.Count; i++)
            {
                Assert.True(expectKeys[i].Equals(gotKeys[i]),
                   string.Format("The set of keys in the dictionary is are not the same as the expected." + Environment.NewLine +
                           "TestUpdate1(cLevel={0}, threads={1}, updatesPerThread={2})", cLevel, threads, updatesPerThread)
                  );
            }
        }

        [Test]
        [TestCase(1, 1, 10000)]
        [TestCase(5, 1, 10000)]
        [TestCase(1, 2, 5000)]
        [TestCase(1, 5, 2001)]
        [TestCase(4, 4, 2001)]
        [TestCase(15, 5, 2001)]
        [TestCase(64, 5, 5000)]
        [TestCase(5, 5, 25000)]
        public static void TestRead1(int cLevel, int threads, int readsPerThread)
        {
            IDictionary<int, int> dict = new ConcurrentDictionary<int, int>(cLevel, 1);

            for (var i = 0; i < readsPerThread; i += 2)
            {
                dict[i] = i;
            }

            var count = threads;
            using (var mre = new ManualResetEvent(false))
            {
                for (var i = 0; i < threads; i++)
                {
                    var ii = i;
                    TaskEx.Run(
                        () =>
                        {
                            for (var j = 0; j < readsPerThread; j++)
                            {
                                if (dict.TryGetValue(j, out var val))
                                {
                                    Assert.AreEqual(0, j % 2);
                                    Assert.AreEqual(j, val);
                                }
                                else
                                {
                                    Assert.AreEqual(1, j % 2);
                                }
                            }
                            if (Interlocked.Decrement(ref count) == 0)
                            {
                                mre.Set();
                            }
                        });
                }
                mre.WaitOne();
            }
        }

        [Test]
        [TestCase(1, 1, 10000)]
        [TestCase(5, 1, 1000)]
        [TestCase(1, 5, 2001)]
        [TestCase(4, 4, 2001)]
        [TestCase(15, 5, 2001)]
        [TestCase(64, 5, 5000)]
        public static void TestRemove1(int cLevel, int threads, int removesPerThread)
        {
            var dict = new ConcurrentDictionary<int, int>(cLevel, 1);
            var methodparameters = string.Format("* TestRemove1(cLevel={0}, threads={1}, removesPerThread={2})", cLevel, threads, removesPerThread);
            var N = 2 * threads * removesPerThread;

            for (var i = 0; i < N; i++)
            {
                dict[i] = -i;
            }

            // The dictionary contains keys [0..N), each key mapped to a value equal to the key.
            // Threads will cooperatively remove all even keys

            var running = threads;
            using (var mre = new ManualResetEvent(false))
            {
                for (var i = 0; i < threads; i++)
                {
                    var ii = i;
                    TaskEx.Run(
                        () =>
                        {
                            for (var j = 0; j < removesPerThread; j++)
                            {
                                var key = 2 * (ii + (j * threads));
                                Assert.True(dict.TryRemove(key, out var value), "Failed to remove an element! " + methodparameters);

                                Assert.AreEqual(-key, value);
                            }

                            if (Interlocked.Decrement(ref running) == 0)
                            {
                                mre.Set();
                            }
                        });
                }
                mre.WaitOne();
            }

            foreach (var pair in dict)
            {
                Assert.AreEqual(pair.Key, -pair.Value);
            }

            var gotKeys = new List<int>();
            foreach (var pair in dict)
            {
                gotKeys.Add(pair.Key);
            }

            gotKeys.Sort();

            var expectKeys = new List<int>();
            for (var i = 0; i < (threads * removesPerThread); i++)
            {
                expectKeys.Add((2 * i) + 1);
            }

            Assert.AreEqual(expectKeys.Count, gotKeys.Count);

            for (var i = 0; i < expectKeys.Count; i++)
            {
                Assert.True(expectKeys[i].Equals(gotKeys[i]), "  > Unexpected key value! " + methodparameters);
            }

            // Finally, let's verify that the count is reported correctly.
            Assert.AreEqual(expectKeys.Count, dict.Count);
            Assert.AreEqual(expectKeys.Count, dict.ToArray().Length);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(5000)]
        public static void TestRemove2(int removesPerThread)
        {
            var dict = new ConcurrentDictionary<int, int>();

            for (var i = 0; i < removesPerThread; i++)
            {
                dict[i] = -i;
            }

            // The dictionary contains keys [0..N), each key mapped to a value equal to the key.
            // Threads will cooperatively remove all even keys.
            const int SIZE = 2;
            var running = SIZE;

            var seen = new bool[SIZE][];
            for (var i = 0; i < SIZE; i++)
            {
                seen[i] = new bool[removesPerThread];
            }

            using (var mre = new ManualResetEvent(false))
            {
                for (var t = 0; t < SIZE; t++)
                {
                    var thread = t;
                    TaskEx.Run(
                        () =>
                        {
                            for (var key = 0; key < removesPerThread; key++)
                            {
                                if (dict.TryRemove(key, out var value))
                                {
                                    seen[thread][key] = true;

                                    Assert.AreEqual(-key, value);
                                }
                            }
                            if (Interlocked.Decrement(ref running) == 0)
                            {
                                mre.Set();
                            }
                        });
                }
                mre.WaitOne();
            }

            Assert.AreEqual(0, dict.Count);

            for (var i = 0; i < removesPerThread; i++)
            {
                Assert.False(seen[0][i] == seen[1][i],
                    string.Format("> FAILED. Two threads appear to have removed the same element. TestRemove2(removesPerThread={0})", removesPerThread)
                    );
            }
        }

        [Test]
        public static void TestRemove3()
        {
            var dict = new ConcurrentDictionary<int, int>();

            dict[99] = -99;

            ICollection<KeyValuePair<int, int>> col = dict;

            // Make sure we cannot "remove" a key/value pair which is not in the dictionary
            for (var i = 0; i < 200; i++)
            {
                if (i != 99)
                {
                    Assert.False(col.Remove(new KeyValuePair<int, int>(i, -99)), "Should not remove not existing a key/value pair - new KeyValuePair<int, int>(i, -99)");
                    Assert.False(col.Remove(new KeyValuePair<int, int>(99, -i)), "Should not remove not existing a key/value pair - new KeyValuePair<int, int>(99, -i)");
                }
            }

            // Can we remove a key/value pair successfully?
            Assert.True(col.Remove(new KeyValuePair<int, int>(99, -99)), "Failed to remove existing key/value pair");

            // Make sure the key/value pair is gone
            Assert.False(col.Remove(new KeyValuePair<int, int>(99, -99)), "Should not remove the key/value pair which has been removed");

            // And that the dictionary is empty. We will check the count in a few different ways:
            Assert.AreEqual(0, dict.Count);
            Assert.AreEqual(0, dict.ToArray().Length);
        }

        [Test]
        public static void TryRemove_KeyValuePair_ArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ((ICollection<KeyValuePair<string, string>>)new ConcurrentDictionary<string, string>()).Add(
                    new KeyValuePair<string, string>(null, null)));
            new ConcurrentDictionary<int, int>().TryRemove(new KeyValuePair<int, int>(0, 0)); // no error when using default value type
            new ConcurrentDictionary<int?, int>().TryRemove(new KeyValuePair<int?, int>(0, 0)); // or nullable
        }

        [Test]
        public static void TryRemove_KeyValuePair_RemovesSuccessfullyAsAppropriate()
        {
            var dict = new ConcurrentDictionary<string, int>();

            for (var i = 0; i < 2; i++)
            {
                Assert.False(dict.TryRemove(KeyValuePair.Create("key", 42)));
                Assert.AreEqual(0, dict.Count);
                Assert.True(dict.TryAdd("key", 42));
                Assert.AreEqual(1, dict.Count);
                Assert.True(dict.TryRemove(KeyValuePair.Create("key", 42)));
                Assert.AreEqual(0, dict.Count);
            }

            Assert.True(dict.TryAdd("key", 42));
            Assert.False(dict.TryRemove(KeyValuePair.Create("key", 43))); // value doesn't match
        }

        [Test]
        public static void TryRemove_KeyValuePair_MatchesKeyWithDefaultComparer()
        {
            var dict = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dict.TryAdd("key", "value");
            Assert.False(dict.TryRemove(KeyValuePair.Create("key", "VALUE")));
            Assert.True(dict.TryRemove(KeyValuePair.Create("KEY", "value")));
        }

        [Test]
        public static void TestGetOrAdd()
        {
            TestGetOrAddOrUpdate(1, 1, 1, 10000, true);
            TestGetOrAddOrUpdate(5, 1, 1, 10000, true);
            TestGetOrAddOrUpdate(1, 1, 2, 5000, true);
            TestGetOrAddOrUpdate(1, 1, 5, 2000, true);
            TestGetOrAddOrUpdate(4, 0, 4, 2000, true);
            TestGetOrAddOrUpdate(16, 31, 4, 2000, true);
            TestGetOrAddOrUpdate(64, 5, 5, 5000, true);
            TestGetOrAddOrUpdate(5, 5, 5, 25000, true);
        }

        [Test]
        public static void TestAddOrUpdate()
        {
            TestGetOrAddOrUpdate(1, 1, 1, 10000, false);
            TestGetOrAddOrUpdate(5, 1, 1, 10000, false);
            TestGetOrAddOrUpdate(1, 1, 2, 5000, false);
            TestGetOrAddOrUpdate(1, 1, 5, 2000, false);
            TestGetOrAddOrUpdate(4, 0, 4, 2000, false);
            TestGetOrAddOrUpdate(16, 31, 4, 2000, false);
            TestGetOrAddOrUpdate(64, 5, 5, 5000, false);
            TestGetOrAddOrUpdate(5, 5, 5, 25000, false);
        }

        private static void TestGetOrAddOrUpdate(int cLevel, int initSize, int threads, int addsPerThread, bool isAdd)
        {
            var dict = new ConcurrentDictionary<int, int>(cLevel, 1);

            var count = threads;
            using (var mre = new ManualResetEvent(false))
            {
                for (var i = 0; i < threads; i++)
                {
                    var ii = i;
                    TaskEx.Run(
                        () =>
                        {
                            for (var j = 0; j < addsPerThread; j++)
                            {
                                if (isAdd)
                                {
                                    //call one of the overloads of GetOrAdd
                                    switch (j % 3)
                                    {
                                        case 0:
                                            dict.GetOrAdd(j, -j);
                                            break;
                                        case 1:
                                            dict.GetOrAdd(j, x => -x);
                                            break;
                                        case 2:
                                            dict.GetOrAdd(j, (x, m) => x * m, -1);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (j % 3)
                                    {
                                        case 0:
                                            dict.AddOrUpdate(j, -j, (_, _) => -j);
                                            break;
                                        case 1:
                                            dict.AddOrUpdate(j, (k) => -k, (k, _) => -k);
                                            break;
                                        case 2:
                                            dict.AddOrUpdate(j, (k,m) => k*m, (k, _, m) => k * m, -1);
                                            break;
                                    }
                                }
                            }
                            if (Interlocked.Decrement(ref count) == 0)
                            {
                                mre.Set();
                            }
                        });
                }
                mre.WaitOne();
            }

            foreach (var pair in dict)
            {
                Assert.AreEqual(pair.Key, -pair.Value);
            }

            var gotKeys = new List<int>();
            foreach (var pair in dict)
            {
                gotKeys.Add(pair.Key);
            }

            gotKeys.Sort();

            var expectKeys = new List<int>();
            for (var i = 0; i < addsPerThread; i++)
            {
                expectKeys.Add(i);
            }

            Assert.AreEqual(expectKeys.Count, gotKeys.Count);

            for (var i = 0; i < expectKeys.Count; i++)
            {
                Assert.True(expectKeys[i].Equals(gotKeys[i]),
                    string.Format("* Test '{4}': Level={0}, initSize={1}, threads={2}, addsPerThread={3})" + Environment.NewLine +
                    "> FAILED.  The set of keys in the dictionary is are not the same as the expected.",
                    cLevel, initSize, threads, addsPerThread, isAdd ? "GetOrAdd" : "GetOrUpdate"));
            }

            // Finally, let's verify that the count is reported correctly.
            Assert.AreEqual(addsPerThread, dict.Count);
            Assert.AreEqual(addsPerThread, dict.ToArray().Length);
        }

        [Test]
        public static void TestBugFix669376()
        {
            var cd = new ConcurrentDictionary<string, int>(new OrdinalStringComparer());
            cd["test"] = 10;
            Assert.True(cd.ContainsKey("TEST"), "Customized comparer didn't work");
        }

        private class OrdinalStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                var xlower = x.ToLowerInvariant();
                var ylower = y.ToLowerInvariant();
                return string.CompareOrdinal(xlower, ylower) == 0;
            }

            public int GetHashCode(string obj)
            {
                return 0;
            }
        }

        [Test]
        public static void TestConstructor()
        {
            var dictionary = new ConcurrentDictionary<int, int>(new[] { new KeyValuePair<int, int>(1, 1) });
            Assert.False(dictionary.IsEmpty);
            Assert.AreEqual(1, dictionary.Keys.Count);
            Assert.AreEqual(1, dictionary.Values.Count);
        }

#if GREATERTHAN_NET35

        [Test]
        public static void TestNullComparer()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AssertDefaultComparerBehavior(
                    new ConcurrentDictionary<EqualityApiSpy, int>((IEqualityComparer<EqualityApiSpy>)null)));

            Assert.Throws<ArgumentNullException>(() =>
                AssertDefaultComparerBehavior(
                    new ConcurrentDictionary<EqualityApiSpy, int>(
                        new[] {new KeyValuePair<EqualityApiSpy, int>(new EqualityApiSpy(), 1)}, null)));

            Assert.Throws<ArgumentNullException>(() =>
                AssertDefaultComparerBehavior(new ConcurrentDictionary<EqualityApiSpy, int>(1,
                    new[] {new KeyValuePair<EqualityApiSpy, int>(new EqualityApiSpy(), 1)}, null)));

            Assert.Throws<ArgumentNullException>(() =>
                AssertDefaultComparerBehavior(new ConcurrentDictionary<EqualityApiSpy, int>(1, 1, null)));

            static void AssertDefaultComparerBehavior(ConcurrentDictionary<EqualityApiSpy, int> dictionary)
            {
                var spyKey = new EqualityApiSpy();

                Assert.True(dictionary.TryAdd(spyKey, 1));
                Assert.False(dictionary.TryAdd(spyKey, 1));

                Assert.False(spyKey.ObjectApiUsed);
                Assert.True(spyKey.IEquatableApiUsed);
            }
        }

#else

        [Test]
        public static void TestNullComparer()
        {
            AssertDefaultComparerBehavior(new ConcurrentDictionary<EqualityApiSpy, int>((IEqualityComparer<EqualityApiSpy>)null));

            AssertDefaultComparerBehavior(new ConcurrentDictionary<EqualityApiSpy, int>(new[] { new KeyValuePair<EqualityApiSpy, int>(new EqualityApiSpy(), 1) }, null));

            AssertDefaultComparerBehavior(new ConcurrentDictionary<EqualityApiSpy, int>(1, new[] { new KeyValuePair<EqualityApiSpy, int>(new EqualityApiSpy(), 1) }, null));

            AssertDefaultComparerBehavior(new ConcurrentDictionary<EqualityApiSpy, int>(1, 1, null));

            static void AssertDefaultComparerBehavior(ConcurrentDictionary<EqualityApiSpy, int> dictionary)
            {
                var spyKey = new EqualityApiSpy();

                Assert.True(dictionary.TryAdd(spyKey, 1));
                Assert.False(dictionary.TryAdd(spyKey, 1));

                Assert.False(spyKey.ObjectApiUsed);
                Assert.True(spyKey.IEquatableApiUsed);
            }
        }

#endif

        private sealed class EqualityApiSpy : IEquatable<EqualityApiSpy>
        {
            public bool ObjectApiUsed { get; private set; }
            public bool IEquatableApiUsed { get; private set; }

            public override bool Equals(object obj)
            {
                ObjectApiUsed = true;
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode() => base.GetHashCode();

            public bool Equals(EqualityApiSpy other)
            {
                IEquatableApiUsed = true;
                return ReferenceEquals(this, other);
            }
        }

        [Test]
        public static void TestConstructor_Negative()
        {
            Assert.Throws<ArgumentNullException>(
               () => _ = new ConcurrentDictionary<int, int>((ICollection<KeyValuePair<int, int>>)null));
            // "TestConstructor:  FAILED.  Constructor didn't throw ANE when null collection is passed");

            Assert.Throws<ArgumentNullException>(
               () => _ = new ConcurrentDictionary<int, int>((ICollection<KeyValuePair<int, int>>)null, EqualityComparer<int>.Default));
            // "TestConstructor:  FAILED.  Constructor didn't throw ANE when null collection and non null IEqualityComparer passed");

            Assert.Throws<ArgumentNullException>(
               () => _ = new ConcurrentDictionary<string, int>(new[] { new KeyValuePair<string, int>(null, 1) }));
            // "TestConstructor:  FAILED.  Constructor didn't throw ANE when collection has null key passed");

            // Duplicate keys.
            Assert.Throws<ArgumentException>(
                () => _ = new ConcurrentDictionary<int, int>(
                    new[]
                    {
                        new KeyValuePair<int, int>(1, 1),
                        new KeyValuePair<int, int>(1, 2)
                    }));

            Assert.Throws<ArgumentNullException>(
               () => _ = new ConcurrentDictionary<int, int>(1, null, EqualityComparer<int>.Default));
            // "TestConstructor:  FAILED.  Constructor didn't throw ANE when null collection is passed");

            Assert.Throws<ArgumentOutOfRangeException>(
               () => _ = new ConcurrentDictionary<int, int>(0, 10));
            // "TestConstructor:  FAILED.  Constructor didn't throw AORE when <1 concurrencyLevel passed");

            Assert.Throws<ArgumentOutOfRangeException>(
               () => _ = new ConcurrentDictionary<int, int>(-1, 0));
            // "TestConstructor:  FAILED.  Constructor didn't throw AORE when < 0 capacity passed");
        }

        [Test]
        public static void TestExceptions()
        {
            var dictionary = new ConcurrentDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(
               () => dictionary.TryAdd(null, 0));
            //  "TestExceptions:  FAILED.  TryAdd didn't throw ANE when null key is passed");

            Assert.Throws<ArgumentNullException>(
               () => dictionary.ContainsKey(null));
            // "TestExceptions:  FAILED.  Contains didn't throw ANE when null key is passed");

            int item;
            Assert.Throws<ArgumentNullException>(
               () => dictionary.TryRemove(null, out item));
            //  "TestExceptions:  FAILED.  TryRemove didn't throw ANE when null key is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.TryGetValue(null, out item));
            // "TestExceptions:  FAILED.  TryGetValue didn't throw ANE when null key is passed");

            Assert.Throws<ArgumentNullException>(
               () => { var x = dictionary[null]; });
            // "TestExceptions:  FAILED.  this[] didn't throw ANE when null key is passed");
            Assert.Throws<KeyNotFoundException>(
               () => { var x = dictionary["1"]; });
            // "TestExceptions:  FAILED.  this[] TryGetValue didn't throw KeyNotFoundException!");

            Assert.Throws<ArgumentNullException>(
               () => dictionary[null] = 1);
            // "TestExceptions:  FAILED.  this[] didn't throw ANE when null key is passed");

            Assert.Throws<ArgumentNullException>(
               () => dictionary.GetOrAdd(null, (_,_) => 0, 0));
            // "TestExceptions:  FAILED.  GetOrAdd didn't throw ANE when null key is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.GetOrAdd("1", null, 0));
            // "TestExceptions:  FAILED.  GetOrAdd didn't throw ANE when null valueFactory is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.GetOrAdd(null, (_) => 0));
            // "TestExceptions:  FAILED.  GetOrAdd didn't throw ANE when null key is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.GetOrAdd("1", null));
            // "TestExceptions:  FAILED.  GetOrAdd didn't throw ANE when null valueFactory is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.GetOrAdd(null, 0));
            // "TestExceptions:  FAILED.  GetOrAdd didn't throw ANE when null key is passed");

            Assert.Throws<ArgumentNullException>(
               () => dictionary.AddOrUpdate(null, (_, _) => 0, (_, _, _) => 0, 42));
            // "TestExceptions:  FAILED.  AddOrUpdate didn't throw ANE when null key is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.AddOrUpdate("1", (_, _) => 0, null, 42));
            // "TestExceptions:  FAILED.  AddOrUpdate didn't throw ANE when null updateFactory is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.AddOrUpdate("1", null, (_, _, _) => 0, 42));
            // "TestExceptions:  FAILED.  AddOrUpdate didn't throw ANE when null addFactory is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.AddOrUpdate(null, (_) => 0, (_, _) => 0));
            // "TestExceptions:  FAILED.  AddOrUpdate didn't throw ANE when null key is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.AddOrUpdate("1", null, (_, _) => 0));
            // "TestExceptions:  FAILED.  AddOrUpdate didn't throw ANE when null updateFactory is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary.AddOrUpdate(null, (_) => 0, null));
            // "TestExceptions:  FAILED.  AddOrUpdate didn't throw ANE when null addFactory is passed");

            // Duplicate key.
            dictionary.TryAdd("1", 1);
            Assert.Throws<ArgumentException>(() => ((IDictionary<string, int>)dictionary).Add("1", 2));
        }

        [Test]
        public static void TestIDictionary()
        {
            IDictionary dictionary = new ConcurrentDictionary<string, int>();
            Assert.False(dictionary.IsReadOnly);

            // Empty dictionary should not enumerate
            Assert.IsEmpty(dictionary);

            const int SIZE = 10;
            for (var i = 0; i < SIZE; i++)
            {
                dictionary.Add(i.ToString(), i);
            }

            Assert.AreEqual(SIZE, dictionary.Count);

            //test contains
            Assert.False(dictionary.Contains(1), "TestIDictionary:  FAILED.  Contain returned true for incorrect key type");
            Assert.False(dictionary.Contains("100"), "TestIDictionary:  FAILED.  Contain returned true for incorrect key");
            Assert.True(dictionary.Contains("1"), "TestIDictionary:  FAILED.  Contain returned false for correct key");

            //test GetEnumerator
            var count = 0;
            foreach (var obj in dictionary)
            {
                var entry = (DictionaryEntry)obj;
                var key = (string)entry.Key;
                var value = (int)entry.Value;
                var expectedValue = int.Parse(key);
                Assert.True(value == expectedValue,
                    string.Format("TestIDictionary:  FAILED.  Unexpected value returned from GetEnumerator, expected {0}, actual {1}", value, expectedValue));
                count++;
            }

            Assert.AreEqual(SIZE, count);
            Assert.AreEqual(SIZE, dictionary.Keys.Count);
            Assert.AreEqual(SIZE, dictionary.Values.Count);

            //Test Remove
            dictionary.Remove("9");
            Assert.AreEqual(SIZE - 1, dictionary.Count);

            //Test this[]
            for (var i = 0; i < dictionary.Count; i++)
            {
                Assert.AreEqual(i, (int)dictionary[i.ToString()]);
            }

            dictionary["1"] = 100; // try a valid setter
            Assert.AreEqual(100, (int)dictionary["1"]);

            //non-existing key
            Assert.Null(dictionary["NotAKey"]);
        }

        [Test]
        public static void TestIDictionary_Negative()
        {
            IDictionary dictionary = new ConcurrentDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(
               () => dictionary.Add(null, 1));
            // "TestIDictionary:  FAILED.  Add didn't throw ANE when null key is passed");

            // Invalid key type.
            Assert.Throws<ArgumentException>(() => dictionary.Add(1, 1));

            // Invalid value type.
            Assert.Throws<ArgumentException>(() => dictionary.Add("1", "1"));

            Assert.Throws<ArgumentNullException>(
               () => dictionary.Contains(null));
            // "TestIDictionary:  FAILED.  Contain didn't throw ANE when null key is passed");

            //Test Remove
            Assert.Throws<ArgumentNullException>(
               () => dictionary.Remove(null));
            // "TestIDictionary:  FAILED.  Remove didn't throw ANE when null key is passed");

            //Test this[]
            Assert.Throws<ArgumentNullException>(
               () => { var val = dictionary[null]; });
            // "TestIDictionary:  FAILED.  this[] getter didn't throw ANE when null key is passed");
            Assert.Throws<ArgumentNullException>(
               () => dictionary[null] = 0);
            // "TestIDictionary:  FAILED.  this[] setter didn't throw ANE when null key is passed");

            // Invalid key type.
            Assert.Throws<ArgumentException>(() => dictionary[1] = 0);

            // Invalid value type.
            Assert.Throws<ArgumentException>(() => dictionary["1"] = "0");
        }

        [Test]
        public static void IDictionary_Remove_NullKeyInKeyValuePair_ThrowsArgumentNullException()
        {
            IDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(() => dictionary.Remove(new KeyValuePair<string, int>(null, 0)));
        }

        [Test]
        public static void TestICollection()
        {
            ICollection dictionary = new ConcurrentDictionary<int, int>();
            Assert.False(dictionary.IsSynchronized, "TestICollection:  FAILED.  IsSynchronized returned true!");

            const int key = -1;
            const int value = +1;
            //add one item to the dictionary
            ((ConcurrentDictionary<int, int>)dictionary).TryAdd(key, value);

            var objectArray = new object[1];
            dictionary.CopyTo(objectArray, 0);

            Assert.AreEqual(key, ((KeyValuePair<int, int>)objectArray[0]).Key);
            Assert.AreEqual(value, ((KeyValuePair<int, int>)objectArray[0]).Value);

            var keyValueArray = new KeyValuePair<int, int>[1];
            dictionary.CopyTo(keyValueArray, 0);
            Assert.AreEqual(key, keyValueArray[0].Key);
            Assert.AreEqual(value, keyValueArray[0].Value);

            var entryArray = new DictionaryEntry[1];
            dictionary.CopyTo(entryArray, 0);
            Assert.AreEqual(key, (int)entryArray[0].Key);
            Assert.AreEqual(value, (int)entryArray[0].Value);
        }

        [Test]
        public static void TestICollection_Negative()
        {
            ICollection dictionary = new ConcurrentDictionary<int, int>();
            Assert.False(dictionary.IsSynchronized, "TestICollection:  FAILED.  IsSynchronized returned true!");

            Assert.Throws<NotSupportedException>(() => { var obj = dictionary.SyncRoot; });
            // "TestICollection:  FAILED.  SyncRoot property didn't throw");
            Assert.Throws<ArgumentNullException>(() => dictionary.CopyTo(null, 0));
            // "TestICollection:  FAILED.  CopyTo didn't throw ANE when null Array is passed");
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.CopyTo(new object[] { }, -1));
            // "TestICollection:  FAILED.  CopyTo didn't throw AORE when negative index passed");

            //add one item to the dictionary
            ((ConcurrentDictionary<int, int>)dictionary).TryAdd(1, 1);
            Assert.Throws<ArgumentException>(() => dictionary.CopyTo(new object[] { }, 0));
            // "TestICollection:  FAILED.  CopyTo didn't throw AE when the Array size is smaller than the dictionary count");
        }

        [Test]
        public static void TestClear()
        {
            var dictionary = new ConcurrentDictionary<int, int>();
            for (var i = 0; i < 10; i++)
            {
                dictionary.TryAdd(i, i);
            }

            Assert.AreEqual(10, dictionary.Count);

            dictionary.Clear();
            Assert.AreEqual(0, dictionary.Count);

            Assert.False(dictionary.TryRemove(1, out _), "TestClear: FAILED.  TryRemove succeeded after Clear");
            Assert.True(dictionary.IsEmpty, "TestClear: FAILED.  IsEmpty returned false after Clear");
        }

#if GREATERTHAN_NET40 || GREATERTHAN_NETSTANDARD10 || GREATERTHAN_NETCOREAPP10

        [Test]
        public static void TestTryUpdate()
        {
            var dictionary = new ConcurrentDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(
               () => dictionary.TryUpdate(null, 0, 0));
            // "TestTryUpdate:  FAILED.  TryUpdate didn't throw ANE when null key is passed");

            for (var i = 0; i < 10; i++)
            {
                dictionary.TryAdd(i.ToString(), i);
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.True(dictionary.TryUpdate(i.ToString(), i + 1, i), "TestTryUpdate:  FAILED.  TryUpdate failed!");
                Assert.AreEqual(i + 1, dictionary[i.ToString()]);
            }

            //test TryUpdate concurrently
            dictionary.Clear();
            for (var i = 0; i < 1000; i++)
            {
                dictionary.TryAdd(i.ToString(), i);
            }

            var mres = new ManualResetEventSlim();
            var tasks = new Task[10];
            var updatedKeys = new ThreadLocal<ThreadData>(true);
            for (var i = 0; i < tasks.Length; i++)
            {
                // We are creating the Task using TaskCreationOptions.LongRunning because...
                // there is no guarantee that the Task will be created on another thread.
                // There is also no guarantee that using this TaskCreationOption will force
                // it to be run on another thread.
                tasks[i] = Task.Factory.StartNew((obj) =>
                {
                    mres.Wait();
                    var index = (((int)obj) + 1) + 1000;
                    updatedKeys.Value = new ThreadData
                    {
                        ThreadIndex = index
                    };

                    for (var j = 0; j < dictionary.Count; j++)
                    {
                        if (dictionary.TryUpdate(j.ToString(), index, j))
                        {
                            if (dictionary[j.ToString()] != index)
                            {
                                updatedKeys.Value.Succeeded = false;
                                return;
                            }
                            updatedKeys.Value.Keys.Add(j.ToString());
                        }
                    }
                }, i, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }

            mres.Set();
            Task.WaitAll(tasks);

            var numberSucceeded = 0;
            var totalKeysUpdated = 0;
            foreach (var threadData in updatedKeys.Values)
            {
                totalKeysUpdated += threadData.Keys.Count;
                if (threadData.Succeeded)
                {
                    numberSucceeded++;
                }
            }

            Assert.True(numberSucceeded == tasks.Length, "One or more threads failed!");
            Assert.True(totalKeysUpdated == dictionary.Count,
               string.Format("TestTryUpdate:  FAILED.  The updated keys count doesn't match the dictionary count, expected {0}, actual {1}", dictionary.Count, totalKeysUpdated));
            foreach (var value in updatedKeys.Values)
            {
                for (var i = 0; i < value.Keys.Count; i++)
                {
                    Assert.True(dictionary[value.Keys[i]] == value.ThreadIndex,
                       string.Format("TestTryUpdate:  FAILED.  The updated value doesn't match the thread index, expected {0} actual {1}", value.ThreadIndex, dictionary[value.Keys[i]]));
                }
            }

            //test TryUpdate with non atomic values (intPtr > 8)
            var dict = new ConcurrentDictionary<int, Struct16>();
            dict.TryAdd(1, new Struct16(1, -1));
            Assert.True(dict.TryUpdate(1, new Struct16(2, -2), new Struct16(1, -1)), "TestTryUpdate:  FAILED.  TryUpdate failed for non atomic values ( > 8 bytes)");
        }

        [Test]
        public void ConcurrentWriteRead_NoTornValues()
        {
            var cd = new ConcurrentDictionary<int, KeyValuePair<long, long>>();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Task.WaitAll(
                TaskEx.Run(() =>
                {
                    for (long i = 0; !cts.IsCancellationRequested; i++)
                    {
                        cd[0] = new KeyValuePair<long, long>(i, i);
                    }
                }),
                TaskEx.Run(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        cd.TryGetValue(0, out var item);
                        try
                        {
                            Assert.AreEqual(item.Key, item.Value);
                        }
                        catch
                        {
                            cts.Cancel();
                            throw;
                        }
                    }
                }));
        }

#endif

#region Helper Classes and Methods

        private class ThreadData
        {
            public int ThreadIndex;
            public bool Succeeded = true;
            public List<string> Keys = new();
        }

        private struct Struct16 : IEqualityComparer<Struct16>
        {
            public long L1, L2;
            public Struct16(long l1, long l2)
            {
                L1 = l1;
                L2 = l2;
            }

            public bool Equals(Struct16 x, Struct16 y)
            {
                return x.L1 == y.L1 && x.L2 == y.L2;
            }

            public int GetHashCode(Struct16 obj)
            {
                return (int)L1;
            }
        }

#endregion
    }
}
