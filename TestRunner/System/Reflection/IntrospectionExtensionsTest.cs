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
            const Type Type = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => Type.GetTypeInfo());
        }
    }
}