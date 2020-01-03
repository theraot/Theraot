#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Core;

namespace Tests.SystemTests.CollectionsTests.GenericTests
{
    [TestFixture]
    internal class HashSetTestEx
    {
        [Test]
        public void NotSafeEnumerator()
        {
            // This is the way Enumerators normally work
            var x = new HashSet<int>
            {
                14
            };
            Assert.Throws
            (
                typeof(InvalidOperationException),
                () =>
                {
                    foreach (var item in x)
                    {
                        x.Add(item * 3);
                    }
                }
            );
        }

        [Test]
        public void TryGetValue()
        {
            var actual = new object();
            var check = new object();
            var equalityComparer = new CustomEqualityComparer<object>(FuncHelper.GetTautologyFunc<object, object>(), _ => 0);
            var hashSet = new HashSetEx<object>(equalityComparer)
            {
                actual
            };
            var test = hashSet.TryGetValue(check, out var found);
            Assert.IsTrue(test);
            Assert.AreSame(actual, found);
            Assert.AreNotSame(check, found);
        }
    }
}