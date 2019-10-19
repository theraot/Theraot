using NUnit.Framework;

namespace Tests.System
{
    [TestFixture]
    internal class ValueTupleTest
    {
        [Test]
        public void Equals()
        {
            var a = ("abc", "cde");
            var b = ("abc", "cde");
            Assert.AreEqual(a, b);
        }

        [Test]
        public void SameHashCode()
        {
            var a = ("abc", "cde");
            var b = ("abc", "cde");
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}