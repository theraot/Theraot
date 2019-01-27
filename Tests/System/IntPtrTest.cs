using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using Theraot.Core;

namespace Tests.System
{
    [TestFixture]
    internal class IntPtrTest
    {
        [Test]
        public void IntPtrAddTest()
        {
            int[] arr = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            unsafe
            {
                fixed (int* parr = arr)
                {
                    var ptr = new IntPtr(parr);
                    // Get the size of an array element.
                    const int Size = sizeof(int);
                    var index = 0;
                    for (var ctr = 0; ctr < arr.Length; ctr++)
                    {
                        var newPtr = IntPtrEx.Add(ptr, ctr * Size);
                        Assert.AreEqual(arr[index], Marshal.ReadInt32(newPtr));
                        index++;
                    }
                }
            }
        }

        [Test]
        public void IntPtrSubtractTest()
        {
            int[] arr = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            unsafe
            {
                fixed (int* parr = arr)
                {
                    // Get the size of an array element.
                    const int Size = sizeof(int);
                    var ptr = IntPtrEx.Add(new IntPtr(parr), Size * (arr.Length - 1));
                    var index = arr.Length - 1;
                    for (var ctr = 0; ctr < arr.Length; ctr++)
                    {
                        var newPtr = IntPtrEx.Subtract(ptr, ctr * Size);
                        Assert.AreEqual(arr[index], Marshal.ReadInt32(newPtr));
                        index--;
                    }
                }
            }
        }
    }
}