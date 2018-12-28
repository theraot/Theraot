using System;
using System.Reflection;

namespace TestRunner.System.Reflection
{
    [TestFixture]
    public static class IntrospectionExtensionsTest
    {
        [Test]
        public static void GetTypeInfoOnNullThrowsNullReferenceException()
        {
            Type type = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => type.GetTypeInfo());
        }
    }
}