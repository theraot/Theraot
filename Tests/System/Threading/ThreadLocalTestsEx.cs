using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixtureAttribute]
    public class ThreadLocalTestsEx
    {
        private int nTimes;
        private ThreadLocal<int> threadLocal;

        [Test]
        public void ThreadLocalDoesNotUseTheDefaultConstructor()
        {
            var local = new ThreadLocal<Random>();

            Assert.AreEqual(null, local.Value);
        }
    }
}