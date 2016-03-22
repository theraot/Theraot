using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class BasicAsyncTest
    {
        private static async Task<string> GetData()
        {
            var data = await SlowOperation();
            return data.ToString(CultureInfo.InvariantCulture);
        }

        private static Task<int> SlowOperation()
        {
            Thread.Sleep(1);
            // FromResult does not exist in .NET 4.0 - TaskEx offer a consistent interface from .NET 2.0 to .NET 4.5
            return TaskEx.FromResult(7);
        }

        [Test]
        public void SimpleTest()
        {
            Assert.AreEqual("7", GetData().Result);
        }
    }
}
