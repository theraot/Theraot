using System.Diagnostics.Contracts;
using NUnit.Framework;
using Theraot;

namespace Tests.SystemTests.DiagnosticsTests.ContractsTests
{
    [TestFixture]
    public static class RequireTests
    {
        public static void Target(string s)
        {
            Contract.Requires(s != null);
            No.Op(s);
        }

        [Test]
        public static void AsNoop()
        {
            Target(null);
            Target("hello");
        }
    }
}