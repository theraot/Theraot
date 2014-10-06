using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MonoTests.System.Collections.Generic
{
    [TestFixture]
    class HashSetTestEx
    {
        [Test]
        public void NotSafeEnumerator()
        {
            // This is the way Enumerators normally work
            var x = new HashSet<int> {14};
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
