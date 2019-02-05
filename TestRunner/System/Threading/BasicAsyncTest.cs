using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace TestRunner.System.Threading
{
    [TestFixture]
    internal static class BasicAsyncTest
    {
        [Test]
        public static void SimpleTest()
        {
            Assert.AreEqual("7", GetDataAsync().Result);
        }

        private static async Task<string> GetDataAsync()
        {
            var data = await SlowOperationAsync().ConfigureAwait(false);
            return data.ToString(CultureInfo.InvariantCulture);
        }

        private static Task<int> SlowOperationAsync()
        {
            Thread.Sleep(1);

            // FromResult does not exist in .NET 4.0 - TaskEx offer a consistent API
            return TaskEx.FromResult(7);
        }
    }
}