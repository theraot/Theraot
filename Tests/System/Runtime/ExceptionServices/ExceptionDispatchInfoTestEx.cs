using System;
using NUnit.Framework;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace MonoTests.System.Runtime.ExceptionServices
{
    [TestFixture]
    public class ExceptionDispatchInfoTestEx
    {
        [Test]
        public void CaptureExtensiveTest()
        {
            Exception original = null;
            ExceptionDispatchInfo info = null;
            string[] original_stack = null;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    throw new NotSupportedException();
                }
                catch (Exception ex)
                {
                    original = ex;
                    original_stack = original.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    info = ExceptionDispatchInfo.Capture(ex);
                }
            }).Wait();

            try
            {
                info.Throw();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(original, ex);
                var stack = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                for (int index = 0; index < original_stack.Length; index++)
                {
                    Assert.AreEqual(stack[index], original_stack[index]);
                }

                Assert.IsTrue(stack.Length > original_stack.Length);
                Assert.IsTrue(stack[original_stack.Length].Contains("---"));
                Assert.IsTrue(stack[original_stack.Length + 1].Contains("System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()"));
            }
        }
    }
}