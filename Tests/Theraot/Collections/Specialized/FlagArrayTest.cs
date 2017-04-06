#if FAT

using NUnit.Framework;
using System;

namespace Theraot.Collections.Specialized
{
    public class FlagArrayTest
    {
        [Test]
        public static void FlagArrayCapacityIsGuaranteed()
        {
            Assert.IsFalse(new FlagArray(12)[11]);
            Assert.IsFalse(new FlagArray(63)[62]);
            Assert.IsFalse(new FlagArray(64)[63]);
            Assert.IsFalse(new FlagArray(65)[64]);
            Assert.IsFalse(new FlagArray(127)[126]);
            Assert.IsFalse(new FlagArray(128)[127]);
            Assert.IsFalse(new FlagArray(129)[128]);
            Assert.IsFalse(new FlagArray(2047)[2046]);
            Assert.IsFalse(new FlagArray(2048)[2047]);
            Assert.IsFalse(new FlagArray(2049)[2048]);
        }

        [Test]
        public static void FlagArrayConstructorTest()
        {
            var flagarray = new FlagArray(12);
            Assert.AreEqual(12, flagarray.Length);
        }

        [Test]
        public static void FlagArrayContainsLongA()
        {
            var flagarray = new FlagArray(60);
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsFalse(flagarray.Contains(true));
            flagarray[20] = true;
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            flagarray[0] = true;
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            for (int index = 1; index < 20; index++)
            {
                flagarray[index] = true;
            }
            for (int index = 21; index < 60; index++)
            {
                flagarray[index] = true;
            }
            Assert.IsFalse(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
        }

        [Test]
        public static void FlagArrayContainsLongB()
        {
            var flagarray = new FlagArray(60);
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsFalse(flagarray.Contains(true));
            for (int index = 0; index < 60; index++)
            {
                flagarray[index] = true;
            }
            Assert.IsFalse(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            flagarray[20] = false;
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            flagarray[0] = false;
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            for (int index = 1; index < 20; index++)
            {
                flagarray[index] = false;
            }
            for (int index = 21; index < 60; index++)
            {
                flagarray[index] = false;
            }
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsFalse(flagarray.Contains(true));
        }

        [Test]
        public static void FlagArrayContainsShort()
        {
            var flagarray = new FlagArray(6);
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsFalse(flagarray.Contains(true));
            flagarray[5] = true;
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            flagarray[0] = true;
            Assert.IsTrue(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
            flagarray[1] = true;
            flagarray[2] = true;
            flagarray[3] = true;
            flagarray[4] = true;
            Assert.IsFalse(flagarray.Contains(false));
            Assert.IsTrue(flagarray.Contains(true));
        }

        [Test]
        public static void FlagArrayCopyTo()
        {
            var flagarray = new FlagArray(6);
            flagarray[0] = true;
            flagarray[5] = true;
            var expected = new[] { true, false, false, false, false, true };
            var found = new bool[6];
            flagarray.CopyTo(found);
            var index = 0;
            foreach (var flag in found)
            {
                Assert.AreEqual(expected[index], flag);
                index++;
            }
        }

        [Test]
        public static void FlagArrayFailsBeyondCapacity()
        {
            Assert.Throws<IndexOutOfRangeException>(() => GC.KeepAlive(new FlagArray(12)[3000]));
        }

        [Test]
        public static void FlagArrayFlags()
        {
            var flagarray = new FlagArray(6);
            var expected = new[] { 0, 5 };
            foreach (var flagIndex in expected)
            {
                flagarray[flagIndex] = true;
            }
            var index = 0;
            foreach (var flagIndex in flagarray.Flags)
            {
                Assert.AreEqual(expected[index], flagIndex);
                index++;
            }
        }

        [Test]
        public static void FlagArrayGetEnumerator()
        {
            var flagarray = new FlagArray(6);
            flagarray[0] = true;
            flagarray[5] = true;
            var expected = new[] { true, false, false, false, false, true };
            var index = 0;
            foreach (var flag in flagarray)
            {
                Assert.AreEqual(expected[index], flag);
                index++;
            }
        }

        [Test]
        public static void FlagArrayIndexOf()
        {
            var flagarray = new FlagArray(12);
            flagarray[5] = true;
            Assert.AreEqual(5, flagarray.IndexOf(true));
            Assert.AreEqual(0, flagarray.IndexOf(false));
            flagarray[0] = true;
            Assert.AreEqual(0, flagarray.IndexOf(true));
            Assert.AreEqual(1, flagarray.IndexOf(false));
        }

        [Test]
        public static void FlagArrayStoresValues()
        {
            var flagarray = new FlagArray(12);
            Assert.IsFalse(flagarray[5]);
            flagarray[5] = true;
            Assert.IsTrue(flagarray[5]);
            Assert.IsFalse(flagarray[9]);
            flagarray[9] = true;
            Assert.IsTrue(flagarray[9]);
        }
    }
}

#endif