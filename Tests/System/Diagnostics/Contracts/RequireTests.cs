using NUnit.Framework;

namespace System.Diagnostics.Contracts.Tests
{
    [TestFixture]
    public static class RequireTests
    {
        public static void Target(string s)
        {
            Contract.Requires(s != null);
            GC.KeepAlive(s);
        }

        [Test]
        public static void AsNoop()
        {
            Target(null);
            Target("hello");
        }
    }
}