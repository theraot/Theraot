#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MonoTests.System.Collections.Generic
{
    [TestFixture]
    internal class HashSetTestEx
    {
        [Test]
        public void NotSafeEnumerator()
        {
            // This is the way Enumerators normally work
            var x = new HashSet<int> { 14 };
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
    }
}