// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Tests.ExpressionCompiler.Array
{
    public static class ObjectArrayBoundsTests
    {
        #region Test methods

        #region Byte sized arrays

        [Test]
        public static void CheckCustomArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyCustomArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyCustom2ArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyDelegateArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyEnumArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyLongEnumArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyFuncArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyInterfaceArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyIEquatableCustomArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyObjectArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyStringArrayWithByteSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithByteSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithByteSize()
        {
            foreach (byte size in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithByteSize<Scs>(size);
            }
        }

        #endregion

        #region Int sized arrays

        [Test]
        public static void CheckCustomArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyCustomArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyCustom2ArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyDelegateArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyEnumArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyLongEnumArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyFuncArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyInterfaceArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyIEquatableCustomArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyObjectArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStringArrayWithIntSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithIntSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithIntSize()
        {
            foreach (int size in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithIntSize<Scs>(size);
            }
        }

        #endregion

        #region Long sized arrays

        [Test]
        public static void CheckCustomArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyCustomArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyCustom2ArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyDelegateArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyEnumArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyLongEnumArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyFuncArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyInterfaceArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyIEquatableCustomArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyObjectArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStringArrayWithLongSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithLongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithLongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithLongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithLongSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithLongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithLongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithLongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithLongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithLongSize()
        {
            foreach (long size in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithLongSize<Scs>(size);
            }
        }

        #endregion

        #region SByte sized arrays

        [Test]
        public static void CheckCustomArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyCustomArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyCustom2ArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyDelegateArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyEnumArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyLongEnumArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyFuncArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyInterfaceArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyIEquatableCustomArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyObjectArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStringArrayWithSByteSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithSByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithSByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithSByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithSByteSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithSByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithSByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithSByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithSByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithSByteSize()
        {
            foreach (sbyte size in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithSByteSize<Scs>(size);
            }
        }

        #endregion

        #region Short sized arrays

        [Test]
        public static void CheckCustomArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyCustomArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyCustom2ArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyDelegateArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyEnumArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyLongEnumArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyFuncArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyInterfaceArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyIEquatableCustomArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyObjectArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStringArrayWithShortSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithShortSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithShortSize()
        {
            foreach (short size in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithShortSize<Scs>(size);
            }
        }

        #endregion

        #region UInt sized arrays

        [Test]
        public static void CheckCustomArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyCustomArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyCustom2ArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyDelegateArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyEnumArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyLongEnumArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyFuncArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyInterfaceArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyIEquatableCustomArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyObjectArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyStringArrayWithUIntSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithUIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithUIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithUIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithUIntSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithUIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithUIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithUIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithUIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithUIntSize()
        {
            foreach (uint size in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithUIntSize<Scs>(size);
            }
        }

        #endregion

        #region ULong sized arrays

        [Test]
        public static void CheckCustomArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyCustomArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyCustom2ArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyDelegateArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyEnumArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyLongEnumArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyFuncArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyInterfaceArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyIEquatableCustomArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyObjectArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyStringArrayWithULongSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithULongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithULongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithULongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithULongSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithULongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithULongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithULongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithULongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithULongSize()
        {
            foreach (ulong size in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithULongSize<Scs>(size);
            }
        }

        #endregion

        #region UShort sized arrays

        [Test]
        public static void CheckCustomArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyCustomArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyCustom2ArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyDelegateArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyEnumArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyLongEnumArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyFuncArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyInterfaceArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyIEquatableCustomArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyObjectArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyStringArrayWithUShortSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithUShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithUShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithUShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithUShortSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithUShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithUShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithUShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithUShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithUShortSize()
        {
            foreach (ushort size in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithUShortSize<Scs>(size);
            }
        }

        #endregion

        #endregion

        #region Verify methods

        #region  verifiers

        private static void VerifyCustomArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithByteSize(byte size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithIntSize(int size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithLongSize(long size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithSByteSize(sbyte size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithShortSize(short size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithUIntSize(uint size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithULongSize(ulong size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyCustomArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<C[]> f = e.Compile();

            // get the array
            C[] result = null;
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
            C[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new C[(long)size];
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

        private static void VerifyCustom2ArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<D[]> f = e.Compile();

            // get the array
            D[] result = null;
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
            D[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new D[(long)size];
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

        private static void VerifyDelegateArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate[]> f = e.Compile();

            // get the array
            Delegate[] result = null;
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
            Delegate[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Delegate[(long)size];
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

        private static void VerifyEnumArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<E[]> f = e.Compile();

            // get the array
            E[] result = null;
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
            E[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new E[(long)size];
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

        private static void VerifyLongEnumArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<El[]> f = e.Compile();

            // get the array
            El[] result = null;
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
            El[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new El[(long)size];
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

        private static void VerifyFuncArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>[]> f = e.Compile();

            // get the array
            Func<object>[] result = null;
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
            Func<object>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Func<object>[(long)size];
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

        private static void VerifyInterfaceArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<I[]> f = e.Compile();

            // get the array
            I[] result = null;
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
            I[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new I[(long)size];
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

        private static void VerifyIEquatableCustomArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>[]> f = e.Compile();

            // get the array
            IEquatable<C>[] result = null;
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
            IEquatable<C>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<C>[(long)size];
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

        private static void VerifyIEquatableCustom2ArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>[]> f = e.Compile();

            // get the array
            IEquatable<D>[] result = null;
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
            IEquatable<D>[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new IEquatable<D>[(long)size];
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

        private static void VerifyObjectArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<object[]> f = e.Compile();

            // get the array
            object[] result = null;
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
            object[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new object[(long)size];
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

        private static void VerifyStringArrayWithUShortSize(ushort size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<string[]> f = e.Compile();

            // get the array
            string[] result = null;
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
            string[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new string[(long)size];
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

        private static void VerifyGenericArrayWithByteSize<T>(byte size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithIntSize<T>(int size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithLongSize<T>(long size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithSByteSize<T>(sbyte size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithShortSize<T>(short size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithUIntSize<T>(uint size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithULongSize<T>(ulong size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericArrayWithUShortSize<T>(ushort size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<T[]> f = e.Compile();

            // get the array
            T[] result = null;
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
            T[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new T[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithByteSize<Tc>(byte size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithIntSize<Tc>(int size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithLongSize<Tc>(long size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithSByteSize<Tc>(sbyte size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithShortSize<Tc>(short size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithUIntSize<Tc>(uint size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithULongSize<Tc>(ulong size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithClassRestrictionArrayWithUShortSize<Tc>(ushort size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc[]> f = e.Compile();

            // get the array
            Tc[] result = null;
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
            Tc[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tc[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithByteSize<TC>(byte size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithIntSize<TC>(int size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithLongSize<TC>(long size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithSByteSize<TC>(sbyte size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithShortSize<TC>(short size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithUIntSize<TC>(uint size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithULongSize<TC>(ulong size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithUShortSize<TC>(ushort size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC[]> f = e.Compile();

            // get the array
            TC[] result = null;
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
            TC[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TC[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithByteSize<Tcn>(byte size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithIntSize<Tcn>(int size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithLongSize<Tcn>(long size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithSByteSize<Tcn>(sbyte size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithShortSize<Tcn>(short size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithUIntSize<Tcn>(uint size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithULongSize<Tcn>(ulong size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithUShortSize<Tcn>(ushort size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn[]> f = e.Compile();

            // get the array
            Tcn[] result = null;
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
            Tcn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Tcn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithByteSize<TCn>(byte size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithIntSize<TCn>(int size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithLongSize<TCn>(long size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithSByteSize<TCn>(sbyte size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithShortSize<TCn>(short size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithUIntSize<TCn>(uint size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithULongSize<TCn>(ulong size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithUShortSize<TCn>(ushort size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn[]> f = e.Compile();

            // get the array
            TCn[] result = null;
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
            TCn[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new TCn[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithByteSize<Ts>(byte size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(byte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithIntSize<Ts>(int size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithLongSize<Ts>(long size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithSByteSize<Ts>(sbyte size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(sbyte))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithShortSize<Ts>(short size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithUIntSize<Ts>(uint size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithULongSize<Ts>(ulong size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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

        private static void VerifyGenericWithStructRestrictionArrayWithUShortSize<Ts>(ushort size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts[]> f = e.Compile();

            // get the array
            Ts[] result = null;
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
            Ts[] expected = null;
            Exception expectedEx = null;
            try
            {
                expected = new Ts[(long)size];
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
