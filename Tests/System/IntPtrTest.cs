using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Theraot.Core;

namespace Tests.System
{
    [TestFixture]
    class IntPtrTest
    {
        [Test]
        public void IntPtrAddTest()
        {
            int[] arr = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            unsafe
            {
                fixed (int* parr = arr)
                {
                    IntPtr ptr = new IntPtr(parr);
                    // Get the size of an array element.
                    int size = sizeof(int);
                    for (int ctr = 0; ctr < arr.Length; ctr++)
                    {
                        IntPtr newPtr = IntPtrHelper.Add(ptr, ctr*size);
                        Console.Write("{0}   ", Marshal.ReadInt32(newPtr));
                    }
                }
            }
        }
    }
}
