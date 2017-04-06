#if !NETCF

using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    internal class BasicAsyncTest
    {
        private static async Task<string> GetDataAsync()
        {
            var data = await SlowOperationAsync().ConfigureAwait(false);
            return data.ToString(CultureInfo.InvariantCulture);
        }

        private static Task<int> SlowOperationAsync()
        {
            Thread.Sleep(1);
            // FromResult does not exist in .NET 4.0 - TaskEx offer a consistent interface from .NET 2.0 to .NET 4.5
            return TaskEx.FromResult(7);
        }

        [Test]
        public void SimpleTest()
        {
            Assert.AreEqual("7", GetDataAsync().Result);
        }
    }
}

#endif