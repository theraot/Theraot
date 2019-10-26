#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Theraot.Reflection;

namespace Tests.Theraot.Reflection
{
    [TestFixture]
    public class DelegateHelperTests
    {
        [Test]
        public void GetTryGetValueDelegate()
        {
            var dict = new Dictionary<string, string>
            {
                { "hello", "world" }
            };
            var del = DelegateHelper.GetDelegateType(typeof(string), typeof(string).MakeByRefType(), typeof(bool));
            var x = typeof(Dictionary<string, string>).GetMethod(nameof(dict.TryGetValue)).CreateDelegate(del, dict);
            var data = new object[] { "hello", null };
            var r = x.DynamicInvoke(data);
            Assert.AreEqual(true, r);
            Assert.AreEqual("world", data[1]);
        }
    }
}