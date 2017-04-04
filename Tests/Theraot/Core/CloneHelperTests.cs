#if FAT

using NUnit.Framework;
using System;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    public class CloneHelperTests
    {
        [Test]
        public void CloneBasicTypes()
        {
            Assert.AreEqual('a', CloneHelper<char>.GetCloner().Clone('a'));

            Assert.AreEqual(false, CloneHelper<bool>.GetCloner().Clone(false));

            Assert.AreEqual(123, CloneHelper<byte>.GetCloner().Clone(123));
            Assert.AreEqual(1234, CloneHelper<ushort>.GetCloner().Clone(1234));
            Assert.AreEqual(123456, CloneHelper<uint>.GetCloner().Clone(123456));
            Assert.AreEqual(12345678901, CloneHelper<ulong>.GetCloner().Clone(12345678901));

            Assert.AreEqual(-123, CloneHelper<sbyte>.GetCloner().Clone(-123));
            Assert.AreEqual(-1234, CloneHelper<short>.GetCloner().Clone(-1234));
            Assert.AreEqual(-123456, CloneHelper<int>.GetCloner().Clone(-123456));
            Assert.AreEqual(-12345678901, CloneHelper<long>.GetCloner().Clone(-12345678901));

            Assert.AreEqual(decimal.Parse("12345678901234567890123456789"), CloneHelper<decimal>.GetCloner().Clone(decimal.Parse("12345678901234567890123456789")));

            Assert.AreEqual(0.5F, CloneHelper<double>.GetCloner().Clone(0.5f));
            Assert.AreEqual(0.5, CloneHelper<double>.GetCloner().Clone(0.5));
        }

        public void CloneDate()
        {
            var date = DateTime.Now;
            var clone = CloneHelper<DateTime>.GetCloner().Clone(date);
            Assert.AreEqual(date, clone);
            Assert.IsFalse(ReferenceEquals(date, clone));
        }

        public void CloneString()
        {
            var str = DateTime.Now.ToString();
            var clone = CloneHelper<string>.GetCloner().Clone(str);
            Assert.AreEqual(str, clone);
            Assert.IsFalse(ReferenceEquals(str, clone));
        }

        public void CloneObject()
        {
            var obj = new object();
            var clone = CloneHelper<object>.GetCloner().Clone(obj);
            Assert.AreEqual(obj, clone);
            Assert.IsFalse(ReferenceEquals(obj, clone));
        }

        public void CloneArray()
        {
            var array = new int[] { 1, 2, 3 };
            var clone = CloneHelper<object>.GetCloner().Clone(array);
            Assert.AreEqual(array, clone);
            Assert.IsFalse(ReferenceEquals(array, clone));
        }
    }
}

#endif