// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace Tests.ExpressionCompiler.Array
{
    public static class ArrayArrayLengthTests
    {
        #region Bool tests

        [Test]
        public static void CheckBoolArrayArrayLengthTest()
        {
            CheckBoolArrayArrayLengthExpression(GenerateBoolArrayArray(0));
            CheckBoolArrayArrayLengthExpression(GenerateBoolArrayArray(1));
            CheckBoolArrayArrayLengthExpression(GenerateBoolArrayArray(5));
        }

        [Test]
        public static void CheckExceptionBoolArrayArrayLengthTest()
        {
            CheckExceptionBoolArrayArrayLength(null);
        }

        #endregion

        #region Byte tests

        [Test]
        public static void CheckByteArrayArrayLengthTest()
        {
            CheckByteArrayArrayLengthExpression(GenerateByteArrayArray(0));
            CheckByteArrayArrayLengthExpression(GenerateByteArrayArray(1));
            CheckByteArrayArrayLengthExpression(GenerateByteArrayArray(5));
        }

        [Test]
        public static void CheckExceptionByteArrayArrayLengthTest()
        {
            CheckExceptionByteArrayArrayLength(null);
        }

        #endregion

        #region Custom tests

        [Test]
        public static void CheckCustomArrayArrayLengthTest()
        {
            CheckCustomArrayArrayLengthExpression(GenerateCustomArrayArray(0));
            CheckCustomArrayArrayLengthExpression(GenerateCustomArrayArray(1));
            CheckCustomArrayArrayLengthExpression(GenerateCustomArrayArray(5));
        }

        [Test]
        public static void CheckExceptionCustomArrayArrayLengthTest()
        {
            CheckExceptionCustomArrayArrayLength(null);
        }

        #endregion

        #region Char tests

        [Test]
        public static void CheckCharArrayArrayLengthTest()
        {
            CheckCharArrayArrayLengthExpression(GenerateCharArrayArray(0));
            CheckCharArrayArrayLengthExpression(GenerateCharArrayArray(1));
            CheckCharArrayArrayLengthExpression(GenerateCharArrayArray(5));
        }

        [Test]
        public static void CheckExceptionCharArrayArrayLengthTest()
        {
            CheckExceptionCharArrayArrayLength(null);
        }

        #endregion

        #region Custom2 tests

        [Test]
        public static void CheckCustom2ArrayArrayLengthTest()
        {
            CheckCustom2ArrayArrayLengthExpression(GenerateCustom2ArrayArray(0));
            CheckCustom2ArrayArrayLengthExpression(GenerateCustom2ArrayArray(1));
            CheckCustom2ArrayArrayLengthExpression(GenerateCustom2ArrayArray(5));
        }

        [Test]
        public static void CheckExceptionCustom2ArrayArrayLengthTest()
        {
            CheckExceptionCustom2ArrayArrayLength(null);
        }

        #endregion

        #region Decimal tests

        [Test]
        public static void CheckDecimalArrayArrayLengthTest()
        {
            CheckDecimalArrayArrayLengthExpression(GenerateDecimalArrayArray(0));
            CheckDecimalArrayArrayLengthExpression(GenerateDecimalArrayArray(1));
            CheckDecimalArrayArrayLengthExpression(GenerateDecimalArrayArray(5));
        }

        [Test]
        public static void CheckExceptionDecimalArrayArrayLengthTest()
        {
            CheckExceptionDecimalArrayArrayLength(null);
        }

        #endregion

        #region Delegate tests

        [Test]
        public static void CheckDelegateArrayArrayLengthTest()
        {
            CheckDelegateArrayArrayLengthExpression(GenerateDelegateArrayArray(0));
            CheckDelegateArrayArrayLengthExpression(GenerateDelegateArrayArray(1));
            CheckDelegateArrayArrayLengthExpression(GenerateDelegateArrayArray(5));
        }

        [Test]
        public static void CheckExceptionDelegateArrayArrayLengthTest()
        {
            CheckExceptionDelegateArrayArrayLength(null);
        }

        #endregion

        #region Double tests

        [Test]
        public static void CheckDoubleArrayArrayLengthTest()
        {
            CheckDoubleArrayArrayLengthExpression(GenerateDoubleArrayArray(0));
            CheckDoubleArrayArrayLengthExpression(GenerateDoubleArrayArray(1));
            CheckDoubleArrayArrayLengthExpression(GenerateDoubleArrayArray(5));
        }

        [Test]
        public static void CheckExceptionDoubleArrayArrayLengthTest()
        {
            CheckExceptionDoubleArrayArrayLength(null);
        }

        #endregion

        #region Enum tests

        [Test]
        public static void CheckEnumArrayArrayLengthTest()
        {
            CheckEnumArrayArrayLengthExpression(GenerateEnumArrayArray(0));
            CheckEnumArrayArrayLengthExpression(GenerateEnumArrayArray(1));
            CheckEnumArrayArrayLengthExpression(GenerateEnumArrayArray(5));
        }

        [Test]
        public static void CheckExceptionEnumArrayArrayLengthTest()
        {
            CheckExceptionEnumArrayArrayLength(null);
        }

        #endregion

        #region EnumLong tests

        [Test]
        public static void CheckEnumLongArrayArrayLengthTest()
        {
            CheckEnumLongArrayArrayLengthExpression(GenerateEnumLongArrayArray(0));
            CheckEnumLongArrayArrayLengthExpression(GenerateEnumLongArrayArray(1));
            CheckEnumLongArrayArrayLengthExpression(GenerateEnumLongArrayArray(5));
        }

        [Test]
        public static void CheckExceptionEnumLongArrayArrayLengthTest()
        {
            CheckExceptionEnumLongArrayArrayLength(null);
        }

        #endregion

        #region Float tests

        [Test]
        public static void CheckFloatArrayArrayLengthTest()
        {
            CheckFloatArrayArrayLengthExpression(GenerateFloatArrayArray(0));
            CheckFloatArrayArrayLengthExpression(GenerateFloatArrayArray(1));
            CheckFloatArrayArrayLengthExpression(GenerateFloatArrayArray(5));
        }

        [Test]
        public static void CheckExceptionFloatArrayArrayLengthTest()
        {
            CheckExceptionFloatArrayArrayLength(null);
        }

        #endregion

        #region Func tests

        [Test]
        public static void CheckFuncArrayArrayLengthTest()
        {
            CheckFuncArrayArrayLengthExpression(GenerateFuncArrayArray(0));
            CheckFuncArrayArrayLengthExpression(GenerateFuncArrayArray(1));
            CheckFuncArrayArrayLengthExpression(GenerateFuncArrayArray(5));
        }

        [Test]
        public static void CheckExceptionFuncArrayArrayLengthTest()
        {
            CheckExceptionFuncArrayArrayLength(null);
        }

        #endregion

        #region Interface tests

        [Test]
        public static void CheckInterfaceArrayArrayLengthTest()
        {
            CheckInterfaceArrayArrayLengthExpression(GenerateInterfaceArrayArray(0));
            CheckInterfaceArrayArrayLengthExpression(GenerateInterfaceArrayArray(1));
            CheckInterfaceArrayArrayLengthExpression(GenerateInterfaceArrayArray(5));
        }

        [Test]
        public static void CheckExceptionInterfaceArrayArrayLengthTest()
        {
            CheckExceptionInterfaceArrayArrayLength(null);
        }

        #endregion

        #region IEquatableCustom tests

        [Test]
        public static void CheckIEquatableCustomArrayArrayLengthTest()
        {
            CheckIEquatableCustomArrayArrayLengthExpression(GenerateIEquatableCustomArrayArray(0));
            CheckIEquatableCustomArrayArrayLengthExpression(GenerateIEquatableCustomArrayArray(1));
            CheckIEquatableCustomArrayArrayLengthExpression(GenerateIEquatableCustomArrayArray(5));
        }

        [Test]
        public static void CheckExceptionIEquatableCustomArrayArrayLengthTest()
        {
            CheckExceptionIEquatableCustomArrayArrayLength(null);
        }

        #endregion

        #region IEquatableCustom2 tests

        [Test]
        public static void CheckIEquatableCustom2ArrayArrayLengthTest()
        {
            CheckIEquatableCustom2ArrayArrayLengthExpression(GenerateIEquatableCustom2ArrayArray(0));
            CheckIEquatableCustom2ArrayArrayLengthExpression(GenerateIEquatableCustom2ArrayArray(1));
            CheckIEquatableCustom2ArrayArrayLengthExpression(GenerateIEquatableCustom2ArrayArray(5));
        }

        [Test]
        public static void CheckExceptionIEquatableCustom2ArrayArrayLengthTest()
        {
            CheckExceptionIEquatableCustom2ArrayArrayLength(null);
        }

        #endregion

        #region Int tests

        [Test]
        public static void CheckIntArrayArrayLengthTest()
        {
            CheckIntArrayArrayLengthExpression(GenerateIntArrayArray(0));
            CheckIntArrayArrayLengthExpression(GenerateIntArrayArray(1));
            CheckIntArrayArrayLengthExpression(GenerateIntArrayArray(5));
        }

        [Test]
        public static void CheckExceptionIntArrayArrayLengthTest()
        {
            CheckExceptionIntArrayArrayLength(null);
        }

        #endregion

        #region Long tests

        [Test]
        public static void CheckLongArrayArrayLengthTest()
        {
            CheckLongArrayArrayLengthExpression(GenerateLongArrayArray(0));
            CheckLongArrayArrayLengthExpression(GenerateLongArrayArray(1));
            CheckLongArrayArrayLengthExpression(GenerateLongArrayArray(5));
        }

        [Test]
        public static void CheckExceptionLongArrayArrayLengthTest()
        {
            CheckExceptionLongArrayArrayLength(null);
        }

        #endregion

        #region Object tests

        [Test]
        public static void CheckObjectArrayArrayLengthTest()
        {
            CheckObjectArrayArrayLengthExpression(GenerateObjectArrayArray(0));
            CheckObjectArrayArrayLengthExpression(GenerateObjectArrayArray(1));
            CheckObjectArrayArrayLengthExpression(GenerateObjectArrayArray(5));
        }

        [Test]
        public static void CheckExceptionObjectArrayArrayLengthTest()
        {
            CheckExceptionObjectArrayArrayLength(null);
        }

        #endregion

        #region Struct tests

        [Test]
        public static void CheckStructArrayArrayLengthTest()
        {
            CheckStructArrayArrayLengthExpression(GenerateStructArrayArray(0));
            CheckStructArrayArrayLengthExpression(GenerateStructArrayArray(1));
            CheckStructArrayArrayLengthExpression(GenerateStructArrayArray(5));
        }

        [Test]
        public static void CheckExceptionStructArrayArrayLengthTest()
        {
            CheckExceptionStructArrayArrayLength(null);
        }

        #endregion

        #region SByte tests

        [Test]
        public static void CheckSByteArrayArrayLengthTest()
        {
            CheckSByteArrayArrayLengthExpression(GenerateSByteArrayArray(0));
            CheckSByteArrayArrayLengthExpression(GenerateSByteArrayArray(1));
            CheckSByteArrayArrayLengthExpression(GenerateSByteArrayArray(5));
        }

        [Test]
        public static void CheckExceptionSByteArrayArrayLengthTest()
        {
            CheckExceptionSByteArrayArrayLength(null);
        }

        #endregion

        #region StructWithString tests

        [Test]
        public static void CheckStructWithStringArrayArrayLengthTest()
        {
            CheckStructWithStringArrayArrayLengthExpression(GenerateStructWithStringArrayArray(0));
            CheckStructWithStringArrayArrayLengthExpression(GenerateStructWithStringArrayArray(1));
            CheckStructWithStringArrayArrayLengthExpression(GenerateStructWithStringArrayArray(5));
        }

        [Test]
        public static void CheckExceptionStructWithStringArrayArrayLengthTest()
        {
            CheckExceptionStructWithStringArrayArrayLength(null);
        }

        #endregion

        #region StructWithStringAndValue tests

        [Test]
        public static void CheckStructWithStringAndValueArrayArrayLengthTest()
        {
            CheckStructWithStringAndValueArrayArrayLengthExpression(GenerateStructWithStringAndValueArrayArray(0));
            CheckStructWithStringAndValueArrayArrayLengthExpression(GenerateStructWithStringAndValueArrayArray(1));
            CheckStructWithStringAndValueArrayArrayLengthExpression(GenerateStructWithStringAndValueArrayArray(5));
        }

        [Test]
        public static void CheckExceptionStructWithStringAndValueArrayArrayLengthTest()
        {
            CheckExceptionStructWithStringAndValueArrayArrayLength(null);
        }

        #endregion

        #region Short tests

        [Test]
        public static void CheckShortArrayArrayLengthTest()
        {
            CheckShortArrayArrayLengthExpression(GenerateShortArrayArray(0));
            CheckShortArrayArrayLengthExpression(GenerateShortArrayArray(1));
            CheckShortArrayArrayLengthExpression(GenerateShortArrayArray(5));
        }

        [Test]
        public static void CheckExceptionShortArrayArrayLengthTest()
        {
            CheckExceptionShortArrayArrayLength(null);
        }

        #endregion

        #region StructWithTwoValues tests

        [Test]
        public static void CheckStructWithTwoValuesArrayArrayLengthTest()
        {
            CheckStructWithTwoValuesArrayArrayLengthExpression(GenerateStructWithTwoValuesArrayArray(0));
            CheckStructWithTwoValuesArrayArrayLengthExpression(GenerateStructWithTwoValuesArrayArray(1));
            CheckStructWithTwoValuesArrayArrayLengthExpression(GenerateStructWithTwoValuesArrayArray(5));
        }

        [Test]
        public static void CheckExceptionStructWithTwoValuesArrayArrayLengthTest()
        {
            CheckExceptionStructWithTwoValuesArrayArrayLength(null);
        }

        #endregion

        #region StructWithValue tests

        [Test]
        public static void CheckStructWithValueArrayArrayLengthTest()
        {
            CheckStructWithValueArrayArrayLengthExpression(GenerateStructWithValueArrayArray(0));
            CheckStructWithValueArrayArrayLengthExpression(GenerateStructWithValueArrayArray(1));
            CheckStructWithValueArrayArrayLengthExpression(GenerateStructWithValueArrayArray(5));
        }

        [Test]
        public static void CheckExceptionStructWithValueArrayArrayLengthTest()
        {
            CheckExceptionStructWithValueArrayArrayLength(null);
        }

        #endregion

        #region String tests

        [Test]
        public static void CheckStringArrayArrayLengthTest()
        {
            CheckStringArrayArrayLengthExpression(GenerateStringArrayArray(0));
            CheckStringArrayArrayLengthExpression(GenerateStringArrayArray(1));
            CheckStringArrayArrayLengthExpression(GenerateStringArrayArray(5));
        }

        [Test]
        public static void CheckExceptionStringArrayArrayLengthTest()
        {
            CheckExceptionStringArrayArrayLength(null);
        }

        #endregion

        #region UInt tests

        [Test]
        public static void CheckUIntArrayArrayLengthTest()
        {
            CheckUIntArrayArrayLengthExpression(GenerateUIntArrayArray(0));
            CheckUIntArrayArrayLengthExpression(GenerateUIntArrayArray(1));
            CheckUIntArrayArrayLengthExpression(GenerateUIntArrayArray(5));
        }

        [Test]
        public static void CheckExceptionUIntArrayArrayLengthTest()
        {
            CheckExceptionUIntArrayArrayLength(null);
        }

        #endregion

        #region ULong tests

        [Test]
        public static void CheckULongArrayArrayLengthTest()
        {
            CheckULongArrayArrayLengthExpression(GenerateULongArrayArray(0));
            CheckULongArrayArrayLengthExpression(GenerateULongArrayArray(1));
            CheckULongArrayArrayLengthExpression(GenerateULongArrayArray(5));
        }

        [Test]
        public static void CheckExceptionULongArrayArrayLengthTest()
        {
            CheckExceptionULongArrayArrayLength(null);
        }

        #endregion

        #region UShort tests

        [Test]
        public static void CheckUShortArrayArrayLengthTest()
        {
            CheckUShortArrayArrayLengthExpression(GenerateUShortArrayArray(0));
            CheckUShortArrayArrayLengthExpression(GenerateUShortArrayArray(1));
            CheckUShortArrayArrayLengthExpression(GenerateUShortArrayArray(5));
        }

        [Test]
        public static void CheckExceptionUShortArrayArrayLengthTest()
        {
            CheckExceptionUShortArrayArrayLength(null);
        }

        #endregion

        #region Generic tests

        [Test]
        public static void CheckGenericCustomArrayArrayLengthTest()
        {
            CheckGenericArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckExceptionGenericCustomArrayArrayLengthTest()
        {
            CheckExceptionGenericArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckGenericEnumArrayArrayLengthTest()
        {
            CheckGenericArrayArrayLengthTestHelper<E>();
        }

        [Test]
        public static void CheckExceptionGenericEnumArrayArrayLengthTest()
        {
            CheckExceptionGenericArrayArrayLengthTestHelper<E>();
        }

        [Test]
        public static void CheckGenericObjectArrayArrayLengthTest()
        {
            CheckGenericArrayArrayLengthTestHelper<object>();
        }

        [Test]
        public static void CheckExceptionGenericObjectArrayArrayLengthTest()
        {
            CheckExceptionGenericArrayArrayLengthTestHelper<object>();
        }

        [Test]
        public static void CheckGenericStructArrayArrayLengthTest()
        {
            CheckGenericArrayArrayLengthTestHelper<S>();
        }

        [Test]
        public static void CheckExceptionGenericStructArrayArrayLengthTest()
        {
            CheckExceptionGenericArrayArrayLengthTestHelper<S>();
        }

        [Test]
        public static void CheckGenericStructWithStringAndValueArrayArrayLengthTest()
        {
            CheckGenericArrayArrayLengthTestHelper<Scs>();
        }

        [Test]
        public static void CheckExceptionGenericStructWithStringAndValueArrayArrayLengthTest()
        {
            CheckExceptionGenericArrayArrayLengthTestHelper<Scs>();
        }

        [Test]
        public static void CheckGenericCustomWithClassRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithClassRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckExceptionGenericCustomWithClassRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithClassRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckGenericObjectWithClassRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithClassRestrictionArrayArrayLengthTestHelper<object>();
        }

        [Test]
        public static void CheckExceptionGenericObjectWithClassRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithClassRestrictionArrayArrayLengthTestHelper<object>();
        }

        [Test]
        public static void CheckGenericCustomWithSubClassRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithSubClassRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckExceptionGenericCustomWithSubClassRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithSubClassRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckGenericCustomWithClassAndNewRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithClassAndNewRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckExceptionGenericCustomWithClassAndNewRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithClassAndNewRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckGenericObjectWithClassAndNewRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithClassAndNewRestrictionArrayArrayLengthTestHelper<object>();
        }

        [Test]
        public static void CheckExceptionGenericObjectWithClassAndNewRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithClassAndNewRestrictionArrayArrayLengthTestHelper<object>();
        }

        [Test]
        public static void CheckGenericCustomWithSubClassAndNewRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckExceptionGenericCustomWithSubClassAndNewRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithSubClassAndNewRestrictionArrayArrayLengthTestHelper<C>();
        }

        [Test]
        public static void CheckGenericEnumWithStructRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithStructRestrictionArrayArrayLengthTestHelper<E>();
        }

        [Test]
        public static void CheckExceptionGenericEnumWithStructRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithStructRestrictionArrayArrayLengthTestHelper<E>();
        }

        [Test]
        public static void CheckGenericStructWithStructRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithStructRestrictionArrayArrayLengthTestHelper<S>();
        }

        [Test]
        public static void CheckExceptionGenericStructWithStructRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithStructRestrictionArrayArrayLengthTestHelper<S>();
        }

        [Test]
        public static void CheckGenericStructWithStringAndValueWithStructRestrictionArrayArrayLengthTest()
        {
            CheckGenericWithStructRestrictionArrayArrayLengthTestHelper<Scs>();
        }

        [Test]
        public static void CheckExceptionGenericStructWithStringAndValueWithStructRestrictionArrayArrayLengthTest()
        {
            CheckExceptionGenericWithStructRestrictionArrayArrayLengthTestHelper<Scs>();
        }

        #endregion

        #region Generic helpers

        public static void CheckGenericArrayArrayLengthTestHelper<T>()
        {
            CheckGenericArrayArrayLengthExpression<T>(GenerateGenericArrayArray<T>(0));
            CheckGenericArrayArrayLengthExpression<T>(GenerateGenericArrayArray<T>(1));
            CheckGenericArrayArrayLengthExpression<T>(GenerateGenericArrayArray<T>(5));
        }

        public static void CheckExceptionGenericArrayArrayLengthTestHelper<T>()
        {
            CheckExceptionGenericArrayArrayLength<T>(null);
        }

        public static void CheckGenericWithClassRestrictionArrayArrayLengthTestHelper<Tc>() where Tc : class
        {
            CheckGenericWithClassRestrictionArrayArrayLengthExpression<Tc>(GenerateGenericWithClassRestrictionArrayArray<Tc>(0));
            CheckGenericWithClassRestrictionArrayArrayLengthExpression<Tc>(GenerateGenericWithClassRestrictionArrayArray<Tc>(1));
            CheckGenericWithClassRestrictionArrayArrayLengthExpression<Tc>(GenerateGenericWithClassRestrictionArrayArray<Tc>(5));
        }

        public static void CheckExceptionGenericWithClassRestrictionArrayArrayLengthTestHelper<Tc>() where Tc : class
        {
            CheckExceptionGenericWithClassRestrictionArrayArrayLength<Tc>(null);
        }

        public static void CheckGenericWithSubClassRestrictionArrayArrayLengthTestHelper<TC>() where TC : C
        {
            CheckGenericWithSubClassRestrictionArrayArrayLengthExpression<TC>(GenerateGenericWithSubClassRestrictionArrayArray<TC>(0));
            CheckGenericWithSubClassRestrictionArrayArrayLengthExpression<TC>(GenerateGenericWithSubClassRestrictionArrayArray<TC>(1));
            CheckGenericWithSubClassRestrictionArrayArrayLengthExpression<TC>(GenerateGenericWithSubClassRestrictionArrayArray<TC>(5));
        }

        public static void CheckExceptionGenericWithSubClassRestrictionArrayArrayLengthTestHelper<TC>() where TC : C
        {
            CheckExceptionGenericWithSubClassRestrictionArrayArrayLength<TC>(null);
        }

        public static void CheckGenericWithClassAndNewRestrictionArrayArrayLengthTestHelper<Tcn>() where Tcn : class, new()
        {
            CheckGenericWithClassAndNewRestrictionArrayArrayLengthExpression<Tcn>(GenerateGenericWithClassAndNewRestrictionArrayArray<Tcn>(0));
            CheckGenericWithClassAndNewRestrictionArrayArrayLengthExpression<Tcn>(GenerateGenericWithClassAndNewRestrictionArrayArray<Tcn>(1));
            CheckGenericWithClassAndNewRestrictionArrayArrayLengthExpression<Tcn>(GenerateGenericWithClassAndNewRestrictionArrayArray<Tcn>(5));
        }

        public static void CheckExceptionGenericWithClassAndNewRestrictionArrayArrayLengthTestHelper<Tcn>() where Tcn : class, new()
        {
            CheckExceptionGenericWithClassAndNewRestrictionArrayArrayLength<Tcn>(null);
        }

        public static void CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthTestHelper<TCn>() where TCn : C, new()
        {
            CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthExpression<TCn>(GenerateGenericWithSubClassAndNewRestrictionArrayArray<TCn>(0));
            CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthExpression<TCn>(GenerateGenericWithSubClassAndNewRestrictionArrayArray<TCn>(1));
            CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthExpression<TCn>(GenerateGenericWithSubClassAndNewRestrictionArrayArray<TCn>(5));
        }

        public static void CheckExceptionGenericWithSubClassAndNewRestrictionArrayArrayLengthTestHelper<TCn>() where TCn : C, new()
        {
            CheckExceptionGenericWithSubClassAndNewRestrictionArrayArrayLength<TCn>(null);
        }

        public static void CheckGenericWithStructRestrictionArrayArrayLengthTestHelper<Ts>() where Ts : struct
        {
            CheckGenericWithStructRestrictionArrayArrayLengthExpression<Ts>(GenerateGenericWithStructRestrictionArrayArray<Ts>(0));
            CheckGenericWithStructRestrictionArrayArrayLengthExpression<Ts>(GenerateGenericWithStructRestrictionArrayArray<Ts>(1));
            CheckGenericWithStructRestrictionArrayArrayLengthExpression<Ts>(GenerateGenericWithStructRestrictionArrayArray<Ts>(5));
        }

        public static void CheckExceptionGenericWithStructRestrictionArrayArrayLengthTestHelper<Ts>() where Ts : struct
        {
            CheckExceptionGenericWithStructRestrictionArrayArrayLength<Ts>(null);
        }

        #endregion

        #region Generate array

        private static bool[][] GenerateBoolArrayArray(int size)
        {
            bool[][] array = new bool[][] { null, new bool[0], new bool[] { true, false }, new bool[100] };
            bool[][] result = new bool[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static byte[][] GenerateByteArrayArray(int size)
        {
            byte[][] array = new byte[][] { null, new byte[0], new byte[] { 0, 1, byte.MaxValue }, new byte[100] };
            byte[][] result = new byte[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static C[][] GenerateCustomArrayArray(int size)
        {
            C[][] array = new C[][] { null, new C[0], new C[] { null, new C(), new D(), new D(0), new D(5) }, new C[100] };
            C[][] result = new C[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static char[][] GenerateCharArrayArray(int size)
        {
            char[][] array = new char[][] { null, new char[0], new char[] { '\0', '\b', 'A', '\uffff' }, new char[100] };
            char[][] result = new char[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static D[][] GenerateCustom2ArrayArray(int size)
        {
            D[][] array = new D[][] { null, new D[0], new D[] { null, new D(), new D(0), new D(5) }, new D[100] };
            D[][] result = new D[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static decimal[][] GenerateDecimalArrayArray(int size)
        {
            decimal[][] array = new decimal[][] { null, new decimal[0], new decimal[] { decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue }, new decimal[100] };
            decimal[][] result = new decimal[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Delegate[][] GenerateDelegateArrayArray(int size)
        {
            Delegate[][] array = new Delegate[][] { null, new Delegate[0], new Delegate[] { null, (Func<object>)delegate () { return null; }, (Func<int, int>)delegate (int i) { return i + 1; }, (Action<object>)delegate { } }, new Delegate[100] };
            Delegate[][] result = new Delegate[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static double[][] GenerateDoubleArrayArray(int size)
        {
            double[][] array = new double[][] { null, new double[0], new double[] { 0, 1, -1, double.MinValue, double.MaxValue, double.Epsilon, double.NegativeInfinity, double.PositiveInfinity, double.NaN }, new double[100] };
            double[][] result = new double[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static E[][] GenerateEnumArrayArray(int size)
        {
            E[][] array = new E[][] { null, new E[0], new E[] { (E)0, E.A, E.B, (E)int.MaxValue, (E)int.MinValue }, new E[100] };
            E[][] result = new E[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static El[][] GenerateEnumLongArrayArray(int size)
        {
            El[][] array = new El[][] { null, new El[0], new El[] { (El)0, El.A, El.B, (El)long.MaxValue, (El)long.MinValue }, new El[100] };
            El[][] result = new El[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static float[][] GenerateFloatArrayArray(int size)
        {
            float[][] array = new float[][] { null, new float[0], new float[] { 0, 1, -1, float.MinValue, float.MaxValue, float.Epsilon, float.NegativeInfinity, float.PositiveInfinity, float.NaN }, new float[100] };
            float[][] result = new float[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Func<object>[][] GenerateFuncArrayArray(int size)
        {
            Func<object>[][] array = new Func<object>[][] { null, new Func<object>[0], new Func<object>[] { null, (Func<object>)delegate () { return null; } }, new Func<object>[100] };
            Func<object>[][] result = new Func<object>[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static I[][] GenerateInterfaceArrayArray(int size)
        {
            I[][] array = new I[][] { null, new I[0], new I[] { null, new C(), new D(), new D(0), new D(5) }, new I[100] };
            I[][] result = new I[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static IEquatable<C>[][] GenerateIEquatableCustomArrayArray(int size)
        {
            IEquatable<C>[][] array = new IEquatable<C>[][] { null, new IEquatable<C>[0], new IEquatable<C>[] { null, new C(), new D(), new D(0), new D(5) }, new IEquatable<C>[100] };
            IEquatable<C>[][] result = new IEquatable<C>[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static IEquatable<D>[][] GenerateIEquatableCustom2ArrayArray(int size)
        {
            IEquatable<D>[][] array = new IEquatable<D>[][] { null, new IEquatable<D>[0], new IEquatable<D>[] { null, new D(), new D(0), new D(5) }, new IEquatable<D>[100] };
            IEquatable<D>[][] result = new IEquatable<D>[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static int[][] GenerateIntArrayArray(int size)
        {
            int[][] array = new int[][] { null, new int[0], new int[] { 0, 1, -1, int.MinValue, int.MaxValue }, new int[100] };
            int[][] result = new int[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static long[][] GenerateLongArrayArray(int size)
        {
            long[][] array = new long[][] { null, new long[0], new long[] { 0, 1, -1, long.MinValue, long.MaxValue }, new long[100] };
            long[][] result = new long[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static object[][] GenerateObjectArrayArray(int size)
        {
            object[][] array = new object[][] { null, new object[0], new object[] { null, new object(), new C(), new D(3) }, new object[100] };
            object[][] result = new object[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static S[][] GenerateStructArrayArray(int size)
        {
            S[][] array = new S[][] { null, new S[0], new S[] { default(S), new S() }, new S[100] };
            S[][] result = new S[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static sbyte[][] GenerateSByteArrayArray(int size)
        {
            sbyte[][] array = new sbyte[][] { null, new sbyte[0], new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue }, new sbyte[100] };
            sbyte[][] result = new sbyte[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Sc[][] GenerateStructWithStringArrayArray(int size)
        {
            Sc[][] array = new Sc[][] { null, new Sc[0], new Sc[] { default(Sc), new Sc(), new Sc(null) }, new Sc[100] };
            Sc[][] result = new Sc[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Scs[][] GenerateStructWithStringAndValueArrayArray(int size)
        {
            Scs[][] array = new Scs[][] { null, new Scs[0], new Scs[] { default(Scs), new Scs(), new Scs(null, new S()) }, new Scs[100] };
            Scs[][] result = new Scs[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static short[][] GenerateShortArrayArray(int size)
        {
            short[][] array = new short[][] { null, new short[0], new short[] { 0, 1, -1, short.MinValue, short.MaxValue }, new short[100] };
            short[][] result = new short[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Sp[][] GenerateStructWithTwoValuesArrayArray(int size)
        {
            Sp[][] array = new Sp[][] { null, new Sp[0], new Sp[] { default(Sp), new Sp(), new Sp(5, 5.0) }, new Sp[100] };
            Sp[][] result = new Sp[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Ss[][] GenerateStructWithValueArrayArray(int size)
        {
            Ss[][] array = new Ss[][] { null, new Ss[0], new Ss[] { default(Ss), new Ss(), new Ss(new S()) }, new Ss[100] };
            Ss[][] result = new Ss[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static string[][] GenerateStringArrayArray(int size)
        {
            string[][] array = new string[][] { null, new string[0], new string[] { null, "", "a", "foo" }, new string[100] };
            string[][] result = new string[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static uint[][] GenerateUIntArrayArray(int size)
        {
            uint[][] array = new uint[][] { null, new uint[0], new uint[] { 0, 1, uint.MaxValue }, new uint[100] };
            uint[][] result = new uint[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static ulong[][] GenerateULongArrayArray(int size)
        {
            ulong[][] array = new ulong[][] { null, new ulong[0], new ulong[] { 0, 1, ulong.MaxValue }, new ulong[100] };
            ulong[][] result = new ulong[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static ushort[][] GenerateUShortArrayArray(int size)
        {
            ushort[][] array = new ushort[][] { null, new ushort[0], new ushort[] { 0, 1, ushort.MaxValue }, new ushort[100] };
            ushort[][] result = new ushort[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static T[][] GenerateGenericArrayArray<T>(int size)
        {
            T[][] array = new T[][] { null, new T[0], new T[] { default(T) }, new T[100] };
            T[][] result = new T[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Tc[][] GenerateGenericWithClassRestrictionArrayArray<Tc>(int size) where Tc : class
        {
            Tc[][] array = new Tc[][] { null, new Tc[0], new Tc[] { null, default(Tc) }, new Tc[100] };
            Tc[][] result = new Tc[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static TC[][] GenerateGenericWithSubClassRestrictionArrayArray<TC>(int size) where TC : C
        {
            TC[][] array = new TC[][] { null, new TC[0], new TC[] { null, default(TC), (TC)new C() }, new TC[100] };
            TC[][] result = new TC[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Tcn[][] GenerateGenericWithClassAndNewRestrictionArrayArray<Tcn>(int size) where Tcn : class, new()
        {
            Tcn[][] array = new Tcn[][] { null, new Tcn[0], new Tcn[] { null, default(Tcn), new Tcn() }, new Tcn[100] };
            Tcn[][] result = new Tcn[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static TCn[][] GenerateGenericWithSubClassAndNewRestrictionArrayArray<TCn>(int size) where TCn : C, new()
        {
            TCn[][] array = new TCn[][] { null, new TCn[0], new TCn[] { null, default(TCn), new TCn(), (TCn)new C() }, new TCn[100] };
            TCn[][] result = new TCn[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        private static Ts[][] GenerateGenericWithStructRestrictionArrayArray<Ts>(int size) where Ts : struct
        {
            Ts[][] array = new Ts[][] { null, new Ts[0], new Ts[] { default(Ts), new Ts() }, new Ts[100] };
            Ts[][] result = new Ts[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = array[i % array.Length];
            }

            return result;
        }

        #endregion

        #region Check length expression

        private static void CheckBoolArrayArrayLengthExpression(bool[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(bool[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckByteArrayArrayLengthExpression(byte[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(byte[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckCustomArrayArrayLengthExpression(C[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(C[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckCharArrayArrayLengthExpression(char[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(char[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckCustom2ArrayArrayLengthExpression(D[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(D[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckDecimalArrayArrayLengthExpression(decimal[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(decimal[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckDelegateArrayArrayLengthExpression(Delegate[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Delegate[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckDoubleArrayArrayLengthExpression(double[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(double[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckEnumArrayArrayLengthExpression(E[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(E[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckEnumLongArrayArrayLengthExpression(El[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(El[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckFloatArrayArrayLengthExpression(float[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(float[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckFuncArrayArrayLengthExpression(Func<object>[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Func<object>[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckInterfaceArrayArrayLengthExpression(I[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(I[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckIEquatableCustomArrayArrayLengthExpression(IEquatable<C>[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(IEquatable<C>[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckIEquatableCustom2ArrayArrayLengthExpression(IEquatable<D>[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(IEquatable<D>[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckIntArrayArrayLengthExpression(int[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(int[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckLongArrayArrayLengthExpression(long[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(long[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckObjectArrayArrayLengthExpression(object[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(object[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckStructArrayArrayLengthExpression(S[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(S[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckSByteArrayArrayLengthExpression(sbyte[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(sbyte[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckStructWithStringArrayArrayLengthExpression(Sc[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Sc[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckStructWithStringAndValueArrayArrayLengthExpression(Scs[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Scs[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckShortArrayArrayLengthExpression(short[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(short[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckStructWithTwoValuesArrayArrayLengthExpression(Sp[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Sp[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckStructWithValueArrayArrayLengthExpression(Ss[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Ss[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckStringArrayArrayLengthExpression(string[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(string[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckUIntArrayArrayLengthExpression(uint[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(uint[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckULongArrayArrayLengthExpression(ulong[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(ulong[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckUShortArrayArrayLengthExpression(ushort[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(ushort[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckGenericArrayArrayLengthExpression<T>(T[][] array)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(T[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckGenericWithClassRestrictionArrayArrayLengthExpression<Tc>(Tc[][] array) where Tc : class
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Tc[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckGenericWithSubClassRestrictionArrayArrayLengthExpression<TC>(TC[][] array) where TC : C
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(TC[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckGenericWithClassAndNewRestrictionArrayArrayLengthExpression<Tcn>(Tcn[][] array) where Tcn : class, new()
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Tcn[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthExpression<TCn>(TCn[][] array) where TCn : C, new()
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(TCn[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        private static void CheckGenericWithStructRestrictionArrayArrayLengthExpression<Ts>(Ts[][] array) where Ts : struct
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.ArrayLength(Expression.Constant(array, typeof(Ts[][]))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile();
            Assert.AreEqual(array.Length, f());
        }

        #endregion

        #region Check exception array length

        private static void CheckExceptionBoolArrayArrayLength(bool[][] array)
        {
            bool success = true;
            try
            {
                CheckBoolArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionByteArrayArrayLength(byte[][] array)
        {
            bool success = true;
            try
            {
                CheckByteArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionCustomArrayArrayLength(C[][] array)
        {
            bool success = true;
            try
            {
                CheckCustomArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionCharArrayArrayLength(char[][] array)
        {
            bool success = true;
            try
            {
                CheckCharArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionCustom2ArrayArrayLength(D[][] array)
        {
            bool success = true;
            try
            {
                CheckCustom2ArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionDecimalArrayArrayLength(decimal[][] array)
        {
            bool success = true;
            try
            {
                CheckDecimalArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionDelegateArrayArrayLength(Delegate[][] array)
        {
            bool success = true;
            try
            {
                CheckDelegateArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionDoubleArrayArrayLength(double[][] array)
        {
            bool success = true;
            try
            {
                CheckDoubleArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionEnumArrayArrayLength(E[][] array)
        {
            bool success = true;
            try
            {
                CheckEnumArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionEnumLongArrayArrayLength(El[][] array)
        {
            bool success = true;
            try
            {
                CheckEnumLongArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionFloatArrayArrayLength(float[][] array)
        {
            bool success = true;
            try
            {
                CheckFloatArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionFuncArrayArrayLength(Func<object>[][] array)
        {
            bool success = true;
            try
            {
                CheckFuncArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionInterfaceArrayArrayLength(I[][] array)
        {
            bool success = true;
            try
            {
                CheckInterfaceArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionIEquatableCustomArrayArrayLength(IEquatable<C>[][] array)
        {
            bool success = true;
            try
            {
                CheckIEquatableCustomArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionIEquatableCustom2ArrayArrayLength(IEquatable<D>[][] array)
        {
            bool success = true;
            try
            {
                CheckIEquatableCustom2ArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionIntArrayArrayLength(int[][] array)
        {
            bool success = true;
            try
            {
                CheckIntArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionLongArrayArrayLength(long[][] array)
        {
            bool success = true;
            try
            {
                CheckLongArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionObjectArrayArrayLength(object[][] array)
        {
            bool success = true;
            try
            {
                CheckObjectArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionStructArrayArrayLength(S[][] array)
        {
            bool success = true;
            try
            {
                CheckStructArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionSByteArrayArrayLength(sbyte[][] array)
        {
            bool success = true;
            try
            {
                CheckSByteArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionStructWithStringArrayArrayLength(Sc[][] array)
        {
            bool success = true;
            try
            {
                CheckStructWithStringArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionStructWithStringAndValueArrayArrayLength(Scs[][] array)
        {
            bool success = true;
            try
            {
                CheckStructWithStringAndValueArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionShortArrayArrayLength(short[][] array)
        {
            bool success = true;
            try
            {
                CheckShortArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionStructWithTwoValuesArrayArrayLength(Sp[][] array)
        {
            bool success = true;
            try
            {
                CheckStructWithTwoValuesArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionStructWithValueArrayArrayLength(Ss[][] array)
        {
            bool success = true;
            try
            {
                CheckStructWithValueArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionStringArrayArrayLength(string[][] array)
        {
            bool success = true;
            try
            {
                CheckStringArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionUIntArrayArrayLength(uint[][] array)
        {
            bool success = true;
            try
            {
                CheckUIntArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionULongArrayArrayLength(ulong[][] array)
        {
            bool success = true;
            try
            {
                CheckULongArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionUShortArrayArrayLength(ushort[][] array)
        {
            bool success = true;
            try
            {
                CheckUShortArrayArrayLengthExpression(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionGenericArrayArrayLength<T>(T[][] array)
        {
            bool success = true;
            try
            {
                CheckGenericArrayArrayLengthExpression<T>(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionGenericWithClassRestrictionArrayArrayLength<Tc>(Tc[][] array) where Tc : class
        {
            bool success = true;
            try
            {
                CheckGenericWithClassRestrictionArrayArrayLengthExpression<Tc>(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionGenericWithSubClassRestrictionArrayArrayLength<TC>(TC[][] array) where TC : C
        {
            bool success = true;
            try
            {
                CheckGenericWithSubClassRestrictionArrayArrayLengthExpression<TC>(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionGenericWithClassAndNewRestrictionArrayArrayLength<Tcn>(Tcn[][] array) where Tcn : class, new()
        {
            bool success = true;
            try
            {
                CheckGenericWithClassAndNewRestrictionArrayArrayLengthExpression<Tcn>(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionGenericWithSubClassAndNewRestrictionArrayArrayLength<TCn>(TCn[][] array) where TCn : C, new()
        {
            bool success = true;
            try
            {
                CheckGenericWithSubClassAndNewRestrictionArrayArrayLengthExpression<TCn>(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        private static void CheckExceptionGenericWithStructRestrictionArrayArrayLength<Ts>(Ts[][] array) where Ts : struct
        {
            bool success = true;
            try
            {
                CheckGenericWithStructRestrictionArrayArrayLengthExpression<Ts>(array); // expect to fail
                success = false;
            }
            catch
            {
            }

            Assert.True(success);
        }

        #endregion
    }
}
