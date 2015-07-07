// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Tests.ExpressionCompiler.Array
{
    public class ObjectNullableArrayBoundsTests
    {
        #region NullableByte sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyCustomArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyCustom2ArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyDelegateArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyEnumArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyLongEnumArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyFuncArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyInterfaceArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyObjectArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyStringArrayWithNullableByteSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithNullableByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithNullableByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithNullableByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithNullableByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericArrayWithNullableByteSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableByteSize()
        {
            foreach (byte? size in new byte?[] { null, 0, 1, byte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableByteSize<Scs>(size);
            }
        }

        #endregion

        #region NullableInt sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyCustomArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyCustom2ArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyDelegateArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyEnumArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyLongEnumArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyFuncArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyInterfaceArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyObjectArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyStringArrayWithNullableIntSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithNullableIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithNullableIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithNullableIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithNullableIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericArrayWithNullableIntSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableIntSize()
        {
            foreach (int? size in new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableIntSize<Scs>(size);
            }
        }

        #endregion

        #region NullableLong sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyCustomArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyCustom2ArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyDelegateArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyEnumArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyLongEnumArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyFuncArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyInterfaceArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyObjectArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyStringArrayWithNullableLongSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithNullableLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithNullableLongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithNullableLongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithNullableLongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericArrayWithNullableLongSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableLongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableLongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableLongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableLongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableLongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableLongSize()
        {
            foreach (long? size in new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableLongSize<Scs>(size);
            }
        }

        #endregion

        #region NullableSByte sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyCustomArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyCustom2ArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyDelegateArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyEnumArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyLongEnumArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyFuncArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyInterfaceArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyObjectArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyStringArrayWithNullableSByteSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithNullableSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithNullableSByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithNullableSByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithNullableSByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericArrayWithNullableSByteSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableSByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableSByteSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableSByteSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableSByteSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableSByteSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableSByteSize()
        {
            foreach (sbyte? size in new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableSByteSize<Scs>(size);
            }
        }

        #endregion

        #region NullableShort sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyCustomArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyCustom2ArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyDelegateArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyEnumArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyLongEnumArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyFuncArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyInterfaceArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyObjectArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyStringArrayWithNullableShortSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithNullableShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithNullableShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithNullableShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithNullableShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericArrayWithNullableShortSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableShortSize()
        {
            foreach (short? size in new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableShortSize<Scs>(size);
            }
        }

        #endregion

        #region NullableUInt sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyCustomArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyCustom2ArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyDelegateArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyEnumArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyLongEnumArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyFuncArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyInterfaceArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyObjectArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyStringArrayWithNullableUIntSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithNullableUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithNullableUIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithNullableUIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithNullableUIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericArrayWithNullableUIntSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableUIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableUIntSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableUIntSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableUIntSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableUIntSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableUIntSize()
        {
            foreach (uint? size in new uint?[] { null, 0, 1, uint.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableUIntSize<Scs>(size);
            }
        }

        #endregion

        #region NullableULong sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyCustomArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyCustom2ArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyDelegateArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyEnumArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyLongEnumArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyFuncArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyInterfaceArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyObjectArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyStringArrayWithNullableULongSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithNullableULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithNullableULongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithNullableULongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithNullableULongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericArrayWithNullableULongSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableULongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableULongSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableULongSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableULongSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableULongSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableULongSize()
        {
            foreach (ulong? size in new ulong?[] { null, 0, 1, ulong.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableULongSize<Scs>(size);
            }
        }

        #endregion

        #region NullableUShort sized arrays

        [Test]
        public static void CheckCustomArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyCustomArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckCustom2ArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyCustom2ArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckDelegateArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyDelegateArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckEnumArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyEnumArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckLongEnumArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyLongEnumArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckFuncArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyFuncArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckInterfaceArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyInterfaceArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustomArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyIEquatableCustomArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckIEquatableCustom2ArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyIEquatableCustom2ArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckObjectArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyObjectArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckStringArrayWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyStringArrayWithNullableUShortSize(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfCustomWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithNullableUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfEnumWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithNullableUShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfObjectWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithNullableUShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithNullableUShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericArrayOfStructWithStringAndValueWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericArrayWithNullableUShortSize<Scs>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfCustomWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassRestrictionArrayOfObjectWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassRestrictionArrayWithNullableUShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassRestrictionArrayOfCustomWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithSubClassRestrictionArrayWithNullableUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfCustomWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithClassAndNewRestrictionArrayOfObjectWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithClassAndNewRestrictionArrayWithNullableUShortSize<object>(size);
            }
        }

        [Test]
        public static void CheckGenericWithSubClassAndNewRestrictionArrayOfCustomWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableUShortSize<C>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfEnumWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableUShortSize<E>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableUShortSize<S>(size);
            }
        }

        [Test]
        public static void CheckGenericWithStructRestrictionArrayOfStructWithStringAndValueWithNullableUShortSize()
        {
            foreach (ushort? size in new ushort?[] { null, 0, 1, ushort.MaxValue })
            {
                VerifyGenericWithStructRestrictionArrayWithNullableUShortSize<Scs>(size);
            }
        }

        #endregion

        #region NullableByte verifiers

        private static void VerifyCustomArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyCustom2ArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyDelegateArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyEnumArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyLongEnumArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyFuncArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyInterfaceArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyObjectArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyStringArrayWithNullableByteSize(byte? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(byte?))),
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
        #region NullableInt verifiers

        private static void VerifyCustomArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyCustom2ArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyDelegateArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyEnumArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyLongEnumArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyFuncArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyInterfaceArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyObjectArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyStringArrayWithNullableIntSize(int? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(int?))),
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
        #region NullableLong verifiers

        private static void VerifyCustomArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyCustom2ArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyDelegateArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyEnumArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyLongEnumArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyFuncArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyInterfaceArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyObjectArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyStringArrayWithNullableLongSize(long? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(long?))),
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
        #region NullableSByte verifiers

        private static void VerifyCustomArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyCustom2ArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyDelegateArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyEnumArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyLongEnumArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyFuncArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyInterfaceArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyObjectArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyStringArrayWithNullableSByteSize(sbyte? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(sbyte?))),
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
        #region NullableShort verifiers

        private static void VerifyCustomArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyCustom2ArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyDelegateArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyEnumArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyLongEnumArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyFuncArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyInterfaceArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyObjectArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyStringArrayWithNullableShortSize(short? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(short?))),
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
        #region NullableUInt verifiers

        private static void VerifyCustomArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyCustom2ArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyDelegateArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyEnumArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyLongEnumArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyFuncArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyInterfaceArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyObjectArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyStringArrayWithNullableUIntSize(uint? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(uint?))),
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
        #region NullableULong verifiers

        private static void VerifyCustomArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyCustom2ArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyDelegateArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyEnumArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyLongEnumArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyFuncArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyInterfaceArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyObjectArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyStringArrayWithNullableULongSize(ulong? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(ulong?))),
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
        #region NullableUShort verifiers

        private static void VerifyCustomArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<C[]>> e =
                Expression.Lambda<Func<C[]>>(
                    Expression.NewArrayBounds(typeof(C),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyCustom2ArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<D[]>> e =
                Expression.Lambda<Func<D[]>>(
                    Expression.NewArrayBounds(typeof(D),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyDelegateArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<Delegate[]>> e =
                Expression.Lambda<Func<Delegate[]>>(
                    Expression.NewArrayBounds(typeof(Delegate),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyEnumArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<E[]>> e =
                Expression.Lambda<Func<E[]>>(
                    Expression.NewArrayBounds(typeof(E),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyLongEnumArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<El[]>> e =
                Expression.Lambda<Func<El[]>>(
                    Expression.NewArrayBounds(typeof(El),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyFuncArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<Func<object>[]>> e =
                Expression.Lambda<Func<Func<object>[]>>(
                    Expression.NewArrayBounds(typeof(Func<object>),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyInterfaceArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<I[]>> e =
                Expression.Lambda<Func<I[]>>(
                    Expression.NewArrayBounds(typeof(I),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyIEquatableCustomArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<IEquatable<C>[]>> e =
                Expression.Lambda<Func<IEquatable<C>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<C>),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyIEquatableCustom2ArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<IEquatable<D>[]>> e =
                Expression.Lambda<Func<IEquatable<D>[]>>(
                    Expression.NewArrayBounds(typeof(IEquatable<D>),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyObjectArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<object[]>> e =
                Expression.Lambda<Func<object[]>>(
                    Expression.NewArrayBounds(typeof(object),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyStringArrayWithNullableUShortSize(ushort? size)
        {
            // generate the expression
            Expression<Func<string[]>> e =
                Expression.Lambda<Func<string[]>>(
                    Expression.NewArrayBounds(typeof(string),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyGenericArrayWithNullableByteSize<T>(byte? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyGenericArrayWithNullableIntSize<T>(int? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyGenericArrayWithNullableLongSize<T>(long? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyGenericArrayWithNullableSByteSize<T>(sbyte? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyGenericArrayWithNullableShortSize<T>(short? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyGenericArrayWithNullableUIntSize<T>(uint? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyGenericArrayWithNullableULongSize<T>(ulong? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyGenericArrayWithNullableUShortSize<T>(ushort? size)
        {
            // generate the expression
            Expression<Func<T[]>> e =
                Expression.Lambda<Func<T[]>>(
                    Expression.NewArrayBounds(typeof(T),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableByteSize<Tc>(byte? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableIntSize<Tc>(int? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableLongSize<Tc>(long? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableSByteSize<Tc>(sbyte? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableShortSize<Tc>(short? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableUIntSize<Tc>(uint? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableULongSize<Tc>(ulong? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyGenericWithClassRestrictionArrayWithNullableUShortSize<Tc>(ushort? size) where Tc : class
        {
            // generate the expression
            Expression<Func<Tc[]>> e =
                Expression.Lambda<Func<Tc[]>>(
                    Expression.NewArrayBounds(typeof(Tc),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableByteSize<TC>(byte? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableIntSize<TC>(int? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableLongSize<TC>(long? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableSByteSize<TC>(sbyte? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableShortSize<TC>(short? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableUIntSize<TC>(uint? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableULongSize<TC>(ulong? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyGenericWithSubClassRestrictionArrayWithNullableUShortSize<TC>(ushort? size) where TC : C
        {
            // generate the expression
            Expression<Func<TC[]>> e =
                Expression.Lambda<Func<TC[]>>(
                    Expression.NewArrayBounds(typeof(TC),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableByteSize<Tcn>(byte? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableIntSize<Tcn>(int? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableLongSize<Tcn>(long? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableSByteSize<Tcn>(sbyte? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableShortSize<Tcn>(short? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableUIntSize<Tcn>(uint? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableULongSize<Tcn>(ulong? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyGenericWithClassAndNewRestrictionArrayWithNullableUShortSize<Tcn>(ushort? size) where Tcn : class, new()
        {
            // generate the expression
            Expression<Func<Tcn[]>> e =
                Expression.Lambda<Func<Tcn[]>>(
                    Expression.NewArrayBounds(typeof(Tcn),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableByteSize<TCn>(byte? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableIntSize<TCn>(int? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableLongSize<TCn>(long? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableSByteSize<TCn>(sbyte? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableShortSize<TCn>(short? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableUIntSize<TCn>(uint? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableULongSize<TCn>(ulong? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyGenericWithSubClassAndNewRestrictionArrayWithNullableUShortSize<TCn>(ushort? size) where TCn : C, new()
        {
            // generate the expression
            Expression<Func<TCn[]>> e =
                Expression.Lambda<Func<TCn[]>>(
                    Expression.NewArrayBounds(typeof(TCn),
                        Expression.Constant(size, typeof(ushort?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableByteSize<Ts>(byte? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(byte?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableIntSize<Ts>(int? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(int?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableLongSize<Ts>(long? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(long?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableSByteSize<Ts>(sbyte? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(sbyte?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableShortSize<Ts>(short? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(short?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableUIntSize<Ts>(uint? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(uint?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableULongSize<Ts>(ulong? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(ulong?))),
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

        private static void VerifyGenericWithStructRestrictionArrayWithNullableUShortSize<Ts>(ushort? size) where Ts : struct
        {
            // generate the expression
            Expression<Func<Ts[]>> e =
                Expression.Lambda<Func<Ts[]>>(
                    Expression.NewArrayBounds(typeof(Ts),
                        Expression.Constant(size, typeof(ushort?))),
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
    }
}
