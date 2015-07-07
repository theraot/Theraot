// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Tests.ExpressionCompiler.Array
{
    public static class ArrayBoundsTests
    {
        #region Test methods

        #region Byte sized arrays

        [Test]
        public static void CheckBoolArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyBoolArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyByteArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyCharArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyDecimalArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyDoubleArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyFloatArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyIntArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyLongArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyStructArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifySByteArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyStructWithStringArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyShortArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyStructWithValueArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyUIntArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyULongArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyUShortArrayWithByteSize(size);
            }
        }

        #endregion

        #region Int sized arrays

        [Test]
        public static void CheckBoolArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyBoolArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyByteArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyCharArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyDecimalArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyDoubleArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyFloatArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyIntArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyLongArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStructArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifySByteArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStructWithStringArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyShortArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStructWithValueArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyUIntArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyULongArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyUShortArrayWithIntSize(size);
            }
        }

        #endregion

        #region Long sized arrays

        [Test]
        public static void CheckBoolArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyBoolArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyByteArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyCharArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyDecimalArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyDoubleArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyFloatArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyIntArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyLongArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStructArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifySByteArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStructWithStringArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyShortArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStructWithValueArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyUIntArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyULongArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyUShortArrayWithLongSize(size);
            }
        }

        #endregion

        #region SByte sized arrays

        [Test]
        public static void CheckBoolArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyBoolArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyByteArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyCharArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyDecimalArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyDoubleArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyFloatArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyIntArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyLongArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStructArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifySByteArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStructWithStringArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyShortArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStructWithValueArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyUIntArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyULongArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyUShortArrayWithSByteSize(size);
            }
        }

        #endregion

        #region Short sized arrays

        [Test]
        public static void CheckBoolArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyBoolArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyByteArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyCharArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyDecimalArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyDoubleArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyFloatArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyIntArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyLongArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStructArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifySByteArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStructWithStringArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyShortArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStructWithValueArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyUIntArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyULongArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyUShortArrayWithShortSize(size);
            }
        }

        #endregion

        #region UInt sized arrays

        [Test]
        public static void CheckBoolArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyBoolArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyByteArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyCharArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyDecimalArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyDoubleArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyFloatArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyIntArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyLongArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyStructArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifySByteArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyStructWithStringArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyShortArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyStructWithValueArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyUIntArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyULongArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyUShortArrayWithUIntSize(size);
            }
        }

        #endregion

        #region ULong sized arrays

        [Test]
        public static void CheckBoolArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyBoolArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyByteArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyCharArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyDecimalArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyDoubleArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyFloatArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyIntArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyLongArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyStructArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifySByteArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyStructWithStringArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyShortArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyStructWithValueArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyUIntArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyULongArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyUShortArrayWithULongSize(size);
            }
        }

        #endregion

        #region UShort sized arrays

        [Test]
        public static void CheckBoolArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyBoolArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckByteArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyByteArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckCharArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyCharArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckDecimalArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyDecimalArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckDoubleArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyDoubleArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckFloatArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyFloatArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckIntArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyIntArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckLongArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyLongArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckStructArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyStructArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckSByteArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifySByteArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyStructWithStringArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithStringAndValueArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyStructWithStringAndValueArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckShortArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyShortArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithTwoValuesArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyStructWithTwoValuesArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckStructWithValueArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyStructWithValueArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckUIntArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyUIntArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckULongArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyULongArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckUShortArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyUShortArrayWithUShortSize(size);
            }
        }

        #endregion

        #endregion

        #region Verify methods

        #region  verifiers

        private static void VerifyBoolArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region  verifiers

        private static void VerifyBoolArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<bool[]>> e =
                Expression.Lambda<Func<bool[]>>(
                    Expression.NewArrayBounds(typeof(bool),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool[]> f = e.Compile();

            // get the array
            bool[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            bool[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new bool[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyByteArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<byte[]>> e =
                Expression.Lambda<Func<byte[]>>(
                    Expression.NewArrayBounds(typeof(byte),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte[]> f = e.Compile();

            // get the array
            byte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            byte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new byte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyCharArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<char[]>> e =
                Expression.Lambda<Func<char[]>>(
                    Expression.NewArrayBounds(typeof(char),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<char[]> f = e.Compile();

            // get the array
            char[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            char[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new char[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDecimalArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<decimal[]>> e =
                Expression.Lambda<Func<decimal[]>>(
                    Expression.NewArrayBounds(typeof(decimal),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal[]> f = e.Compile();

            // get the array
            decimal[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            decimal[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new decimal[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyDoubleArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<double[]>> e =
                Expression.Lambda<Func<double[]>>(
                    Expression.NewArrayBounds(typeof(double),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double[]> f = e.Compile();

            // get the array
            double[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            double[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new double[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyFloatArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<float[]>> e =
                Expression.Lambda<Func<float[]>>(
                    Expression.NewArrayBounds(typeof(float),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float[]> f = e.Compile();

            // get the array
            float[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            float[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new float[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyIntArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<int[]>> e =
                Expression.Lambda<Func<int[]>>(
                    Expression.NewArrayBounds(typeof(int),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int[]> f = e.Compile();

            // get the array
            int[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            int[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new int[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyLongArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<long[]>> e =
                Expression.Lambda<Func<long[]>>(
                    Expression.NewArrayBounds(typeof(long),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long[]> f = e.Compile();

            // get the array
            long[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            long[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new long[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<S[]>> e =
                Expression.Lambda<Func<S[]>>(
                    Expression.NewArrayBounds(typeof(S),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<S[]> f = e.Compile();

            // get the array
            S[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            S[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new S[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifySByteArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<sbyte[]>> e =
                Expression.Lambda<Func<sbyte[]>>(
                    Expression.NewArrayBounds(typeof(sbyte),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte[]> f = e.Compile();

            // get the array
            sbyte[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            sbyte[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new sbyte[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<Sc[]>> e =
                Expression.Lambda<Func<Sc[]>>(
                    Expression.NewArrayBounds(typeof(Sc),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc[]> f = e.Compile();

            // get the array
            Sc[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sc[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithStringAndValueArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<Scs[]>> e =
                Expression.Lambda<Func<Scs[]>>(
                    Expression.NewArrayBounds(typeof(Scs),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs[]> f = e.Compile();

            // get the array
            Scs[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Scs[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Scs[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyShortArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<short[]>> e =
                Expression.Lambda<Func<short[]>>(
                    Expression.NewArrayBounds(typeof(short),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short[]> f = e.Compile();

            // get the array
            short[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            short[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new short[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithTwoValuesArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<Sp[]>> e =
                Expression.Lambda<Func<Sp[]>>(
                    Expression.NewArrayBounds(typeof(Sp),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp[]> f = e.Compile();

            // get the array
            Sp[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Sp[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Sp[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyStructWithValueArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<Ss[]>> e =
                Expression.Lambda<Func<Ss[]>>(
                    Expression.NewArrayBounds(typeof(Ss),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss[]> f = e.Compile();

            // get the array
            Ss[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            Ss[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ss[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUIntArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<uint[]>> e =
                Expression.Lambda<Func<uint[]>>(
                    Expression.NewArrayBounds(typeof(uint),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint[]> f = e.Compile();

            // get the array
            uint[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            uint[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new uint[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyULongArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<ulong[]>> e =
                Expression.Lambda<Func<ulong[]>>(
                    Expression.NewArrayBounds(typeof(ulong),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong[]> f = e.Compile();

            // get the array
            ulong[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ulong[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ulong[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        private static void VerifyUShortArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<ushort[]>> e =
                Expression.Lambda<Func<ushort[]>>(
                    Expression.NewArrayBounds(typeof(ushort),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort[]> f = e.Compile();

            // get the array
            ushort[] result = null;
            Exception creationEx = null;
            try
            {
                result = f();
            }
            catch (Exception ex)
            {
                creationEx = ex;
            }

            // generate expected array
            ushort[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new ushort[(long)size];
            }
            catch (Exception ex)
            {
                expectedEx = ex;
            }

            // if one failed, verify the other did, too
            if (creationEx != null || expectedEx != null)
            {
                Assert.NotNull(creationEx);
                Assert.NotNull(expectedEx);
                Assert.AreEqual(expectedEx.GetType(), creationEx.GetType());
            }
            else
            {
                // otherwise, verify the contents array
                Assert.AreEqual(expected.Length, result.Length);
                for (int i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expected[i], result[i]);
                }
            }
        }

        #endregion

        #endregion
    }
}
